using System;
using VRT.Resume.Application.Common.Abstractions;

namespace VRT.Resume.Application.Fakes
{
    internal sealed class FakeDateTimeService : IDateTimeService
    {        
        public DateTime Now => Defaults.Today;
    }
}
