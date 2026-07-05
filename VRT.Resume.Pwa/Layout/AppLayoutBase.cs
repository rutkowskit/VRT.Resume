using Microsoft.AspNetCore.Components;
using VRT.Resume.Pwa.Services;

namespace VRT.Resume.Pwa.Layout;

public abstract class AppLayoutBase : LayoutComponentBase, IDisposable
{
    [Inject] private PwaCultureService CultureService { get; set; } = null!;

    /// <summary>
    /// Responsive drawer: open by default on desktop; MudBlazor closes it below <see cref="Breakpoint.Md"/>.
    /// </summary>
    protected bool DrawerOpen { get; set; } = true;

    protected override void OnInitialized() => CultureService.CultureChanged += OnCultureChanged;

    protected void DrawerToggle() => DrawerOpen = !DrawerOpen;

    private void OnCultureChanged() => _ = InvokeAsync(StateHasChanged);

    public void Dispose() => CultureService.CultureChanged -= OnCultureChanged;
}