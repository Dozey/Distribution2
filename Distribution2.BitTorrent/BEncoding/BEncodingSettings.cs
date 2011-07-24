using System;
using System.Collections.Generic;
using System.Text;

namespace Distribution2.BitTorrent.BEncoding
{
    public enum BEncodingParserMode { Loose, Strict }

    public class BEncodingSettings
    {
        private static readonly Encoding defaultEncoding = Encoding.UTF8;
        private static readonly Encoding streamEncoding = Encoding.ASCII;
        private static readonly BEncodingParserMode defaultParserMode = BEncodingParserMode.Strict;
        private static char[] numericMaskChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        internal const char DictionaryStart = 'd';
        internal const char DictionaryEnd = 'e';
        internal const char ListStart = 'l';
        internal const char ListEnd = 'e';
        internal const char IntegerStart = 'i';
        internal const char IntegerEnd = 'e';
        internal const char StringStart = ':';

        static BEncodingSettings()
        {
            ParserMode = BEncodingParserMode.Strict;
        }


        internal static Encoding DefaultEncoding
        {
            get { return defaultEncoding; }
        }

        internal static Encoding StreamEncoding
        {
            get { return streamEncoding; }
        }

        internal static char[] NumericMask
        {
            get { return numericMaskChars; }
        }

        public static BEncodingParserMode ParserMode { get; set; }
    }
}
