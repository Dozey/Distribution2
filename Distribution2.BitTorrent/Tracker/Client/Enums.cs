
namespace Distribution2.BitTorrent.Tracker.Client
{
    public enum TrackerProtocol
    {
        HTTP = 2,
        HTTPS = 4,
        UDP = 8
    }

    public enum TrackerRequestType
    {
        Announce,
        Scrape
    }

    public enum HttpTrackerEvent
    {
        None,
        Started,
        Stopped,
        Completed
    }

    enum UdpTrackerAction
    {
        Connect = 0,
        Announce = 1,
        Scrape = 2,
        Error = 3
    }

    enum UdpTrackerEvent
    {
        None = 0,
        Completed = 1,
        Started = 2,
        Stopped = 3
    }
}
