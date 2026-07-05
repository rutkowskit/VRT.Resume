using CSharpFunctionalExtensions;

namespace VRT.Resume.Pwa.Features.Mediator;

public sealed class MediatorSendOutcome<T>
{
    private static readonly IReadOnlyDictionary<string, string[]> EmptyErrors =
        new Dictionary<string, string[]>();

    public required Result<T> Result { get; init; }

    public IReadOnlyDictionary<string, string[]> FieldErrors { get; init; } = EmptyErrors;

    public bool HasFieldErrors => FieldErrors.Count > 0;

    public static MediatorSendOutcome<T> FromResult(Result<T> result) =>
        new() { Result = result };

    public static MediatorSendOutcome<T> FromValidation(
        string error,
        IReadOnlyDictionary<string, string[]> fieldErrors) =>
        new()
        {
            Result = CSharpFunctionalExtensions.Result.Failure<T>(error),
            FieldErrors = fieldErrors,
        };
}

public sealed class MediatorSendOutcome
{
    private static readonly IReadOnlyDictionary<string, string[]> EmptyErrors =
        new Dictionary<string, string[]>();

    public required Result Result { get; init; }

    public IReadOnlyDictionary<string, string[]> FieldErrors { get; init; } = EmptyErrors;

    public bool HasFieldErrors => FieldErrors.Count > 0;

    public static MediatorSendOutcome FromResult(Result result) =>
        new() { Result = result };

    public static MediatorSendOutcome FromValidation(
        string error,
        IReadOnlyDictionary<string, string[]> fieldErrors) =>
        new()
        {
            Result = CSharpFunctionalExtensions.Result.Failure(error),
            FieldErrors = fieldErrors,
        };
}