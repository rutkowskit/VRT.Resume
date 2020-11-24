using System.Web;
using VRT.Resume.Application.Common.Abstractions;

namespace VRT.Resume.Web.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private HttpContextBase _httpContext;

        public CurrentUserService(HttpContextBase httpContext)
        {
            _httpContext = httpContext;
        }
        public string UserId
            => _httpContext?.User.AsUserLoginViewModel()?.Email;
    }
}