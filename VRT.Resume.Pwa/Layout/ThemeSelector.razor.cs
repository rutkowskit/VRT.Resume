using Microsoft.AspNetCore.Components;
using MudBlazor;
using VRT.Resume.Pwa.Services;
using VRT.Resume.Resources;

namespace VRT.Resume.Pwa.Layout;

public partial class ThemeSelector : IDisposable
{
    [Inject] private PwaThemeService ThemeService { get; set; } = null!;

    private string _icon = Icons.Material.Outlined.DarkMode;
    private string _title = string.Empty;

    protected override void OnInitialized()
    {
        ThemeService.ThemeChanged += OnThemeChanged;
        RefreshState();
    }

    private void ToggleTheme() => ThemeService.ToggleDarkMode();

    private void OnThemeChanged()
    {
        RefreshState();
        _ = InvokeAsync(StateHasChanged);
    }

    private void RefreshState()
    {
        var isDark = ThemeService.IsDarkMode;
        _icon = isDark ? Icons.Material.Outlined.LightMode : Icons.Material.Outlined.DarkMode;
        _title = isDark
            ? LabelNames.ThemeSwitchToLight.GetLabelText()
            : LabelNames.ThemeSwitchToDark.GetLabelText();
    }

    public void Dispose() => ThemeService.ThemeChanged -= OnThemeChanged;
}