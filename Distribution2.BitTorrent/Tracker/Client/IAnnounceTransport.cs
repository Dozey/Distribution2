namespace Distribution2.BitTorrent.Tracker.Client
{
    public interface IAnnounceTransport
    {
        IAnnounceRequest Request { get; }
        IAnnounceResponse GetResponse();
    }
}
