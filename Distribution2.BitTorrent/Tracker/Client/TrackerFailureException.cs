using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distribution2.BitTorrent.Tracker.Client
{
    public class TrackerFailureException : TrackerException
    {
        internal TrackerFailureException(string message) : base(message) { }
        internal TrackerFailureException(string message, Exception innerException) : base(message, innerException) { }
    }
}
