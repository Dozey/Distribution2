namespace Distribution2.BitTorrent.Tracker.Client.Udp
{
    class UdpAnnounceTransportFactory : IAnnounceTransportFactory
    {
        public UdpAnnounceTransportFactory(Tracker tracker)
        {
            Tracker = tracker;
        }

        public Tracker Tracker { get; private set; }

        public IAnnounceTransport CreateTransport(IAnnounceRequest request)
        {
            UdpAnnounceRequestPacket requestPacket = new UdpAnnounceRequestPacket();
            requestPacket.info_hash = request.InfoHash;
            requestPacket.peer_id = request.PeerId;
            requestPacket.downloaded = (long)request.Downloaded;
            requestPacket.left = (long)request.Left;
            requestPacket.uploaded = (long)request.Uploaded;
            requestPacket.clientEvent = (int)UdpTrackerEvent.None;
            requestPacket.ip = request.Ip;
            requestPacket.port = (ushort)request.Port;

            UdpAnnounceTransport transport = new UdpAnnounceTransport(Tracker.Ip, Tracker.AnnounceUrl.Port, 60, requestPacket);
            transport.Request = request;

            return transport;
        }
    }
}
