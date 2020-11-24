using System.Web.Mvc;
using System.Web.Routing;

namespace VRT.Resume.Web
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = filterContext.HttpContext.User.Identity.IsAuthenticated
                ? new HttpUnauthorizedResult()
                : new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account" }));            
        }
    }
}