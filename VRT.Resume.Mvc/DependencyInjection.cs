using VRT.Resume.Application;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Application.Common.Services;
using VRT.Resume.Mvc.Services;

namespace VRT.Resume.Mvc;

internal static partial class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton<IDateTimeService, DateTimeService>();
        services.AddSingleton<IProfileImageService, ProfileImageService>();
        services.AddTransient<ICultureService, CultureService>();
        services.AddTransient<ICurrentUserService, CurrentUserService>();
        services.AddApplication();
        return services;
    }
}
