using VRT.Resume.Application.Common.Abstractions;

namespace VRT.Resume.Pwa.Services;

public sealed class DummyCurrentUserService(IServiceScopeFactory scopeFactory)
    : ICurrentUserService, IActiveProfileContext
{
    public event Action? ContextChanged;

    public string UserId { get; private set; } = string.Empty;

    public bool HasActiveContext => !string.IsNullOrEmpty(UserId);

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await WithStorageAsync(async storage =>
        {
            UserId = await storage.GetActiveUserIdAsync(cancellationToken) ?? string.Empty;
        });
    }

    public async Task SetContextAsync(string userId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        UserId = userId;
        await WithStorageAsync(storage => storage.SetActiveUserIdAsync(userId, cancellationToken));
        ContextChanged?.Invoke();
    }

    public void SetContext(string userId)
    {
        SetContextAsync(userId).GetAwaiter().GetResult();
    }

    private async Task WithStorageAsync(Func<ProfileContextStorage, Task> action)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var storage = scope.ServiceProvider.GetRequiredService<ProfileContextStorage>();
        await action(storage);
    }
}