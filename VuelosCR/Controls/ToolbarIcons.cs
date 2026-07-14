using System.Drawing;
using System.Drawing.Drawing2D;

namespace VuelosCRSA.Controls
{
    public static class ToolbarIcons
    {
        private static readonly Color _color = Color.FromArgb(210, 210, 210);

        public static Image Delete { get; } = MakeIcon(DrawDelete);
        public static Image ClearAll { get; } = MakeIcon(DrawClearAll);
        public static Image Export { get; } = MakeIcon(DrawExport);
        public static Image More { get; } = MakeIcon(DrawMore);

        private delegate void DrawAction(Graphics g, Pen p, SolidBrush b);

        private static Image MakeIcon(DrawAction draw)
        {
            var bmp = new Bitmap(16, 16);
            using (var g = Graphics.FromImage(bmp))
            using (var p = new Pen(_color, 1.4f))
            using (var b = new SolidBrush(_color))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                draw(g, p, b);
            }
            return bmp;
        }

        private static void DrawDelete(Graphics g, Pen p, SolidBrush b)
        {
            g.DrawRectangle(p, 3, 5, 10, 9);
            g.DrawLine(p, 2, 5, 14, 5);
            g.DrawLine(p, 6, 2, 10, 2);
            g.DrawLine(p, 6, 2, 6, 5);
            g.DrawLine(p, 10, 2, 10, 5);
            g.DrawLine(p, 6, 8, 6, 12);
            g.DrawLine(p, 10, 8, 10, 12);
        }

        private static void DrawClearAll(Graphics g, Pen p, SolidBrush b)
        {
            g.DrawLine(p, 3, 3, 13, 13);
            g.DrawLine(p, 13, 3, 3, 13);
        }

        private static void DrawExport(Graphics g, Pen p, SolidBrush b)
        {
            g.DrawLine(p, 8, 3, 8, 12);
            g.DrawLine(p, 4, 9, 8, 12);
            g.DrawLine(p, 12, 9, 8, 12);
            g.DrawLine(p, 3, 14, 13, 14);
        }

        private static void DrawMore(Graphics g, Pen p, SolidBrush b)
        {
            g.FillEllipse(b, 3, 6, 4, 4);
            g.FillEllipse(b, 9, 6, 4, 4);
            g.FillEllipse(b, 15, 6, 4, 4);
        }
    }
}
