using System;

namespace Hub3c.Mentify.Service
{
    public static class TimeHelper
    {
        public static DateTime FromUnixTime(this long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }
        public static long ToUnixTime(this DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalSeconds);
        }
        public static DateTime? FromUnixTime(this long? unixTime)
        {
            return unixTime.HasValue ? FromUnixTime(unixTime.Value) : new DateTime?();
        }
        public static long? ToUnixTime(this DateTime? date)
        {
            return date.HasValue ? ToUnixTime(date.Value) : new long?();
        }
    }
}
