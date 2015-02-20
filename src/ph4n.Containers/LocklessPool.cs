using System;
using System.Threading;
using JetBrains.Annotations;
using ph4n.Common;

namespace ph4n.Containers
{
    /// <summary>
    /// Creates a object pool that is lockless and non blocking
    /// There is no limit to the number of items created by the pool
    /// If the pool is empty; new items will continue to be created
    /// As created items are returned to the pool they will be stored for use later
    /// No more than capacity number of items will be stored
    /// If the pool is fill to capacity and an item is return it will be disposed
    /// This pool can be usefull when you want to reuse objects (and reduces the amount of memory allocations used)
    /// but don't want the stop a thread from operating just because all of your current objects are in use
    /// instead the pool will create a new object and still recycle old ones as they become available
    /// </summary>
    /// <typeparam name="T">The type of item</typeparam>
    public class LocklessPool<T> : IPool<T>, IDisposable
    {
        private bool _disposed;
        private Func<T> _factory;
        private IItemStore<PooledItem<T>> _itemStore;
        private int _capacity;
        private int _currentPoolSize;

        public LocklessPool(int capacity, [NotNull] Func<T> factory, AccessMode mode)
        {
            Validate.ArgumentNotNull(factory, "factory");
            Validate.ArgumementGreaterThan(capacity, 0, "capacity");

            _disposed = false;
            _currentPoolSize = 0;
            _capacity = capacity;
            _factory = factory;

            _itemStore = CreateItemStore(mode);

            if (_itemStore == null)
            {
                throw new ArgumentException("Unknown access mode", "mode");
            }
        }

        private IItemStore<PooledItem<T>> CreateItemStore(AccessMode mode)
        {
            switch (mode)
            {
                case AccessMode.FIFO:
                    return new LocklessQueueStore<PooledItem<T>>();
                case AccessMode.LIFO:
                    return new LocklessFixedSizeStackStore<PooledItem<T>>(_capacity);
            }
            return null;
        }

        public bool IsDisposed { get; private set; }
        
        [NotNull]
        public IPooledItem<T> Acquire()
        {
            var nextAvailableItem = _itemStore.Fetch();
            if (nextAvailableItem == null)
            {
                //no available items in the pool
                //return a new item
                return new PooledItem<T>(this, _factory());
            }
            return nextAvailableItem;
        }

        public void Release([NotNull] IPooledItem<T> item)
        {
            Validate.ArgumentNotNull(item, "item");

            if (IsDisposed)
            {
                //tell the object it is being removed from the pool and exit
                ((PooledItem<T>)item).RemovedFromPool();
                return;
            }

            var size = Interlocked.Increment(ref _currentPoolSize);
            if (size <= _capacity)
            {
                //add to pool for use later
                _itemStore.Store((PooledItem<T>)item);
            }
            else
            {
                //size has been reach so remove the item from the pool
                Interlocked.Decrement(ref _currentPoolSize);
                ( (PooledItem<T>) item).RemovedFromPool();
            }
        }

        public void Dispose()
        {
            if (IsDisposed) return;

            _disposed = true;

            while (_itemStore.Count > 0)
            {
                _itemStore.Fetch().RemovedFromPool();
            }

            Interlocked.Exchange(ref _currentPoolSize, 0);
        }

        #region IItemStore
        private interface IItemStore<TStore>
        {
            TStore Fetch();
            void Store(TStore item);
            int Count { get; }
        }

        private class LocklessQueueStore<TStore> : IItemStore<TStore>
        {
            private LocklessQueue<TStore> _queue = new LocklessQueue<TStore>();

            public TStore Fetch()
            {
                return _queue.Dequeue();
            }

            public void Store(TStore item)
            {
                _queue.Enqueue(item);
            }

            public int Count { get { return _queue.Count; } }
        }

        private class LocklessFixedSizeStackStore<TStore> : IItemStore<TStore>
        {
            private LocklessFixedSizeStack<TStore> _stack;

            public LocklessFixedSizeStackStore(int capacity)
            {
                _stack = new LocklessFixedSizeStack<TStore>(capacity);
            }

            public TStore Fetch()
            {
                return _stack.TryPop();
            }

            public void Store(TStore item)
            {
                _stack.TryPush(item);
            }

            public int Count { get { return _stack.Count; } }
        }
        #endregion IItemStore

        #region PooledItem
        private class PooledItem<TItem> : IPooledItem<TItem>
        {
            internal PooledItem(IPool<TItem> pool, TItem item)
            {
                Pool = pool;
                Target = item;
            }

            internal void RemovedFromPool()
            {
                if (typeof(IDisposable).IsAssignableFrom(typeof(T)))
                {
                    ((IDisposable)Target).Dispose();
                }
            }

            public void Dispose()
            {
                if (Pool.IsDisposed)
                {
                    RemovedFromPool();
                }
                else
                {
                    Pool.Release(this);
                }
            }

            public TItem Target { get; private set; }
            private IPool<TItem> Pool { get; set; }
        }
        #endregion PooledItem
    }
}
