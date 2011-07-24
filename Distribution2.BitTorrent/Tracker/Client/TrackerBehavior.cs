using System;
using System.IO;
using System.Net;
using Distribution2.BitTorrent.BEncoding;
using Distribution2.BitTorrent.Tracker.Client.Http;
using Distribution2.BitTorrent.Tracker.Client.Udp;
using System.Text.RegularExpressions;
using System.Web;

namespace Distribution2.BitTorrent.Tracker.Client
{
    public class TrackerBehavior
    {
        // D2-0001-Profile
        private static readonly byte[] discoveryPeerIdPrefix = new byte[16] { 45, 68, 50, 45, 48, 48, 48, 49, 45, 80, 114, 111, 102, 105, 108, 101 };
        // ProfilingTracker
        private static readonly byte[] discoveryInfoHashPrefix = new byte[16] { 80, 114, 111, 102, 105, 108, 105, 110, 103, 84, 114, 97, 99, 107, 101, 114 };

        private const int discoveryPort = 6969;
        private const string httpMultiScrapeResponseStartFragment = "d5:filesdee";
        private const string httpMultiScrapeResponseEndFragment = "dee";
        private const int httpInfoHashQueryFieldSize = 30;
        private static readonly Random random = new Random();

        private TrackerBehavior(Tracker tracker)
        {
            SupportsScrape = tracker.SupportsScrape;
            SupportsFullScrape = false;
            DefaultInterval = TrackerSettings.DefaultInterval;
        }

        public bool SupportsScrape { get; private set; }
        public int ScrapeThreshold { get; private set; }
        public bool SupportsMultiScrape { get; private set; }
        public int MultiScrapeRange { get; private set; }
        public int MultiScrapeThreshold { get; private set; }
        public bool SupportsFullScrape { get; private set; }
        public bool IndicatesPasskey { get; private set; }
        public bool IndicatesPrivacy { get; private set; }
        public bool IndicatesRegistration { get; private set; }
        public bool IndicatesClientRestriction { get; private set; }
        public TimeSpan DefaultInterval { get; private set; }

        public static TrackerBehavior Discover(Tracker tracker)
        {
            TrackerBehavior behavior = new TrackerBehavior(tracker);

            
            behavior.SupportsScrape = tracker.SupportsScrape;
            behavior.SupportsMultiScrape = false;
            behavior.MultiScrapeRange = 0;
            behavior.MultiScrapeThreshold = -1;
            behavior.SupportsFullScrape = false;
            behavior.IndicatesPasskey = false;
            behavior.IndicatesPrivacy = false;
            behavior.DefaultInterval = TrackerSettings.DefaultInterval;

            PeerId peerId1 = CreateDiscoveryPeerId();
            PeerId peerId2 = CreateDiscoveryPeerId();
            InfoHash infoHash1 = CreateDiscoveryInfoHash();
            InfoHash infoHash2 = CreateDiscoveryInfoHash();

            if (tracker.Protocol == TrackerProtocol.HTTP || tracker.Protocol == TrackerProtocol.HTTPS)
            {
                
                string[] announcePathFragments = tracker.AnnounceUrl.LocalPath.Split('/');
                string announceQueryLower = tracker.AnnounceUrl.Query.ToLower();

                if (announcePathFragments.Length > 2)
                {
                    if (Regex.IsMatch(announcePathFragments[announcePathFragments.Length - 2], @"\A[0-9a-fA-F]*\z"))
                    {
                        behavior.IndicatesPasskey = true;
                        behavior.IndicatesPrivacy = true;
                        goto EndExposureAssessment;
                    }
                }

                string passkey = HttpUtility.ParseQueryString(announceQueryLower)["passkey"];

                if (!String.IsNullOrEmpty(passkey))
                {
                    if (Regex.IsMatch(passkey, @"\A[0-9a-fA-F]*\z"))
                    {
                        behavior.IndicatesPasskey = true;
                        behavior.IndicatesPrivacy = true;
                    }
                }

            EndExposureAssessment: ;

            }

            if (behavior.SupportsScrape)
            {
                try
                {
                    behavior.DefaultInterval = tracker.CreateAnnounceRequest(infoHash1, peerId1, discoveryPort, 0, 0, 0).GetTransport().GetResponse().Interval;
                }
                catch (TrackerFailureException tfe)
                {
                    string messageLower = tfe.Message.ToLower();

                    if (messageLower.Contains("client") || messageLower.Contains("protocol"))
                        behavior.IndicatesClientRestriction = true;
                    else
                        behavior.IndicatesRegistration = true;
                }
                catch (Exception)
                {
                    throw;
                }
                
                if (tracker.Protocol == TrackerProtocol.HTTP || tracker.Protocol == TrackerProtocol.HTTPS)
                {
                    try
                    {
                        HttpScrapeTransport scrapeTestTransport = (HttpScrapeTransport)tracker.CreateScrapeRequest().GetTransport();
                        HttpWebResponse scrapeTestResponse = (HttpWebResponse)scrapeTestTransport.HttpRequest.GetResponse();

                        if (scrapeTestResponse.StatusCode == HttpStatusCode.OK)
                        {
                            char[] scrapeStartTestResponseBuffer = new char[httpMultiScrapeResponseStartFragment.Length];
                            char[] scrapeEndTestResponseBuffer = new char[httpMultiScrapeResponseEndFragment.Length];

                            using (StreamReader scrapeTestResponseReader = new StreamReader(scrapeTestResponse.GetResponseStream()))
                            {
                                if (
                                    scrapeTestResponseReader.Read(scrapeStartTestResponseBuffer, 0, scrapeStartTestResponseBuffer.Length) == scrapeStartTestResponseBuffer.Length &&
                                    scrapeTestResponseReader.Read(scrapeEndTestResponseBuffer, 0, scrapeEndTestResponseBuffer.Length) == scrapeEndTestResponseBuffer.Length
                                    )
                                {
                                    if (new string(scrapeStartTestResponseBuffer) == httpMultiScrapeResponseStartFragment)
                                    {
                                        if (scrapeTestResponse.ContentLength != -1 && new string(scrapeEndTestResponseBuffer) != httpMultiScrapeResponseEndFragment)
                                        {
                                            behavior.ScrapeThreshold = (int)Math.Ceiling((decimal)scrapeTestResponse.ContentLength / (decimal)(tracker.ScrapeUrl.ToString().Length + httpInfoHashQueryFieldSize));
                                            behavior.MultiScrapeThreshold = (int)Math.Ceiling((decimal)scrapeTestResponse.ContentLength / (decimal)httpInfoHashQueryFieldSize);
                                        }
                                        else
                                        {
                                            behavior.ScrapeThreshold = TrackerSettings.DefaultScrapeThreshold;
                                            behavior.MultiScrapeThreshold = TrackerSettings.DefaultMultiScrapeThreshold;
                                        }

                                        behavior.SupportsFullScrape = true;
                                    }
                                }
                            }
                        }
                    }
                    catch (TrackerFailureException tfe)
                    {
                        if (tfe.Message.ToLower().Contains("client"))
                            behavior.IndicatesClientRestriction = true;
                        else
                            behavior.IndicatesPrivacy = true;
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                    if (!behavior.IndicatesRegistration && !behavior.IndicatesPrivacy)
                    {
                        try
                        {
                            tracker.CreateAnnounceRequest(infoHash2, peerId2, discoveryPort, 0, 0, 0).GetResponse();

                            IScrapeResponse scrapeResponse = tracker.CreateScrapeRequest(new InfoHash[2] { infoHash1, infoHash2 }).GetResponse();

                            if (scrapeResponse.Files.ContainsKey(infoHash1) && scrapeResponse.Files.ContainsKey(infoHash2))
                            {
                                behavior.SupportsMultiScrape = true;
                                behavior.MultiScrapeRange = HttpScrapeTransport.MultiScrapeRange;
                            }
                        }
                        catch (TrackerFailureException tfe)
                        {
                            string mutliScrapeError = tfe.Message.ToLower();

                            if (mutliScrapeError.Contains("register") && !mutliScrapeError.Contains("pass") && !mutliScrapeError.Contains("key"))
                                behavior.SupportsMultiScrape = true;
                        }
                    }
                    else
                    {
                        // Tracker is private, but multi-scrape is probably enabled
                        behavior.SupportsMultiScrape = true;
                    }
                }
                else if (tracker.Protocol == TrackerProtocol.UDP)
                {
                    behavior.SupportsMultiScrape = true;
                    behavior.MultiScrapeRange = UdpScrapeTransport.MultiScrapeRange;
                }
            }

            return behavior;
        }

        private static PeerId CreateDiscoveryPeerId()
        {
            byte[] peerId = new byte[20];

            Array.Copy(discoveryPeerIdPrefix, 0, peerId, 0, 16);
            Array.Copy(BitConverter.GetBytes(random.Next()), 0, peerId, 16, 4);

            return new PeerId(peerId);
        }

        private static InfoHash CreateDiscoveryInfoHash()
        {
            byte[] infoHash = new byte[20];

            Array.Copy(discoveryInfoHashPrefix, 0, infoHash, 0, 16);
            Array.Copy(BitConverter.GetBytes(random.Next()), 0, infoHash, 16, 4);

            return new InfoHash(infoHash);
        }
    }
}
