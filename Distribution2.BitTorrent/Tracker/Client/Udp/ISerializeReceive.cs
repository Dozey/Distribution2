namespace Distribution2.BitTorrent.Tracker.Client.Udp
{
    interface ISerializeReceive
    {
        void FromByteArray(ref byte[] datagram);
    }
}
