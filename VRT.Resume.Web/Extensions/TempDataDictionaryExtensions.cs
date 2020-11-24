using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VRT.Resume.Web
{
    public static class TempDataDictionaryExtensions
    {
        public static bool TryGetNonEmpty(this TempDataDictionary dic, string key, 
            out string value)
        {            
            value = dic.GetValueOrDefault(key);                
            return !string.IsNullOrWhiteSpace(value);                
        }

        public static string GetValueOrDefault(this TempDataDictionary dic, string key, 
            string defaultValue=null)
        {
            var isInDic = dic.TryGetValue(key, out var result);
            var value = isInDic && result != null
                ? result.ToString()
                : string.Empty;

            return !string.IsNullOrWhiteSpace(value)
                ? value
                : defaultValue;
        }
    }
}