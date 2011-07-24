using System.Net;

namespace Distribution2.BitTorrent.Tracker.Client
{
    public struct Peer
    {
        private PeerId _id;
        private IPAddress _ip;
        private string _hostNameOrAddress;
        private int _port;

        internal Peer(PeerId id, string hostNameOrAddress, int port)
        {
            _id = id;
            _ip = null;
            _hostNameOrAddress = hostNameOrAddress;
            _port = port;
        }

        public PeerId Id { get { return _id; } }

        public IPAddress Ip
        {
            get
            {
                if (_ip == null)
                {
                    IPAddress[] result = Dns.GetHostAddresses(HostNameOrAddress);

                    if (result.Length > 0)
                        _ip = result[0];
                }

                return _ip;
            }
        }

        public string HostNameOrAddress { get { return _hostNameOrAddress; } }

        public int Port { get { return _port; } }
    }
}
