using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using VRT.Resume.Pwa.Features.Profiles;
using VRT.Resume.Pwa.Services;

namespace VRT.Resume.Pwa.Components;

public partial class ProfileRequiredRouteView
{
    [Inject] private NavigationManager Navigation { get; set; } = null!;
    [Inject] private IActiveProfileContext ActiveProfile { get; set; } = null!;

    [Parameter] public RouteData? RouteData { get; set; }
    [Parameter] public Type? DefaultLayout { get; set; }

    private bool RequiresProfile =>
        RouteData?.PageType is { } pageType
        && !typeof(IProfileExemptPage).IsAssignableFrom(pageType);

    protected override void OnParametersSet()
    {
        if (RequiresProfile && !ActiveProfile.HasActiveContext)
            Navigation.NavigateTo(Routes.Profiles.List, replace: true);
    }
}