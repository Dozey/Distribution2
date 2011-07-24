using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distribution2.BitTorrent.BEncoding;

namespace Distribution2.BitTorrent
{
    public interface IAnnounceEntry
    {
        IBEncodedValue Container { get; }
        bool IsTier { get; }
        object GetValue();
    }
}
