using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distribution2.BitTorrent.Tracker.Client.Extensions
{
    public class CoalescedAnnounceResponse : ReadOnlyDictionary<InfoHash, IAnnounceResponse>
    {
        internal CoalescedAnnounceResponse(InternalAnnounceResponseCollection announceResponses)
            : base(announceResponses)
        {
        }
    }
}
