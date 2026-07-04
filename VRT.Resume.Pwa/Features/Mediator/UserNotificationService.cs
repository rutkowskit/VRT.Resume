using MudBlazor;

namespace VRT.Resume.Pwa.Features.Mediator;

public sealed class UserNotificationService(ISnackbar snackbar)
{
    public void ShowSuccess(string message)
    {
        snackbar.Add(message, Severity.Success);
    }

    public void ShowError(string message)
    {
        snackbar.Add(message, Severity.Error, options =>
        {
            options.VisibleStateDuration = 6000;
            options.ShowCloseIcon = true;
        });
    }
}