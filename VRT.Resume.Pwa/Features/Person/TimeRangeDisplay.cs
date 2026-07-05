using VRT.Resume.Application.Common.Abstractions;

namespace VRT.Resume.Pwa.Features.Person;

internal static class TimeRangeDisplay
{
    public static string Format(ITimeRange? range, string format = "MM-yyyy", string separator = " - ")
    {
        if (range is null)
            return string.Empty;

        return range.ToDate is null
            ? range.FromDate.ToString(format)
            : $"{range.FromDate.ToString(format)}{separator}{range.ToDate.Value.ToString(format)}";
    }
}