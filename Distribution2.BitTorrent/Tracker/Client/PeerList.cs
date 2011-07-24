using System.Collections.ObjectModel;

namespace Distribution2.BitTorrent.Tracker.Client
{
    public class PeerList : ReadOnlyCollection<Peer>
    {
        InternalPeerList _peers;

        internal PeerList(InternalPeerList peers)
            : base(peers)
        {
            _peers = peers;
        }
        
        public Peer this[PeerId peerId]
        {
            get
            {
                return _peers[peerId];
            }
        }
    }
}
