using System;

namespace VRT.Resume.Web
{
    public static class DateTimeExtensions
    {
        public static string ToDateString(this DateTime? date, string format = "yyyy-MM-dd")
        {
            return date.HasValue
                ? date.Value.ToDateString(format)
                : string.Empty;
        }
        public static string ToDateString(this DateTime date, string format="yyyy-MM-dd")
        {
            return date.ToString(format);
        }
    }
}