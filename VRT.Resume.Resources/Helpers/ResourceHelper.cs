using System.Globalization;
using System.Resources;

namespace VRT.Resume.Resources
{
    public static class ResourceHelper
    {
        /// <summary>Optional host hook (PWA sets this from <c>PwaCultureService</c>).</summary>
        public static Func<CultureInfo>? ResolveCulture { get; set; }

        public static string GetLabelText(this string key, params object[]formatParams)
        => LabelResource.ResourceManager.GetString(key, formatParams);

        public static string GetMessageText(this string key, params object[] formatParams)
            =>MessageResource.ResourceManager.GetString(key, formatParams);

        private static CultureInfo GetCulture() =>
            ResolveCulture?.Invoke() ?? CultureInfo.CurrentUICulture;

        private static string GetString(this ResourceManager resManager, string key, params object[] formatParams)
        {
            var value = resManager.GetString(key, GetCulture()) ?? "";
            value = string.IsNullOrWhiteSpace(value)
                ? key
                : string.Format(value, formatParams);
            return value;
        }
    }
}
