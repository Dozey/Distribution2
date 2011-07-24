using System;
using System.IO;
using Distribution2.BitTorrent.BEncoding;

namespace Distribution2.BitTorrent.Tracker.Client.Http
{
    class HttpAnnounceResponseFactory
    {
        public IAnnounceResponse CreateResponse(Stream responseStream)
        {
            using (responseStream)
            {
                HttpAnnounceResponse response = new HttpAnnounceResponse();
                BEncodingSettings.ParserMode = BEncodingParserMode.Loose;
                BEncodedDictionary responseDictionary = BEncodedDictionary.Decode(responseStream);
                InternalPeerList peers = new InternalPeerList();

                if (responseDictionary.ContainsKey("failure reason"))
                    throw new TrackerFailureException((BEncodedString)responseDictionary["failure reason"]);

                if (responseDictionary["peers"] is BEncodedList)
                {
                    BEncodedList responsePeers = (BEncodedList)responseDictionary["peers"];
                    foreach (BEncodedDictionary peer in responsePeers)
                        peers.Add(new Peer(new PeerId((BEncodedString)peer["peer id"]), (BEncodedString)peer["ip"], (BEncodedInteger)peer["port"]));
                }
                else if (responseDictionary["peers"] is BEncodedString)
                {
                    BEncodedString responsePeers = ((BEncodedString)responseDictionary["peers"]);
                    if (responsePeers.Bytes.Length % 6 != 0)
                        throw new TrackerException("Invalid compact response");

                    for (int i = 0; i < responsePeers.Bytes.Length; i += 6)
                    {
                        byte[] ipAddress = new byte[4];
                        Array.Copy(responsePeers.Bytes, i, ipAddress, 0, 4);

                        peers.Add(new Peer(PeerId.Empty, (BEncodedString)ipAddress.ToString(), BitConverter.ToUInt16(responsePeers.Bytes, i + 4)));
                    }
                }
                else
                {
                    throw new TrackerException("Invalid response");
                }

                if (!responseDictionary.ContainsKey("complete") || !responseDictionary.ContainsKey("incomplete") || !responseDictionary.ContainsKey("interval"))
                    throw new TrackerException("Invalid announce response");

                response.Complete = (BEncodedInteger)responseDictionary["complete"];
                response.Incomplete = (BEncodedInteger)responseDictionary["incomplete"];
                response.Interval = TimeSpan.FromSeconds((BEncodedInteger)responseDictionary["interval"]);
                response.Peers = new PeerList(peers);

                return response;
            }
        }
    }
}
