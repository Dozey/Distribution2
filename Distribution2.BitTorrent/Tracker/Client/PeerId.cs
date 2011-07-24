using System;
using System.Security.Cryptography;

namespace Distribution2.BitTorrent.Tracker.Client
{
    public struct PeerId : IEquatable<PeerId>
    {
        private byte[] value;

        public PeerId(byte[] peerId)
        {
            if (peerId == null)
                throw new ArgumentNullException("peerId");

            if (peerId.Length != 20)
                throw new ArgumentException("peerId must be 20 bytes long");

            value = peerId;
        }
        
        public string Hex
        {
            get { return HexEncodingUtil.ToHexString(this); }
        }

        public static PeerId Empty
        {
            get { return new PeerId(); }
        }       

        public static implicit operator byte[](PeerId peerId)
        {
            return peerId.value != null ? peerId.value : new byte[20];
        }

        public static bool operator ==(PeerId a, PeerId b)
        {
            return a.GetHashCode() == b.GetHashCode();
        }

        public static bool operator !=(PeerId a, PeerId b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return BitConverter.ToInt32(MD5.Create().ComputeHash(this), 0);
        }

        #region IEquatable<PeerId> Members

        public bool Equals(PeerId other)
        {
            return this == other;
        }

        #endregion
    }
}
