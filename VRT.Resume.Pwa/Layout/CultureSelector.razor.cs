using Microsoft.AspNetCore.Components;
using MudBlazor;
using VRT.Resume.Pwa.Services;
using VRT.Resume.Resources;

namespace VRT.Resume.Pwa.Layout;

public partial class CultureSelector : IDisposable
{
    [Inject] private PwaCultureService CultureService { get; set; } = null!;

    private string _selectedCulture = string.Empty;
    private bool _menuOpen;

    protected override void OnInitialized()
    {
        _selectedCulture = CultureService.GetCurrentCulture();
        CultureService.CultureChanged += OnCultureChanged;
    }

    private void OnCultureChanged()
    {
        _selectedCulture = CultureService.GetCurrentCulture();
        _ = InvokeAsync(StateHasChanged);
    }

    private void SelectCulture(string culture)
    {
        if (string.Equals(_selectedCulture, culture, StringComparison.OrdinalIgnoreCase))
        {
            _menuOpen = false;
            return;
        }

        CultureService.SetCurrentCulture(culture);
        _selectedCulture = culture;
        _menuOpen = false;
        StateHasChanged();
    }

    private string GetActivatorLabel()
    {
        var (_, caption) = CultureService.GetCurrentLanguage();
        return $"{LabelNames.Language.GetLabelText()}: {caption}";
    }

    private string? GetListIcon(string cultureKey)
        => string.Equals(_selectedCulture, cultureKey, StringComparison.OrdinalIgnoreCase)
            ? Icons.Material.Outlined.Check
            : null;

    private string? GetListItemClass(string cultureKey)
        => string.Equals(_selectedCulture, cultureKey, StringComparison.OrdinalIgnoreCase)
            ? "culture-selector__item--active"
            : null;

    public void Dispose() => CultureService.CultureChanged -= OnCultureChanged;
}