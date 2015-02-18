using System;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ph4n.Containers;

namespace ph4n.Test.Containers
{
    [TestClass()]
    public class BlockingQueueTests
    {
        private class Foo
        {
            public string TestStr { get; set; }
        }

        [TestMethod()]
        public void Enqueue_into_empty_queue()
        {
            var queue = new BlockingQueue<Foo>();
            queue.Enqueue(new Foo());
        }

        [TestMethod()]
        public void Dequeue_test()
        {
            var queue = new BlockingQueue<Foo>();
            var foo = new Foo() {TestStr = "my test"};
            queue.Enqueue(foo);
            Assert.AreSame(foo, queue.Dequeue(), "Did not dequeue the correct item");
        }

        [TestMethod()]
        public void Dequeue_empty_should_block_test()
        {
            var queue = new BlockingQueue<Foo>();
            Foo dequeuedValue = null;
            var dequeuecompleted = new ManualResetEvent(false);

            var t = Task.Factory.StartNew(() => dequeuedValue = queue.Dequeue()).ContinueWith(result => dequeuecompleted.Set());

            Assert.IsFalse(dequeuecompleted.WaitOne(TimeSpan.FromSeconds(1)), "Dequeue task completed");
            Assert.IsNull(dequeuedValue, "Dequeued value is set");
        }

        [TestMethod()]
        public void Dequeue_stops_blocking_on_enqueue()
        {
            var queue = new BlockingQueue<Foo>();
            Foo dequeuedValue = null;
            var dequeuecompleted = new ManualResetEvent(false);
            var t = Task.Factory.StartNew(() => dequeuedValue = queue.Dequeue()).ContinueWith(result => dequeuecompleted.Set());
            Assert.IsFalse(dequeuecompleted.WaitOne(TimeSpan.FromSeconds(1)), "Dequeue task completed");
            Assert.IsNull(dequeuedValue, "Dequeued value is set");

            Foo enqueuedValue = new Foo() {TestStr = "enqueued"};
            queue.Enqueue(enqueuedValue);

            Assert.IsTrue(dequeuecompleted.WaitOne(TimeSpan.FromMilliseconds(5)), "Dequeue task did not continue");
            Assert.AreSame(enqueuedValue, dequeuedValue, "Incorrect value was dequeued");
        }

        [TestMethod()]
        public void Clear_Test()
        {
            var queue = new BlockingQueue<Foo>();
            Foo dequeuedValue = null;
            Foo enqueuedValue = new Foo() {TestStr = "test"};
            var complete = new ManualResetEvent(false);

            queue.Enqueue(enqueuedValue);
            queue.Clear();

            var t = Task.Factory.StartNew(() => dequeuedValue = queue.Dequeue()).ContinueWith(result => complete.Set());
            Assert.IsFalse(complete.WaitOne(TimeSpan.FromSeconds(1)), "Dequeue task completed");
            Assert.IsNull(dequeuedValue, "Dequeued value is set");
        }
    }
}
