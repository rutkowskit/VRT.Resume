namespace VRT.Resume.Web.Models
{
    public class CultureSettingsViewModel
    {
        public CultureSettingsViewModel(string key, string caption)
        {
            Key = key;
            Caption = caption;
        }
        public string Key { get; }
        public string Caption { get; }
    }
}