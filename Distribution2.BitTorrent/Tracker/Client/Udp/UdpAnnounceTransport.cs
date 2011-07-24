using System;
using System.Net;

namespace Distribution2.BitTorrent.Tracker.Client.Udp
{
    class UdpAnnounceTransport : UdpTransport, IAnnounceTransport
    {
        private UdpAnnounceResponseFactory responseFactory;

        public UdpAnnounceTransport(IPAddress address, int port, int timeout, UdpAnnounceRequestPacket requestPacket)
            : base(address, port, timeout)
        {
            responseFactory = new UdpAnnounceResponseFactory();
            UdpRequset = requestPacket;
        }

        public IAnnounceRequest Request { get; internal set; }
        public UdpAnnounceRequestPacket UdpRequset { get; private set; }
        public UdpAnnounceResponsePacket UdpResponse { get; private set; }

        #region IAnnounceTransport Members

        public IAnnounceResponse GetResponse()
        {
            Random random = new Random();

            UdpConnectRequestPacket connectRequest = new UdpConnectRequestPacket();
            connectRequest.action = 0;
            connectRequest.transaction_id = random.Next();

            Send(connectRequest);
            UdpConnectResponsePacket connectResponse = Receive<UdpConnectResponsePacket>(
                response => response.transaction_id == connectRequest.transaction_id,
                new Action(() => Send(connectRequest))
                );

            UdpAnnounceRequestPacket announceRequest = UdpRequset;
            announceRequest.connection_id = connectResponse.connection_id;
            announceRequest.transaction_id = random.Next();
            announceRequest.ip = Dns.GetHostAddresses(Dns.GetHostName())[0];
            announceRequest.key = random.Next();
            announceRequest.extensions = 0;

            Send(announceRequest);
            UdpAnnounceResponsePacket announceResponse = Receive<UdpAnnounceResponsePacket>(
                response => response.transaction_id == announceRequest.transaction_id,
                new Action(() => Send(announceRequest))
                );

            return responseFactory.CreateResponse(announceResponse);
        }

        #endregion
    }
}
