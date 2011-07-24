namespace Distribution2.BitTorrent.Tracker.Client.Udp
{
    class UdpScrapeRequest : IScrapeRequest, IScrapeRequest2
    {
        private IScrapeTransportFactory transportFactory;
        private InfoHashList infoHashList = new InfoHashList();

        internal UdpScrapeRequest(Tracker tracker)
        {
            transportFactory = new UdpScrapeTransportFactory(tracker);
            Tracker = tracker;
        }

        public Tracker Tracker { get; private set; }

        #region IScrapeRequest Members

        InfoHash IScrapeRequest.InfoHash { get; set; }

        public IScrapeResponse GetResponse()
        {
            return GetTransport().GetResponse();
        }

        public IScrapeTransport GetTransport()
        {
            return transportFactory.CreateTransport(this);
        }

        #endregion

        #region IScrapeRequest2 Members

        InfoHashList IScrapeRequest2.InfoHashList
        {
            get { return infoHashList; }
        }

        #endregion
    }
}
