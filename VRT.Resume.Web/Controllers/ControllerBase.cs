using CSharpFunctionalExtensions;
using MediatR;
using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace VRT.Resume.Web.Controllers
{
    public abstract class ControllerBase : Controller
    {
        protected IMediator Mediator { get; }

        protected ControllerBase(IMediator mediator)
        {
            Mediator = mediator;
        }

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
        
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {               
            var langCookie = Request?.Cookies["culture"];
            var lang = langCookie == null
                ? GetDefaultLanguage()
                : langCookie.Value;
            
            SetThreadLanguage(lang);
            // AddLanguageCookie(lang); TODO: add language settings
            return base.BeginExecuteCore(callback, state);
        }

        private string GetDefaultLanguage()
        {
            var userLanguage = Request?.UserLanguages;
            var userLang = userLanguage != null 
                ? userLanguage[0] 
                : "";

            return string.IsNullOrWhiteSpace(userLang)
                ? "pl-PL"
                : userLang;
        }

        private void AddLanguageCookie(string language)
        {
            if (Response != null)
            {
                var langCookie = new HttpCookie("culture", language)
                {
                    Expires = DateTime.Now.AddYears(1)
                };
                Response.Cookies.Add(langCookie);
            }
        }

        private void SetThreadLanguage(string language)
        {
            try
            {
                var culture = new CultureInfo(language);
                var cultureInfo = CultureInfo.CreateSpecificCulture(culture.Name);
                Thread.CurrentThread.CurrentCulture = cultureInfo;
                Thread.CurrentThread.CurrentUICulture = cultureInfo;                
            }
            catch
            {
                // ignore
            }
        }

        protected ControllerBase SetResult(Result result)
        {
            return result.IsFailure
                ? SetError(result.Error)
                : SetSuccess();            
        }

        protected ControllerBase SetSuccess(string message = null)
        {
            TempData[TempDataKeys.SuccessMsg] = message ?? Resources.MessageResource.DataSavedSuccess;
            TempData.Remove(TempDataKeys.ErrorMsg);
            return this;
        }

        protected ControllerBase SetError(string message)
        {
            TempData[TempDataKeys.ErrorMsg] = message ?? Resources.MessageResource.ErrorOccured;
            TempData.Remove(TempDataKeys.SuccessMsg);
            return this;
        }
    }
}