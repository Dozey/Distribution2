using System;

namespace Distribution2.BitTorrent.Tracker.Client
{
    public interface IAnnounceResponse2 : IAnnounceResponse
    {
        string WarningMessage { get; }
        TimeSpan MinInterval { get; }
        string TrackerId { get; }
    }
}
