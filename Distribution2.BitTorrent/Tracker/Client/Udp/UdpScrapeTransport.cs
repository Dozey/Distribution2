using System;
using System.Net;

namespace Distribution2.BitTorrent.Tracker.Client.Udp
{
    class UdpScrapeTransport : UdpTransport, IScrapeTransport
    {
        private UdpScrapeResponseFactory responseFactory = new UdpScrapeResponseFactory();
        private UdpScrapeRequestPacket scrapeRequest;
        public const int MultiScrapeRange = 73; // UDP datagram can only accommodate 73 info hashes in a datagram under 1500 bytes MTU

        public UdpScrapeTransport(IPAddress address, int port, int timeout, UdpScrapeRequestPacket udpRequest)
            : base(address, port, timeout)
        {
            UdpRequset = udpRequest;
        }

        public IScrapeRequest Request { get; internal set; }
        public UdpScrapeRequestPacket UdpRequset { get; private set; }
        public UdpScrapeResponsePacket UdpResponse { get; private set; }

        #region IScrapeTransport Members

        public IScrapeResponse GetResponse()
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

            UdpScrapeRequestPacket scrapeRequest = UdpRequset;
            scrapeRequest.connection_id = connectResponse.connection_id;
            scrapeRequest.transaction_id = random.Next();

            Send(scrapeRequest);
            UdpScrapeResponsePacket scrapeResponse = Receive<UdpScrapeResponsePacket>(
                response => response.transaction_id == scrapeRequest.transaction_id,
                new Action(() => Send(scrapeRequest))
                );

            UdpResponse = scrapeResponse;

            return responseFactory.CreateResponse(ref scrapeRequest, ref scrapeResponse);
        }

        #endregion
    }
}
