using System.Web.Mvc;
using MediatR;

namespace VRT.Resume.Web.Controllers
{
    public abstract class PersonEditControllerBase : ControllerBase
    {
        protected PersonEditControllerBase(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        public virtual ActionResult Cancel() => ToProfile();
              
       
        protected ActionResult ToProfile(string selectedTab=TabNames.Profile)
        {
            TempData[TempDataKeys.TabName] = selectedTab;            
            return RedirectToActionPermanent("Index", "Person");
        }
        protected ActionResult ToProfileAfterSave(string selectedTab = TabNames.Profile)
        {
            SetSuccess();
            TempData[TempDataKeys.TabName] = selectedTab;
            return RedirectToActionPermanent("Index", "Person");
        }
    }
}