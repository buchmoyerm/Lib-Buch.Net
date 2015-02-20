using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ph4n.Containers;

namespace ph4n.Test.Containers
{
    [TestClass]
    public class LocklessPoolTest
    {
        private int _int;
        public class PoolTestObject : IDisposable
        {
            public PoolTestObject(int i)
            {
                Value = i;
                _disposed = false;
                _disposeEvent = new ManualResetEvent(false);
            }

            public void Dispose()
            {
                _disposed = true;
                _disposeEvent.Set();
                _disposeEvent.Dispose();
                _disposeEvent = null;
            }

            private bool _disposed;
            private ManualResetEvent _disposeEvent;

            public int Value { get; private set; }

            public bool WaitForDispose(TimeSpan ts)
            {
                if (_disposeEvent != null)
                    return _disposeEvent.WaitOne(ts);
                return true;
            }

            public void DisposeCheck()
            {
                if (_disposed)
                    throw new ObjectDisposedException("PoolTestObject");
            }
        }

        public PoolTestObject TestObjFactory()
        {
            return new PoolTestObject(++_int);
        }

        [TestInitialize]
        public void TestInit()
        {
            _int = 0;
        }

        [TestMethod]
        public void Acquire_many_without_blocking_test()
        {
            IPool<PoolTestObject> pool = new LocklessPool<PoolTestObject>(2, TestObjFactory, AccessMode.FIFO);

            var one = pool.Acquire();
            Assert.AreEqual(1, one.Target.Value, "wrong value for one");
            var two = pool.Acquire();
            Assert.AreEqual(2, two.Target.Value, "wrong value for two");
            var three = pool.Acquire();
            Assert.AreEqual(3, three.Target.Value, "wrong value for three");
        }

        [TestMethod]
        public void Acquire_after_dispose_test()
        {
            IPool<PoolTestObject> pool = new LocklessPool<PoolTestObject>(2, TestObjFactory, AccessMode.FIFO);

            var one = pool.Acquire();
            one.Dispose();
            one = null;

            one = pool.Acquire();
            Assert.AreEqual(1, one.Target.Value, "did not get the released item from the pool");
        }

        [TestMethod]
        public void Acquire_lifo_order_test()
        {
            IPool<PoolTestObject> pool = new LocklessPool<PoolTestObject>(3, TestObjFactory, AccessMode.LIFO);

            var one = pool.Acquire();
            var two = pool.Acquire();
            var three = pool.Acquire();

            three.Dispose();
            two.Dispose();
            one.Dispose();

            one = pool.Acquire();
            two = pool.Acquire();
            three = pool.Acquire();

            Assert.AreEqual(1, one.Target.Value, "wrong value in one");
            Assert.AreEqual(2, two.Target.Value, "wrong value for two");
            Assert.AreEqual(3, three.Target.Value, "wrong value for three");
        }

        [TestMethod]
        public void Acquire_fifo_order_test()
        {
            IPool<PoolTestObject> pool = new LocklessPool<PoolTestObject>(3, TestObjFactory, AccessMode.FIFO);

            var one = pool.Acquire();
            var two = pool.Acquire();
            var three = pool.Acquire();

            one.Dispose();
            two.Dispose();
            three.Dispose();
            
            one = pool.Acquire();
            two = pool.Acquire();
            three = pool.Acquire();

            Assert.AreEqual(1, one.Target.Value, "wrong value in one");
            Assert.AreEqual(2, two.Target.Value, "wrong value for two");
            Assert.AreEqual(3, three.Target.Value, "wrong value for three");
        }

        [TestMethod]
        public void using_doesnt_dispose_test()
        {
            IPool<PoolTestObject> pool = new LocklessPool<PoolTestObject>(3, TestObjFactory, AccessMode.FIFO);

            PoolTestObject obj = null;
            using (var one = pool.Acquire())
            {
                obj = one.Target;
            }//one.Dispose() will be called which should release the object back to the pool but not dispose of the target object

            Assert.IsFalse(obj.WaitForDispose(TimeSpan.FromSeconds(.5)), "target object incorrectly disposed");
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void using_dispose_when_pool_is_full_test()
        {
            IPool<PoolTestObject> pool = new LocklessPool<PoolTestObject>(1, TestObjFactory, AccessMode.FIFO);
            var one = pool.Acquire();
            PoolTestObject obj = null;
            using (var two = pool.Acquire())
            {
                one.Dispose(); //will fille the pool to capacity
                obj = two.Target;
            }//two.Dispose() will try to release the object to the pool...which is now full

            obj.DisposeCheck();
        }
    }
}
