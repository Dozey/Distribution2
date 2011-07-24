using System.IO;

namespace Distribution2.BitTorrent.Tracker.Client
{
    public interface IScrapeResponseFactory
    {
        IScrapeResponse CreateResponse(Stream responseStream);
    }
}
