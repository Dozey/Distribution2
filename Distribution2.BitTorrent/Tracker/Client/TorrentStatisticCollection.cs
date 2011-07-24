namespace Distribution2.BitTorrent.Tracker.Client
{
    public class TorrentStatisticCollection : ReadOnlyDictionary<InfoHash, TorrentStatistic>
    {
        internal TorrentStatisticCollection(InternalTorrentStatisticCollection torrentStatistics) : base(torrentStatistics) { }
    }
}
