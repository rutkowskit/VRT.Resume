using SqliteWasmBlazor;

namespace VRT.Resume.Pwa.Services;

internal static class PwaSqliteStartup
{
    public static async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        const int maxAttempts = 8;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                await services.InitializeSqliteWasmAsync();
                return;
            }
            catch (Exception ex) when (IsOpfsLockError(ex) && attempt < maxAttempts)
            {
                await Task.Delay(250 * attempt, cancellationToken);
            }
        }
    }

    private static bool IsOpfsLockError(Exception exception)
    {
        var messages = exception is AggregateException aggregate
            ? aggregate.Flatten().InnerExceptions.Select(e => e.Message)
            : [exception.Message];

        return messages.Any(message =>
            message.Contains("another browser tab", StringComparison.OrdinalIgnoreCase)
            || message.Contains("another open Access Handle", StringComparison.OrdinalIgnoreCase)
            || message.Contains("Access Handles cannot be created", StringComparison.OrdinalIgnoreCase)
            || message.Contains("database is locked", StringComparison.OrdinalIgnoreCase));
    }
}