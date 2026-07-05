using Microsoft.AspNetCore.Components;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Pwa.Services;

namespace VRT.Resume.Pwa.Layout;

public partial class NavMenu : IDisposable
{
    [Inject] private IActiveProfileContext ActiveProfile { get; set; } = null!;
    [Inject] private ICurrentUserService CurrentUser { get; set; } = null!;
    [Inject] private LocalProfileService ProfileService { get; set; } = null!;
    [Inject] private PwaCultureService CultureService { get; set; } = null!;

    private string? _activeProfileName;

    protected override void OnInitialized()
    {
        ActiveProfile.ContextChanged += OnContextChanged;
        CultureService.CultureChanged += OnCultureChanged;
    }

    protected override async Task OnInitializedAsync() => await LoadActiveProfileNameAsync();

    private void OnContextChanged()
    {
        _ = InvokeAsync(async () =>
        {
            await LoadActiveProfileNameAsync();
            StateHasChanged();
        });
    }

    private async Task LoadActiveProfileNameAsync()
    {
        if (!ActiveProfile.HasActiveContext)
        {
            _activeProfileName = null;
            return;
        }

        _activeProfileName = await ProfileService.GetDisplayNameAsync(CurrentUser.UserId);
    }

    private void OnCultureChanged() => _ = InvokeAsync(StateHasChanged);

    public void Dispose()
    {
        ActiveProfile.ContextChanged -= OnContextChanged;
        CultureService.CultureChanged -= OnCultureChanged;
    }
}