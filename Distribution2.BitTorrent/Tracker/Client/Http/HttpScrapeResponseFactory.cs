using System;
using System.Collections.Generic;
using System.IO;
using Distribution2.BitTorrent.BEncoding;

namespace Distribution2.BitTorrent.Tracker.Client.Http
{
    class HttpScrapeResponseFactory
    {
        public IScrapeResponse CreateResponse(Stream responseStream)
        {
            using (responseStream)
            {
                InternalTorrentStatisticCollection files = new InternalTorrentStatisticCollection();
                HttpScrapeResponse response = new HttpScrapeResponse();
                BEncodingSettings.ParserMode = BEncodingParserMode.Loose;
                BEncodedDictionary responseDictionary = BEncodedDictionary.Decode(responseStream);

                if (responseDictionary.ContainsKey("failure reason"))
                    throw new TrackerFailureException((BEncodedString)responseDictionary["failure reason"]);

                foreach (KeyValuePair<BEncodedString, IBEncodedValue> file in (BEncodedDictionary)responseDictionary["files"])
                {
                    BEncodedDictionary value = (BEncodedDictionary)file.Value;

                    files.Add(new InfoHash((BEncodedString)file.Key), new TorrentStatistic(
                        (BEncodedInteger)value["complete"],
                        (BEncodedInteger)value["downloaded"],
                        (BEncodedInteger)value["incomplete"],
                        value.ContainsKey("name") ? ((string)(BEncodedString)value["name"]) : String.Empty
                    ));
                }

                response.Files = new TorrentStatisticCollection(files);

                return response;
            }
        }
    }
}
