using Microsoft.AspNetCore.Components;
using VRT.Resume.Pwa.Features.Profiles;
using VRT.Resume.Pwa.Layout;

namespace VRT.Resume.Pwa.Pages;

[Route(Routes.About)]
public partial class About : IProfileExemptPage
{
    private static readonly string[] _featureKeys =
    [
        LabelNames.AboutFeatureOffline,
        LabelNames.AboutFeatureLocalProfiles,
        LabelNames.AboutFeaturePersonData,
        LabelNames.AboutFeatureResumeVariants,
        LabelNames.AboutFeatureBackup,
        LabelNames.AboutFeatureLanguages,
        LabelNames.AboutFeatureOpenSource,
    ];
}