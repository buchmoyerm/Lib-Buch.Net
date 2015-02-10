using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ph4n.Containers;

namespace ph4n.Test.Containers
{
    [TestClass]
    public class LocklessStackTest
    {
        class Foo
        {
            public string TestStr { get; set; }
            public int TestInt { get; set; }
        }

        [TestMethod]
        public void valid_ctor_test()
        {
            var stack = new LocklessStack<Foo>(10);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void invalid_ctor_test()
        {
            var stack = new LocklessStack<Foo>(0);
        }

        [TestMethod]
        public void Push_onto_empty_stack()
        {
            var stack = new LocklessStack<Foo>(2);
            Assert.IsTrue(stack.Push(new Foo()), "Indicates could not push");
        }

        [TestMethod]
        public void Push_onto_full_stack()
        {
            var stack = new LocklessStack<Foo>(1);
            Assert.IsTrue(stack.Push(new Foo()), "Could not push first item");
            Assert.IsFalse(stack.Push(new Foo()), "Should not be able to push past capacity");
        }

        [TestMethod]
        public void Pop_from_empty_stack()
        {
            var stack = new LocklessStack<Foo>(2);
            Assert.IsNull(stack.Pop(), "Did not pop null");
        }

        [TestMethod]
        public void Pop_first_item()
        {
            var f = new Foo() {TestStr = "test"};
            var stack = new LocklessStack<Foo>(2);
            stack.Push(f);
            Assert.AreEqual(f, stack.Pop(), "Popped item is not the same as pushed");
        }

        [TestMethod]
        public void Push_replaces_popped_item()
        {
            var first = new Foo() {TestStr = "first"};
            var second = new Foo() {TestStr = "second"};
            var stack = new LocklessStack<Foo>(2);

            stack.Push(first);
            stack.Pop();
            stack.Push(second);
            Assert.AreSame(second, stack.Pop(), "Incorrect item popped");
        }
    }
}
