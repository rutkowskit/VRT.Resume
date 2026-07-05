using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace VRT.Resume.Mvc.Services
{
    public sealed class LoggingService : ILogger
    {
        IDisposable? ILogger.BeginScope<TState>(TState state)
            => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            Debug.WriteLine(state?.ToString());
        }
    }
}