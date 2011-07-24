using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Distribution2.BitTorrent.BEncoding
{
    internal interface IBencodedValueWithBinaryEncoder : IBEncodedValue
    {
        void Encode(BinaryWriter encodingWriter);
    }
}
