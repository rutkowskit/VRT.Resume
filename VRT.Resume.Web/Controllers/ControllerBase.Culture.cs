using System;
using System.Globalization;
using System.Threading;
using System.Web;

namespace VRT.Resume.Web.Controllers
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
                : langCookie.Value;
        }

        private string GetDefaultLanguage()
        {
            var userLanguage = Request?.UserLanguages;
            var userLang = userLanguage != null 
                ? userLanguage[0] 
                : "";

            return string.IsNullOrWhiteSpace(userLang)
                ? "pl-PL"
                : userLang;
        }

        protected void AddLanguageCookie(string language)
        {
            if (Response == null) return;
            var langCookie = new HttpCookie(CultureCookieKey, language)
            {
                Expires = DateTime.Now.AddYears(1)
            };
            Response.Cookies.Add(langCookie);
        }

        private void SetThreadLanguage(string language)
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