using Microsoft.EntityFrameworkCore;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Persistence.Data;

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

        if (string.IsNullOrEmpty(UserId))
            return;

        await using var scope = scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var exists = await context.UserPerson
            .AsNoTracking()
            .AnyAsync(u => u.UserId == UserId, cancellationToken);

        if (!exists)
        {
            UserId = string.Empty;
            await WithStorageAsync(storage => storage.SetActiveUserIdAsync(null, cancellationToken));
        }
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