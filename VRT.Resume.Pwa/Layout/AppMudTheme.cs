using MudBlazor;

namespace VRT.Resume.Pwa.Layout;

/// <summary>
/// Professional document-studio palette for the offline CV generator.
/// </summary>
public static class AppMudTheme
{
    public const string PrimaryHex = "#1B4D72";
    public const string AppBarHex = "#143A5C";

    public static MudTheme Theme { get; } = Create();

    private static MudTheme Create() => new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = PrimaryHex,
            Secondary = "#0E7490",
            Tertiary = "#475569",
            Info = "#0284C7",
            Success = "#059669",
            Warning = "#D97706",
            Error = "#DC2626",
            Dark = "#0F2D46",

            TextPrimary = "#0F172A",
            TextSecondary = "#475569",
            TextDisabled = "rgba(15, 23, 42, 0.38)",

            ActionDefault = "#64748B",
            ActionDisabled = "rgba(15, 23, 42, 0.26)",
            ActionDisabledBackground = "rgba(15, 23, 42, 0.12)",

            Background = "#F0F4F8",
            BackgroundGray = "#E8EEF4",
            Surface = "#FFFFFF",

            DrawerBackground = "#F8FAFC",
            DrawerText = "#334155",
            DrawerIcon = "#64748B",

            AppbarBackground = AppBarHex,
            AppbarText = "#F8FAFC",

            LinesDefault = "#E2E8F0",
            LinesInputs = "#CBD5E1",
            TableLines = "#E2E8F0",
            Divider = "#E2E8F0",
            DividerLight = "rgba(15, 45, 70, 0.06)",

            PrimaryContrastText = "#FFFFFF",
            SecondaryContrastText = "#FFFFFF",
            TertiaryContrastText = "#FFFFFF",
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "10px",
            AppbarHeight = "56px",
        },
        Typography = new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily =
                [
                    "system-ui",
                    "-apple-system",
                    "Segoe UI",
                    "Roboto",
                    "Helvetica Neue",
                    "Arial",
                    "sans-serif",
                ],
            },
        },
    };
}