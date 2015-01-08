using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ph4n.Common;

namespace ph4n.Test.Common
{
    [TestClass]
    public class StringExTests
    {
        [TestMethod]
        public void SafeSubstring_begining()
        {
            var str = "this is a normal string";
            var substr = "this";
            Assert.AreEqual(substr, str.SafeSubstring(0,4), "Did not return correct substring");
        }

        [TestMethod]
        public void SafeSubstring_middle()
        {
            var str = "01234string5678";
            var substr = "string";
            Assert.AreEqual(substr, str.SafeSubstring(5, 6), "Did not return the correct substring");
        }

        [TestMethod]
        public void SafeSubstring_end_valid()
        {
            var str = "01234string";
            var substring = "string";
            Assert.AreEqual(substring, str.SafeSubstring(5,6), "Did not return the correct substring");
        }

        [TestMethod]
        public void SafeSubstring_end_pastend()
        {
            var str = "01234string";
            var substring = "string";
            Assert.AreEqual(substring, str.SafeSubstring(5, 100), "Did not return the correct substring");
        }

        [TestMethod]
        public void SafeSubstring_start_pastend()
        {
            var str = "01234";
            Assert.AreEqual(string.Empty, str.SafeSubstring(100,200), "Did not an empty string");
        }

        [TestMethod]
        public void SafeSubstring_negative_startindex_returns_empty()
        {
            var str = "01234";
            Assert.AreEqual(string.Empty, str.SafeSubstring(-1, 200), "Did not an empty string");
        }
    }
}
