using System;

namespace CIM.RemoteManager.Core.Helpers
{
    public static class DateTimeHelper
    {
        /// <summary>
        /// Converts Unix datetime to Windows local time.
        /// </summary>
        /// <param name="unixTimeStamp">The unix time stamp.</param>
        /// <returns>Windows local time.</returns>
        public static DateTime UnixTimeStampToDateTime(this int unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            var utcDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            utcDateTime = utcDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return utcDateTime;
        }
    }
}
