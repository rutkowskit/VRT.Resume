using System.Web.Mvc;

namespace VRT.Resume.Web.Controllers
{
    // Partial for handling unexpected exceptions
    partial class ControllerBase
    {
        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;
            var controller = RouteData.Values["Controller"] as string;
            var action = RouteData.Values["Action"] as string;

            var exceptionInfo = new HandleErrorInfo(
                filterContext.Exception, controller, action);
            
            TempData[TempDataKeys.ExceptionInfo] = exceptionInfo;
            filterContext.Result = RedirectToAction("Index", "ErrorHandler");
        }
    }
}