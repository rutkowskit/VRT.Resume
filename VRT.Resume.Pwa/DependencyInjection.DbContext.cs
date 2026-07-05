using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SqliteWasmBlazor;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Pwa;

internal static partial class DependencyInjection
{
    public static IServiceCollection AddPwaDbContext(this IServiceCollection services, string baseAddress)
    {
        services.AddDbContextFactory<AppDbContext>(options =>
        {
            var connection = new SqliteWasmConnection($"Data Source={PwaDatabaseNames.FileName}");
            options.ConfigureWarnings(w => w.Ignore(SqliteEventId.SchemaConfiguredWarning));
            options.UseSqliteWasm(connection);
        });

        services.AddTransient<AppDbContext>(sp =>
            sp.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext());

        services.AddSqliteWasm(o => o.BaseHref = new Uri(baseAddress).AbsolutePath);

        return services;
    }
}