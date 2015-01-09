using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ph4n.Common;

namespace ph4n.Test.Common
{
    [TestClass]
    public class CharExTests
    {
        [TestMethod]
        public void IsVulgarFraction_true()
        {
            for (long c = 0x00BC; c <= 0x00BE; ++c)
            {
                Assert.IsTrue(((char) c).IsVulgarFraction(), "IsVulgarFractionReturned false for {0}", (char) c);
            }
            for (long c = 0x2150; c <= 0x215E; ++c)
            {
                Assert.IsTrue(((char)c).IsVulgarFraction(), "IsVulgarFractionReturned false for {0}", (char)c);
            }
        }

        [TestMethod]
        public void IsVulgarFraction_false()
        {
            long c = 0x00BC - 1;
            Assert.IsFalse(((char)c).IsVulgarFraction(), "IsVulgarFractionReturned true for {0}", (char)c);

            c = 0x00BE + 1;
            Assert.IsFalse(((char)c).IsVulgarFraction(), "IsVulgarFractionReturned true for {0}", (char)c);

            c = 0x2150 - 1;
            Assert.IsFalse(((char)c).IsVulgarFraction(), "IsVulgarFractionReturned true for {0}", (char)c);

            c = 0x215E + 1;
            Assert.IsFalse(((char)c).IsVulgarFraction(), "IsVulgarFractionReturned true for {0}", (char)c);
        }
    }
}
