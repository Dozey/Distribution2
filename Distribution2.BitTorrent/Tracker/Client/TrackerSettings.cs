using System;

namespace Distribution2.BitTorrent.Tracker.Client
{
    class TrackerSettings
    {
        public static readonly string DefaultFailureReason = String.Empty;
        public static readonly string DefaultWarningMessage = String.Empty;
        public static readonly TimeSpan DefaultInterval = TimeSpan.FromMinutes(10);
        public static readonly TimeSpan DefaultMinInterval = TimeSpan.FromMinutes(5);
        public static readonly string DefaultTrackerId = String.Empty;
        public static readonly int DefaultScrapeThreshold = 1700393;
        public static readonly int DefaultMultiScrapeThreshold = 4194304;

        public static readonly int MaxRequestRetries = 3;
        public static readonly int MaxConcurrency = 5;
    }
}
