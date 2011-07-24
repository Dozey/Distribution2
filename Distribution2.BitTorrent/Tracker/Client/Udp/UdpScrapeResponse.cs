namespace Distribution2.BitTorrent.Tracker.Client.Udp
{
    class UdpScrapeResponse : IScrapeResponse
    {
        internal UdpScrapeResponse() { }

        #region IScrapeResponse Members

        public TorrentStatisticCollection Files { get; internal set; }

        #endregion
    }
}
