using System;
using VRT.Resume.Application.Common.Abstractions;

namespace VRT.Resume.Application.Common.Services
{
    public sealed class DateTimeService : IDateTimeService
    {
        public DateTime Now => DateTime.UtcNow;
    }
}
