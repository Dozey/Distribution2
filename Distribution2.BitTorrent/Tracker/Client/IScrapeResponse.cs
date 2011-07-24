namespace Distribution2.BitTorrent.Tracker.Client
{
    public interface IScrapeResponse
    {
        TorrentStatisticCollection Files { get; }
    }
}
