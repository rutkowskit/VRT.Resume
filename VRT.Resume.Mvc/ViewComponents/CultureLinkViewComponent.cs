using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Application.Common.Queries.GetSupportedLanguages;

namespace VRT.Resume.Mvc.ViewComponents
{
    public sealed class CultureLinkViewComponent : ViewComponent
    {
        private readonly ICultureService _cultureService;

        public CultureLinkViewComponent(ICultureService cultureService)
        {
            _cultureService = cultureService ?? throw new ArgumentNullException(nameof(cultureService));
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            await Task.Yield();
            var lang = _cultureService.GetCurrentCulture();
            var key = lang.Length >= 2 ? lang.Substring(0, 2) : "pl";
            var supportedLangDic = _cultureService.GetSupportedLanguages();

            var vm = supportedLangDic.TryGetValue(key, out var culture)
                ? culture
                : supportedLangDic.Values.First();
            TempData[TempDataKeys.CultureKey] = vm.key;
            TempData[TempDataKeys.ReturnUrlFromCulture] = Request.Headers["Referer"].ToString();
            return View(new LanguageVM(vm.key, vm.caption));            
        }       
    }
}
