using System.IO;
using MiscUtil.Conversion;
using MiscUtil.IO;

namespace Distribution2.BitTorrent.Tracker.Client.Udp
{
    struct UdpConnectRequestPacket : ISerializeTransmit
    {
        public static readonly long connection_id = 0x41727101980;
        public int action;
        public int transaction_id;

        #region ISerializeTransmit Members

        public byte[] ToByteArray()
        {
            using (MemoryStream buffer = new MemoryStream(16))
            {
                using (EndianBinaryWriter bw = new EndianBinaryWriter(new BigEndianBitConverter(), buffer))
                {
                    bw.Write(connection_id);
                    bw.Write(action);
                    bw.Write(transaction_id);
                }

                return buffer.ToArray();
            }
        }

        #endregion
    }
}
