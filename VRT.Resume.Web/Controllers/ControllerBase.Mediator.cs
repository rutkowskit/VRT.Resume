using CSharpFunctionalExtensions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace VRT.Resume.Web.Controllers
{
    // Partial with mediator helper methods
    public  abstract partial class ControllerBase : Controller
    {
        /// <summary>
        /// Method wraps Mediator.Send method and handles validation exception from mediator pipeline
        /// </summary>
        /// <typeparam name="TRequest">Request type parameter</typeparam>
        /// <param name="request">Request to send</param>
        /// <returns>Result of the request</returns>
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

        /// <summary>
        /// Method sets model state error based on validation errors from fluent validator
        /// </summary>
        /// <param name="vex">Validation exception</param>
        private void SetModelStateErrors(ValidationException vex)
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