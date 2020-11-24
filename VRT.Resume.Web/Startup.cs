using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(VRT.Resume.Web.Startup))]
namespace VRT.Resume.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
