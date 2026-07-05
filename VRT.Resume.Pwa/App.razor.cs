using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using VRT.Resume.Pwa.Features.Mediator;
using VRT.Resume.Pwa.Services;
using VRT.Resume.Resources;

namespace VRT.Resume.Pwa;

public partial class App
{
    [Inject] private PwaStartupState Startup { get; set; } = null!;
    [Inject] private IJSRuntime Js { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender || !Startup.IsReady)
        {
            return;
        }

        try
        {
            var updated = await Js.InvokeAsync<bool>("__pwaConsumeUpdateFlag");
            if (!updated)
            {
                return;
            }

            Snackbar.Add(MessageKeys.AppUpdatedSuccess.GetMessageText(), Severity.Info, options =>
            {
                options.VisibleStateDuration = 5000;
                options.ShowCloseIcon = true;
            });
        }
        catch (JSException)
        {
            // Boot script not available (e.g. test host).
        }
    }
}