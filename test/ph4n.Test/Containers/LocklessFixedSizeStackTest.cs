using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ph4n.Containers;

namespace ph4n.Test.Containers
{
    [TestClass]
    public class LocklessFixedSizeStackTest
    {
        class Foo
        {
            public string TestStr { get; set; }
            public int TestInt { get; set; }
        }

        [TestMethod]
        public void valid_ctor_test()
        {
            var stack = new LocklessFixedSizeStack<Foo>(10);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void invalid_ctor_test()
        {
            var stack = new LocklessFixedSizeStack<Foo>(0);
        }

        [TestMethod]
        public void TryPush_onto_empty_stack()
        {
            var stack = new LocklessFixedSizeStack<Foo>(2);
            Assert.IsTrue(stack.TryPush(new Foo()), "Indicates could not push");
        }

        [TestMethod]
        public void TryPush_onto_full_stack()
        {
            var stack = new LocklessFixedSizeStack<Foo>(1);
            Assert.IsTrue(stack.TryPush(new Foo()), "Could not push first item");
            Assert.IsFalse(stack.TryPush(new Foo()), "Should not be able to push past capacity");
        }

        [TestMethod]
        public void TryPop_from_empty_stack()
        {
            var stack = new LocklessFixedSizeStack<Foo>(2);
            Assert.IsNull(stack.TryPop(), "Did not pop null");
        }

        [TestMethod]
        public void TryPop_first_item()
        {
            var f = new Foo() {TestStr = "test"};
            var stack = new LocklessFixedSizeStack<Foo>(2);
            stack.TryPush(f);
            Assert.AreEqual(f, stack.TryPop(), "Popped item is not the same as pushed");
        }

        [TestMethod]
        public void TryPush_replaces_popped_item()
        {
            var first = new Foo() {TestStr = "first"};
            var second = new Foo() {TestStr = "second"};
            var stack = new LocklessFixedSizeStack<Foo>(2);

            stack.TryPush(first);
            stack.TryPop();
            stack.TryPush(second);
            Assert.AreSame(second, stack.TryPop(), "Incorrect item popped");
        }
    }
}
