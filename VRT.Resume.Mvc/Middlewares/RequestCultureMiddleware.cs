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
                var culture = GetCultureByNameOrDefault(currentCulture);
                culture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
            }            
            await _next(context);
        }
        private static CultureInfo GetCultureByNameOrDefault(string culture)
        {
            try
            {
                var langCulture = new CultureInfo(culture);
                return CultureInfo.CreateSpecificCulture(langCulture.Name);
            }
            catch
            {
                return CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            }
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
