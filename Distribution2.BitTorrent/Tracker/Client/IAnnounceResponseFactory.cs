using System.IO;

namespace Distribution2.BitTorrent.Tracker.Client
{
    public interface IAnnounceResponseFactory
    {
        IAnnounceResponse CreateResponse(Stream responseStream);
    }
}
