using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Distribution2.BitTorrent.BEncoding
{
    public class BEncodedInteger : IBencodedValueWithBinaryEncoder
    {
        private string _value;

        private BEncodedInteger(string value)
        {
            _value = value;
        }

        public BEncodedInteger(short value)
        {
            _value = value.ToString();
        }

        public BEncodedInteger(ushort value)
        {
            _value = value.ToString();
        }

        public BEncodedInteger(int value)
        {
            _value = value.ToString();
        }

        public BEncodedInteger(uint value)
        {
            _value = value.ToString();
        }

        public BEncodedInteger(long value)
        {
            _value = value.ToString();
        }

        public BEncodedInteger(ulong value)
        {
            _value = value.ToString();
        }

        public BEncodedInteger(float value)
        {
            _value = value.ToString();
        }

        public BEncodedInteger(double value)
        {
            _value = value.ToString();
        }

        public static BEncodedInteger Decode(string value)
        {
            using (MemoryStream stream = new MemoryStream(BEncodingSettings.StreamEncoding.GetBytes(value), false))
            {
                return Decode(stream);
            }
        }

        public static bool TryDecode(string value, out BEncodedInteger bencodedInteger)
        {
            try
            {
                bencodedInteger = Decode(value);
            }
            catch (Exception e)
            {
                bencodedInteger = null;
                return false;
            }
            return true;
        }

        public static BEncodedInteger Decode(byte[] value)
        {
            return Decode(new MemoryStream(value, false));
        }

        public static bool TryDecode(byte[] value, out BEncodedInteger bencodedInteger)
        {
            try
            {
                bencodedInteger = Decode(value);
            }
            catch (Exception e)
            {
                bencodedInteger = null;
                return false;
            }
            return true;
        }

        public static BEncodedInteger Decode(Stream value)
        {
            return Decode(new BinaryReader(value, BEncodingSettings.StreamEncoding));
        }

        public static bool TryDecode(Stream value, out BEncodedInteger bencodedInteger)
        {
            try
            {
                bencodedInteger = Decode(value);
            }
            catch (Exception e)
            {
                bencodedInteger = null;
                return false;
            }
            return true;
        }

        internal static BEncodedInteger Decode(BinaryReader reader)
        {
            if (!reader.BaseStream.CanSeek) throw new NotSupportedException("Cannot decode non-seekable streams");

            string stringBuffer = String.Empty;

            if ((char)reader.PeekChar() == BEncodingSettings.IntegerStart)
            {
                reader.ReadChar();

                while (reader.PeekChar() != BEncodingSettings.IntegerEnd)
                {
                    if (Array.IndexOf(BEncodingSettings.NumericMask, (char)reader.PeekChar()) != -1 || reader.PeekChar() == '-' || reader.PeekChar() == '.')
                    {
                        stringBuffer += (reader.ReadChar()).ToString();
                    }
                    else
                    {
                        // Encountered unexpected character
                        throw BEncodedFormatDecodeException.CreateTraced("Encountered non-numeric character", reader.BaseStream);
                    }
                }

                if (stringBuffer.Length > 0)
                {
                    if (stringBuffer.Length > 1 && stringBuffer.StartsWith("0"))
                        throw BEncodedFormatDecodeException.CreateTraced("BEncoded integers cannot start with '0'", reader.BaseStream);

                    if ((char)reader.PeekChar() == BEncodingSettings.IntegerEnd)
                    {
                       reader.ReadChar();
                       return new BEncodedInteger(stringBuffer);
                    }
                    else
                    {
                        throw BEncodedFormatDecodeException.CreateTraced("Expected integer end token", reader.BaseStream);
                    }
                }
                else
                {
                    throw BEncodedFormatDecodeException.CreateTraced("Empty integer value", reader.BaseStream);
                }
            }
            else
            {
                throw BEncodedFormatDecodeException.CreateTraced("Expected integer start token", reader.BaseStream);
            }
        }

        internal static bool TryDecode(BinaryReader reader, out BEncodedInteger bencodedInteger)
        {
            try
            {
                bencodedInteger = Decode(reader);
            }
            catch (Exception e)
            {
                bencodedInteger = null;
                return false;
            }
            return true;
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        #region IBencodedValueWithBinaryEncoder Members

        void IBencodedValueWithBinaryEncoder.Encode(BinaryWriter encodingWriter)
        {
            encodingWriter.Write(BEncodingSettings.IntegerStart);
            encodingWriter.Write(_value.ToString().ToCharArray());
            encodingWriter.Write(BEncodingSettings.IntegerEnd);
        }

        #endregion

        #region IBEncodedValue Members

        public Encoding TextEncoding
        {
            get
            {
                return BEncodingSettings.StreamEncoding;
            }
            set
            {
                throw new NotSupportedException("IBEncodedInteger will always use " + BEncodingSettings.StreamEncoding.EncodingName);
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

        public static implicit operator short(BEncodedInteger integer)
        {
            return short.Parse(integer._value);
        }

        public static implicit operator ushort(BEncodedInteger integer)
        {
            return ushort.Parse(integer._value);
        }

        public static implicit operator int(BEncodedInteger integer)
        {
            return int.Parse(integer._value);
        }

        public static implicit operator uint(BEncodedInteger integer)
        {
            return uint.Parse(integer._value);
        }

        public static implicit operator long(BEncodedInteger integer)
        {
            return long.Parse(integer._value);
        }

        public static implicit operator ulong(BEncodedInteger integer)
        {
            return ulong.Parse(integer._value);
        }

        public static implicit operator float(BEncodedInteger integer)
        {
            return float.Parse(integer._value);
        }

        public static implicit operator double(BEncodedInteger integer)
        {
            return double.Parse(integer._value);
        }

        public static implicit operator BEncodedInteger(short value)
        {
            return new BEncodedInteger(value);
        }

        public static implicit operator BEncodedInteger(ushort value)
        {
            return new BEncodedInteger(value);
        }

        public static implicit operator BEncodedInteger(int value)
        {
            return new BEncodedInteger(value);
        }

        public static implicit operator BEncodedInteger(uint value)
        {
            return new BEncodedInteger(value);
        }

        public static implicit operator BEncodedInteger(long value)
        {
            return new BEncodedInteger(value);
        }

        public static implicit operator BEncodedInteger(ulong value)
        {
            return new BEncodedInteger(value);
        }

        public static implicit operator BEncodedInteger(float value)
        {
            return new BEncodedInteger(value);
        }

        public static implicit operator BEncodedInteger(double value)
        {
            return new BEncodedInteger(value);
        }
    }
}
