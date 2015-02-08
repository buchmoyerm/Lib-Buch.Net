using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ph4n.Common;

namespace ph4n.Test.Common
{
    [TestClass]
    public class ValidateTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgumentNotNull_null_throws_ArgumentNullException()
        {
            Validate.ArgumentNotNull(null, "param");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgumentNotNull_null_string_throws_ArgumentNullException()
        {
            string test = null;
            Validate.ArgumentNotNull(test, "test");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgumentNotNull_null_object_throws_ArgumentNullException()
        {
            object test = null;
            Validate.ArgumentNotNull(test, "test");
        }

        [TestMethod]
        public void ArgumentNotNull_not_null_string()
        {
            string test = "string";
            Validate.ArgumentNotNull(test, "test");
        }

        [TestMethod]
        public void ArgumentNotNull_not_null_object()
        {
            object test = new object();
            Validate.ArgumentNotNull(test, "test");
        }

        [TestMethod]
        public void ArgumentNotNull_not_null_int()
        {
            int test = 9;
            Validate.ArgumentNotNull(test, "test");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgumentNotNullOrEmpty_null_throws_ArgumentNullException()
        {
            string test = null;
            Validate.ArgumentNotNullOrEmpty(test, "test");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ArgumentNotNullOrEmpty_empty_throws_ArgumentException()
        {
            string test = string.Empty;
            Validate.ArgumentNotNullOrEmpty(test, "test");
        }

        [TestMethod]
        public void ArgumentNotNullOrEmpty_valid()
        {
            string test = "my test string";
            Validate.ArgumentNotNullOrEmpty(test, "test");
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void ArgumentTypeIsEnum_null_throws_ArgumentNullException()
        {
            Validate.ArgumentTypeIsEnum(null, "test");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ArgumentTypeIsEnum_int_type_throws_ArgumentException()
        {
            Type testType = typeof (int);
            Validate.ArgumentTypeIsEnum(testType, "testType");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ArgumentTypeIsEnum_CultureInfo_type_throws_ArgumentException()
        {
            Type testType = typeof(CultureInfo);
            Validate.ArgumentTypeIsEnum(testType, "testType");
        }

        [TestMethod]
        public void ArgumentTypeIsEnum_AddressFamily()
        {
            Type testType = typeof(System.Net.Sockets.AddressFamily);
            Validate.ArgumentTypeIsEnum(testType, "testType");
        }

        [TestMethod]
        public void ArgumentContainsOnly_IsLetter_true()
        {
            string valid_alpha = "AlphaString";
            Validate.ArgumentContainsOnly(valid_alpha, char.IsLetter, "valid_alpha");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ArgumentContainsOnly_IsLetter_false_throw()
        {
            string invalid_alpha = "AlphaString1";
            Validate.ArgumentContainsOnly(invalid_alpha, char.IsLetter, "invalid_alpha");
        }

        [TestMethod]
        public void ArgumentContains_IsLetter_true()
        {
            string valid_input = "123a";
            Validate.ArgumentContains(valid_input, char.IsLetter, "valid_input");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ArgumentContains_IsLetter_false_throws()
        {
            string invalid_input = "1235";
            Validate.ArgumentContains(invalid_input, char.IsLetter, "invalid_input");
        }

        [TestMethod]
        public void ArgumementGreaterThan_int_is_greater()
        {
            int valid_input = 1;
            Validate.ArgumementGreaterThan(valid_input, 0, "valid_input");
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentException))]
        public void ArgumementGreaterThan_int_is_equal()
        {
            int invalid_input = 0;
            Validate.ArgumementGreaterThan(invalid_input, 0, "invalid_input");
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentException))]
        public void ArgumentGreaterThan_int_is_less()
        {
            int invalid_input = -1;
            Validate.ArgumementGreaterThan(invalid_input, 0, "invalid_input");
        }

        public void ArgumementGreaterThan_float_is_greater()
        {
            float valid_input = (float)0.002;
            Validate.ArgumementGreaterThan(valid_input, (float)0.0, "valid_input");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ArgumementGreaterThan_float_is_equal()
        {
            float invalid_input = (float)0.0;
            Validate.ArgumementGreaterThan(invalid_input, (float)0.0, "invalid_input");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ArgumentGreaterThan_float_is_less()
        {
            float invalid_input = (float)-0.01;
            Validate.ArgumementGreaterThan(invalid_input, (float)0.0, "invalid_input");
        }
    }
}
