using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using VRT.Resume.Pwa.Features.Mediator;
using VRT.Resume.Pwa.Services;
using VRT.Resume.Resources;

namespace VRT.Resume.Pwa.Components;

public partial class StartupErrorView
{
    [Inject] private PwaStartupState Startup { get; set; } = null!;
    [Inject] private IJSRuntime Js { get; set; } = null!;

    private string _title = string.Empty;
    private string _body = string.Empty;
    private string _retry = string.Empty;

    protected override void OnInitialized()
    {
        if (Startup.IsAnotherTabError)
        {
            _title = MessageKeys.OpfsAnotherTabTitle.GetMessageText();
            _body = MessageKeys.OpfsAnotherTabBody.GetMessageText();
        }
        else
        {
            _title = MessageKeys.OpfsStartupFailedTitle.GetMessageText();
            _body = string.IsNullOrWhiteSpace(Startup.ErrorMessage)
                ? MessageKeys.ErrorOccured.GetMessageText()
                : Startup.ErrorMessage;
        }

        _retry = MessageKeys.OpfsRetryButton.GetMessageText();
    }

    private async Task ReloadAsync() => await Js.InvokeVoidAsync("location.reload");
}