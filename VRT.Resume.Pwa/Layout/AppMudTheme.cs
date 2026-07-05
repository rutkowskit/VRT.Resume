using MudBlazor;

namespace VRT.Resume.Pwa.Layout;

/// <summary>
/// Professional document-studio palette for the offline CV generator.
/// </summary>
public static class AppMudTheme
{
    public const string PrimaryHex = "#1B4D72";
    public const string AppBarHex = "#143A5C";
    public const string LightChromeHex = "#143A5C";
    public const string DarkChromeHex = "#0B1220";

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
        PaletteDark = new PaletteDark
        {
            Primary = "#5B9BD5",
            Secondary = "#22D3EE",
            Tertiary = "#94A3B8",
            Info = "#38BDF8",
            Success = "#34D399",
            Warning = "#FBBF24",
            Error = "#F87171",
            Dark = "#020617",

            TextPrimary = "#E2E8F0",
            TextSecondary = "#94A3B8",
            TextDisabled = "rgba(226, 232, 240, 0.38)",

            ActionDefault = "#94A3B8",
            ActionDisabled = "rgba(226, 232, 240, 0.26)",
            ActionDisabledBackground = "rgba(226, 232, 240, 0.12)",

            Background = "#0B1220",
            BackgroundGray = "#111827",
            Surface = "#1E293B",

            DrawerBackground = "#111827",
            DrawerText = "#CBD5E1",
            DrawerIcon = "#94A3B8",

            AppbarBackground = "#0F172A",
            AppbarText = "#F8FAFC",

            LinesDefault = "#334155",
            LinesInputs = "#475569",
            TableLines = "#334155",
            Divider = "#334155",
            DividerLight = "rgba(148, 163, 184, 0.12)",

            PrimaryContrastText = "#0B1220",
            SecondaryContrastText = "#0B1220",
            TertiaryContrastText = "#0B1220",
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