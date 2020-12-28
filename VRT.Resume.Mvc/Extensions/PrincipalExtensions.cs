using System.Security.Claims;
using System.Security.Principal;
using VRT.Resume.Mvc.Models;

namespace VRT.Resume.Mvc
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