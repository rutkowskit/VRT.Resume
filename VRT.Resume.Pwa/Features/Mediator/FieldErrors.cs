namespace VRT.Resume.Pwa.Features.Mediator;

public static class FieldErrors
{
    public static string? GetFieldError(
        IReadOnlyDictionary<string, string[]> fieldErrors,
        string propertyName)
    {
        if (!fieldErrors.TryGetValue(propertyName, out var messages) || messages.Length == 0)
            return null;

        return string.Join(' ', messages);
    }

    public static bool HasFieldError(
        IReadOnlyDictionary<string, string[]> fieldErrors,
        string propertyName) =>
        GetFieldError(fieldErrors, propertyName) is not null;
}