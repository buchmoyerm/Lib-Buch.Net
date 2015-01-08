﻿using System;
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
    }
}
