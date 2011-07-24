using System;
using System.Collections.Specialized;
using System.Web;

namespace Distribution2.BitTorrent.Tracker.Client.Http
{
    class HttpAnnounceTransportFactory : HttpTrackerTransportFactory, IAnnounceTransportFactory
    {
        private Uri baseAnnounceUri;
        private NameValueCollection baseAnnounceQuery;

        public HttpAnnounceTransportFactory(Tracker tracker)
            : base(tracker)
        {
            baseAnnounceUri = UriWithoutQuery(Tracker.AnnounceUrl);
            baseAnnounceQuery = HttpUtility.ParseQueryString(Tracker.AnnounceUrl.Query);
        }

        public IAnnounceTransport CreateTransport(IAnnounceRequest request)
        {
            if (request.InfoHash == null)
                throw new NullReferenceException("InfoHash");

            if (request.PeerId == null)
                throw new NullReferenceException("PeerId");

            if (request.Port == null)
                throw new NullReferenceException("Port");

            if (request.Uploaded == null)
                throw new NullReferenceException("Uploaded");

            if (request.Downloaded == null)
                throw new NullReferenceException("Downloaded");

            if (request.Left == null)
                throw new NullReferenceException("Left");



            NameValueCollection query = new NameValueCollection(baseAnnounceQuery);
            query.Add("info_hash", HttpUtility.UrlEncode(request.InfoHash));
            query.Add("peer_id", HttpUtility.UrlEncode(request.PeerId));
            query.Add("port", request.Port.ToString());
            query.Add("uploaded", request.Uploaded.ToString());
            query.Add("downloaded", request.Downloaded.ToString());
            query.Add("left", request.Left.ToString());

            if (request.Event != null && request.Event != HttpTrackerEvent.None)
                query.Add("event", Enum.GetName(typeof(HttpTrackerEvent), request.Event).ToLower());

            if (request.Ip != null)
                query.Add("ip", request.Ip.ToString());

            if (request is IAnnounceRequest2)
            {
                IAnnounceRequest2 request2 = (IAnnounceRequest2)request;

                if(request2.Compact != null)
                    if (request2.Compact == true)
                        query.Add("compact", Convert.ToInt16(request2.Compact).ToString());
            }

            if (request is IAnnounceRequest3)
            {
                IAnnounceRequest3 request3 = (IAnnounceRequest3)request;

                if(request3.NoPeerId != null)
                    if (request3.NoPeerId == true)
                        query.Add("no_peer_id", Convert.ToInt16(request3.NoPeerId).ToString());

                if(request3.NumWant != null)
                    if (request3.NumWant >= 0)
                        query.Add("numwant", request3.NumWant.ToString());

                if (!String.IsNullOrEmpty(request3.Key))
                    query.Add("key", request3.Key);

                if (!String.IsNullOrEmpty(request3.TrackerId))
                    query.Add("trackerid", request3.TrackerId);
            }


            UriBuilder requestUriBuilder = new UriBuilder(baseAnnounceUri);
            requestUriBuilder.Query = BuildQueryString(query);

            HttpAnnounceTransport transport = new HttpAnnounceTransport(requestUriBuilder.Uri);
            transport.Request = request;

            return transport;
        }
    }
}
