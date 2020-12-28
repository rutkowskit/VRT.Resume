namespace VRT.Resume.Mvc.Models
{
    public sealed class AuthConfigSection
    {
        public sealed class AuthProvider
        {
            public string Name { get; set; }
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
            public string CallbackPath { get; set; }
        }
        public AuthProvider[] Providers { get; set; }
    }
    
}
