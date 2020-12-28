using System.Resources;

namespace VRT.Resume.Resources
{
    public static class ResourceHelper
    {
        public static string GetLabelText(this string key, params object[]formatParams)
        => LabelResource.ResourceManager.GetString(key, formatParams);

        public static string GetMessageText(this string key, params object[] formatParams)
            =>MessageResource.ResourceManager.GetString(key, formatParams);        

        private static string GetString(this ResourceManager resManager, string key, params object[] formatParams)
        {
            var value = resManager.GetString(key) ?? "";
            value = string.IsNullOrWhiteSpace(value)
                ? key
                : string.Format(value, formatParams);
            return value;
        }
    }
}
