using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Distribution2.BitTorrent.BEncoding
{
    public class BEncodedList : BEncodedListBase, IBencodedValueWithBinaryEncoder
    {
        private Encoding _encoding;

        public BEncodedList() : base() { }
        public BEncodedList(Encoding encoding)
            : base()
        {
            if (encoding == null) throw new ArgumentNullException("encoding");
            _encoding = encoding;
        }
        public BEncodedList(int capacity) : base(capacity) { }
        public BEncodedList(int capacity, Encoding encoding)
            : base(capacity)
        {
            if (encoding == null) throw new ArgumentNullException("encoding");
            _encoding = encoding;
        }
        public BEncodedList(IEnumerable<IBEncodedValue> collection) : base(collection) { }
        public BEncodedList(IEnumerable<IBEncodedValue> collection, Encoding encoding)
            : base(collection)
        {
            if (encoding == null) throw new ArgumentNullException("encoding");
            _encoding = encoding;
        }

        public static BEncodedList Decode(byte[] value)
        {
            return Decode(new MemoryStream(value, false));
        }

        public static bool TryDecode(byte[] value, out BEncodedList bencodedList)
        {
            try
            {
                bencodedList = Decode(value, BEncodingSettings.DefaultEncoding);
            }
            catch (Exception e)
            {
                bencodedList = null;
                return false;
            }
            return true;
        }

        public static BEncodedList Decode(byte[] value, Encoding encoding)
        {
            if (encoding == null) throw new ArgumentNullException("encoding");

            using (MemoryStream stream = new MemoryStream(value, false))
            {
                return Decode(stream, encoding);
            }
        }

        public static bool TryDecode(byte[] value, Encoding encoding, out BEncodedList bencodedList)
        {
            try
            {
                bencodedList = Decode(value, encoding);
            }
            catch (Exception e)
            {
                bencodedList = null;
                return false;
            }
            return true;
        }

        public static BEncodedList Decode(Stream value)
        {
            return Decode(value, BEncodingSettings.DefaultEncoding);
        }

        public static bool TryDecode(Stream value, out BEncodedList bencodedList)
        {
            try
            {
                bencodedList = Decode(value);
            }
            catch (Exception e)
            {
                bencodedList = null;
                return false;
            }
            return true;
        }

        public static BEncodedList Decode(Stream value, Encoding encoding)
        {
            if (encoding == null) throw new ArgumentNullException("encoding");
            return Decode(new BinaryReader(value, BEncodingSettings.StreamEncoding), encoding);
        }

        public static bool TryDecode(Stream value, Encoding encoding, out BEncodedList bencodedList)
        {
            try
            {
                bencodedList = Decode(value, encoding);
            }
            catch (Exception e)
            {
                bencodedList = null;
                return false;
            }
            return true;
        }

        internal static BEncodedList Decode(BinaryReader reader, Encoding encoding)
        {
            if (encoding == null) throw new ArgumentNullException("encoding");
            if (!reader.BaseStream.CanSeek) throw new NotSupportedException("Cannot decode non-seekable streams");

            IBEncodedValue value;
            BEncodedList list = new BEncodedList(encoding);
            char peekChar = (char)reader.PeekChar();

            if (peekChar == BEncodingSettings.ListStart)
            {
                reader.ReadChar();
                peekChar = (char)reader.PeekChar();

                while (peekChar != BEncodingSettings.ListEnd)
                {
                    try
                    {
                        switch (peekChar)
                        {
                            case BEncodingSettings.DictionaryStart:
                                value = BEncodedDictionary.Decode(reader, (Encoding)encoding.Clone());
                                break;
                            case BEncodingSettings.ListStart:
                                value = BEncodedList.Decode(reader, (Encoding)encoding.Clone());
                                break;
                            case BEncodingSettings.IntegerStart:
                                value = BEncodedInteger.Decode(reader);
                                break;
                            default:
                                if (Array.IndexOf(BEncodingSettings.NumericMask, peekChar) != -1)
                                {
                                    value = BEncodedString.Decode(reader, (Encoding)encoding.Clone());
                                }
                                else
                                {
                                    throw BEncodedFormatDecodeException.CreateTraced("Expected integer value", reader.BaseStream);
                                }
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }

                    list.Add(value);
                    peekChar = (char)reader.PeekChar();
                }
                reader.ReadChar();
                return list;
            }
            else
            {
                throw BEncodedFormatDecodeException.CreateTraced("Expected list start token", reader.BaseStream);
            }
        }

        internal static bool TryDecode(BinaryReader reader, Encoding encoding, out BEncodedList bencodedList)
        {
            try
            {
                bencodedList = Decode(reader, encoding);
            }
            catch (Exception e)
            {
                bencodedList = null;
                return false;
            }
            return true;
        }

        #region IBencodedValueWithBinaryEncoder Members

        void IBencodedValueWithBinaryEncoder.Encode(BinaryWriter encodingWriter)
        {
            encodingWriter.Write(BEncodingSettings.ListStart);
            foreach (IBEncodedValue value in this) ((IBencodedValueWithBinaryEncoder)value).Encode(encodingWriter);
            encodingWriter.Write(BEncodingSettings.ListEnd);
        }

        #endregion

        #region IBEncodedValue Members

        public Encoding TextEncoding
        {
            get
            {
                return _encoding;
            }
            set
            {
                if (value == null) throw new NullReferenceException();
                _encoding = value;
            }
        }

        public virtual byte[] Encode()
        {
            byte[] buffer;
            MemoryStream encodingStream = new MemoryStream();
            Encode(encodingStream);
            buffer = encodingStream.ToArray();
            encodingStream.Close();
            return buffer;
        }

        public virtual void Encode(Stream encodingStream)
        {
            ((IBencodedValueWithBinaryEncoder)this).Encode(new BinaryWriter(encodingStream, BEncodingSettings.StreamEncoding));
        }

        #endregion

        public static implicit operator BEncodedList(string[] values)
        {
            BEncodedList list = new BEncodedList();
            foreach (string value in values)
            {
                list.Add((BEncodedString)value);
            }
            return list;
        }
    }
}
