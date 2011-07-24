using System;
using System.Collections.Specialized;
using System.Web;

namespace Distribution2.BitTorrent.Tracker.Client.Http
{
    class HttpScrapeTransportFactory : HttpTrackerTransportFactory, IScrapeTransportFactory
    {
        private Uri baseScrapeUri;
        private NameValueCollection baseScrapeQuery;

        public HttpScrapeTransportFactory(Tracker tracker)
            : base(tracker)
        {
            baseScrapeUri = UriWithoutQuery(Tracker.ScrapeUrl);
            baseScrapeQuery = HttpUtility.ParseQueryString(Tracker.ScrapeUrl.Query);
        }

        #region IScrapeTransportFactory Members

        public IScrapeTransport CreateTransport(IScrapeRequest request)
        {
            NameValueCollection query = new NameValueCollection(baseScrapeQuery);

            if (request.InfoHash != null)
                query.Add("info_hash", HttpUtility.UrlEncode(request.InfoHash));

            if (request is IScrapeRequest2)
            {
                IScrapeRequest2 request2 = (IScrapeRequest2)request;
                
                foreach(InfoHash infoHash in request2.InfoHashList)
                    query.Add("info_hash", HttpUtility.UrlEncode(infoHash));
            }

            UriBuilder requestUriBuilder = new UriBuilder(baseScrapeUri);
            requestUriBuilder.Query = BuildQueryString(query, false);

            HttpScrapeTransport transport = new HttpScrapeTransport(requestUriBuilder.Uri);
            transport.Request = request;

            return transport;
        }

        #endregion
    }
}
