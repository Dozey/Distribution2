namespace Distribution2.BitTorrent.Tracker.Client.Http
{
    class HttpScrapeResponse : IScrapeResponse
    {
        internal HttpScrapeResponse() { }

        #region IScrapeResponse Members

        public TorrentStatisticCollection Files { get; internal set; }

        #endregion
    }
}
