using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;

namespace VRT.Resume.Mvc.Controllers
{
    // Partial for handling redirections
    partial class ControllerBase
    {
        protected ActionResult ToActionResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
                return View(result.Value);
            SetError(result.Error);
            return DoRedirection(View());
        }

        protected ActionResult ToRequestReferer()
            => DoRedirection(new RedirectResult(Request.GetReferer().AbsoluteUri));        

        protected virtual ActionResult ToReturnUrl()
        {
            var url = TempData[TempDataKeys.ReturnUrl]?.ToString();            
            return string.IsNullOrWhiteSpace(url)
                ? null
                : DoRedirection(new RedirectResult(url));
        }

        protected ActionResult ToHome() => DoRedirection(Redirect("~/"));        
        protected ActionResult ToProfile(string selectedTab = TabNames.Profile)
        {            
            TempData[TempDataKeys.TabName] = selectedTab;
            return DoRedirection(RedirectToActionPermanent("Index", "Person"));
        }
        private ActionResult DoRedirection(ActionResult result)
        {            
            TempData[TempDataKeys.ReturnUrl] = null;
            return result;
        }
    }
}