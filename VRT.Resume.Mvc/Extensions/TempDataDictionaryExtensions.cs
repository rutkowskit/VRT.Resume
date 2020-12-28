using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace VRT.Resume.Mvc
{
    public static class TempDataDictionaryExtensions
    {
        public static bool TryGetNonEmpty(this ITempDataDictionary dic, string key, 
            out string value)
        {            
            value = dic.GetValueOrDefault(key);                
            return !string.IsNullOrWhiteSpace(value);                
        }

        public static string GetValueOrDefault(this ITempDataDictionary dic, string key, 
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