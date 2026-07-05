using System.Text;
using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using VRT.Resume.Resources;

namespace VRT.Resume.Pwa.Features.Mediator;

public sealed class MediatorSender(IMediator mediator, UserNotificationService notifications)
{
    public Task<MediatorSendOutcome<T>> SendAsync<T>(
        IRequest<Result<T>> request,
        CancellationToken cancellationToken = default) =>
        SendAsync(request, MediatorSendOptions.Default, cancellationToken);

    public Task<MediatorSendOutcome<T>> SendAsync<T>(
        IRequest<Result<T>> request,
        MediatorSendOptions options,
        CancellationToken cancellationToken = default) =>
        SendCoreAsync(
            () => mediator.Send(request, cancellationToken),
            options,
            cancellationToken);

    public Task<MediatorSendOutcome> SendAsync(
        IRequest<Result> request,
        CancellationToken cancellationToken = default) =>
        SendAsync(request, MediatorSendOptions.Default, cancellationToken);

    public Task<MediatorSendOutcome> SendAsync(
        IRequest<Result> request,
        MediatorSendOptions options,
        CancellationToken cancellationToken = default) =>
        SendCoreAsync(
            () => mediator.Send(request, cancellationToken),
            options,
            cancellationToken);

    public Task<T> SendQueryAsync<T>(
        IRequest<T> request,
        CancellationToken cancellationToken = default) =>
        mediator.Send(request, cancellationToken);

    private async Task<MediatorSendOutcome<T>> SendCoreAsync<T>(
        Func<Task<Result<T>>> send,
        MediatorSendOptions options,
        CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        try
        {
            var result = await send();
            NotifyResult(result.IsSuccess, result.IsFailure ? result.Error : null, options);
            return MediatorSendOutcome<T>.FromResult(result);
        }
        catch (ValidationException vex)
        {
            var message = FormatValidationMessage(vex);
            var fieldErrors = ToFieldErrors(vex);
            if (options.NotifyOnFailure)
                notifications.ShowError(message);
            return MediatorSendOutcome<T>.FromValidation(message, fieldErrors);
        }
    }

    private async Task<MediatorSendOutcome> SendCoreAsync(
        Func<Task<Result>> send,
        MediatorSendOptions options,
        CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        try
        {
            var result = await send();
            NotifyResult(result.IsSuccess, result.IsFailure ? result.Error : null, options);
            return MediatorSendOutcome.FromResult(result);
        }
        catch (ValidationException vex)
        {
            var message = FormatValidationMessage(vex);
            var fieldErrors = ToFieldErrors(vex);
            if (options.NotifyOnFailure)
                notifications.ShowError(message);
            return MediatorSendOutcome.FromValidation(message, fieldErrors);
        }
    }

    private void NotifyResult(bool isSuccess, string? error, MediatorSendOptions options)
    {
        if (isSuccess && options.NotifyOnSuccess)
        {
            var message = options.SuccessMessage
                ?? MessageKeys.DataSavedSuccess.GetMessageText();
            notifications.ShowSuccess(message);
        }
        else if (!isSuccess && options.NotifyOnFailure && !string.IsNullOrWhiteSpace(error))
        {
            notifications.ShowError(error);
        }
    }

    private static string FormatValidationMessage(ValidationException vex)
    {
        var builder = new StringBuilder(MessageKeys.ValidationFailed.GetMessageText())
            .Append(' ');

        var errors = vex.Errors.ToArray();
        if (errors.Length == 0)
            return builder.ToString().Trim();

        var count = 0;
        foreach (var error in errors)
        {
            builder.AppendLine($"{error.PropertyName} - {error.ErrorMessage}");
            if (++count > 10)
            {
                builder.AppendLine("[...]");
                break;
            }
        }

        return builder.ToString().Trim();
    }

    private static IReadOnlyDictionary<string, string[]> ToFieldErrors(ValidationException vex) =>
        vex.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).Distinct().ToArray());
}