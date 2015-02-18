using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ph4n.Containers;

namespace ph4n.Test.Containers
{
    [TestClass]
    public class BlockingPoolTest
    {
        private int _int;
        public int IntFactory()
        {
            return ++_int;
        }

        [TestInitialize]
        public void TestInit()
        {
            _int = 0;
        }

        [TestMethod]
        public void Aquire_eager_fifo_test()
        {
            //will fill all 10 values right away
            IPool<int> pool = new BlockingPool<int>(10, IntFactory, LoadingMode.Eager, AccessMode.FIFO);

            for (int expectedVal = 1; expectedVal <= 10; ++expectedVal)
            {
                using (var p = pool.Acquire())
                {
                    Assert.AreEqual(expectedVal, p.Target, "values aquired in wrong order");
                }
            }
        }

        [TestMethod]
        public void Aquire_lazy_fifo_test()
        {
            //does not fill any values initially
            IPool<int> pool = new BlockingPool<int>(10, IntFactory, LoadingMode.Lazy, AccessMode.FIFO);

            for (int acquireNum = 1; acquireNum <= 10; ++acquireNum)
            {
                //lazily grabs first value and returns it before grabbing the second
                //So all acquired targets are 1
                using (var p = pool.Acquire())
                {
                    Assert.AreEqual(1, p.Target, "values aquired in wrong order");
                }
            }
        }

        [TestMethod]
        public void Aquire_lazyexpanding_fifo_test()
        {
            //does not fill any values initially
            IPool<int> pool = new BlockingPool<int>(10, IntFactory, LoadingMode.LazyExpanding, AccessMode.FIFO);

            for (int expectedVal = 1; expectedVal <= 10; ++expectedVal)
            {
                //lazily grabs all values until list is full before recycling values
                using (var p = pool.Acquire())
                {
                    Assert.AreEqual(expectedVal, p.Target, "values aquired in wrong order");
                }
            }
        }

        [TestMethod]
        public void Aquire_eager_lifo_test()
        {
            //will fill all 10 values right away
            IPool<int> pool = new BlockingPool<int>(10, IntFactory, LoadingMode.Eager, AccessMode.LIFO);

            for (int acquireNum = 1; acquireNum <= 10; ++acquireNum)
            {
                //keep reusing the last value in
                using (var p = pool.Acquire())
                {
                    Assert.AreEqual(10, p.Target, "values aquired in wrong order");
                }
            }
        }

        [TestMethod]
        public void Aquire_lazy_lifo_test()
        {
            //does not fill any values initially
            IPool<int> pool = new BlockingPool<int>(10, IntFactory, LoadingMode.Lazy, AccessMode.LIFO);

            for (int acquireNum = 1; acquireNum <= 10; ++acquireNum)
            {
                //keep reusing the first (and last) value in
                using (var p = pool.Acquire())
                {
                    Assert.AreEqual(1, p.Target, "values aquired in wrong order");
                }
            }
        }

        [TestMethod]
        public void Aquire_lazyexpanding_lifo_test()
        {
            //does not fill any values initially
            IPool<int> pool = new BlockingPool<int>(10, IntFactory, LoadingMode.LazyExpanding, AccessMode.LIFO);

            for (int expectedVal = 1; expectedVal <= 10; ++expectedVal)
            {
                //lazily grabs all values until list is full before recycling values
                using (var p = pool.Acquire())
                {
                    Assert.AreEqual(expectedVal, p.Target, "values aquired in wrong order");
                }
            }
        }

        [TestMethod]
        public void Aquire_full_blocks_test()
        {
            IPool<int> pool = new BlockingPool<int>(1, IntFactory);

            var first = pool.Acquire();
            Assert.AreEqual(1, first.Target, "first value is wrong");
            //list as it capacity

            IPooledItem<int> test = null;
            var continued = new ManualResetEvent(false);
            var t = Task.Factory.StartNew(() => test = pool.Acquire()).ContinueWith(state => continued.Set());
            //acquire should block and ContinueWith will not execute

            Assert.IsFalse(continued.WaitOne(TimeSpan.FromSeconds(.5)), "pool did not block");
            Assert.IsNull(test, "test item is not null");
        }

        [TestMethod]
        public void Aquire_unblocks_test()
        {
            IPool<int> pool = new BlockingPool<int>(1, IntFactory);

            var first = pool.Acquire();
            Assert.AreEqual(1, first.Target, "first value is wrong");
            //pool is at capacity now so acquire should block

            IPooledItem<int> test = null;
            var continued = new ManualResetEvent(false);
            var t = Task.Factory.StartNew(() => test = pool.Acquire()).ContinueWith(state => continued.Set());

            //disposing a pooled item returns it to the list
            first.Dispose();

            Assert.IsTrue(continued.WaitOne(TimeSpan.FromSeconds(.5)), "blocked aquire did not unblock");
            Assert.AreEqual(1, test.Target, "wrong value returned to the pool");
        }
    }
}
