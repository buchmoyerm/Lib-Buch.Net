using System;
using Buch.Net.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Buch.Net.Test.Common
{
    [TestClass]
    public class SwapTests
    {

        [TestMethod]
        public void Swap_TwoInts()
        {
            int origleft, left;
            int origright, right;
            
            origleft = left = 24;
            origright = right = 89;

            GenericUtil.Swap(ref left, ref right);

            Assert.AreEqual(origleft, right, "right hand side value is wrong.");
            Assert.AreEqual(origright, left, "left hand value is wrong");
        }

        [TestMethod]
        public void Swap_Doubles()
        {
            double origleft, left;
            double origright, right;

            origleft = left = 37.864;
            origright = right = 1209.46;

            GenericUtil.Swap(ref left, ref right);

            Assert.AreEqual(origleft, right, "right hand side value is wrong.");
            Assert.AreEqual(origright, left, "left hand value is wrong");
        }

        [TestMethod]
        public void Swap_Strings()
        {
            string origleft, left;
            string origright, right;

            origleft = left = "lhs string";
            origright = right = "rhs string value";

            GenericUtil.Swap(ref left, ref right);

            Assert.AreEqual(origleft, right, "right hand side value is wrong.");
            Assert.AreEqual(origright, left, "left hand value is wrong");
        }

        [TestMethod]
        public void Swap_Decimals()
        {
            decimal origleft, left;
            decimal origright, right;

            origleft = left = (decimal) 45.678;
            origright = right = (decimal) 3.876;

            GenericUtil.Swap(ref left, ref right);

            Assert.AreEqual(origleft, right, "right hand side value is wrong.");
            Assert.AreEqual(origright, left, "left hand value is wrong");
        }
    }
}
