namespace Distribution2.BitTorrent.Tracker.Client.Udp
{
    class UdpScrapeResponseFactory
    {
        public IScrapeResponse CreateResponse(ref UdpScrapeRequestPacket requestPacket, ref UdpScrapeResponsePacket responsePacket)
        {
            UdpScrapeResponse response = new UdpScrapeResponse();
            InternalTorrentStatisticCollection files = new InternalTorrentStatisticCollection();

            for (int i = 0; i < responsePacket.files.Length; i++)
                files.Add(requestPacket.info_hash[i], responsePacket.files[i]);

            response.Files = new TorrentStatisticCollection(files);

            return response;
        }
    }
}
