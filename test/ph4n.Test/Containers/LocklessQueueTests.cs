using Microsoft.VisualStudio.TestTools.UnitTesting;
using ph4n.Containers;

namespace ph4n.Test.Containers
{
    [TestClass()]
    public class LocklessQueueTests
    {
        [TestMethod()]
        public void Enqueue_to_empty_queue_test()
        {
            var queue = new LocklessQueue<string>();
            queue.Enqueue("hello world");
        }

        [TestMethod()]
        public void Dequeue_from_empty_queue_test()
        {
            var queue = new LocklessQueue<string>();
            Assert.IsNull(queue.Dequeue(), "Empty queue returned something other than null");
        }

        [TestMethod]
        public void Count_after_enqueu_test()
        {
            var queue = new LocklessQueue<int>();
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);

            Assert.AreEqual(3, queue.Count, "queue has wrong count");
        }

        [TestMethod]
        public void Count_after_enqueu_and_dequeue_test()
        {
            var queue = new LocklessQueue<int>();
            queue.Enqueue(1);
            queue.Enqueue(2);
            Assert.AreEqual(2, queue.Count, "queue has wrong count after enqueu");
            queue.Dequeue();
            Assert.AreEqual(1, queue.Count, "queue has wrong count after dequeue");

            queue.Enqueue(3);
            queue.Enqueue(4);
            queue.Dequeue();
            queue.Enqueue(5);
            queue.Dequeue();
            queue.Dequeue();

            Assert.AreEqual(1, queue.Count, "queue has wrong count after multiple enqueu and dequeue");
            Assert.AreEqual(5, queue.Dequeue(), "queue dequeued the wrong number");
            Assert.AreEqual(0, queue.Count, "queue should show zero count");
        }


        [TestMethod]
        public void Dequeue_correct_order_test()
        {
            var queue = new LocklessQueue<int>();
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);

            Assert.AreEqual(1, queue.Dequeue(), "First dequeue incorrect");
            Assert.AreEqual(2, queue.Dequeue(), "Second dequeue incorrect");
            Assert.AreEqual(3, queue.Dequeue(), "Third dequeue incorrect");
            Assert.AreEqual(default(int), queue.Dequeue(), "queue dequeued too many items");
        }

        [TestMethod]
        public void Enqueue_resets_after_empty()
        {
            var queue = new LocklessQueue<int>();
            queue.Enqueue(1);
            queue.Enqueue(2);
            Assert.AreEqual(1, queue.Dequeue(), "First dequeue incorrect");
            Assert.AreEqual(2, queue.Dequeue(), "Second dequeue incorrect");
            Assert.AreEqual(default(int), queue.Dequeue(), "queue dequeued too many items");

            queue.Enqueue(3);
            Assert.AreEqual(3, queue.Dequeue(), "Enqueue didn't reset after empty queue");
        } 
    }
}
