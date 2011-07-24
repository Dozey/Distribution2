using System.IO;
using System.Text;
using MiscUtil.Conversion;
using MiscUtil.IO;

namespace Distribution2.BitTorrent.Tracker.Client.Udp
{
    struct UdpErrorResponsePacket : ISerializeReceive
    {
        public int action;
        public int transaction_id;
        public string error;

        #region ISerializeReceive Members

        public void FromByteArray(ref byte[] datagram)
        {
            using (MemoryStream buffer = new MemoryStream(datagram))
            {
                using (EndianBinaryReader br = new EndianBinaryReader(new LittleEndianBitConverter(), buffer))
                {
                    action = br.ReadInt32();
                    transaction_id = br.ReadInt32();
                    error = Encoding.UTF8.GetString(br.ReadBytes(datagram.Length - 8));
                }
            }
        }

        #endregion
    }
}
