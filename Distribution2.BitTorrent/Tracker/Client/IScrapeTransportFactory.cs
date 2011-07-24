
namespace Distribution2.BitTorrent.Tracker.Client
{
    public interface IScrapeTransportFactory
    {
        IScrapeTransport CreateTransport(IScrapeRequest request);
    }
}
