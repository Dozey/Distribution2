using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Distribution2.BitTorrent.Tracker.Client.Http
{
    abstract class HttpTrackerTransportFactory
    {

        public HttpTrackerTransportFactory(Tracker tracker)
        {
            if ((int)(tracker.Protocol & (TrackerProtocol.HTTP | TrackerProtocol.HTTPS)) == 0)
                throw new NotSupportedException();

            Tracker = tracker;
        }

        protected Tracker Tracker { get; private set; }

        protected static Uri UriWithoutQuery(Uri uri)
        {
            UriBuilder uriBuilder = new UriBuilder(uri);
            uriBuilder.Query = String.Empty;

            return uriBuilder.Uri;
        }

        protected static string BuildQueryString(NameValueCollection query)
        {
            return BuildQueryString(query, true);
        }

        protected static string BuildQueryString(NameValueCollection query, bool useArrayKeys)
        {
            List<string> queryItems = new List<string>(query.Count);

            foreach (string key in query)
            {
                string[] values = query.GetValues(key);

                if (values.Length == 1)
                {
                    queryItems.Add(String.Concat(key, '=', values[0]));
                }
                else
                {
                    string arrayKey = useArrayKeys ? String.Concat(key, "[]=") : key;

                    foreach (string value in values)
                        queryItems.Add(String.Concat(arrayKey, "=", value));
                }
            }

            return String.Join("&", queryItems.ToArray());
        }
    }
}
