using System;
using System.Security.Cryptography;

namespace Distribution2.BitTorrent
{
    public struct InfoHash : IEquatable<InfoHash>
    {
        private byte[] value;

        public InfoHash(byte[] infoHash)
        {
            if (infoHash == null)
                throw new ArgumentNullException("infoHash");

            if (infoHash.Length != 20)
                throw new ArgumentException("infoHash must be 20 bytes long");

            value = infoHash;
        }

        public static InfoHash Empty
        {
            get { return new InfoHash(); }
        }

        public string Hex
        {
            get { return HexEncodingUtil.ToHexString(this); }
        }

        public static implicit operator byte[](InfoHash infoHash)
        {
            return infoHash.value != null ? infoHash.value : new byte[20];
        }

        public static bool operator ==(InfoHash a, InfoHash b)
        {
            return a.GetHashCode() == b.GetHashCode();
        }

        public static bool operator !=(InfoHash a, InfoHash b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            int hashCode = BitConverter.ToInt32(MD5.Create().ComputeHash(this), 0);

            return hashCode;
        }

        #region IEquatable<InfoHash> Members

        public bool Equals(InfoHash other)
        {
            return this == other;
        }

        #endregion
    }
}
