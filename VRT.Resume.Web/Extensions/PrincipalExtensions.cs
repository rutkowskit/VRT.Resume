using System.Security.Claims;
using System.Security.Principal;
using VRT.Resume.Web.Models;

namespace VRT.Resume.Web
{
    public static class PrincipalExtensions
    {
        public static UserLoginViewModel AsUserLoginViewModel(this IPrincipal principal)
        {
            var claims = principal?.Identity as ClaimsIdentity;
            if (null == claims) 
                return null;

            return UserLoginViewModel.Create(claims);
        }
    }
}