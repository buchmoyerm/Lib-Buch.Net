using System;
using Buch.Net.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Buch.Net.Test.Common
{
    [TestClass]
    public class UpdateValTests
    {
        [TestMethod]
        public void UpdateVal_Int_Changed()
        {
            int val = 40;
            int newval = 6;

            var change = GenericUtil.UpdateVal(ref val, newval);
            Assert.AreEqual(newval, val, "value was not updated");
            Assert.IsTrue(change, "No change reported");
        }

        [TestMethod]
        public void UpdateVal_Int_NotChanged()
        {
            int val = 40;
            int newval = 40;

            var change = GenericUtil.UpdateVal(ref val, newval);
            Assert.AreEqual(newval, val, "value incorrectly changed");
            Assert.IsFalse(change, "change incorrectly reported");
        }

        [TestMethod]
        public void UpdateVal_Double_Changed()
        {
            double val = 40.325;
            double newval = 6.98;

            var change = GenericUtil.UpdateVal(ref val, newval);
            Assert.AreEqual(newval, val, "value was not updated");
            Assert.IsTrue(change, "No change reported");
        }

        [TestMethod]
        public void UpdateVal_Double_NotChanged()
        {
            double val = 40.325;
            double newval = 40.325;

            var change = GenericUtil.UpdateVal(ref val, newval);
            Assert.AreEqual(newval, val, "value incorrectly changed");
            Assert.IsFalse(change, "change incorrectly reported");
        }

        [TestMethod]
        public void UpdateVal_String_Changed()
        {
            string val = "original string";
            string newval = "new string value";

            var change = GenericUtil.UpdateVal(ref val, newval);
            Assert.AreEqual(newval, val, "value was not updated");
            Assert.IsTrue(change, "No change reported");
        }

        [TestMethod]
        public void UpdateVal_String_NotChanged()
        {
            string val = "original string";
            string newval = "original string";

            var change = GenericUtil.UpdateVal(ref val, newval);
            Assert.AreEqual(newval, val, "value incorrectly changed");
            Assert.IsFalse(change, "change incorrectly reported");
        }

        [TestMethod]
        public void UpdateVal_Decimal_Changed()
        {
            decimal val = (decimal) 40.0;
            decimal newval = (decimal) 6.230;

            var change = GenericUtil.UpdateVal(ref val, newval);
            Assert.AreEqual(newval, val, "value was not updated");
            Assert.IsTrue(change, "No change reported");
        }

        [TestMethod]
        public void UpdateVal_Decimal_NotChanged()
        {
            decimal val = (decimal)40.0;
            decimal newval = (decimal)40.0;

            var change = GenericUtil.UpdateVal(ref val, newval);
            Assert.AreEqual(newval, val, "value incorrectly changed");
            Assert.IsFalse(change, "change incorrectly reported");
        }
    }
}
