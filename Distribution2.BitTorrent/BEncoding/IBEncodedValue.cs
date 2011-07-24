using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Distribution2.BitTorrent.BEncoding
{
    public interface IBEncodedValue
    {
        Encoding TextEncoding
        {
            get;
            set;
        }

        byte[] Encode();

        void Encode(Stream encodingStream);
    }
}
