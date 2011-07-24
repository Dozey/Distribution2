namespace Distribution2.BitTorrent.Tracker.Client
{
    public interface IScrapeRequest
    {
        InfoHash InfoHash { get; set; }

        IScrapeResponse GetResponse();
        IScrapeTransport GetTransport();
    }
}
