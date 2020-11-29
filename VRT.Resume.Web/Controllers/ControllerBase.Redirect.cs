﻿using CSharpFunctionalExtensions;
using System.Web.Mvc;

namespace VRT.Resume.Web.Controllers
{
    // Partial for handling current thread culture
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

        protected ActionResult ToHome()
        {
            return Redirect("~/");
        }
    }
}