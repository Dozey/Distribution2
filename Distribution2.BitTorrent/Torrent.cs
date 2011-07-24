using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using Distribution2.BitTorrent.BEncoding;

namespace Distribution2.BitTorrent
{
    public class Torrent : BEncodedDictionary
    {
        private Torrent()
        {
        }

        public InfoHash InfoHash
        {
            get { return new InfoHash(SHA1.Create().ComputeHash(this["info"].Encode())); }
        }

        public AnnounceUri Announce
        {
            get { return new AnnounceUri((BEncodedString)this["announce"]); }
            set { this["announce"] = value.Container; }
        }

        public AnnounceList AnnounceList
        {
            get
            {
                if (!ContainsKey("announce-list"))
                    Add("announce-list", new BEncodedList());

                return new AnnounceList((BEncodedList)this["announce-list"]);
            }
        }

        public string Name
        {
            get
            {
                return (BEncodedString)Info["name"];
            }
        }

        public BEncodedDictionary Info
        {
            get { return (BEncodedDictionary)this["info"]; }
        }

        public static Torrent Load(string torrentAddress)
        {
            return Load(new Uri(torrentAddress));
        }

        public static Torrent Load(Uri torrentAddress)
        {
            Torrent torrent = new Torrent();
            byte[] data = null;

            if (torrentAddress.IsFile)
            {
                data = File.ReadAllBytes(torrentAddress.LocalPath);
            }
            else if (torrentAddress.Scheme == Uri.UriSchemeHttp || torrentAddress.Scheme == Uri.UriSchemeHttps || torrentAddress.Scheme == Uri.UriSchemeFtp)
            {
                WebClient client = new WebClient();
                data = client.DownloadData(torrentAddress);
            }
            else
            {
                throw new NotSupportedException();
            }

            if(!torrentAddress.IsFile)
                
            BEncodingSettings.ParserMode = BEncodingParserMode.Loose;
            BEncodedDictionary torrentData = BEncodedDictionary.Decode(data);

            foreach (KeyValuePair<BEncodedString, IBEncodedValue> item in torrentData)
                torrent.Add(item.Key, item.Value);

            return torrent;
        }

        public void Save(string path)
        {
            File.WriteAllBytes(path, Encode());
        }

        public override byte[] Encode()
        {

            return base.Encode();
        }

        public override void Encode(System.IO.Stream encodingStream)
        {
            base.Encode(encodingStream);
        }
            
    }
}
