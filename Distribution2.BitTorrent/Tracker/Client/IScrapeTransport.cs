namespace Distribution2.BitTorrent.Tracker.Client
{
    public interface IScrapeTransport
    {
        IScrapeRequest Request { get; }
        IScrapeResponse GetResponse();
    }
}
