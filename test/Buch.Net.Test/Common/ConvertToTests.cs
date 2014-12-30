using System;
using Buch.Net.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Buch.Net.Test.Common
{
    [TestClass]
    public class ConvertToTests
    {
        [TestMethod]
        public void IntToString()
        {
            int num = 23;
            string expectedStr = "23";

            var actualStr = num.ConvertTo<string>();
            Assert.AreEqual(expectedStr, actualStr, "incorrect conversion");
        }

        [TestMethod]
        public void DoubleToString()
        {
            double num = 23.33;
            string expectedStr = "23.33";

            var actualStr = num.ConvertTo<string>();
            Assert.AreEqual(expectedStr, actualStr, "incorrect conversion");
        }

        [TestMethod]
        public void DecimalToString()
        {
            decimal num = (decimal) 23.33;
            string expectedStr = "23.33";

            var actualStr = num.ConvertTo<string>();
            Assert.AreEqual(expectedStr, actualStr, "incorrect conversion");
        }

        [TestMethod]
        public void StringToString()
        {
            string str = "mystring";
            string expectedStr = "mystring";

            var actualStr = str.ConvertTo<string>();
            Assert.AreEqual(expectedStr, actualStr, "incorrect conversion");
        }
    }
}
