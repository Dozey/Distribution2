using System.IO;
using MiscUtil.Conversion;
using MiscUtil.IO;

namespace Distribution2.BitTorrent.Tracker.Client.Udp
{
    struct UdpScrapeRequestPacket : ISerializeTransmit
    {
        public long connection_id;
        public const int action = (int)UdpTrackerAction.Scrape;
        public int transaction_id;
        public InfoHash[] info_hash;

        #region ISerializeTransmit Members

        public byte[] ToByteArray()
        {
            using(MemoryStream buffer = new MemoryStream(16 + (20 * info_hash.Length)))
            {
                using(EndianBinaryWriter bw = new EndianBinaryWriter(new BigEndianBitConverter(), buffer))
                {
                    bw.Write(connection_id);
                    bw.Write(action);
                    bw.Write(transaction_id);

                    for (int i = 0; i < info_hash.Length; i++)
                    {
                        bw.Write(info_hash[i]);
                    }
                }

                return buffer.ToArray();
            }
        }

        #endregion
    }
}
