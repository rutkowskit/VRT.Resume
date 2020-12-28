using CSharpFunctionalExtensions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VRT.Resume.Resources;
using System.Text;

namespace VRT.Resume.Mvc.Controllers
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
        protected async Task<Result<TResponse>> Send<TResponse>(IRequest<Result<TResponse>> request)            
        {
            try
            {
                return await Mediator.Send(request);                                
            }
            catch (ValidationException vex)
            {
                SetModelStateErrors(vex);
                var msg = GetFullValidationMessage(vex);                    
                SetError(msg);
                return Result.Failure<TResponse>(msg);                    
            }            
        }

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
                return result;
            }
            catch (ValidationException vex)
            {
                SetModelStateErrors(vex);
                var msg = GetFullValidationMessage(vex);
                SetError(msg);
                return Result.Failure(msg);
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

        private static string GetFullValidationMessage(ValidationException vex)
        {
            var result = new StringBuilder(MsgNames.ValidationFailed.GetMessageText())
                .Append(". ");
            var errors = vex?.Errors?.ToArray() ?? new ValidationFailure[0];
            if (errors.Length == 0)
                return result.ToString();
            var cnt = 0;
            foreach (var err in errors)
            {
                result.AppendLine($"{err.PropertyName} - {err.ErrorMessage}");
                if (++cnt > 10)
                {
                    result.AppendLine($"[...]");
                    break;
                } 
            }
            return result.ToString();
        }
    }
}