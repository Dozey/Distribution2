using System;
using System.IO;
using MiscUtil.Conversion;
using MiscUtil.IO;

namespace Distribution2.BitTorrent.Tracker.Client.Udp
{
    struct UdpScrapeResponsePacket : ISerializeReceive
    {
        public int action;
        public int transaction_id;
        public TorrentStatistic[] files;

        #region ISerializeReceive Members

        public void FromByteArray(ref byte[] datagram)
        {
            using (MemoryStream buffer = new MemoryStream(datagram))
            {
                using (EndianBinaryReader br = new EndianBinaryReader(new BigEndianBitConverter(), buffer))
                {
                    action = br.ReadInt32();
                    transaction_id = br.ReadInt32();

                    files = new TorrentStatistic[((datagram.Length - 8) / 12)];

                    for (int i = 0; i < files.Length; i++)
                    {
                        files[i] = new TorrentStatistic(br.ReadInt32(), br.ReadInt32(), br.ReadInt32(), String.Empty);

                    }
                }
            }
        }

        #endregion
    }
}
