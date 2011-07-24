using System;

namespace Distribution2.BitTorrent.Tracker.Client.Udp
{
    class UdpScrapeTransportFactory : IScrapeTransportFactory
    {
        public UdpScrapeTransportFactory(Tracker tracker)
        {
            Tracker = tracker;
        }

        public Tracker Tracker { get; private set;}

        public IScrapeTransport CreateTransport(IScrapeRequest request)
        {
            UdpScrapeRequestPacket requestPacket = new UdpScrapeRequestPacket();

            if (request is IScrapeRequest2)
            {
                IScrapeRequest2 request2 = (IScrapeRequest2)request;

                if (request2.InfoHashList.Count > UdpScrapeTransport.MultiScrapeRange)
                    throw new ArgumentOutOfRangeException("request", String.Format("UdpTransport supports IScrapeRequest2 with up to {0} items using MTU=1500 - {2} items specified", UdpScrapeTransport.MultiScrapeRange, 1500, request2.InfoHashList.Count));

                requestPacket.info_hash = request2.InfoHashList.ToArray();
            }
            else
            {
                requestPacket.info_hash = new InfoHash[1] { request.InfoHash };
            }

            UdpScrapeTransport transport =  new UdpScrapeTransport(Tracker.Ip, Tracker.AnnounceUrl.Port, 60, requestPacket);
            transport.Request = request;

            return transport;
        }
    }
}
