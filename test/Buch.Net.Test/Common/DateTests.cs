using System;
using System.Collections.Generic;
using Buch.Net.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Buch.Net.Test.Common
{
    [TestClass]
    public class DateTests
    {
        [TestMethod]
        public void IsWeekday_MondayThruFriday_True()
        {
            var monday = new DateTime(2014, 12, 29);//is a monday
            var weekdays = new Dictionary<string, DateTime>();
            weekdays.Add("Monday", monday);
            weekdays.Add("Tuesday", monday.AddDays(1));
            weekdays.Add("Wednesday", monday.AddDays(2));
            weekdays.Add("Thursday", monday.AddDays(3));
            weekdays.Add("Friday", monday.AddDays(4));

            foreach (var weekday in weekdays)
            {
                Assert.IsTrue(weekday.Value.IsWeekday(), "{0} is a weekday", weekday.Key);
            }
        }

        [TestMethod]
        public void IsWeekday_SaturdaySunday_False()
        {
            var saturday = new DateTime(2015, 1, 3);//is a saturday
            var weekdays = new Dictionary<string, DateTime>();
            weekdays.Add("Saturday", saturday);
            weekdays.Add("Sunday", saturday.AddDays(1));

            foreach (var weekday in weekdays)
            {
                Assert.IsFalse(weekday.Value.IsWeekday(), "{0} is not a weekday", weekday.Key);
            }
        }

        [TestMethod]
        public void IsWeekend_MondayThruFriday_False()
        {
            var monday = new DateTime(2014, 12, 29);//is a monday
            var weekdays = new Dictionary<string, DateTime>();
            weekdays.Add("Monday", monday);
            weekdays.Add("Tuesday", monday.AddDays(1));
            weekdays.Add("Wednesday", monday.AddDays(2));
            weekdays.Add("Thursday", monday.AddDays(3));
            weekdays.Add("Friday", monday.AddDays(4));

            foreach (var weekday in weekdays)
            {
                Assert.IsFalse(weekday.Value.IsWeekend(), "{0} is not a weekend", weekday.Key);
            }
        }

        [TestMethod]
        public void IsWeekend_SaturdaySunday_True()
        {
            var saturday = new DateTime(2015, 1, 3);//is a saturday
            var weekdays = new Dictionary<string, DateTime>();
            weekdays.Add("Saturday", saturday);
            weekdays.Add("Sunday", saturday.AddDays(1));

            foreach (var weekday in weekdays)
            {
                Assert.IsTrue(weekday.Value.IsWeekend(), "{0} is a weekend", weekday.Key);
            }
        }
    }
}
