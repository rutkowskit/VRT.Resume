using System.Collections.Generic;

namespace VRT.Resume.Application.Common.Abstractions
{
    public interface ICultureService
    {
        IReadOnlyDictionary<string, (string key, string caption)> GetSupportedLanguages();
        (string key, string name) GetCurrentLanguage();
        string GetCurrentCulture();
        void SetCurrentCulture(string culture);
    }
}
