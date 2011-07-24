using System.IO;
using MiscUtil.Conversion;
using MiscUtil.IO;

namespace Distribution2.BitTorrent.Tracker.Client.Udp
{
    struct UdpConnectResponsePacket : ISerializeReceive
    {
        public int action;
        public int transaction_id;
        public long connection_id;

        #region ISerializeReceive Members

        public void FromByteArray(ref byte[] datagram)
        {
            using (MemoryStream buffer = new MemoryStream(datagram))
            {
                using (EndianBinaryReader br = new EndianBinaryReader(new BigEndianBitConverter(), buffer))
                {
                    action = br.ReadInt32();
                    transaction_id = br.ReadInt32();
                    connection_id = br.ReadInt64();
                }
            }
        }

        #endregion
    }
}
