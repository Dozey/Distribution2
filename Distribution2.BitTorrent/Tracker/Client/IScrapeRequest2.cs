namespace Distribution2.BitTorrent.Tracker.Client
{
    public interface IScrapeRequest2 : IScrapeRequest
    {
        InfoHashList InfoHashList { get; }
    }
}
