using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distribution2.BitTorrent.Tracker
{
    public class MultipleException<T> : Exception where T : Exception
    {
        public MultipleException(string message, IEnumerable<T> exceptions)
            : base(message)
        {
            Exceptions = exceptions.ToArray();
        }

        public T[] Exceptions { get; private set; }
    }
}
