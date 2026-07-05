using Microsoft.AspNetCore.Components;
using VRT.Resume.Pwa.Services;

namespace VRT.Resume.Pwa.Components;

public partial class AppThemeProvider : IDisposable
{
    [Inject] private PwaThemeService ThemeService { get; set; } = null!;

    protected override void OnInitialized() => ThemeService.ThemeChanged += OnThemeChanged;

    private void OnThemeChanged() => _ = InvokeAsync(StateHasChanged);

    public void Dispose() => ThemeService.ThemeChanged -= OnThemeChanged;
}