using System.IO;
using System.Net;
using MiscUtil.Conversion;
using MiscUtil.IO;

namespace Distribution2.BitTorrent.Tracker.Client.Udp
{
    struct UdpAnnounceResponsePacket : ISerializeReceive
    {
        public int action;
        public int transaction_id;
        public int interval;
        public int leechers;
        public int seeders;
        public UdpPeer[] peers;

        #region ISerializeReceive Members

        public void FromByteArray(ref byte[] datagram)
        {
            using (MemoryStream buffer = new MemoryStream(datagram))
            {
                using (EndianBinaryReader br = new EndianBinaryReader(new BigEndianBitConverter(), buffer))
                {
                    action = br.ReadInt32();
                    transaction_id = br.ReadInt32();
                    interval = br.ReadInt32();
                    leechers = br.ReadInt32();
                    seeders = br.ReadInt32();

                    peers = new UdpPeer[((datagram.Length - 20) / 6)];

                    for (int i = 0; i < peers.Length; i++)
                        peers[i] = new UdpPeer(new IPAddress(br.ReadBytes(4)), br.ReadUInt16());
                }
            }
        }

        #endregion
    }
}
