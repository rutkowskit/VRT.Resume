using Microsoft.AspNetCore.Components;
using VRT.Resume.Pwa.Services;

namespace VRT.Resume.Pwa.Components;

/// <summary>
/// Re-renders when <see cref="PwaCultureService"/> culture changes (required for localized Mud* labels).
/// </summary>
public abstract class CultureAwareComponentBase : ComponentBase, IDisposable
{
    [Inject] protected PwaCultureService CultureService { get; set; } = null!;

    protected override void OnInitialized() =>
        CultureService.CultureChanged += HandleCultureChanged;

    private void HandleCultureChanged() => _ = InvokeAsync(StateHasChanged);

    public void Dispose() => CultureService.CultureChanged -= HandleCultureChanged;
}