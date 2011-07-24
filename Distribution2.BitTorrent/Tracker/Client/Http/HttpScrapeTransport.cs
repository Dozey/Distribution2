using System;
using System.IO;
using System.Net;

namespace Distribution2.BitTorrent.Tracker.Client.Http
{
    public class HttpScrapeTransport : HttpTrackerTransport, IScrapeTransport
    {
        private HttpScrapeResponseFactory responseFactory = new HttpScrapeResponseFactory();
        public const int MultiScrapeRange = -1;

        internal HttpScrapeTransport(Uri requestUri)
        {
            HttpRequest = (HttpWebRequest)WebRequest.Create(requestUri);
        }

        public IScrapeRequest Request { get; internal set; }
        public HttpWebRequest HttpRequest { get; private set; }
        public HttpWebResponse HttpResponse { get; private set; }

        #region IScrapeTransport Members

        public IScrapeResponse GetResponse()
        {
            if (HttpRequest == null)
                throw new NullReferenceException("Request is null");

            if (HttpResponse == null)
                HttpResponse = (HttpWebResponse)HttpRequest.GetResponse();

            Stream responseStream = HttpResponse.GetResponseStream();

            return responseFactory.CreateResponse(BufferResponseStream(responseStream));
        }

        #endregion
    }
}
