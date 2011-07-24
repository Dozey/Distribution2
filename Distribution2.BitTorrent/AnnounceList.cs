using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Distribution2.BitTorrent.BEncoding;

namespace Distribution2.BitTorrent
{
    public class AnnounceList : IList<IAnnounceEntry>, IAnnounceEntry
    {
        private BEncodedList _container;

        internal AnnounceList(BEncodedList container)
        {
            _container = container;
        }

        public AnnounceList(IAnnounceEntry[] items)
        {
            _container = new BEncodedList();

            foreach (IAnnounceEntry item in items)
                Add(item);
        }

        public AnnounceUri[] AllTiers
        {
            get
            {
                Stack<BEncodedList> remaining = new Stack<BEncodedList>();
                List<AnnounceUri> visited = new List<AnnounceUri>();

                remaining.Push(_container);

                while (remaining.Count > 0)
                {
                    BEncodedList currentNode = remaining.Pop();
                    foreach (IBEncodedValue node in currentNode)
                    {
                        if (node is BEncodedList)
                            remaining.Push((BEncodedList)node);
                        else if (node is BEncodedString)
                            visited.Add(new AnnounceUri((BEncodedString)node));
                        else
                            throw new Exception("Unsupported element");
                    }
                }

                return visited.ToArray();
            }
        }

        public void AddTier(AnnounceUri announceUri)
        {
            Add(new AnnounceList(new IAnnounceEntry[] { announceUri }));
        }

        #region IList<IAnnounceEntry> Members

        public int IndexOf(IAnnounceEntry item)
        {
            return _container.IndexOf(item.Container);
        }

        public void Insert(int index, IAnnounceEntry item)
        {
            _container.Insert(index, item.Container);
        }

        public void RemoveAt(int index)
        {
            _container.RemoveAt(index);
        }

        public IAnnounceEntry this[int index]
        {
            get { return ToAnnounceEntry(_container[index]); }
            set { _container[index] = value.Container; }
        }

        #endregion

        #region ICollection<IAnnounceEntry> Members

        public void Add(IAnnounceEntry item)
        {
            _container.Add(item.Container);
        }

        public void Clear()
        {
            _container.Clear();
        }

        public bool Contains(IAnnounceEntry item)
        {
            return _container.Contains(item.Container);
        }

        public void CopyTo(IAnnounceEntry[] array, int arrayIndex)
        {
            if (array.Length >= (_container.Count - arrayIndex))
            {
                for (int i = arrayIndex, j = 0; i < Count - 1; i++, j++)
                    array[j] = ToAnnounceEntry(_container[i]);
            }
        }

        public int Count
        {
            get { return _container.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(IAnnounceEntry item)
        {
            return _container.Remove(item.Container);
        }

        #endregion

        #region IEnumerable<IAnnounceEntry> Members

        public IEnumerator<IAnnounceEntry> GetEnumerator()
        {
            foreach (IBEncodedValue value in _container)
                yield return ToAnnounceEntry(value);
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IAnnounceEntry Members

        public IBEncodedValue Container
        {
            get { return _container; }
        }

        public bool IsTier
        {
            get { return true; }
        }

        public object GetValue()
        {
            return this.ToArray();
        }

        #endregion

        private static IAnnounceEntry ToAnnounceEntry(IBEncodedValue bencodedValue)
        {
            if (bencodedValue is BEncodedString)
            {
                return new AnnounceUri((BEncodedString)bencodedValue);
            }
            else if (bencodedValue is BEncodedList)
            {
                return new AnnounceList((BEncodedList)bencodedValue);
            }
            else
            {
                throw new Exception(String.Format("Unsupported type {0}", bencodedValue.GetType().Name));
            }
        }
    }
}
