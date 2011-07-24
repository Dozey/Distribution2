using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distribution2.BitTorrent.Tracker.Client.Extensions
{
    public class CoalescedScrapeResponse : TorrentStatisticCollection
    {
        internal CoalescedScrapeResponse(InternalTorrentStatisticCollection torrentStatistics)
            : base(torrentStatistics)
        {
        }
    }
}
