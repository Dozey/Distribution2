using System;

namespace Distribution2.BitTorrent.Tracker.Client.Udp
{
    class UdpAnnounceResponseFactory
    {
        public IAnnounceResponse CreateResponse(UdpAnnounceResponsePacket responsePacket)
        {
            UdpAnnounceResponse response = new UdpAnnounceResponse();
            InternalPeerList peers = new InternalPeerList();

            response.Interval = new TimeSpan(0, 0, responsePacket.interval);
            response.Complete = responsePacket.seeders;
            response.Incomplete = responsePacket.leechers;
            foreach (UdpPeer peer in responsePacket.peers)
                peers.Add(new Peer(PeerId.Empty, peer.Ip.ToString(), peer.Port));

            response.Peers = new PeerList(peers);

            return response;
        }
    }
}
