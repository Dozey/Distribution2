using System;
using System.Collections.Generic;
using System.Text;

namespace Distribution2.BitTorrent.BEncoding
{
    public class BEncodedListBase : List<IBEncodedValue>
    {
        public BEncodedListBase() : base() { }
        public BEncodedListBase(int capacity) : base(capacity) { }
        public BEncodedListBase(IEnumerable<IBEncodedValue> collection) : base(collection) { }

        public new void Add(IBEncodedValue item)
        {
            // List<T> accepts null values, null references are unacceptable for BEncodedDictionaries
            if (item == null) throw new ArgumentNullException("item");
            base.Add(item);
        }

        public new void AddRange(IEnumerable<IBEncodedValue> collection)
        {
            foreach (IBEncodedValue item in collection)
            {
                // List<T> accepts null values, null references are unacceptable for BEncodedDictionaries
                if (item == null) throw new NullReferenceException("collection contains a null item");
            }
            base.AddRange(collection);
        }

        public new IBEncodedValue this[int index]
        {
            get { return base[index]; }
            set
            {
                // List<T> accepts null values, null references are unacceptable for BEncodedDictionaries
                if (value == null) throw new NullReferenceException("item cannot be null");
                base[index] = value;
            }
        }
    }
}
