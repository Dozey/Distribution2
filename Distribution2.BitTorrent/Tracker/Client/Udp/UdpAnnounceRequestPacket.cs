using System.IO;
using System.Net;
using MiscUtil.Conversion;
using MiscUtil.IO;

namespace Distribution2.BitTorrent.Tracker.Client.Udp
{
    struct UdpAnnounceRequestPacket : ISerializeTransmit
    {
        public long connection_id;
        public const int action = (int)UdpTrackerAction.Announce;
        public int transaction_id;
        public InfoHash info_hash;
        public PeerId peer_id;
        public long downloaded;
        public long left;
        public long uploaded;
        public int clientEvent;
        public IPAddress ip;
        public int key;
        public int num_want;
        public ushort port;
        public ushort extensions;

        #region ISerializeTransmit Members

        public byte[] ToByteArray()
        {
            using (MemoryStream buffer = new MemoryStream(100))
            {
                using (EndianBinaryWriter bw = new EndianBinaryWriter(new BigEndianBitConverter(), buffer))
                {
                    bw.Write(connection_id);
                    bw.Write(action);
                    bw.Write(transaction_id);
                    bw.Write(info_hash);
                    bw.Write(peer_id);
                    bw.Write(downloaded);
                    bw.Write(left);
                    bw.Write(uploaded);
                    bw.Write(clientEvent);
                    bw.Write(ip.GetAddressBytes());
                    bw.Write(key);
                    bw.Write(num_want);
                    bw.Write(port);
                    bw.Write(extensions);
                }

                return buffer.ToArray();
            }
        }

        #endregion
    }
}
