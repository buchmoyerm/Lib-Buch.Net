using System;
using JetBrains.Annotations;

namespace ph4n.Containers
{
    public interface IPooledItem<out T> : IDisposable
    {
        T Target { get; }
    }

    public interface IPool<T>
    {
        bool IsDisposed { get; }
        IPooledItem<T> Acquire();
        void Release(IPooledItem<T> item);
    }
}