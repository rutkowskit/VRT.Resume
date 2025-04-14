using Microsoft.EntityFrameworkCore;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Mvc;

internal static partial class DependencyInjection
{
    public static IServiceCollection AddDbContext(this IServiceCollection services,
        IConfiguration config)
    {
        var setupAction = GetDbSetupAction(config);
        services.AddDbContext<AppDbContext>(setupAction, ServiceLifetime.Transient, ServiceLifetime.Transient);
        return services;
    }

    private static Action<DbContextOptionsBuilder> GetDbSetupAction(IConfiguration config)
    {
        var provider = config.GetValue<string>("DbProvider")?.ToLower();
        var connString = config.GetValue<string>($"ConnectionStrings:ResumeData:{provider}");

        return provider switch
        {
            "mssql" => (DbContextOptionsBuilder opt) => ConfigureSqlServer(opt, connString!),
            _ => (DbContextOptionsBuilder opt) => opt.UseSqlite(connString),
        };
    }
    private static void ConfigureSqlServer(DbContextOptionsBuilder opt, string connString)
    {
        opt.UseSqlServer(connString, cfg =>
        {
            cfg.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            cfg.CommandTimeout(45);
        });
    }
}
