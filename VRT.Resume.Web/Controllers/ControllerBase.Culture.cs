using System;
using System.Globalization;
using System.Threading;
using System.Web;

namespace VRT.Resume.Web.Controllers
{
    // Partial for handling current thread culture
    partial class ControllerBase
    {
        partial void SetupThreadCulture()
        {
            var langCookie = Request?.Cookies["culture"];
            var lang = langCookie == null
                ? GetDefaultLanguage()
                : langCookie.Value;

            SetThreadLanguage(lang);
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

        private void AddLanguageCookie(string language)
        {
            if (Response == null) return;            
            var langCookie = new HttpCookie("culture", language)
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