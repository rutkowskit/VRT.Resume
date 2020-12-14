using CSharpFunctionalExtensions;
using System.Web.Mvc;

namespace VRT.Resume.Web.Controllers
{
    // Partial for handling redirections
    partial class ControllerBase
    {
        protected ActionResult ToActionResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
                return View(result.Value);
            SetError(result.Error);
            return View();
        }

        protected ActionResult ToRequestReferer()
        {
            return new RedirectResult(Request.UrlReferrer.AbsoluteUri);
        }

        protected virtual ActionResult ToReturnUrl()
        {
            var url = TempData[TempDataKeys.ReturnUrl]?.ToString();                
            return string.IsNullOrWhiteSpace(url)
                ? null
                : new RedirectResult(url);
        }

        protected ActionResult ToHome()
        {
            return Redirect("~/");
        }
        protected ActionResult ToProfile(string selectedTab = TabNames.Profile)
        {
            TempData[TempDataKeys.TabName] = selectedTab;
            return RedirectToActionPermanent("Index", "Person");
        }
    }
}