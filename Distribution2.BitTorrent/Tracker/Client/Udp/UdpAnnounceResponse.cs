using System;

namespace Distribution2.BitTorrent.Tracker.Client.Udp
{
    class UdpAnnounceResponse : IAnnounceResponse
    {
        internal UdpAnnounceResponse()
        {
        }

        #region IAnnounceResponse Members

        public TimeSpan Interval { get; internal set; }
        public int Complete { get; internal set; }
        public int Incomplete { get; internal set; }
        public PeerList Peers { get; internal set; }

        #endregion
    }
}
