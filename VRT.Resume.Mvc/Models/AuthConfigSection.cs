namespace VRT.Resume.Mvc.Models
{
    public sealed class AuthConfigSection
    {
        public sealed class AuthProvider
        {
            public required string Name { get; set; }
            public required string ClientId { get; set; }
            public required string ClientSecret { get; set; }
            public required string CallbackPath { get; set; }
        }
        public AuthProvider[]? Providers { get; set; }
    }
    
}