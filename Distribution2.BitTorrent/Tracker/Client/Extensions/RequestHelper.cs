using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distribution2.BitTorrent.Tracker.Client.Extensions
{
    internal static class RequestHelper
    {
        public static T TryRequest<T>(Func<T> request, Predicate<Exception> exceptionTrigger, int tries)
        {
            List<Exception> exceptionTracker;
            return TryRequest<T>(request, exceptionTrigger, tries, out exceptionTracker);
        }

        public static T TryRequest<T>(Func<T> request, Predicate<Exception> exceptionTrigger, int tries, out List<Exception> exceptionTracker)
        {
            exceptionTracker = new List<Exception>();
            T result = default(T);

            for (int i = 0; i < tries; i++)
            {
                try
                {
                    result = request();
                }
                catch (Exception e)
                {
                    if (exceptionTrigger != null)
                        if (exceptionTrigger(e))
                            throw e;

                    exceptionTracker.Add(e);

                    continue;
                }
            }

            return result;
        }
    }
}
