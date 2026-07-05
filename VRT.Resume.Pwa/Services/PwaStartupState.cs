namespace VRT.Resume.Pwa.Services;

public sealed class PwaStartupState
{
    public bool IsReady { get; private set; }

    public string? ErrorMessage { get; private set; }

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public bool IsAnotherTabError =>
        ContainsAny(ErrorMessage, "another browser tab", "another open Access Handle", "Access Handles cannot be created");

    public void MarkReady() => IsReady = true;

    public void SetFailure(Exception exception) =>
        ErrorMessage = FlattenMessage(exception);

    private static string FlattenMessage(Exception exception)
    {
        if (exception is AggregateException aggregate)
        {
            var messages = aggregate.Flatten().InnerExceptions
                .Select(e => e.Message)
                .Where(m => !string.IsNullOrWhiteSpace(m));
            return string.Join(' ', messages.DefaultIfEmpty(aggregate.Message));
        }

        return exception.Message;
    }

    private static bool ContainsAny(string? message, params string[] needles)
    {
        if (string.IsNullOrWhiteSpace(message))
            return false;

        return needles.Any(needle => message.Contains(needle, StringComparison.OrdinalIgnoreCase));
    }
}