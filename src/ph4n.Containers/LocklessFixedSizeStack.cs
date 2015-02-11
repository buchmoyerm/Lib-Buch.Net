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
    public class LocklessFixedSizeStack<T>
    {
        private T[] _array;
        private int _pushTo = -1;
        private int _popFrom = 0;
        private int _capacity;

        public LocklessFixedSizeStack(int capacity)
        {
            Validate.ArgumementGreaterThan(capacity, 0, "capacity");

            _capacity = capacity;
            _array = new T[_capacity];
        }

        public bool TryPush([NotNull] T item)
        {
            Validate.ArgumentNotNull(item,"item");

            //increment when not at the top
            var pushLocation = Interlocked.Increment(ref _pushTo);
            if ( pushLocation >= 0 && pushLocation < _capacity)
            {
                _array[pushLocation] = item;
                Interlocked.Increment(ref _popFrom);
                return true;
            }
            
            //could not push, undo uncrement
            Interlocked.Decrement(ref _pushTo);
            return false;
        }

        [CanBeNull]
        public T TryPop()
        {
            //decrement the popFrom
            var popLocation = Interlocked.Decrement(ref _popFrom);
            if (popLocation < _capacity && popLocation > -1)
            {
                var ret = _array[popLocation];
                _array[popLocation] = default(T); //remove reference to item so it can be garbage collected
                Interlocked.Decrement(ref _pushTo);
                return ret;
            }

            //could not pop, undo decrement
            Interlocked.Increment(ref _popFrom);
            return default(T);
        }
    }
}
