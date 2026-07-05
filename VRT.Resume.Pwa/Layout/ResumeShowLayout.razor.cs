using Microsoft.AspNetCore.Components;
using VRT.Resume.Pwa.Services;

namespace VRT.Resume.Pwa.Layout;

public partial class ResumeShowLayout : IDisposable
{
    [Inject] private PwaCultureService CultureService { get; set; } = null!;

    private bool _drawerOpen = true;

    protected override void OnInitialized() => CultureService.CultureChanged += OnCultureChanged;

    private void OnCultureChanged() => _ = InvokeAsync(StateHasChanged);

    private void DrawerToggle() => _drawerOpen = !_drawerOpen;

    public void Dispose() => CultureService.CultureChanged -= OnCultureChanged;
}