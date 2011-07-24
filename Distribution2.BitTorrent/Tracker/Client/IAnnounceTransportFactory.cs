namespace Distribution2.BitTorrent.Tracker.Client
{
    public interface IAnnounceTransportFactory
    {
        IAnnounceTransport CreateTransport(IAnnounceRequest request);
    }
}
