using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distribution2.BitTorrent.BEncoding;

namespace Distribution2.BitTorrent
{
    public class AnnounceUri : IAnnounceEntry, IComparable<AnnounceUri>
    {
        internal AnnounceUri(BEncodedString container)
        {
            Container = container;
        }

        public AnnounceUri(Uri announceUri)
        {
            Container = new BEncodedString(announceUri.ToString());
        }

        #region IAnnounceEntry Members

        public IBEncodedValue Container { get; private set; }

        public bool IsTier
        {
            get { return false; }
        }

        public object GetValue()
        {
            return (Uri)this;
        }

        #endregion

        public static implicit operator Uri(AnnounceUri announceUri)
        {
            return new Uri((BEncodedString)announceUri.Container);
        }

        public override string ToString()
        {
            return Container.ToString();
        }

        #region IComparable<AnnounceUri> Members

        public int CompareTo(AnnounceUri other)
        {
            return ((BEncodedString)Container).CompareTo((BEncodedString)other.Container);
        }

        #endregion

        public static bool operator ==(AnnounceUri a, AnnounceUri b)
        {
            return a.Container.ToString() == b.Container.ToString();
        }

        public static bool operator !=(AnnounceUri a, AnnounceUri b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return Container.ToString().GetHashCode();
        }
    }
}
