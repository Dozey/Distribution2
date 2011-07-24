using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distribution2.BitTorrent.Tracker.Client.Extensions
{
    internal struct InternalAnnounceResponse : IAnnounceResponse
    {
        private TimeSpan _interval;
        private int _complete;
        private int _incomplete;
        private PeerList _peers;

        public InternalAnnounceResponse(TimeSpan interval, int complete, int incomplete, PeerList peers)
        {
            _interval = interval;
            _complete = complete;
            _incomplete = incomplete;
            _peers = peers;
        }

        #region IAnnounceResponse Members

        public TimeSpan Interval
        {
            get { return _interval; }
        }

        public int Complete
        {
            get { return _complete; }
        }

        public int Incomplete
        {
            get { return _incomplete; }
        }

        public PeerList Peers
        {
            get { return _peers; }
        }

        #endregion
    }
}
