using System;
using System.Net;
using System.Collections.Generic;

namespace Distribution2.BitTorrent.Tracker.Client
{
    public partial class Tracker
    {
        private const string Announce = "announce";
        private const string Scrape = "scrape";
        private readonly object syncRoot = new object();
        private IPAddress ip;

        public Tracker(string trackerUri) : this(new Uri(trackerUri)) { }

        public Tracker(Uri trackerUri)
        {
            switch (trackerUri.Scheme)
            {
                case "http":
                case "https":
                    Protocol = (TrackerProtocol)Enum.Parse(typeof(TrackerProtocol), trackerUri.Scheme, true);
                    string[] pathFragments = trackerUri.LocalPath.Split('/');

                    if (pathFragments.Length > 0)
                    {
                        string directory = pathFragments[pathFragments.Length - 1];

                        if (directory.StartsWith(Announce))
                        {
                            AnnounceUrl = trackerUri;

                            UriBuilder trackerUriBuilder = new UriBuilder(trackerUri);
                            pathFragments[pathFragments.Length - 1] = String.Concat(Scrape, pathFragments[pathFragments.Length - 1].Substring(8));
                            trackerUriBuilder.Path = String.Join("/", pathFragments);
                            ScrapeUrl = trackerUriBuilder.Uri;
                            SupportsScrape = true;

                            return;
                        }
                        else if (directory.StartsWith(Scrape))
                        {
                            ScrapeUrl = trackerUri;
                            SupportsScrape = true;

                            UriBuilder trackerUriBuilder = new UriBuilder(trackerUri);
                            pathFragments[pathFragments.Length - 1] = String.Concat(Announce, pathFragments[pathFragments.Length - 1].Substring(6));
                            trackerUriBuilder.Path = String.Join("/", pathFragments);
                            AnnounceUrl = trackerUriBuilder.Uri;

                            return;
                        }
                    }

                    AnnounceUrl = trackerUri;
                    SupportsScrape = false;
                    break;
                case "udp":
                    Protocol = (TrackerProtocol)Enum.Parse(typeof(TrackerProtocol), trackerUri.Scheme, true);
                    AnnounceUrl = trackerUri;
                    ScrapeUrl = trackerUri;
                    SupportsScrape = true;
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        public TrackerProtocol Protocol { get; set; }

        public IPAddress Ip
        {
            get
            {
                if(ip == null)
                {
                    IPAddress[] addresses = Dns.GetHostAddresses(AnnounceUrl.Host);
                    ip = addresses[0];
                }

                return ip;
            }
        }

        public Uri AnnounceUrl { get; private set; }
        public Uri ScrapeUrl { get; private set; }
        public bool SupportsScrape { get; private set; }

        public TrackerBehavior GetBehavior()
        {
            return GetBehavior(false);
        }

        public TrackerBehavior GetBehavior(bool clean)
        {
            lock (syncRoot)
            {
                if (BehaviorCache.Instance[this] == null || clean)
                    BehaviorCache.Instance[this] = TrackerBehavior.Discover(this);

                return BehaviorCache.Instance[this];
            }
        }

        public class BehaviorCache
        {
            private readonly object syncRoot = new object();
            private static readonly TimeSpan defaultLifetime = new TimeSpan(1, 0, 0);
            private static readonly BehaviorCache instance = new BehaviorCache();
            private readonly Dictionary<Uri, BehaviorCacheItem> cache = new Dictionary<Uri, BehaviorCacheItem>();

            private BehaviorCache(){}

            public static BehaviorCache Instance
            {
                get { return instance; }
            }

            public TrackerBehavior this[Tracker tracker]
            {
                get
                {
                    lock (syncRoot)
                    {
                        if (tracker == null)
                            throw new ArgumentNullException("tracker");

                        if (cache.ContainsKey(tracker.AnnounceUrl))
                        {
                            BehaviorCacheItem item = cache[tracker.AnnounceUrl];

                            if (!item.Expired)
                                return item.Behavior;
                        }

                        return null;
                    }
                }
                set
                {
                    lock (syncRoot)
                    {
                        if (tracker == null)
                            throw new ArgumentNullException("tracker");

                        if (cache.ContainsKey(tracker.AnnounceUrl))
                            cache[tracker.AnnounceUrl] = new BehaviorCacheItem(value, cache[tracker.AnnounceUrl].LifeTime);
                        else
                            cache.Add(tracker.AnnounceUrl, new BehaviorCacheItem(value));
                    }
                }
            }

            public static void Clear()
            {
                lock (instance.syncRoot)
                {
                    instance.cache.Clear();
                }
            }

            private class BehaviorCacheItem
            {
                public BehaviorCacheItem(TrackerBehavior behavior) : this(behavior, defaultLifetime) { }

                public BehaviorCacheItem(TrackerBehavior behavior, TimeSpan lifetime)
                {
                    Behavior = behavior;
                    LifeTime = lifetime;
                    Expiry = DateTime.Now.Add(lifetime);
                }


                public TimeSpan LifeTime { get; private set; }
                public DateTime Expiry { get; private set; }
                public bool Expired { get { return DateTime.Now > Expiry; } }
                public TrackerBehavior Behavior { get; private set; }

                public void Touch()
                {
                    Expiry = DateTime.Now.Add(LifeTime);
                }
            }
        }
    }
}
