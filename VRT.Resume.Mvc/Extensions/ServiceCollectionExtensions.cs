using FluentValidation;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Application.Common.Behaviours;
using VRT.Resume.Application.Common.Services;
using VRT.Resume.Mvc.Services;
using VRT.Resume.Application;

namespace VRT.Resume.Mvc
{
    internal static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<IDateTimeService, DateTimeService>();
            services.AddTransient<ICultureService, CultureService>();
            services.AddTransient<ICurrentUserService, CurrentUserService>();
            services.AddAutoMapper(typeof(Startup));
            services.AddApplication();            
            return services;
        }
    }
}
