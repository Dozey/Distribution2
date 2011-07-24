using System;
using System.Collections.Generic;
using System.Text;

namespace Distribution2.BitTorrent.BEncoding
{
    public class BEncodedDictionaryBase : SortedDictionary<BEncodedString, IBEncodedValue>
    {
        public BEncodedDictionaryBase() : base(new BEncodedStringComparer()) { }
        public BEncodedDictionaryBase(IDictionary<BEncodedString, IBEncodedValue> dictionary) : base(dictionary) { }

        public void Add(string key, IBEncodedValue value)
        {
            Add(new BEncodedString(key), value);
        }

        public new void Add(BEncodedString key, IBEncodedValue value)
        {
            // SortedDictionary<TKey, TValue> accepts null values, null references are unacceptable for BEncodedDictionaries
            if (value == null) throw new ArgumentNullException("value");
            base.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return base.ContainsKey(new BEncodedString(key));
        }

        public new IBEncodedValue this[BEncodedString key]
        {
            get
            {
                return base[key];
            }
            set
            {
                // SortedDictionary accepts null values, null references are unacceptable for BEncodedDictionaries
                if (value == null) throw new NullReferenceException("value cannot be null");
                base[key] = value;
            }
        }

        public IBEncodedValue this[string key]
        {
            get
            {
                return base[new BEncodedString(key)];
            }
            set
            {
                // SortedDictionary accepts null values, null references are unacceptable for BEncodedDictionaries
                if (value == null) throw new NullReferenceException("value cannot be null");
                base[new BEncodedString(key)] = value;
            }
        }
    }
}
