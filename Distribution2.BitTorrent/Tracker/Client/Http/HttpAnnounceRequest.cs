using System.Net;

namespace Distribution2.BitTorrent.Tracker.Client.Http
{
    public class HttpAnnounceRequest : IAnnounceRequest, IAnnounceRequest2, IAnnounceRequest3
    {
        private IAnnounceTransportFactory transportFactory;

        internal HttpAnnounceRequest(Tracker tracker)
        {
            transportFactory = new HttpAnnounceTransportFactory(tracker);
        }

        #region IAnnounceRequest Members

        InfoHash IAnnounceRequest.InfoHash { get; set; }
        PeerId IAnnounceRequest.PeerId { get; set; }
        int? IAnnounceRequest.Port { get; set; }
        long? IAnnounceRequest.Uploaded { get; set; }
        long? IAnnounceRequest.Downloaded { get; set; }
        long? IAnnounceRequest.Left { get; set; }
        HttpTrackerEvent IAnnounceRequest.Event { get; set; }
        IPAddress IAnnounceRequest.Ip { get; set; }

        public IAnnounceResponse GetResponse()
        {
            IAnnounceTransport transport = GetTransport();
            return transport.GetResponse();
        }

        public IAnnounceTransport GetTransport()
        {
            return transportFactory.CreateTransport(this);
        }

        #endregion

        #region IAnnounceRequest2 Members

        bool? IAnnounceRequest2.Compact { get; set; }

        #endregion

        #region IAnnounceRequest3 Members

        int? IAnnounceRequest3.NumWant { get; set; }
        bool? IAnnounceRequest3.NoPeerId { get; set; }
        string IAnnounceRequest3.Key { get; set; }
        string IAnnounceRequest3.TrackerId { get; set; }

        #endregion
    }
}
