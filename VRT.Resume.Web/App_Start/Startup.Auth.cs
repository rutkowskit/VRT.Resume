using Microsoft.Owin;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Microsoft.Owin.Security;
using System.Configuration;

namespace VRT.Resume.Web
{
    public partial class Startup
    {        
        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                LoginPath = new PathString("/Account/Index"),
                SlidingExpiration = true
            });
            app.UseGoogleAuthentication(GetGoogleAuthOptions());
        }
        private GoogleOAuth2AuthenticationOptions GetGoogleAuthOptions()
        {
            var clientId = ConfigurationManager.AppSettings["google:ClientId"];
            var secret = ConfigurationManager.AppSettings["google:Secret"];
            var result = new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = clientId,
                ClientSecret = secret,
                CallbackPath = new PathString("/signin-google")
            };
            return result;
        }
    }
}
