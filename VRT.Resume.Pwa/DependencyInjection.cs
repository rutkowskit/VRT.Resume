using VRT.Resume.Application;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Application.Common.Services;
using VRT.Resume.Pwa.Features.Mediator;
using VRT.Resume.Pwa.Services;

namespace VRT.Resume.Pwa;

internal static partial class DependencyInjection
{
    public static IServiceCollection AddPwaDependencies(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeService, DateTimeService>();
        services.AddSingleton<IProfileImageService, ProfileImageService>();
        services.AddSingleton<PwaCultureService>();
        services.AddSingleton<ICultureService>(sp => sp.GetRequiredService<PwaCultureService>());
        services.AddSingleton<DummyCurrentUserService>();
        services.AddSingleton<ICurrentUserService>(sp => sp.GetRequiredService<DummyCurrentUserService>());
        services.AddSingleton<IActiveProfileContext>(sp => sp.GetRequiredService<DummyCurrentUserService>());
        services.AddSingleton<ICurrentPersonIdAccessor>(sp => sp.GetRequiredService<DummyCurrentUserService>());
        services.AddScoped<ProfileContextStorage>();
        services.AddScoped<LocalProfileService>();
        services.AddScoped<UserNotificationService>();
        services.AddScoped<MediatorSender>();
        services.AddSingleton<DatabaseInitializer>();
        services.AddApplication();
        return services;
    }
}