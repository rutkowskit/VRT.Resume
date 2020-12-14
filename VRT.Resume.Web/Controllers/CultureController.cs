using System.Web.Mvc;
using MediatR;
using VRT.Resume.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace VRT.Resume.Web.Controllers
{
    [AllowAnonymous]
    public class CultureController : ControllerBase
    {
        private static readonly Dictionary<string, CultureSettingsViewModel> SupportedLangDic
            = new Dictionary<string, CultureSettingsViewModel>()
            {
                ["pl"] = new CultureSettingsViewModel("pl", "Polski"),
                ["en"] = new CultureSettingsViewModel("en", "English")
            };
        public CultureController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        public ActionResult Index()
        {
            var lang = GetCurrentLanguage();
            var key = lang.Length >= 2 ? lang.Substring(0, 2) : "pl";
            var vm = SupportedLangDic.TryGetValue(key, out var culture)
                ? culture
                : SupportedLangDic.Values.First();
            TempData[TempDataKeys.CultureKey] = vm.Key;
            TempData[TempDataKeys.ReturnUrlFromCulture] = Request.UrlReferrer;

            return PartialView(vm);
        }

        [HttpGet]
        public ActionResult Change()
        {
            return View(SupportedLangDic.Values);
        }

        [HttpPost]
        public ActionResult Change(string lang)
        {
            AddLanguageCookie(lang);
            return ToReturnUrl() ?? ToHome();
        }        
        public ActionResult Cancel() => ToReturnUrl() ?? ToHome();

        protected override ActionResult ToReturnUrl()
        {
            var url = TempData[TempDataKeys.ReturnUrlFromCulture]?.ToString();
            return string.IsNullOrWhiteSpace(url)
                ? null
                : new RedirectResult(url);
        }

    }
}