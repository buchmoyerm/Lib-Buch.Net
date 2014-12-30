using System;
using Buch.Net.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Buch.Net.Test.Common
{
    [TestClass]
    public class UpdateValTests
    {
        [TestMethod]
        public void SwapInts()
        {
            int val = 40;
            int newval = 6;

            var change = GenericUtil.UpdateVal(ref val, newval);
            Assert.AreEqual(newval, val, "value was not updated");
            Assert.IsTrue(change, "No change reported");

            change = GenericUtil.UpdateVal(ref val, newval);
            Assert.AreEqual(newval, val, "value incorrectly changed");
            Assert.IsFalse(change, "change incorrectly reported");
        }

        [TestMethod]
        public void SwapDouble()
        {
            double val = 40.325;
            double newval = 6.98;

            var change = GenericUtil.UpdateVal(ref val, newval);
            Assert.AreEqual(newval, val, "value was not updated");
            Assert.IsTrue(change, "No change reported");

            change = GenericUtil.UpdateVal(ref val, newval);
            Assert.AreEqual(newval, val, "value incorrectly changed");
            Assert.IsFalse(change, "change incorrectly reported");
        }

        [TestMethod]
        public void SwapString()
        {
            string val = "original string";
            string newval = "new string value";

            var change = GenericUtil.UpdateVal(ref val, newval);
            Assert.AreEqual(newval, val, "value was not updated");
            Assert.IsTrue(change, "No change reported");

            change = GenericUtil.UpdateVal(ref val, newval);
            Assert.AreEqual(newval, val, "value incorrectly changed");
            Assert.IsFalse(change, "change incorrectly reported");
        }

        [TestMethod]
        public void SwapDecimal()
        {
            decimal val = (decimal) 40.0;
            decimal newval = (decimal) 6.230;

            var change = GenericUtil.UpdateVal(ref val, newval);
            Assert.AreEqual(newval, val, "value was not updated");
            Assert.IsTrue(change, "No change reported");

            change = GenericUtil.UpdateVal(ref val, newval);
            Assert.AreEqual(newval, val, "value incorrectly changed");
            Assert.IsFalse(change, "change incorrectly reported");
        }
    }
}
