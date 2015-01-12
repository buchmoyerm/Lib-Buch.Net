using Microsoft.VisualStudio.TestTools.UnitTesting;
using ph4n.Common;

namespace ph4n.Test.Common
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

        [TestMethod]
        public void ConvertTo_Int_FromDouble_Truncate()
        {
            double num = 450.56;
            int expected = 450;

            var actual = num.ConvertTo<int>();
            Assert.AreEqual(expected, actual, "incorrect conversion");
        }

        [TestMethod]
        public void ConvertTo_Int_FromNegativeDouble_Truncate()
        {
            double num = -450.56;
            int expected = -450;

            var actual = num.ConvertTo<int>();
            Assert.AreEqual(expected, actual, "incorrect conversion");
        }

        [TestMethod]
        public void ConvertTo_Int_FromDecimal_Truncate()
        {
            decimal num = (decimal) 450.56;
            int expected = 450;

            var actual = num.ConvertTo<int>();
            Assert.AreEqual(expected, actual, "incorrect conversion");
        }

        [TestMethod]
        public void ConvertTo_Int_FromNegativeDecimal_Truncate()
        {
            decimal num = (decimal)-450.56;
            int expected = -450;

            var actual = num.ConvertTo<int>();
            Assert.AreEqual(expected, actual, "incorrect conversion");
        }

        [TestMethod]
        public void ConvertTo_Int_FromString_WholeNumber()
        {
            string num = "12";
            int expected = 12;

            var actual = num.ConvertTo<int>();
            Assert.AreEqual(expected, actual, "incorrect conversion");
        }

        [TestMethod]
        public void ConvertTo_Int_FromString_FloatingNumber()
        {
            string num = "12.68";
            int expected = 12;

            var actual = num.ConvertTo<int>();
            Assert.AreEqual(expected, actual, "incorrect conversion");
        }

        [TestMethod]
        public void ConvertTo_Double_FromString_WholeNumber()
        {
            string num = "12.0";
            double expected = 12.0;

            var actual = num.ConvertTo<double>();
            Assert.AreEqual(expected, actual, "incorrect conversion");
        }

        [TestMethod]
        public void ConvertTo_Double_FromString_FloatingNumber()
        {
            string num = "12.68";
            double expected = 12.68;

            var actual = num.ConvertTo<double>();
            Assert.AreEqual(expected, actual, "incorrect conversion");
        }

        [TestMethod]
        public void ConvertTo_Double_FromInt()
        {
            int num = 34;
            double expected = 34.0;

            var actual = num.ConvertTo<double>();
            Assert.AreEqual(expected, actual, "incorrect conversion");
        }

        [TestMethod]
        public void ConvertTo_Decimal_FromInt()
        {
            int num = 34;
            decimal expected = 34;

            var actual = num.ConvertTo<decimal>();
            Assert.AreEqual(expected, actual, "incorrect conversion");
        }

        [TestMethod]
        public void ConvertTo_Decimal_FromDouble()
        {
            double num = 34.985;
            decimal expected = (decimal) 34.985;

            var actual = num.ConvertTo<decimal>();
            Assert.AreEqual(expected, actual, "incorrect conversion");
        }

        [TestMethod]
        public void ConvertTo_Decimal_FromString()
        {
            string num = "35.21";
            decimal expected = (decimal)35.21;

            var actual = num.ConvertTo<decimal>();
            Assert.AreEqual(expected, actual, "incorrect conversion");
        }

        [TestMethod]
        public void ConvertTo_Enum_FromString_Valid()
        {
            var expected = System.Net.Sockets.AddressFamily.InterNetwork;
            var str = "InterNetwork";
            var actual = str.ConvertTo<System.Net.Sockets.AddressFamily>();
            Assert.AreEqual(expected, actual, "incorrect conversion");
        }

        [TestMethod]
        public void ConvertTo_Enum_FromString_Invalid()
        {
            var expected = default(System.Net.Sockets.AddressFamily);
            var str = "No network";
            var actual = str.ConvertTo<System.Net.Sockets.AddressFamily>();
            Assert.AreEqual(expected, actual, "incorrect conversion");
        }
    }
}
