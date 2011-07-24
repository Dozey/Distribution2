namespace Distribution2.BitTorrent.Tracker.Client.Http
{
    class HttpScrapeRequest : IScrapeRequest, IScrapeRequest2
    {
        private IScrapeTransportFactory transportFactory;
        private InfoHashList infoHashList = new InfoHashList();

        internal HttpScrapeRequest(Tracker tracker)
        {
            transportFactory = new HttpScrapeTransportFactory(tracker);
        }

        #region IScrapeRequest Members

        InfoHash IScrapeRequest.InfoHash { get; set; }

        public IScrapeResponse GetResponse()
        {
            IScrapeTransport transport = GetTransport();
            return transport.GetResponse();
        }

        public IScrapeTransport GetTransport()
        {
            return transportFactory.CreateTransport(this);
        }

        #endregion

        #region IScrapeRequest2 Members

        InfoHashList IScrapeRequest2.InfoHashList { get { return infoHashList; } }

        #endregion
    }
}
