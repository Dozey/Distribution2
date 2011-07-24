using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distribution2.BitTorrent.Tracker.Client.Extensions
{
    class InternalAnnounceResponseCollection : Dictionary<InfoHash, IAnnounceResponse>
    {
        public InternalAnnounceResponseCollection() : base() { }
    }
}
