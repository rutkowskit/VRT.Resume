using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using VRT.Resume.Application.Common.Behaviours;

namespace VRT.Resume.Application;
public static class DependencyInjection
{    
    private record Marker;
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services
            .AddMediatorElements(typeof(Marker).Assembly)
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));            
        return services;
    }
    public static IServiceCollection AddMediatorElements(this IServiceCollection services,
            Assembly fromAssembly)
    {
        services
             .AddMediatR(config => config.RegisterServicesFromAssembly(fromAssembly))
             .AddValidatorsFromAssembly(fromAssembly);             
        return services;
    }
}
