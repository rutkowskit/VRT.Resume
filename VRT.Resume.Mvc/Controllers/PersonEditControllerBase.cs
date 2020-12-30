using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace VRT.Resume.Mvc.Controllers
{
    public abstract class PersonEditControllerBase : ControllerBase
    {
        protected PersonEditControllerBase(IMediator mediator) : base(mediator)
        {
        }
        
        public virtual ActionResult Add() => View();

        [HttpGet]
        public virtual ActionResult Cancel() => ToProfile();

        [HttpGet]
        public virtual ActionResult ConfirmDelete(int id)
        {
            var ctrl= ControllerContext.RouteData.Values["controller"]?.ToString();
            var data = new Models.EditDeleteToolbarData(ctrl, id);
            return View("ConfirmDelete", data);
        }                 
    }
}