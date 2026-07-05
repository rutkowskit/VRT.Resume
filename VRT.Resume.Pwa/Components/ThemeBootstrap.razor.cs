using Microsoft.AspNetCore.Components;
using VRT.Resume.Pwa.Services;

namespace VRT.Resume.Pwa.Components;

/// <summary>
/// Restores dark-mode preference from localStorage after JS interop is ready.
/// </summary>
public partial class ThemeBootstrap : ComponentBase
{
    [Inject] private PwaThemeService ThemeService { get; set; } = null!;

    private bool _initialized;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender || _initialized)
            return;

        _initialized = true;
        await ThemeService.InitializeFromStorageAsync();
    }
}