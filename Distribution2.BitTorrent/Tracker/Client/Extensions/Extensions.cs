using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Distribution2.BitTorrent.BEncoding;
using Distribution2.BitTorrent.Tracker;

namespace Distribution2.BitTorrent.Tracker.Client.Extensions
{
    public static class TrackerExtensions
    {
        #if DEBUG
        private static readonly object debugSyncRoot = new object();
        #endif

        public static IEnumerable<IScrapeResponse> Scrape(this Tracker tracker, IEnumerable<IScrapeRequest> requests)
        {
            TrackerBehavior extendedBehavior = tracker.GetBehavior();

            if(!tracker.SupportsScrape)
                throw new NotSupportedException("Tracker does not support scraping");

            foreach (IScrapeRequest request in requests)
                yield return request.GetResponse();
        }

        public static CoalescedScrapeResponse CoalescedScrape(this Tracker tracker, params InfoHash[] scrapeList)
        {
            object syncRoot = new object();
            TrackerBehavior extendedBehavior = tracker.GetBehavior();
            InternalTorrentStatisticCollection torrentStatistics = new InternalTorrentStatisticCollection();

            if (scrapeList.Length > 0)
            {
                if (extendedBehavior.SupportsScrape)
                {
                    if (
                        extendedBehavior.SupportsFullScrape &&
                        !(extendedBehavior.SupportsMultiScrape && scrapeList.Length < extendedBehavior.MultiScrapeThreshold) &&
                        !(extendedBehavior.SupportsMultiScrape == false && scrapeList.Length < extendedBehavior.ScrapeThreshold)
                        )
                    {

                        TorrentStatisticCollection files = null;

                        #if DEBUG
                        lock (debugSyncRoot)
                        {
                            System.Diagnostics.Debug.WriteLine(String.Format("Full scrape[{0}]", scrapeList.Length));
                            System.Diagnostics.Debug.Write(tracker.AnnounceUrl);
                            System.Diagnostics.Debug.Write(" ");
                            System.Diagnostics.Debug.WriteLine("{");
                            System.Diagnostics.Debug.Indent();
                            foreach (InfoHash infoHash in scrapeList)
                                System.Diagnostics.Debug.WriteLine(infoHash.Hex);
                            System.Diagnostics.Debug.Unindent();
                            System.Diagnostics.Debug.WriteLine("}");
                        }
                        #endif

                        IAsyncResult result = tracker.BeginScrape(tracker.CreateScrapeRequest(scrapeList), null, null);
                        IScrapeResponse response = null;

                        try
                        {
                            response = tracker.EndScrape(result);
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }

                        if (response != null)
                        {
                            lock (syncRoot)
                            {
                                foreach (InfoHash infoHash in scrapeList)
                                    if (!torrentStatistics.ContainsKey(infoHash) && files.ContainsKey(infoHash))
                                        torrentStatistics.Add(infoHash, files[infoHash]);
                            }
                        }
                        else
                        {
                            throw new TimeoutException(String.Format("Full scrape[{0}] failed", scrapeList.Length));
                        }
                    }
                    else if (extendedBehavior.SupportsMultiScrape && scrapeList.Length > 1)
                    {
                        int range = (extendedBehavior.MultiScrapeRange > scrapeList.Length ? scrapeList.Length : (extendedBehavior.MultiScrapeRange > 0 ? extendedBehavior.MultiScrapeRange : scrapeList.Length));
                        int coalescedCount = (int)Math.Ceiling((decimal)range / (decimal)scrapeList.Length);
                        Semaphore requestSemaphore = new Semaphore(TrackerSettings.MaxConcurrency, TrackerSettings.MaxConcurrency);

                        IAsyncResult[] results = new IAsyncResult[coalescedCount];
                        List<Exception> exceptions = new List<Exception>();

                        for (int i = 0, j = Math.Min(scrapeList.Length - i, range), k = 0; i < scrapeList.Length; j = Math.Min(scrapeList.Length - i, range), i += j, k++)
                        {
                            requestSemaphore.WaitOne();
                            InfoHash[] coalescingBuffer = new InfoHash[j];
                            Array.Copy(scrapeList, i, coalescingBuffer, 0, j);

                            #if DEBUG
                            lock (debugSyncRoot)
                            {
                                System.Diagnostics.Debug.WriteLine(String.Format("Multi scrape[{0}]({1} - {2}) {3} out of {4}", scrapeList.Length, i, i + j, k + 1, coalescedCount));
                                System.Diagnostics.Debug.Write(tracker.AnnounceUrl);
                                System.Diagnostics.Debug.Write(" ");
                                System.Diagnostics.Debug.WriteLine("{");
                                System.Diagnostics.Debug.Indent();
                                foreach (InfoHash infoHash in coalescingBuffer)
                                    System.Diagnostics.Debug.WriteLine(infoHash.Hex);
                                System.Diagnostics.Debug.Unindent();
                                System.Diagnostics.Debug.WriteLine("}");
                            }
                            #endif

                            object state = new int[4] { scrapeList.Length, i, i + j, k };
                            results[k] = tracker.BeginScrape(tracker.CreateScrapeRequest(coalescingBuffer), new AsyncCallback((result) => requestSemaphore.Release()), state);
                        }

                        foreach (IAsyncResult result in results)
                        {
                            int[] state = (int[])result.AsyncState;
                            IScrapeResponse response = null;

                            try
                            {
                                response = tracker.EndScrape(result);
                            }
                            catch (Exception e)
                            {
                                exceptions.Add(e);

                                continue;
                            }

                            if (response != null)
                            {
                                lock (syncRoot)
                                {
                                    foreach (KeyValuePair<InfoHash, TorrentStatistic> file in response.Files)
                                        if (!torrentStatistics.ContainsKey(file.Key))
                                            torrentStatistics.Add(file.Key, file.Value);
                                }
                            }
                            else
                            {
                                exceptions.Add(new TimeoutException(String.Format("Multi scrape[{0}]({1} - {2}) {3} out of {4} failed", state[0], state[1], state[2], state[3] + 1, coalescedCount)));
                            }
                        }

                        if (exceptions.Count == coalescedCount)
                            ThrowMultipleException(exceptions);
                    }
                    else
                    {
                        Semaphore requestSemaphore = new Semaphore(TrackerSettings.MaxConcurrency, TrackerSettings.MaxConcurrency);
                        IAsyncResult[] results = new IAsyncResult[scrapeList.Length];
                        List<Exception> exceptions = new List<Exception>();

                        for(int i = 0; i < scrapeList.Length; i++)
                        {
                            requestSemaphore.WaitOne();

                            #if DEBUG
                            lock (debugSyncRoot)
                            {
                                System.Diagnostics.Debug.WriteLine("Single scrape[1]");
                                System.Diagnostics.Debug.Write(tracker.AnnounceUrl);
                                System.Diagnostics.Debug.Write(" ");
                                System.Diagnostics.Debug.WriteLine("{");
                                System.Diagnostics.Debug.Indent();
                                System.Diagnostics.Debug.WriteLine(scrapeList[i].Hex);
                                System.Diagnostics.Debug.Unindent();
                                System.Diagnostics.Debug.WriteLine("}");
                            }
                            #endif

                            results[i] = tracker.BeginScrape(tracker.CreateScrapeRequest(scrapeList[i]), new AsyncCallback((result) => requestSemaphore.Release()), scrapeList[i]);
                        }

                        foreach (IAsyncResult result in results)
                        {
                            InfoHash infoHash = (InfoHash)result.AsyncState;
                            IScrapeResponse response = null;

                            try
                            {
                                response = tracker.EndScrape(result);
                            }
                            catch(Exception e)
                            {
                                exceptions.Add(e);

                                continue;
                            }

                            if (response != null)
                            {
                                if (!torrentStatistics.ContainsKey(infoHash) && response.Files.ContainsKey(infoHash))
                                torrentStatistics.Add(infoHash, response.Files[infoHash]);
                            }
                            else
                            {
                                exceptions.Add(new TimeoutException("Single scrape[1] failed"));
                            }
                        }

                        if (exceptions.Count == scrapeList.Length)
                            ThrowMultipleException(exceptions);
                    }
                }
                else
                {
                    throw new NotSupportedException("Tracker does not support scraping");
                }
            }
            
            return new CoalescedScrapeResponse(torrentStatistics);
        }

        public static IEnumerable<IAnnounceResponse> Announce(this Tracker tracker, IEnumerable<IAnnounceRequest> requests)
        {
            foreach (IAnnounceRequest request in requests)
                yield return request.GetResponse();
        }

        public static CoalescedAnnounceResponse CoalescedAnnounce(this Tracker tracker, params IAnnounceRequest[] announceList)
        {
            InternalAnnounceResponseCollection annnounceResponses = new InternalAnnounceResponseCollection();
            Semaphore requestSemaphore = new Semaphore(TrackerSettings.MaxConcurrency, TrackerSettings.MaxConcurrency);
            IAsyncResult[] results = new IAsyncResult[announceList.Length];

            for(int i = 0; i < announceList.Length; i++)
                results[i] = tracker.BeginAnnounce(announceList[i], new AsyncCallback((result) => requestSemaphore.Release()), announceList[i].InfoHash);

            foreach (IAsyncResult result in results)
            {
                IAnnounceResponse response = null;

                try
                {
                    response = tracker.EndAnnounce(result);
                }
                catch { }

                if(response != null)
                    annnounceResponses.Add((InfoHash)result.AsyncState, new InternalAnnounceResponse(
                        response.Interval,
                        response.Complete,
                        response.Incomplete,
                        response.Peers)
                        );
            }

            return new CoalescedAnnounceResponse(annnounceResponses);
        }

        public static CoalescedAnnounceResponse CoalescedNullAnnounce(this Tracker tracker, params InfoHash[] announceList)
        {
            return tracker.CoalescedAnnounce(announceList.Select<InfoHash, IAnnounceRequest>((infoHash) => tracker.CreateAnnounceRequest(infoHash, new PeerId(), 0, 0, 0, 0)).ToArray());
        }

        private static void ThrowMultipleException(IEnumerable<Exception> exceptions)
        {
            string combinedMessage = String.Concat("Multiple exceptions were encountered, see InnerException:", Environment.NewLine, String.Join(Environment.NewLine, exceptions.Select<Exception, string>(bfde => bfde.Message).ToArray()));

            if (exceptions.All((exception) => exception is BEncodedFormatDecodeException))
            {
                throw new BEncodedFormatDecodeException("Multiple exceptions were encountered", new MultipleException<BEncodedFormatDecodeException>(combinedMessage, exceptions.Cast<BEncodedFormatDecodeException>()));
            }
            else if (exceptions.All((exception) => exception is TrackerFailureException))
            {
                throw new TrackerFailureException("Multiple exceptions were encountered", new MultipleException<TrackerFailureException>(combinedMessage, exceptions.Cast<TrackerFailureException>()));
            }
            else if (exceptions.All((exception) => exception is TrackerException))
            {
                throw new TrackerException("Multiple exceptions were encountered", new MultipleException<TrackerException>(combinedMessage, exceptions.Cast<TrackerException>()));
            }
            else if (exceptions.All((exception) => exception is TimeoutException))
            {
                throw new TimeoutException("Multiple exceptions were encountered", new MultipleException<TimeoutException>(combinedMessage, exceptions.Cast<TimeoutException>()));
            }
            else if (exceptions.All((exception) => exception is System.IO.IOException))
            {
                throw new System.IO.IOException("Multiple exceptions were encountered", new MultipleException<System.IO.IOException>(combinedMessage, exceptions.Cast<System.IO.IOException>()));
            }
            else
            {
                throw new Exception("Multiple exceptions were encountered", new MultipleException<Exception>(combinedMessage, exceptions.Cast<Exception>()));
            }
        }
    }
}
