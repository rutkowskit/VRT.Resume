namespace VRT.Resume.Application.Common.Queries.GetSupportedLanguages
{
    public class LanguageVM
    {
        public LanguageVM(string key, string caption)
        {
            Key = key;
            Caption = caption;
        }
        public string Key { get; }
        public string Caption { get; }
    }
}