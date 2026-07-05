using Microsoft.EntityFrameworkCore;
using VRT.Resume.Persistence;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Pwa.Services;

public sealed class DatabaseInitializer(IDbContextFactory<AppDbContext> dbContextFactory)
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        await context.InitDatabaseAsync(cancellationToken);
    }
}