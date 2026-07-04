using CSharpFunctionalExtensions;
using Microsoft.Data.SqlClient;
using Respawn;
using Respawn.Graph;
using System.Data.Common;
using VRT.Resume.Domain.Entities;
using VRT.Resume.Persistence;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Fixtures;

public sealed class ApplicationFixture : BaseFixture
{
    private Respawner? _checkpoint;
    public ApplicationFixture()
    {
        ServiceCollection
            .AddApplication()
            .AddIntegrationTestInfrastructure();
    }
    protected override async Task OnServiceProviderBuid(IServiceProvider provider)
    {
        await Result.Success(provider.GetRequiredService<AppDbContext>())
            .Ensure(a => a is not null, "Invalid db context")
            .Tap(context => context.Database.EnsureDeleted())
            .Tap(context => context.InitDatabase())
            .Map(_ => CreateCheckpoint())
            .Tap(checkpoint => _checkpoint = checkpoint);
    }
    public async Task ResetState()
    {
        if (_checkpoint != null)
        {
            await using var connection = GetDbConnection();
            await _checkpoint.ResetAsync(connection);
        }
    }

    private static async Task<Respawner> CreateCheckpoint()
    {
        var options = new RespawnerOptions()
        {
            TablesToIgnore = new[]
            {
                new Table("__EFMigrationsHistory"),                                
                //TODO: Add tables that should stay as when reset applies e.g. static dictionaries
                new Table(nameof(SkillType)),
                //new Table(nameof(ProductType))
            }
        };
        await using var connection = GetDbConnection();
        var result = await Respawner.CreateAsync(connection, options);
        return result;
    }

    private static DbConnection GetDbConnection()
    {
        var connection = new SqlConnection
        {
            ConnectionString = Defaults.TestDbConnectionString
        };
        connection.Open();
        return connection;
    }
}
