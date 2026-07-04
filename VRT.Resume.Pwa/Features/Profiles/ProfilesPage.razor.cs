using Microsoft.AspNetCore.Components;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Pwa.Services;

namespace VRT.Resume.Pwa.Features.Profiles;

[Route(Routes.Profiles.List)]
public partial class ProfilesPage : IProfileExemptPage
{
    [Inject] private LocalProfileService ProfileService { get; set; } = null!;
    [Inject] private DummyCurrentUserService ProfileContext { get; set; } = null!;
    [Inject] private ICurrentUserService CurrentUser { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;

    private readonly List<LocalProfileDto> _profiles = [];
    private bool _loading = true;

    protected override async Task OnInitializedAsync() => await LoadProfilesAsync();

    private async Task LoadProfilesAsync()
    {
        _loading = true;
        try
        {
            _profiles.Clear();
            _profiles.AddRange(await ProfileService.GetAllAsync());
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task SelectProfileAsync(string userId)
    {
        await ProfileContext.SetContextAsync(userId);
        Navigation.NavigateTo(Routes.Home);
    }
}
