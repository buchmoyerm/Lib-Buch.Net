using System;
using System.Threading;
using JetBrains.Annotations;

namespace ph4n.Containers
{
    public class LocklessQueue<T>
    {
        private LlNode<T> _enqueueNode;
        private LlNode<T> _dequeueNode;
        private int _nodeCount;

        public LocklessQueue()
        {
            _nodeCount = 0;
            _enqueueNode = _dequeueNode = new LlNode<T>(default(T));
        }

        public int Count { get { return _nodeCount; } }

        // LocklessQueue is a linked list where items are Enqued at the tail and dequeued at the head
        //
        //    -------------              -------------              -------------
        //   |             |            |             |            |             |
        //   |             |   link     |             |   link     |             |   link
        //   |   dequeue   | -------->  |   item2     | -------->  |   enqueue   | -------> null
        //   |             |            |             |            |             |
        //   |             |            |             |            |             |
        //    -------------              -------------              -------------
        // The dequeued (head) node has 
        // already been removed from the queue
        // A call to Dequeue() will return item2.value
        // and advance the dequeue node to point to item2 node
        // when dequeue and enqueue are pointing to the
        // same node then we know that it has already been
        // dequeued and the queue is empty
        // Enqueue(T value) will then advance enqueue so that
        // Dequeue() has a new value to return

        public void Enqueue(T value)
        {
            LlNode<T> oldEnqueNode = null;
            LlNode<T> oldEqueueNext = null;

            var node = new LlNode<T>(value);
            bool updatedLink = false;
            while (!updatedLink)
            {
                //get local copies
                oldEnqueNode = _enqueueNode;
                oldEqueueNext = oldEnqueNode.Next; //get from local copy in case another thread changed _enqueueNode

                //has _enqueueNode changed?
                if (_enqueueNode == oldEnqueNode)
                {
                    if (oldEqueueNext == null)
                    {
                        updatedLink = SyncMethods.CAS(ref _enqueueNode.Next, null, node);
                    }
                    else
                    {
                        //another thread is updating so try to advance
                        SyncMethods.CAS(ref _enqueueNode, oldEnqueNode, oldEqueueNext);
                    }
                }
            }

            Interlocked.Increment(ref _nodeCount);

            //try updating the enqueue field to point to the node being added
            //if we can't update thats because another thread is also enqueue at will succeed
            SyncMethods.CAS(ref _enqueueNode, oldEnqueNode, node);
        }

        public T Dequeue()
        {
            T result = default(T);

            bool haveAdvancedDequeue = false;
            while (!haveAdvancedDequeue)
            {
                //make local copies of Enqueue, Dequeue, and Dequeue next
                var oldDequeue = _dequeueNode;
                var oldEnqueue = _enqueueNode;
                var oldDequeueNext = oldDequeue.Next; //get from local copy in case _dequeueNode changed

                //has the field changed
                if (oldDequeue == _dequeueNode)
                {
                    if (oldDequeue == oldEnqueue)
                    {
                        if (oldDequeueNext == null)
                        {
                            return default(T);
                        }

                        //if dequeue's next field is non-null and head and dequeue == enqueue
                        //then we have a lagging tail: try to update it
                        SyncMethods.CAS(ref _enqueueNode, oldEnqueue, oldDequeueNext);
                    }
                    else
                    {
                        //grab item to dequeue and try to advance dequeue
                        result = oldDequeueNext.Value;
                        haveAdvancedDequeue = SyncMethods.CAS(ref _dequeueNode, oldDequeue, oldDequeueNext);
                    }
                }
            }
            Interlocked.Decrement(ref _nodeCount);
            return result;
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

        public static class SyncMethods
        {
            //Compare and swap
            public static bool CAS<T>(ref T location, T comparand, T newValue) where T : class
            {
                return
                    (object)comparand ==
                    (object)Interlocked.CompareExchange<T>(ref location, newValue, comparand);
            }
        }
    }
}
