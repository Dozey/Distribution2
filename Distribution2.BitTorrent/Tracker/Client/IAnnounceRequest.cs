using System.Net;

namespace Distribution2.BitTorrent.Tracker.Client
{
    public interface IAnnounceRequest
    {
        InfoHash InfoHash { get; set; }
        PeerId PeerId { get; set; }
        int? Port { get; set; }
        long? Uploaded { get; set; }
        long? Downloaded { get; set; }
        long? Left { get; set; }
        HttpTrackerEvent Event { get; set; }
        IPAddress Ip { get; set; }

        IAnnounceResponse GetResponse();
        IAnnounceTransport GetTransport();
    }
}
