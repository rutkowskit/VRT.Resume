using Microsoft.AspNetCore.Components;
using VRT.Resume.Pwa.Services;

namespace VRT.Resume.Pwa.Layout;

public partial class CultureSelector : IDisposable
{
    [Inject] private PwaCultureService CultureService { get; set; } = null!;

    private string _selectedCulture = string.Empty;

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

    private Task OnCultureChangedAsync(string culture)
    {
        CultureService.SetCurrentCulture(culture);
        _selectedCulture = culture;
        StateHasChanged();
        return Task.CompletedTask;
    }

    public void Dispose() => CultureService.CultureChanged -= OnCultureChanged;
}