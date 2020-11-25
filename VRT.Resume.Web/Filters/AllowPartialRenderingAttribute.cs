using System.Web.Mvc;

namespace VRT.Resume.Web
{
    /// <summary>
    /// Converts result view to partial view if the request is of ajax type
    /// </summary>
    public class AllowPartialRenderingAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            var result = filterContext.Result as ViewResult;

            if (request == null || result==null)
                return;

            if (request.IsAjaxRequest())
            {
                filterContext.Result = new PartialViewResult
                {
                    TempData = result.TempData,
                    View = result.View,
                    ViewData = result.ViewData,
                    ViewEngineCollection = result.ViewEngineCollection,
                    ViewName = result.ViewName,
                };
            }
        }
    }
}