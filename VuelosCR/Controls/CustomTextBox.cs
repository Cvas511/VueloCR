using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VuelosCRSA.Controls
{
    public class CustomTextBox : TextBox
    {
        private const int WM_PAINT = 0x000F;
        private const int EM_SETMARGINS = 0x00D3;
        private const int EC_LEFTMARGIN = 0x0001;
        private const int EC_RIGHTMARGIN = 0x0002;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private int _borderRadius = 10;
        private Color _borderColor = Color.FromArgb(50, 50, 50);
        private Color _borderFocusColor = Color.FromArgb(16, 163, 127);
        private Color _borderHoverColor = Color.FromArgb(70, 70, 70);
        private bool _hovered;
        private bool _focused;

        [Category("Appearance")]
        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = value; Invalidate(); }
        }

        [Category("Appearance")]
        public Color BorderColor
        {
            get => _borderColor;
            set { _borderColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        public Color BorderFocusColor
        {
            get => _borderFocusColor;
            set { _borderFocusColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        public Color BorderHoverColor
        {
            get => _borderHoverColor;
            set { _borderHoverColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        public Color CustomBackColor
        {
            get => BackColor;
            set { BackColor = value; Invalidate(); }
        }

        public CustomTextBox()
        {
            BorderStyle = BorderStyle.None;
            BackColor = Color.FromArgb(30, 30, 30);
            ForeColor = Color.FromArgb(200, 200, 200);
            Font = new Font("Segoe UI", 10f);
            DoubleBuffered = true;

            Enter += (s, e) => { _focused = true; Invalidate(); };
            Leave += (s, e) => { _focused = false; Invalidate(); };
            MouseEnter += (s, e) => { _hovered = true; Invalidate(); };
            MouseLeave += (s, e) => { _hovered = false; Invalidate(); };
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            // Add 8px text margins so text doesn't touch the rounded border
            var margin = (IntPtr)((8 << 16) | 8);
            SendMessage(Handle, EM_SETMARGINS, (IntPtr)(EC_LEFTMARGIN | EC_RIGHTMARGIN), margin);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_PAINT)
            {
                using (var g = CreateGraphics())
                    PaintBorder(g);
            }
        }

        private void PaintBorder(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            var borderCol = _focused ? _borderFocusColor :
                            _hovered ? _borderHoverColor : _borderColor;

            using (var path = RoundRect(rect, _borderRadius))
            using (var pen = new Pen(borderCol, 1.5f))
                g.DrawPath(pen, path);

            if (_focused)
            {
                using (var glow = new Pen(Color.FromArgb(35, 16, 163, 127), 3f))
                using (var gp = RoundRect(new Rectangle(1, 1, Width - 3, Height - 3), _borderRadius + 1))
                    g.DrawPath(glow, gp);
            }
        }

        private static GraphicsPath RoundRect(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            var d = radius * 2;
            if (d > r.Width) d = r.Width;
            if (d > r.Height) d = r.Height;
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
