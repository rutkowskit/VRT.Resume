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
            return View();
        }

        protected ActionResult ToRequestReferer()
        {
            return new RedirectResult(Request.GetReferer().AbsoluteUri);
        }

        protected virtual ActionResult ToReturnUrl()
        {
            var url = TempData[TempDataKeys.ReturnUrl]?.ToString();
            TempData[TempDataKeys.ReturnUrl] = null;
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