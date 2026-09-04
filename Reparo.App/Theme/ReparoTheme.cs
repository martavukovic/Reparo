using MudBlazor;

namespace Reparo.App.Theme;

public static class ReparoTheme
{
    public static MudTheme Create() => new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#5B6AD0",
            PrimaryDarken = "#4756C0",
            PrimaryLighten = "#7B8AE0",
            Secondary = "#7C3AED",
            AppbarBackground = "#FFFFFF",
            AppbarText = "#1A1A2E",
            Background = "#F4F5F9",
            Surface = "#FFFFFF",
            DrawerBackground = "#FFFFFF",
            DrawerText = "#1A1A2E",
            TextPrimary = "#1A1A2E",
            TextSecondary = "#6B7280",
            ActionDefault = "#6B7280",
            Divider = "#E5E7EB",
            Success = "#10B981",
            Warning = "#F59E0B",
            Error = "#EF4444",
            Info = "#3B82F6",
            TableHover = "#F9FAFB",
            TableStriped = "#F3F4F6",
        },
        PaletteDark = new PaletteDark
        {
            Primary = "#7B8AE0",
            PrimaryDarken = "#5B6AD0",
            PrimaryLighten = "#9BA8F0",
            Secondary = "#9D65F5",
            AppbarBackground = "#13131F",
            AppbarText = "#E5E7EB",
            Background = "#0D0D1A",
            Surface = "#13131F",
            DrawerBackground = "#13131F",
            DrawerText = "#E5E7EB",
            TextPrimary = "#F9FAFB",
            TextSecondary = "#9CA3AF",
            ActionDefault = "#9CA3AF",
            Divider = "#1F2937",
            Success = "#10B981",
            Warning = "#F59E0B",
            Error = "#EF4444",
            Info = "#3B82F6",
            TableHover = "#1A1A2E",
            TableStriped = "#161626",
        },
        Typography = new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = new[] { "Inter", "sans-serif" },
                FontSize = "0.875rem",
            },
            H4 = new H4Typography
            {
                FontFamily = new[] { "Inter", "sans-serif" },
                FontSize = "1.5rem",
                FontWeight = "700",
            },
            H5 = new H5Typography
            {
                FontFamily = new[] { "Inter", "sans-serif" },
                FontSize = "1.25rem",
                FontWeight = "600",
            },
            H6 = new H6Typography
            {
                FontFamily = new[] { "Inter", "sans-serif" },
                FontSize = "1rem",
                FontWeight = "600",
            },
            Subtitle1 = new Subtitle1Typography
            {
                FontSize = "0.9rem",
                FontWeight = "600",
            },
            Body1 = new Body1Typography
            {
                FontSize = "0.875rem",
            },
            Body2 = new Body2Typography
            {
                FontSize = "0.8125rem",
            },
            Caption = new CaptionTypography
            {
                FontSize = "0.75rem",
                FontWeight = "500",
            }
        },
    };
}