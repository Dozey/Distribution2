using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Distribution2.BitTorrent.BEncoding
{
    public class BEncodedDictionary : BEncodedDictionaryBase, IBencodedValueWithBinaryEncoder
    {
        private Encoding _encoding;

        public BEncodedDictionary() : base() { }
        public BEncodedDictionary(Encoding encoding)
            : base()
        {
            if (encoding == null) throw new ArgumentNullException("encoding");
            _encoding = encoding;
        }
        public BEncodedDictionary(IDictionary<BEncodedString, IBEncodedValue> dictionary) : base(dictionary) { }
        public BEncodedDictionary(IDictionary<BEncodedString, IBEncodedValue> dictionary, Encoding encoding)
            : base(dictionary)
        {
            if (encoding == null) throw new ArgumentNullException("encoding");
            _encoding = encoding;
        }

        public static BEncodedDictionary Decode(byte[] value)
        {
            return Decode(new MemoryStream(value, false));
        }

        public static bool TryDecode(byte[] value, out BEncodedDictionary bencodedDictionary)
        {
            try
            {
                bencodedDictionary = Decode(value);
            }
            catch (Exception e)
            {
                bencodedDictionary = null;
                return false;
            }
            return true;
        }

        public static BEncodedDictionary Decode(byte[] value, Encoding encoding)
        {
            if (encoding == null) throw new ArgumentNullException("encoding");

            using (MemoryStream stream = new MemoryStream(value, false))
            {
                return Decode(stream, encoding);
            }
        }

        public static bool TryDecode(byte[] value, Encoding encoding, out BEncodedDictionary bencodedDictionary)
        {
            try
            {
                bencodedDictionary = Decode(value, encoding);
            }
            catch (Exception e)
            {
                bencodedDictionary = null;
                return false;
            }
            return true;
        }

        public static BEncodedDictionary Decode(Stream value)
        {
            return Decode(value, BEncodingSettings.DefaultEncoding);
        }

        public static bool TryDecode(Stream value, out BEncodedDictionary bencodedDictionary)
        {
            try
            {
                bencodedDictionary = Decode(value);
            }
            catch (Exception e)
            {
                bencodedDictionary = null;
                return false;
            }
            return true;
        }

        public static BEncodedDictionary Decode(Stream value, Encoding encoding)
        {
            if (encoding == null) throw new ArgumentNullException("encoding");
            return Decode(new BinaryReader(value, BEncodingSettings.StreamEncoding), encoding);
        }

        public static bool TryDecode(Stream value, Encoding encoding, out BEncodedDictionary bencodedDictionary)
        {
            try
            {
                bencodedDictionary = Decode(value, encoding);
            }
            catch (Exception e)
            {
                bencodedDictionary = null;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Decodes the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns></returns>
        internal static BEncodedDictionary Decode(BinaryReader reader, Encoding encoding)
        {
            if (encoding == null) throw new ArgumentNullException("encoding");
            if (!reader.BaseStream.CanSeek) throw new NotSupportedException("Cannot decode non-seekable streams");

            // Decoded key
            BEncodedString key;
            // Buffer for key decoding
            BEncodedString keyBuffer;
            // Decoded Value
            IBEncodedValue value;
            // BEncodedDictionary containing KeyValueParis
            BEncodedDictionary dictionary = new BEncodedDictionary(encoding);
            // Preview next character in the stream

            int peek = reader.PeekChar();

            if (peek == -1)
                throw BEncodedFormatDecodeException.CreateTraced("Unexpected end of dictionary", reader.BaseStream);

            char peekChar = (char)peek;

            // Ensure the current stream position represents a BEncodedDictionary
            if (peekChar == BEncodingSettings.DictionaryStart)
            {
                // Seek past the BEncoded.DictionaryStart field
                reader.ReadChar();
                // Preview the next character in the stream, this should represent a constituent IBEncodedValue
                peekChar = (char)reader.PeekChar();
                // Set the key to null before the first key value pair is decoded, necessary for lexographical comparison
                key = null;

                // Decode all constituent IBEncodedValues until the BEncoded.DictionaryEnd field is reached
                while (peekChar != BEncodingSettings.DictionaryEnd)
                {
                    try
                    {
                        // Try to parse a BEncodedString key
                        keyBuffer = BEncodedString.Decode(reader, encoding);
                    }
                    catch (BEncodedFormatDecodeException e)
                    {
                        throw BEncodedFormatDecodeException.CreateTraced("Failed to read dictionary key", e, reader.BaseStream);
                    }
                    
                    // Check whether the key has appeared in lexographical order by comparing keyBuffer with the previous key, if present
                    if (key == null || keyBuffer.Value.CompareTo(key.Value) >= 0)
                    {
                        // keyBuffer has appeared in valid lexical order
                        key = keyBuffer;
                    }
                    else if (BEncodingSettings.ParserMode == BEncodingParserMode.Loose)
                    {
                        // The key, keyBuffer is not in lexographical order, ie. belongs somewhere before the former key
                        key = keyBuffer;
                    }
                    else
                    {
                        throw BEncodedFormatDecodeException.CreateTraced("Dictionary keys must be ordered lexographically in strict mode", reader.BaseStream);
                    }

                    // Preview next character in the stream, this should represent an IBEncodedValue following the key
                    peekChar = (char)reader.PeekChar();

                    // Attempt to decode the value
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
                                // Capture BEncodedStrigns beginning with numeric characters
                                if (Array.IndexOf(BEncodingSettings.NumericMask, peekChar) != -1)
                                {
                                    value = BEncodedString.Decode(reader, (Encoding)encoding.Clone());
                                }
                                else
                                {
                                    // Unrecognised token encountered
                                    throw BEncodedFormatDecodeException.CreateTraced("Unrecognised dictionary element encountered", reader.BaseStream);
                                }
                                break;
                        }

                        // Add key and value to the dictionary

                        if (BEncodingSettings.ParserMode == BEncodingParserMode.Loose)
                        {
                            if(!dictionary.ContainsKey(key))
                                dictionary.Add(key, value);

                            // Discard duplicate key in loose mode
                        }
                        else
                        {
                            dictionary.Add(key, value);
                        }
                    }
                    catch (ArgumentException e)
                    {
                        // Decoding the value has failed
                        throw BEncodedFormatDecodeException.CreateTraced("Failed to decode dictionary value", e, reader.BaseStream);
                    }

                    // Preview next character in the stream
                    peekChar = (char)reader.PeekChar();
                }

                // Stream position is currently at the BEncoded.DictionaryEnd field, seek past the field in order to complete decoding
                reader.ReadChar();
                // Return the decoded dictionary
                return dictionary;
            }
            else
            {
                // The stream does not contain a valid BEncodedDictionary at the current position
                throw BEncodedFormatDecodeException.CreateTraced("Expected dictionary start token", reader.BaseStream);
            }
        }

        /// <summary>
        /// Tries to decode the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="bencodedDictionary">The bencoded dictionary.</param>
        /// <returns></returns>
        internal static bool TryDecode(BinaryReader reader, Encoding encoding, out BEncodedDictionary bencodedDictionary)
        {
            bencodedDictionary = null;

            try
            {
                bencodedDictionary = Decode(reader, encoding);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        #region IBencodedValueWithBinaryEncoder Members

        /// <summary>
        /// Encodes using specified <see>BinaryWriter</see>.
        /// </summary>
        /// <param name="encodingWriter">The encoding writer.</param>
        void IBencodedValueWithBinaryEncoder.Encode(BinaryWriter encodingWriter)
        {
            // Write the BEncoded.DictionaryStart field
            encodingWriter.Write(BEncodingSettings.DictionaryStart);
            // Encode each KeyValuePair
            foreach (KeyValuePair<BEncodedString, IBEncodedValue> item in this)
            {
                ((IBencodedValueWithBinaryEncoder)item.Key).Encode(encodingWriter);
                ((IBencodedValueWithBinaryEncoder)item.Value).Encode(encodingWriter);
            }
            // Write the BEncoded.DictionaryEnd field
            encodingWriter.Write(BEncodingSettings.DictionaryEnd);
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
    }
}
