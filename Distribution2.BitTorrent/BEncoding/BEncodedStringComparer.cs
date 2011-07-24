using System;
using System.Collections.Generic;
using System.Text;

namespace Distribution2.BitTorrent.BEncoding
{
    class BEncodedStringComparer : IComparer<BEncodedString>
    {
        #region IComparer<IBEncodedString> Members

        public int Compare(BEncodedString x, BEncodedString y)
        {
            return x.Value.CompareTo(y.Value);
        }

        #endregion
    }
}
