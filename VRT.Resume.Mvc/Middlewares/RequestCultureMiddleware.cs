using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Threading.Tasks;
using VRT.Resume.Application.Common.Abstractions;

namespace VRT.Resume.Mvc.Middlewares
{
    public class RequestCultureMiddleware
    {        
        private readonly RequestDelegate _next;

        public RequestCultureMiddleware(RequestDelegate next)
        {         
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ICultureService cultureService)
        {
            var currentCulture = cultureService.GetCurrentCulture();
            if (!string.IsNullOrWhiteSpace(currentCulture))
            {                
                var langCulture = new CultureInfo(currentCulture);
                var culture = CultureInfo.CreateSpecificCulture(langCulture.Name);
                culture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
            }            
            await _next(context);
        }
    }
    public static class RequestCultureMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestCulture(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestCultureMiddleware>();
        }
    }
}
