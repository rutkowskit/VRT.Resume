using System;

namespace VRT.Resume.Application.Common.Abstractions
{
    public interface IDateTimeService
    {
        DateTime Now { get; }
    }
}
