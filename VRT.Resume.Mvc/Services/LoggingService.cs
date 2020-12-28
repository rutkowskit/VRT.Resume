using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace VRT.Resume.Mvc.Services
{
    public sealed class LoggingService : ILogger
    {
        public IDisposable BeginScope<TState>(TState state)
            => default;        

        public bool IsEnabled(LogLevel logLevel) => true;        

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Debug.WriteLine(state?.ToString());
        }
    }
}