namespace Distribution2.BitTorrent.Tracker.Client
{
    /// <summary>
    /// Various proposed enhancements
    /// </summary>
    public interface IAnnounceRequest3 : IAnnounceRequest2
    {
        int? NumWant { get; set; }
        bool? NoPeerId { get; set; }
        string Key { get; set; }
        string TrackerId { get; set; }
    }
}
