using Microsoft.EntityFrameworkCore;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Pwa.Services;

public sealed class DummyCurrentUserService(IServiceScopeFactory scopeFactory)
    : ICurrentUserService, IActiveProfileContext, ICurrentPersonIdAccessor
{
    public event Action? ContextChanged;

    public string UserId { get; private set; } = string.Empty;

    public int? PersonId { get; private set; }

    public bool HasActiveContext => PersonId is int id && id > 0;

    public bool TryGetPersonId(out int personId)
    {
        if (PersonId is int id && id > 0)
        {
            personId = id;
            return true;
        }

        personId = 0;
        return false;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await WithStorageAsync(async storage =>
        {
            UserId = await storage.GetActiveUserIdAsync(cancellationToken) ?? string.Empty;
        });

        if (string.IsNullOrEmpty(UserId))
        {
            PersonId = null;
            return;
        }

        var personId = await ResolvePersonIdAsync(UserId, cancellationToken);
        if (personId is null)
        {
            await ClearContextAsync(cancellationToken);
            return;
        }

        PersonId = personId;
    }

    public async Task SetContextAsync(string userId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        var personId = await ResolvePersonIdAsync(userId, cancellationToken);
        if (personId is null)
            throw new InvalidOperationException($"Profile '{userId}' was not found in the local database.");

        UserId = userId;
        PersonId = personId;
        await WithStorageAsync(storage => storage.SetActiveUserIdAsync(userId, cancellationToken));
        ContextChanged?.Invoke();
    }

    public void SetContext(string userId)
    {
        SetContextAsync(userId).GetAwaiter().GetResult();
    }

    private async Task<int?> ResolvePersonIdAsync(string userId, CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var personId = await context.UserPerson
            .AsNoTracking()
            .Where(u => u.UserId == userId)
            .Select(u => u.PersonId)
            .FirstOrDefaultAsync(cancellationToken);

        return personId > 0 ? personId : null;
    }

    private async Task ClearContextAsync(CancellationToken cancellationToken)
    {
        UserId = string.Empty;
        PersonId = null;
        await WithStorageAsync(storage => storage.SetActiveUserIdAsync(null, cancellationToken));
    }

    private async Task WithStorageAsync(Func<ProfileContextStorage, Task> action)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var storage = scope.ServiceProvider.GetRequiredService<ProfileContextStorage>();
        await action(storage);
    }
}