using System.Threading;
using JetBrains.Annotations;

namespace ph4n.Containers
{
    public class LocklessQueue<T>
    {
        private LlNode<T> _enqueueNode;
        private LlNode<T> _dequeNode;
        private int _nodeCount;

        public LocklessQueue()
        {
            _nodeCount = 0;
            _enqueueNode = null;
            _dequeNode = null;
        }

        public int Count { get { return _nodeCount; } }

        public void Enqueue(T value)
        {
            var newNode = new LlNode<T>(value);

            //create new enqueue location
            var originalInsertLocation = Interlocked.Exchange(ref _enqueueNode, newNode);
            var origCount = Interlocked.Increment(ref _nodeCount);

            if (origCount == 0 || originalInsertLocation == null)//first item added to empty queue
            {
                Interlocked.CompareExchange(ref _dequeNode, _enqueueNode, null); //prepare dequeue if dequeue is null
            }
            else
            {
                //create link
                originalInsertLocation.Next = newNode;
            }
        }

        public T Dequeue()
        {
            var retNode = Interlocked.Exchange(ref _dequeNode, GetNext(_dequeNode));
            if (retNode != null)
            {
                Interlocked.Decrement(ref _nodeCount);
                //if returning the the only node set the deque node to null
                Interlocked.CompareExchange(ref _enqueueNode, null, retNode);
                return retNode.Value;
            }
            return default(T);
        }

        [CanBeNull]
        private static LlNode<T> GetNext([CanBeNull] LlNode<T> node)
        {
            if (node != null)
            {
                return node.Next;
            }
            return null;
        }
    }
}
