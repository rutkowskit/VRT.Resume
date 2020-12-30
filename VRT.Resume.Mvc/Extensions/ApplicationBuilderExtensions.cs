using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using VRT.Resume.Mvc.Middlewares;
using VRT.Resume.Persistence;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Mvc
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseRequestCulture(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestCultureMiddleware>();
        }
        public static IApplicationBuilder UseAppDatabase(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
                context.InitDatabase();
            }
            return app;
        }
    }
}
