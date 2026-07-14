using System.Drawing;

namespace VuelosCRSA
{
    public static class ThemeColors
    {
        // ── Base ──
        public static Color Background      => Color.FromArgb(18, 18, 18);
        public static Color Surface         => Color.FromArgb(22, 22, 22);
        public static Color Card            => Color.FromArgb(28, 28, 28);
        public static Color Border          => Color.FromArgb(50, 50, 50);

        // ── Text ──
        public static Color TextPrimary     => Color.FromArgb(200, 200, 200);
        public static Color TextSecondary   => Color.FromArgb(130, 130, 130);
        public static Color TextMuted       => Color.FromArgb(90, 90, 90);

        // ── Accent ──
        public static Color Accent          => Color.FromArgb(16, 163, 127);
        public static Color AccentHover     => Color.FromArgb(19, 185, 145);
        public static Color AccentDark      => Color.FromArgb(12, 122, 95);

        // ── Status ──
        public static Color Success         => Color.FromArgb(16, 163, 127);
        public static Color Error           => Color.FromArgb(220, 50, 50);
        public static Color Warning         => Color.FromArgb(220, 160, 30);
        public static Color Info            => Color.FromArgb(30, 130, 210);

        // ── Grid ──
        public static Color GridBg          => Color.FromArgb(18, 18, 18);
        public static Color GridHeader      => Color.FromArgb(28, 28, 28);
        public static Color GridAlt         => Color.FromArgb(28, 28, 28);
        public static Color GridLine        => Color.FromArgb(45, 45, 45);

        // ── Controls ──
        public static Color InputBg         => Color.FromArgb(30, 30, 30);
        public static Color ToolbarBg       => Color.FromArgb(25, 25, 25);
        public static Color ScrollbarBg     => Color.FromArgb(30, 30, 30);
        public static Color ScrollbarThumb  => Color.FromArgb(60, 60, 60);
    }
}
