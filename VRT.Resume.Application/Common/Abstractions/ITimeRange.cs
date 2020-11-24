using System;

namespace VRT.Resume.Application.Common.Abstractions
{
    public interface ITimeRange
    {
        DateTime FromDate { get; }
        DateTime? ToDate { get; }
    }
}