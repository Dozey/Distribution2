namespace Distribution2.BitTorrent.Tracker.Client
{
    /// <summary>
    /// Compact response enhancement
    /// http://www.bittorrent.org/beps/bep_0023.html
    /// </summary>
    public interface IAnnounceRequest2 : IAnnounceRequest
    {
        bool? Compact { get; set; }
    }
}
