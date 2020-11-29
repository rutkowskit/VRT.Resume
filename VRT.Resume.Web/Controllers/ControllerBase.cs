using CSharpFunctionalExtensions;
using MediatR;
using System;
using System.Web.Mvc;

namespace VRT.Resume.Web.Controllers
{
    public  abstract partial class ControllerBase : Controller
    {
        protected IMediator Mediator { get; }

        protected ControllerBase(IMediator mediator)
        {
            Mediator = mediator;
        }
        
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            SetupThreadCulture();
            // AddLanguageCookie(lang); TODO: add language settings
            return base.BeginExecuteCore(callback, state);
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