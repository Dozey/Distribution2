using System.Net;

namespace Distribution2.BitTorrent.Tracker.Client.Udp
{
    struct UdpPeer
    {
        private IPAddress _ip;
        private ushort _port;

        public UdpPeer(IPAddress ip, ushort port)
        {
            _ip = ip;
            _port = port;
        }

        public IPAddress Ip { get { return _ip; } }
        public ushort Port { get { return _port; } }
    }
}
