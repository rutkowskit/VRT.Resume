using MudBlazor;

namespace VRT.Resume.Pwa.Components;

internal static class PwaDialogOptions
{
    /// <summary>Form editors: no close on backdrop click (replaces DisableBackdropClick="true").</summary>
    public static DialogOptions Form { get; } = new()
    {
        BackdropClick = false,
    };

    /// <summary>Wide skill picker dialog.</summary>
    public static DialogOptions LargeForm { get; } = new()
    {
        BackdropClick = false,
        MaxWidth = MaxWidth.Large,
        FullWidth = true,
    };

    /// <summary>Resume add/edit — pass to DialogService.ShowAsync (MudDialog Options alone is not applied).</summary>
    public static DialogOptions ResumeForm { get; } = new()
    {
        BackdropClick = false,
        MaxWidth = MaxWidth.Large,
        FullWidth = true,
    };
}