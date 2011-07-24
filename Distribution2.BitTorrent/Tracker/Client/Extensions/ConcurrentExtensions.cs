using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Distribution2.BitTorrent.Tracker.Client;

namespace Distribution2.BitTorrent.Tracker.Client.Extensions
{
    public static class ConcurrentExtensions
    {
        private class AsyncAnnounceParameters
        {
            public AsyncAnnounceParameters(IAnnounceRequest request, AsyncCallback callback, object state)
            {
                Request = request;
                Result = new AsyncAnnounceResult(state);
                Callback = callback;
            }

            public IAnnounceRequest Request { get; private set; }
            public AsyncAnnounceResult Result { get; private set; }
            public AsyncCallback Callback { get; private set; }
        }

        private class AsyncAnnounceResult : IAsyncResult
        {
            public AsyncAnnounceResult(object state)
            {
                AsyncState = state;
                AsyncWaitHandle = new ManualResetEvent(false);
                CompletedSynchronously = false;
                IsCompleted = false;
            }

            public IAnnounceResponse AnnounceResult { get; internal set; }
            public Exception Exception { get; internal set; }

            #region IAsyncResult Members

            public object AsyncState { get; internal set; }
            public ManualResetEvent AsyncWaitHandle { get; internal set; }
            public bool CompletedSynchronously { get; internal set; }
            public bool IsCompleted { get; internal set; }

            #endregion

            #region IAsyncResult Members

            object IAsyncResult.AsyncState { get { return AsyncState; } }
            WaitHandle IAsyncResult.AsyncWaitHandle { get { return AsyncWaitHandle; } }
            bool IAsyncResult.CompletedSynchronously { get { return CompletedSynchronously; } }
            bool IAsyncResult.IsCompleted { get { return IsCompleted; } }

            #endregion
        }


        private class AsyncScrapeParameters
        {
            public AsyncScrapeParameters(IScrapeRequest request, AsyncCallback callback, object state)
            {
                Request = request;
                Result = new AsyncScrapeResult(state);
                Callback = callback;
            }

            public IScrapeRequest Request { get; private set; }
            public AsyncScrapeResult Result { get; private set; }
            public AsyncCallback Callback { get; private set; }
        }

        private class AsyncScrapeResult : IAsyncResult
        {
            public AsyncScrapeResult(object state)
            {
                AsyncState = state;
                AsyncWaitHandle = new ManualResetEvent(false);
                CompletedSynchronously = false;
                IsCompleted = false;
            }

            public IScrapeResponse ScrapeResult { get; internal set; }
            public Exception Exception { get; internal set; }

            #region IAsyncResult Members

            public object AsyncState { get; internal set; }
            public ManualResetEvent AsyncWaitHandle { get; internal set; }
            public bool CompletedSynchronously { get; internal set; }
            public bool IsCompleted { get; internal set; }

            #endregion

            #region IAsyncResult Members

            object IAsyncResult.AsyncState { get { return AsyncState; } }
            WaitHandle IAsyncResult.AsyncWaitHandle { get { return AsyncWaitHandle; } }
            bool IAsyncResult.CompletedSynchronously { get { return CompletedSynchronously; } }
            bool IAsyncResult.IsCompleted { get { return IsCompleted; } }

            #endregion
        }

        internal static IAsyncResult BeginAnnounce(this Tracker tracker, IAnnounceRequest request, AsyncCallback callback, object state)
        {
            AsyncAnnounceParameters parameters = new AsyncAnnounceParameters(request, callback, state);
            ThreadPool.QueueUserWorkItem(new WaitCallback(Announce), parameters);

            return parameters.Result;
        }

        internal static IAnnounceResponse EndAnnounce(this Tracker tracker, IAsyncResult result)
        {
            AsyncAnnounceResult announceResult = (AsyncAnnounceResult)result;
            
            if (announceResult.Exception != null)
                throw announceResult.Exception;

            if (!result.IsCompleted)
            {
                result.AsyncWaitHandle.WaitOne();
                announceResult.CompletedSynchronously = true;
            }

            return announceResult.AnnounceResult;
        }

        private static void Announce(object state)
        {
            AsyncAnnounceParameters parameters = (AsyncAnnounceParameters)state;

            try
            {
                parameters.Result.AnnounceResult = RequestHelper.TryRequest<IAnnounceResponse>(() => parameters.Request.GetResponse(), new Predicate<Exception>((e) => e is TrackerFailureException), TrackerSettings.MaxRequestRetries);
            }
            catch (Exception e)
            {
                parameters.Result.Exception = e;
            }
            finally
            {
                if (parameters.Callback != null)
                {
                    try
                    {
                        parameters.Callback(parameters.Result);
                    }
                    catch { }

                    parameters.Result.AsyncWaitHandle.Set();
                }
            }
        }

        internal static IAsyncResult BeginScrape(this Tracker tracker, IScrapeRequest request, AsyncCallback callback, object state)
        {
            AsyncScrapeParameters parameters = new AsyncScrapeParameters(request, callback, state);
            ThreadPool.QueueUserWorkItem(new WaitCallback(Scrape), parameters);

            return parameters.Result;
        }

        internal static IScrapeResponse EndScrape(this Tracker tracker, IAsyncResult result)
        {
            AsyncScrapeResult scrapeResult = (AsyncScrapeResult)result;

            if (scrapeResult.Exception != null)
                throw scrapeResult.Exception;

            if (!result.IsCompleted)
            {
                result.AsyncWaitHandle.WaitOne();
                scrapeResult.CompletedSynchronously = true;
            }

            return scrapeResult.ScrapeResult;
        }

        private static void Scrape(object state)
        {
            AsyncScrapeParameters parameters = (AsyncScrapeParameters)state;

            try
            {
                parameters.Result.ScrapeResult = RequestHelper.TryRequest<IScrapeResponse>(() => parameters.Request.GetResponse(), new Predicate<Exception>((e) => e is TrackerFailureException), TrackerSettings.MaxRequestRetries);
            }
            catch (Exception e)
            {
                parameters.Result.Exception = e;
            }
            finally
            {
                if (parameters.Callback != null)
                {
                    try
                    {
                        parameters.Callback(parameters.Result);
                    }
                    catch { }

                    parameters.Result.AsyncWaitHandle.Set();
                }
            }
        }
    }
}
