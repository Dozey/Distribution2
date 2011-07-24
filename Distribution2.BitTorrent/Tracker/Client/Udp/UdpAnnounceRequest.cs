using System;
using System.Net;

namespace Distribution2.BitTorrent.Tracker.Client.Udp
{
    public class UdpAnnounceRequest : IAnnounceRequest, IAnnounceRequest2
    {
        private IAnnounceTransportFactory transportFactory;
        private bool compact;

        public UdpAnnounceRequest(Tracker tracker)
        {
            transportFactory = new UdpAnnounceTransportFactory(tracker);
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

        bool? IAnnounceRequest2.Compact
        {
            get { return compact; }
            set
            {
                throw new NotSupportedException();
            }
        }

        #endregion
    }
}
