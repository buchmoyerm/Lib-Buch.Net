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

        public bool TryPush([NotNull] T item)
        {
            Validate.ArgumentNotNull(item,"item");

            //increment when not at the top
            var pushLocation = Interlocked.CompareExchange(ref _top, _top + 1, _capacity);
            if ( pushLocation >= 0 && pushLocation < _capacity)
            {
                _array[pushLocation] = item;
                return true;
            }

            return false;
        }

        [CanBeNull]
        public T TryPop()
        {
            //decrement when not at the bottom
            var popLocation = Interlocked.CompareExchange(ref _top, _top - 1, -1);
            if (popLocation < _capacity && popLocation > -1)
            {
                return _array[popLocation];
            }

            return default(T);
        }
    }
}
