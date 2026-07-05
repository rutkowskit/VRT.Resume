using Microsoft.AspNetCore.Components;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Pwa.Services;
using VRT.Resume.Resources;

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

    private string GetActiveProfileDisplayName()
        => ActiveProfile.HasActiveContext
            ? _activeProfileName ?? "…"
            : LabelNames.ActiveProfileNone.GetLabelText();

    private string GetActiveProfileNameClass()
        => ActiveProfile.HasActiveContext
            ? "nav-menu-profile__name"
            : "nav-menu-profile__name mud-text-secondary";

    public void Dispose()
    {
        ActiveProfile.ContextChanged -= OnContextChanged;
        CultureService.CultureChanged -= OnCultureChanged;
    }
}