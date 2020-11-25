using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using CSharpFunctionalExtensions;
using FluentValidation;
using FluentValidation.Results;
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

        [HttpGet]
        public virtual ActionResult ConfirmDelete(int id)
        {
            var ctrl= ControllerContext.RouteData.Values["controller"]?.ToString();
            var data = new Models.EditDeleteToolbarData(ctrl, id);
            return View("ConfirmDelete", data);
        }

        protected ActionResult ToProfile(string selectedTab=TabNames.Profile)
        {
            TempData[TempDataKeys.TabName] = selectedTab;            
            return RedirectToActionPermanent("Index", "Person");
        }
        protected ActionResult ToProfileAfterSave(string selectedTab = TabNames.Profile)
        {            
            TempData[TempDataKeys.TabName] = selectedTab;
            return RedirectToActionPermanent("Index", "Person");
        }


        protected async Task<Result<TModel>> Send<TRequest, TModel>(TRequest request)
            where TRequest : IRequest<Result<TModel>>
        {
            try
            {
                var result=await Mediator.Send(request);
                if (result.IsSuccess)
                    return result.Value;
                return Result.Failure<TModel>(result.Error);
            }
            catch (ValidationException vex)
            {
                SetModelStateErrors(vex);
                return Result.Failure<TModel>(Resources.MessageResource.ValidationFailed);
            }
        }

        protected async Task<Result> Send<TRequest>(TRequest request)
            where TRequest : IRequest<Result>
        {
            try
            {
                var result = await Mediator.Send(request);
                SetResult(result);
                return result.IsSuccess
                    ? result
                    : Result.Failure(result.Error);                
            }
            catch (ValidationException vex)
            {
                SetModelStateErrors(vex);
                SetError(Resources.MessageResource.ValidationFailed);
                return Result.Failure(Resources.MessageResource.ValidationFailed);
            }
        }
        
        private void SetModelStateErrors(ValidationException vex )
        {
            var errors = vex?.Errors?.ToArray() ?? new ValidationFailure[0];
            if (errors.Length == 0)
                return;
            foreach (var err in errors)
            {
                ModelState.AddModelError(err.PropertyName, err.ErrorMessage);
            }
        }
    }
}