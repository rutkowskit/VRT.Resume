using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Application.Fakes;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application;

internal static class DependencyInjection
{
    internal static IServiceCollection AddIntegrationTestInfrastructure(this IServiceCollection services)
    {
        return services
            .AddLogging()
            .AddDbContext()
            .AddDbContextFactory()    
            .AddScoped<IDateTimeService, FakeDateTimeService>()
            .AddScoped<ICurrentUserService, FakeCurrentUserService>();
    }

    internal static IServiceCollection AddDbContextFactory(this IServiceCollection services)
    {
        services.AddDbContextFactory<AppDbContext>(SetupDbContext);
        return services;
    }
    internal static IServiceCollection AddDbContext(this IServiceCollection services)
    {
        var lifeTime = ServiceLifetime.Transient;
        services.AddDbContext<AppDbContext>(SetupDbContext, lifeTime, lifeTime);
        return services;
    }
    private static void SetupDbContext(IServiceProvider serviceProvider, DbContextOptionsBuilder options)
    {
        options.UseSqlServer(Defaults.TestDbConnectionString);
    }
}
