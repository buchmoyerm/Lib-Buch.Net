using System;
using Buch.Net.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Buch.Net.Test.Common
{
    [TestClass]
    public class ConvertToTests
    {
        [TestMethod]
        public void ConvertTo_String_FromInt()
        {
            int num = 23;
            string expectedStr = "23";

            var actualStr = num.ConvertTo<string>();
            Assert.AreEqual(expectedStr, actualStr, "incorrect conversion");
        }

        [TestMethod]
        public void ConvertTo_String_FromDouble()
        {
            double num = 23.33;
            string expectedStr = "23.33";

            var actualStr = num.ConvertTo<string>();
            Assert.AreEqual(expectedStr, actualStr, "incorrect conversion");
        }

        [TestMethod]
        public void ConvertTo_String_FromDecimal()
        {
            decimal num = (decimal) 23.33;
            string expectedStr = "23.33";

            var actualStr = num.ConvertTo<string>();
            Assert.AreEqual(expectedStr, actualStr, "incorrect conversion");
        }

        [TestMethod]
        public void ConvertTo_String_FromString()
        {
            string str = "mystring";
            string expectedStr = "mystring";

            var actualStr = str.ConvertTo<string>();
            Assert.AreEqual(expectedStr, actualStr, "incorrect conversion");
        }
    }
}
