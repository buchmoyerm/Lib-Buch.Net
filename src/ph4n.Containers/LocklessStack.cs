using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using ph4n.Common;

namespace ph4n.Containers
{
    public class LocklessStack<T>
    {
        private T[] _array;
        private int _top = -1;
        private int _capacity;

        public LocklessStack(int capacity)
        {
            Validate.ArgumementGreaterThan(capacity, 0, "capacity");

            _capacity = capacity;
            _array = new T[_capacity];
        }

        public bool Push([NotNull] T item)
        {
            Validate.ArgumentNotNull(item,"item");

            var pushLocation = Interlocked.Increment(ref _top);

            //valid location
            if (pushLocation < _capacity)
            {
                _array[pushLocation] = item;
                return true;
            }

            //invalid location
            Interlocked.Decrement(ref _top);
            return false;
        }

        [CanBeNull]
        public T Pop()
        {
            var popLocation = Interlocked.Decrement(ref _top) + 1;

            //valid location
            if (popLocation >= 0)
            {
                return _array[popLocation];
            }

            //invalid location
            Interlocked.Increment(ref _top);
            return default(T);
        }
    }
}
