using VRT.Resume.Application.Common.Abstractions;

namespace VRT.Resume.Mvc
{
    public static class TimeRangeExtensions
    {
        public static string AsTimeRange(this ITimeRange range, 
            string format="MM-yyyy", string rangeSeparator=" - ")        
        {
            if (range==null) 
                return string.Empty;
            
            return range.ToDate==null
                ? range.FromDate.ToString(format)
                : $"{range.FromDate.ToString(format)}{rangeSeparator}{range.ToDate.Value.ToString(format)}";
        }
    }
}