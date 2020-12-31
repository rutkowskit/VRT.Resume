using CSharpFunctionalExtensions;
using VRT.Resume.Resources;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace VRT.Resume.Mvc.Controllers
{
    public  abstract partial class ControllerBase : Controller
    {
        protected IMediator Mediator { get; }

        protected ControllerBase(IMediator mediator)
        {
            Mediator = mediator;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            SetupThreadCulture();
            base.OnActionExecuting(context);
        }
        
        partial void SetupThreadCulture();
       
        protected ControllerBase SetResult(Result result)            
        {
            return result.IsFailure
                ? SetError(result.Error)
                : SetSuccess();            
        }

        protected ControllerBase SetSuccess(string message = null)
        {
            TempData[TempDataKeys.SuccessMsg] = message ?? MsgNames.DataSavedSuccess.GetMessageText();                
            TempData.Remove(TempDataKeys.ErrorMsg);
            return this;
        }

        protected ControllerBase SetError(string message)
        {
            TempData[TempDataKeys.ErrorMsg] = message?? MsgNames.ErrorOccured.GetMessageText();            
            TempData.Remove(TempDataKeys.SuccessMsg);
            return this;
        }       
    }
}