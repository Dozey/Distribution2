using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distribution2.BitTorrent.Tracker.Client
{
    public class TrackerException : Exception
    {
        public TrackerException() : base() { }
        public TrackerException(string message) : base(message) { }
        public TrackerException(string message, Exception innerException) : base(message, innerException) { }
    }
}
