using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;

namespace VRT.Resume.Mvc.Controllers
{
    // Partial for handling current thread culture
    partial class ControllerBase
    {
        protected const string CultureCookieKey = "culture";
        partial void SetupThreadCulture()
        {
            var lang = GetCurrentLanguage();
            AddLanguageCookie(lang);
            SetThreadLanguage(lang);
        }

        protected string GetCurrentLanguage()
        {
            var langCookie = Request?.Cookies[CultureCookieKey];
            return langCookie == null
                ? GetDefaultLanguage()
                : langCookie;
        }

        private string GetDefaultLanguage()
        {
            var userLanguage = Request.Headers["Accept-Language"].ToString();
            var firstLang = userLanguage.Split(',').FirstOrDefault();
            var defaultLang = string.IsNullOrEmpty(firstLang) ? "pl-PL" : firstLang;
            //return Task.FromResult(new ProviderCultureResult(defaultLang, defaultLang));
            
            return string.IsNullOrWhiteSpace(firstLang)
                ? defaultLang
                : firstLang;
        }

        protected void AddLanguageCookie(string language)
        {
            if (Response == null) return;            
            var opt = new Microsoft.AspNetCore.Http.CookieOptions()
            {
                Expires = DateTime.Now.AddYears(1)
            };
            Response.Cookies.Append(CultureCookieKey, language,opt);
        }

        private static void SetThreadLanguage(string language)
        {
            try
            {
                var culture = new CultureInfo(language);
                var cultureInfo = CultureInfo.CreateSpecificCulture(culture.Name);
                cultureInfo.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
                Thread.CurrentThread.CurrentCulture = cultureInfo;
                Thread.CurrentThread.CurrentUICulture = cultureInfo;                
            }
            catch
            {
                // ignore
            }
        }        
    }
}