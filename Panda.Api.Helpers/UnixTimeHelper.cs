using System;

namespace Panda.Api.Helpers
{
    public static class UnixTimeHelper
    {
        /// <summary>
        /// Converts a unix timestamp to a DateTime instance
        /// </summary>
        /// <param name="timestamp">Timestamp to convert</param>
        /// <returns></returns>
        public static DateTime ConvertUnixTimeToDateTime(double timestamp)
        {
            return (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(timestamp);
        }

        /// <summary>
        /// Converts a DateTime instance to a unix timestamp
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static double ConvertDateTimeToUnixTime(DateTime date)
        {
            DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (date - unixEpoch).TotalSeconds;
        }
    }
}