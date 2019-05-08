using Microsoft.VisualStudio.TestTools.UnitTesting;
using Panda.Api.Helpers;
using System;

namespace Panda.API.Helpers.Tests
{
    [TestClass]
    public class UnixTimeHelperTests
    {
        [TestMethod]
        public void ConvertUnixTimeToDateTime_Should_BeCorrect()
        {
            double timestamp = 1557311131;

            var result = UnixTimeHelper.ConvertUnixTimeToDateTime(timestamp);
            var expected = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(timestamp);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ConvertDateTimeToUnixTime_Should_BeCorrect()
        {
            var date = DateTime.Now;

            var result = UnixTimeHelper.ConvertDateTimeToUnixTime(date);
            var expected = (date - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;

            Assert.AreEqual(expected, result);
        }
    }
}