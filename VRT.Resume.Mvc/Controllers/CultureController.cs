using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using VRT.Resume.Application.Common.Queries.GetSupportedLanguages;
using System.Threading.Tasks;
using VRT.Resume.Application.Common.Commands.SetUserLanguage;

namespace VRT.Resume.Mvc.Controllers
{
    [AllowAnonymous]
    public class CultureController : ControllerBase
    {        
        public CultureController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        public async Task<ActionResult> Change()
        {
            var query = new GetSupportedLanguagesQuery();
            var result = await Send(query);
            return ToActionResult(result);            
        }

        [HttpPost]
        public async Task<ActionResult> Change(string lang)
        {
            var cmd = new SetUserCultureCommand(lang);
            await Mediator.Send(cmd);
            return Cancel();
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