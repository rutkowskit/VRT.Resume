namespace VRT.Resume.Pwa.Features.Mediator;

public sealed class MediatorSendOptions
{
    public static MediatorSendOptions Default { get; } = new();

    public bool NotifyOnFailure { get; init; } = true;

    public bool NotifyOnSuccess { get; init; }

    public string? SuccessMessage { get; init; }
}