using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Mvc
{
    internal static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDbContext(this IServiceCollection services,
            IConfiguration config)
        {
            var setupAction = GetDbSetupAction(config);
            services.AddDbContext<AppDbContext>(setupAction,ServiceLifetime.Transient, ServiceLifetime.Transient);
            return services;
        }

        private static Action<DbContextOptionsBuilder> GetDbSetupAction(IConfiguration config)
        {
            var provider = config.GetValue<string>("DbProvider")?.ToLower();
            var connString = config.GetValue<string>($"ConnectionStrings:ResumeData:{provider}");

            return provider switch
            {
                "mssql" => (DbContextOptionsBuilder opt) => opt.UseSqlServer(connString),
                _ => (DbContextOptionsBuilder opt) => opt.UseSqlite(connString),
            };
        }        
    }
}
