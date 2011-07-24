using System.Collections.ObjectModel;

namespace Distribution2.BitTorrent.Tracker.Client
{
    internal class InternalPeerList : KeyedCollection<PeerId, Peer>
    {
        protected override PeerId GetKeyForItem(Peer item)
        {
            return item.Id;
        }
    }
}
