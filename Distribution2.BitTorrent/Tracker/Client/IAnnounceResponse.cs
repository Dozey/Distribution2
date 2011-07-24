using System;

namespace Distribution2.BitTorrent.Tracker.Client
{
    public interface IAnnounceResponse
    {
        TimeSpan Interval { get; }
        int Complete { get; }
        int Incomplete { get; }
        PeerList Peers { get; }
    }
}
