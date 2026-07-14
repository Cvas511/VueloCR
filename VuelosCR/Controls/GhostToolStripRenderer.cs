using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace VuelosCRSA.Controls
{
    public class GhostToolStripRenderer : ToolStripProfessionalRenderer
    {
        private static Color _hoverBg = Color.FromArgb(42, 42, 42);
        private static Color _pressBg = Color.FromArgb(16, 163, 127);

        public GhostToolStripRenderer() : base(new GhostColorTable()) { }

        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            var item = e.Item;
            if (!item.Selected && !item.Pressed) return;

            var g = e.Graphics;
            var bounds = new Rectangle(Point.Empty, item.Size);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var path = RoundedRect(bounds, 4))
            using (var brush = new SolidBrush(item.Pressed ? _pressBg : _hoverBg))
            {
                g.FillPath(brush, path);
            }
        }

        protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
        {
            OnRenderButtonBackground(e);
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = Color.FromArgb(200, 200, 200);
            base.OnRenderItemText(e);
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            var g = e.Graphics;
            var h = e.Item.Height;
            using (var pen = new Pen(Color.FromArgb(55, 55, 55), 1f))
            {
                g.DrawLine(pen, 0, 6, 0, h - 6);
            }
        }

        private static GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0) { path.AddRectangle(r); return path; }
            var d = radius * 2;
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    public class GhostColorTable : ProfessionalColorTable
    {
        public override Color MenuStripGradientBegin => Color.FromArgb(25, 25, 25);
        public override Color MenuStripGradientEnd => Color.FromArgb(20, 20, 20);
        public override Color ToolStripDropDownBackground => Color.FromArgb(35, 35, 35);
        public override Color ImageMarginGradientBegin => Color.FromArgb(35, 35, 35);
        public override Color ImageMarginGradientMiddle => Color.FromArgb(35, 35, 35);
        public override Color ImageMarginGradientEnd => Color.FromArgb(35, 35, 35);
        public override Color MenuItemSelected => Color.FromArgb(16, 163, 127);
        public override Color MenuItemSelectedGradientBegin => Color.FromArgb(16, 163, 127);
        public override Color MenuItemSelectedGradientEnd => Color.FromArgb(19, 185, 145);
        public override Color MenuItemBorder => Color.FromArgb(40, 40, 40);
        public override Color SeparatorDark => Color.FromArgb(55, 55, 55);
        public override Color SeparatorLight => Color.FromArgb(55, 55, 55);
    }
}
