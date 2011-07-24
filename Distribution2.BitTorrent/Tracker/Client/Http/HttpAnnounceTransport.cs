using System;
using System.Net;

namespace Distribution2.BitTorrent.Tracker.Client.Http
{
    public class HttpAnnounceTransport : HttpTrackerTransport, IAnnounceTransport
    {
        private HttpAnnounceResponseFactory responseFactory = new HttpAnnounceResponseFactory();

        public HttpAnnounceTransport(Uri uri)
        {
            HttpRequest = (HttpWebRequest)WebRequest.Create(uri);
        }

        public IAnnounceRequest Request { get; internal set; }
        public HttpWebRequest HttpRequest { get; private set; }
        public HttpWebResponse HttpResponse { get; private set; }

        public IAnnounceResponse GetResponse()
        {
            if (HttpRequest == null)
                throw new NullReferenceException("Request is null");

            if(HttpResponse == null)
                HttpResponse = (HttpWebResponse)HttpRequest.GetResponse();

            return responseFactory.CreateResponse(BufferResponseStream(HttpResponse.GetResponseStream()));
        }
    }
}
