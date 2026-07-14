using System.Drawing;
using System.Windows.Forms;

namespace VuelosCRSA.Controls
{
    public class DarkToolStripRenderer : ToolStripProfessionalRenderer
    {
        public DarkToolStripRenderer() : base(new DarkColorTable()) { }

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            e.ArrowColor = Color.FromArgb(160, 160, 160);
            base.OnRenderArrow(e);
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = Color.FromArgb(200, 200, 200);
            base.OnRenderItemText(e);
        }
    }

    public class DarkColorTable : ProfessionalColorTable
    {
        public override Color MenuItemSelected => Color.FromArgb(16, 163, 127);
        public override Color MenuItemSelectedGradientBegin => Color.FromArgb(16, 163, 127);
        public override Color MenuItemSelectedGradientEnd => Color.FromArgb(19, 185, 145);
        public override Color MenuItemBorder => Color.FromArgb(40, 40, 40);
        public override Color MenuStripGradientBegin => Color.FromArgb(25, 25, 25);
        public override Color MenuStripGradientEnd => Color.FromArgb(20, 20, 20);
        public override Color ToolStripDropDownBackground => Color.FromArgb(35, 35, 35);
        public override Color ImageMarginGradientBegin => Color.FromArgb(35, 35, 35);
        public override Color ImageMarginGradientMiddle => Color.FromArgb(35, 35, 35);
        public override Color ImageMarginGradientEnd => Color.FromArgb(35, 35, 35);
        public override Color SeparatorDark => Color.FromArgb(55, 55, 55);
        public override Color SeparatorLight => Color.FromArgb(55, 55, 55);
        public override Color ButtonSelectedHighlight => Color.FromArgb(50, 50, 50);
        public override Color ButtonSelectedHighlightBorder => Color.FromArgb(60, 60, 60);
        public override Color ButtonPressedHighlight => Color.FromArgb(16, 163, 127);
        public override Color ButtonPressedHighlightBorder => Color.FromArgb(16, 163, 127);
        public override Color ButtonCheckedHighlight => Color.FromArgb(40, 40, 40);
        public override Color ButtonCheckedHighlightBorder => Color.FromArgb(50, 50, 50);
        public override Color OverflowButtonGradientBegin => Color.FromArgb(25, 25, 25);
        public override Color OverflowButtonGradientEnd => Color.FromArgb(25, 25, 25);
    }
}
