using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distribution2.BitTorrent
{
    internal static class HexEncodingUtil
    {
        public static string ToHexString(byte[] bytes)
        {
            char[] hexChars = new char[bytes.Length * 2];
            byte b;
            for (int i = 0; i < bytes.Length; ++i)
            {
                b = ((byte)(bytes[i] >> 4));
                hexChars[i * 2] = (char)(b > 9 ? b + 0x37 : b + 0x30);
                b = ((byte)(bytes[i] & 0xF));
                hexChars[i * 2 + 1] = (char)(b > 9 ? b + 0x37 : b + 0x30);
            }

            return new string(hexChars);
        }
    }
}
