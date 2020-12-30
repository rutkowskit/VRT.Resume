using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using VRT.Resume.Application.Common.Abstractions;

namespace VRT.Resume.Mvc.Services
{
    public sealed class CultureService : ICultureService
    {
        private const string CultureCookieKey = "VRT.Resume.Culture";
        private const string DefaultLanguage = "pl";

        private static readonly Dictionary<string, (string key, string caption)> SupportedLangDic
            = new Dictionary<string, (string key, string caption)>()
            {
                ["pl"] = ("pl", "Polski"),
                ["en"] = ("en", "English")
            };

        private readonly IHttpContextAccessor _httpContext;

        public CultureService(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }
        public IReadOnlyDictionary<string, (string key, string caption)> GetSupportedLanguages()
        {
            return SupportedLangDic;
        }

        public (string key, string name) GetCurrentLanguage()
        {
            var lang = GetCurrentCulture();
            var key = lang.Length >= 2 ? lang.Substring(0, 2) : DefaultLanguage;
            
            var result = SupportedLangDic.TryGetValue(key, out var culture)
                ? culture
                : SupportedLangDic.Values.First();
            return result;
        }


        public string GetCurrentCulture()
        {
            var request = _httpContext.HttpContext.Request;
            var langCookie = request?.Cookies[CultureCookieKey];
            return langCookie == null
                ? GetDefaultLanguage()
                : langCookie;
        }

        public void SetCurrentCulture(string culture)
        {
            AddLanguageCookie(culture);            
        }    

        private string GetDefaultLanguage()
        {
            var request = _httpContext.HttpContext.Request;
            var userLanguage = request.Headers["Accept-Language"].ToString();
            var firstLang = userLanguage.Split(',').FirstOrDefault();
            var defaultLang = string.IsNullOrEmpty(firstLang) 
                ? DefaultLanguage
                : firstLang;
            
            return string.IsNullOrWhiteSpace(firstLang)
                ? defaultLang
                : firstLang;
        }
        private void AddLanguageCookie(string language)
        {
            var response = _httpContext.HttpContext.Response;
            if (response == null) return;
            var opt = new CookieOptions()
            {
                Expires = DateTime.Now.AddYears(1)
            };
            response.Cookies.Append(CultureCookieKey, language, opt);
        }
    }
}
