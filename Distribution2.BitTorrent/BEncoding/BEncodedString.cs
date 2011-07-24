using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Distribution2.BitTorrent.BEncoding
{
    public class BEncodedString : IBencodedValueWithBinaryEncoder, IComparable<BEncodedString>, IComparable<string>
    {
        protected Encoding _encoding;
        protected byte[] _value;

        public BEncodedString(byte[] value)
        {
            _encoding = BEncodingSettings.DefaultEncoding;
            _value = value;
        }

        public BEncodedString(byte[] value, Encoding encoding)
        {
            if (encoding == null) throw new ArgumentNullException("encoding");

            _value = value;
            _encoding = encoding;
        }

        public BEncodedString(string value)
        {
            _encoding = BEncodingSettings.DefaultEncoding;
            _value = _encoding.GetBytes(value);
        }

        public BEncodedString(string value, Encoding encoding)
        {
            if (encoding == null) throw new ArgumentNullException("encoding");

            _value = encoding.GetBytes(value);
            _encoding = encoding;
        }

        public BEncodedString(char[] value)
        {
            _encoding = BEncodingSettings.DefaultEncoding;
            _value = _encoding.GetBytes(value);
        }

        public BEncodedString(char[] value, Encoding encoding)
        {
            if (encoding == null) throw new ArgumentNullException("encoding");

            _value = encoding.GetBytes(value);
            _encoding = encoding;
        }

        public byte[] Bytes
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public string Value
        {
            get
            {
                return _encoding.GetString(_value);
            }
            set
            {
                _value = _encoding.GetBytes(value);
            }
        }

        public static BEncodedString Decode(byte[] value)
        {
            return Decode(new MemoryStream(value, false), BEncodingSettings.DefaultEncoding);
        }

        public static bool TryDecode(byte[] value, out BEncodedString bencodedString)
        {
            try
            {
                bencodedString = Decode(value);
            }
            catch (Exception e)
            {
                bencodedString = null;
                return false;
            }
            return true;
        }

        public static BEncodedString Decode(byte[] value, Encoding encoding)
        {
            if (encoding == null) throw new ArgumentNullException("encoding");

            using (MemoryStream stream = new MemoryStream(value, false))
            {
                return Decode(stream, encoding);
            }
        }

        public static bool TryDecode(byte[] value, Encoding encoding, out BEncodedString bencodedString)
        {
            try
            {
                bencodedString = Decode(value, encoding);
            }
            catch (Exception e)
            {
                bencodedString = null;
                return false;
            }
            return true;
        }

        public static BEncodedString Decode(Stream value)
        {
            return Decode(new BinaryReader(value, BEncodingSettings.StreamEncoding), BEncodingSettings.DefaultEncoding);
        }

        public static bool TryDecode(Stream value, out BEncodedString bencodedString)
        {
            try
            {
                bencodedString = Decode(value);
            }
            catch (Exception e)
            {
                bencodedString = null;
                return false;
            }
            return true;
        }

        public static BEncodedString Decode(Stream value, Encoding encoding)
        {
            if (encoding == null) throw new ArgumentNullException("encoding");
            return Decode(new BinaryReader(value, BEncodingSettings.StreamEncoding), encoding);
        }

        public static bool TryDecode(Stream value, Encoding encoding, out BEncodedString bencodedString)
        {
            try
            {
                bencodedString = Decode(value, encoding);
            }
            catch (Exception e)
            {
                bencodedString = null;
                return false;
            }

            return true;
        }

        internal static BEncodedString Decode(BinaryReader reader, Encoding encoding)
        {
            if (encoding == null) throw new ArgumentNullException("encoding");
            if (!reader.BaseStream.CanSeek) throw new NotSupportedException("Cannot decode non-seekable streams");

            byte[] valueBuffer;
            string lengthBuffer = String.Empty;

            while (Array.IndexOf(BEncodingSettings.NumericMask, (char)reader.PeekChar()) != -1)
            {
                lengthBuffer += reader.ReadChar();
            }
            if (lengthBuffer.Length > 0)
            {
                if ((char)reader.PeekChar() == BEncodingSettings.StringStart)
                {
                    reader.ReadChar();
                    valueBuffer = new byte[Int32.Parse(lengthBuffer)];
                    
                    if (reader.Read(valueBuffer, 0, valueBuffer.Length) != valueBuffer.Length)
                    {
                        throw BEncodedFormatDecodeException.CreateTraced("Failed to read string value", reader.BaseStream);
                    }
                    else
                    {
                        return new BEncodedString(valueBuffer, encoding);
                    }
                }
                else
                {
                    throw BEncodedFormatDecodeException.CreateTraced("Expected string start token", reader.BaseStream);
                }
            }
            else
            {
                throw BEncodedFormatDecodeException.CreateTraced("Expected string length value", reader.BaseStream);
            }
        }

        internal static bool TryDecode(BinaryReader reader, Encoding encoding, out BEncodedString bencodedString)
        {
            try
            {
                bencodedString = Decode(reader, encoding);
            }
            catch (Exception e)
            {
                bencodedString = null;
                return false;
            }
            return true;
        }

        public override string ToString()
        {
            return Value;
        }


        #region IBencodedValueWithBinaryEncoder Members

        void IBencodedValueWithBinaryEncoder.Encode(BinaryWriter encodingWriter)
        {
            encodingWriter.Write(_value.Length.ToString().ToCharArray());
            encodingWriter.Write(BEncodingSettings.StringStart);
            encodingWriter.Write(_value);
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

        public static implicit operator string(BEncodedString value)
        {
            return value.ToString();
        }

        public static implicit operator BEncodedString(string value)
        {
            return new BEncodedString(value);
        }

        public static implicit operator byte[](BEncodedString value)
        {
            return value.Bytes;
        }

        public static implicit operator BEncodedString(byte[] value)
        {
            return new BEncodedString(value);
        }

        #region IComparable<BEncodedString> Members

        public int CompareTo(BEncodedString other)
        {
            return Value.CompareTo(other.Value);
        }

        #endregion

        #region IComparable<string> Members

        public int CompareTo(string other)
        {
            return Value.CompareTo(other);
        }

        #endregion
    }
}
