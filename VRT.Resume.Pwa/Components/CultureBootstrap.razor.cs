using Microsoft.AspNetCore.Components;
using VRT.Resume.Pwa.Services;

namespace VRT.Resume.Pwa.Components;

/// <summary>
/// Restores culture from localStorage after the Blazor circuit and JS interop are ready.
/// </summary>
public partial class CultureBootstrap : ComponentBase
{
    [Inject] private PwaCultureService CultureService { get; set; } = null!;

    private bool _initialized;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender || _initialized)
            return;

        _initialized = true;
        await CultureService.InitializeFromStorageAsync();
    }
}