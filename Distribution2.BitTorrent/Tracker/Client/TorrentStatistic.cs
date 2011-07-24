namespace Distribution2.BitTorrent.Tracker.Client
{
    public struct TorrentStatistic
    {
        private int _complete;
        private int _downloaded;
        private int _incomplete;
        private string _name;

        internal TorrentStatistic(int complete, int downloaded, int incomplete, string name)
        {
            _complete = complete;
            _downloaded = downloaded;
            _incomplete = incomplete;
            _name = name;
        }

        public int Complete { get { return _complete; } }
        public int Downloaded { get { return _downloaded; } }
        public int Incomplete { get { return _incomplete; } }
        public string Name { get { return _name; } }
    }
}
