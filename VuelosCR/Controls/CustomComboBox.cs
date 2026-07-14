using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VuelosCRSA.Controls
{
    public class CustomComboBox : ComboBox
    {
        // ── P/Invoke ──────────────────────────────────────────────
        private const int CB_GETCOMBOBOXINFO = 0x0164;
        private const int WM_PAINT = 0x000F;
        private const int WM_NCPAINT = 0x0085;
        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_MOUSELEAVE = 0x02A3;
        private const int WM_SETFOCUS = 0x0007;
        private const int WM_KILLFOCUS = 0x0008;

        [DllImport("user32.dll")]
        private static extern bool GetComboBoxInfo(IntPtr hWnd, ref COMBOBOXINFO info);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateRoundRectRgn(int x1, int y1, int x2, int y2, int cx, int cy);

        [DllImport("user32.dll")]
        private static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x80000;
        private const int WS_EX_DROPSHADOW = 0x00020000;

        [StructLayout(LayoutKind.Sequential)]
        private struct COMBOBOXINFO
        {
            public int cbSize;
            public RECT rcItem;
            public RECT rcButton;
            public int stateButton;
            public IntPtr hwndCombo;
            public IntPtr hwndEdit;
            public IntPtr hwndList;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        // ── Properties ────────────────────────────────────────────
        private int _borderRadius = 14;
        private Color _borderColor = Color.FromArgb(50, 50, 50);
        private Color _borderHoverColor = Color.FromArgb(16, 163, 127);
        private Color _borderFocusColor = Color.FromArgb(16, 163, 127);
        private Color _arrowColor = Color.FromArgb(140, 140, 140);
        private Color _arrowHoverColor = Color.FromArgb(16, 163, 127);
        private Color _dropBackColor = Color.FromArgb(30, 30, 30);
        private Color _itemHoverColor = Color.FromArgb(16, 163, 127);
        private Color _itemTextColor = Color.FromArgb(200, 200, 200);
        private Color _itemTextHoverColor = Color.White;
        private Color _dividerColor = Color.FromArgb(45, 45, 45);

        private bool _hovered;
        private bool _droppedDown;

        [Category("Appearance")]
        public int BorderRadius { get => _borderRadius; set { _borderRadius = value; Invalidate(); } }

        [Category("Appearance")]
        public Color BorderColor { get => _borderColor; set { _borderColor = value; Invalidate(); } }

        [Category("Appearance")]
        public Color BorderHoverColor { get => _borderHoverColor; set { _borderHoverColor = value; Invalidate(); } }

        [Category("Appearance")]
        public Color BorderFocusColor { get => _borderFocusColor; set { _borderFocusColor = value; Invalidate(); } }

        [Category("Appearance")]
        public Color ArrowColor { get => _arrowColor; set { _arrowColor = value; Invalidate(); } }

        [Category("Appearance")]
        public Color ArrowHoverColor { get => _arrowHoverColor; set { _arrowHoverColor = value; Invalidate(); } }

        [Category("Appearance")]
        public Color ItemHoverColor { get => _itemHoverColor; set { _itemHoverColor = value; Invalidate(); } }

        [Category("Appearance")]
        public int DropItemHeight { get; set; } = 32;

        // ── Constructor ───────────────────────────────────────────
        public CustomComboBox()
        {
            DrawMode = DrawMode.OwnerDrawFixed;
            DropDownStyle = ComboBoxStyle.DropDownList;
            FlatStyle = FlatStyle.Flat;
            BackColor = _dropBackColor;
            ForeColor = _itemTextColor;
            Font = new Font("Segoe UI", 10f);
            ItemHeight = DropItemHeight;
            DropDownHeight = 200;

            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint, true);

            SetWindowTheme(Handle, "", "");

            DropDown += OnDropDown;
            DropDownClosed += OnDropDownClosed;
            MouseEnter += (s, e) => { _hovered = true; Invalidate(); };
            MouseLeave += (s, e) => { _hovered = false; Invalidate(); };
        }

        // ── OnHandleCreated ───────────────────────────────────────
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            SetWindowTheme(Handle, "", "");
        }

        // ── Paint everything manually ─────────────────────────────
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var rect = ClientRectangle;
            rect.Width--;
            rect.Height--;

            var borderCol = _hovered || _droppedDown ? _borderHoverColor : _borderColor;
            if (Focused) borderCol = _borderFocusColor;

            // Background fill
            using (var bgBrush = new SolidBrush(BackColor))
            {
                var clip = RoundRect(rect, _borderRadius);
                g.FillPath(bgBrush, clip);
                clip.Dispose();
            }

            // Selected item text
            var text = SelectedItem?.ToString() ?? "";
            var textRect = new Rectangle(12, 0, rect.Width - Height - 12, rect.Height);
            using (var tb = new SolidBrush(ForeColor))
            using (var sf = new StringFormat { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Near })
            {
                var fmt = TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter | TextFormatFlags.Left;
                TextRenderer.DrawText(g, text, Font, textRect, ForeColor, fmt);
            }

            // Custom dropdown arrow
            var arrowRect = new Rectangle(rect.Right - Height + 2, 0, Height - 4, rect.Height);
            DrawArrow(g, arrowRect, _hovered || _droppedDown ? _arrowHoverColor : _arrowColor);

            // Border
            DrawBorder(g, rect, borderCol);
        }

        // ── Border ────────────────────────────────────────────────
        private void DrawBorder(Graphics g, Rectangle rect, Color color)
        {
            using (var path = RoundRect(rect, _borderRadius))
            using (var pen = new Pen(color, 1.5f))
            {
                g.DrawPath(pen, path);
            }
        }

        // ── Arrow ─────────────────────────────────────────────────
        private void DrawArrow(Graphics g, Rectangle rect, Color color)
        {
            var cx = rect.X + rect.Width / 2;
            var cy = rect.Y + rect.Height / 2 + 1;
            var s = 5f;
            var pts = new[]
            {
                new PointF(cx - s, cy - s / 2f),
                new PointF(cx + s, cy - s / 2f),
                new PointF(cx, cy + s / 2f)
            };
            using (var brush = new SolidBrush(color))
                g.FillPolygon(brush, pts);
        }

        // ── Dropdown items ────────────────────────────────────────
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var r = e.Bounds;
            var isHover = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            var isOdd = e.Index % 2 == 1;

            // Background
            Color itemBg;
            if (isHover)
                itemBg = _itemHoverColor;
            else if (isOdd)
                itemBg = Color.FromArgb(38, 38, 38);
            else
                itemBg = _dropBackColor;

            using (var bgBrush = new SolidBrush(itemBg))
                g.FillRectangle(bgBrush, r);

            // Left accent bar on hover
            if (isHover)
            {
                using (var barBrush = new SolidBrush(Color.FromArgb(16, 163, 127)))
                    g.FillRectangle(barBrush, r.X, r.Y + 4, 3, r.Height - 8);
            }

            // Divider
            if (!isHover)
            {
                using (var pen = new Pen(_dividerColor))
                    g.DrawLine(pen, r.X + 12, r.Bottom - 1, r.Right - 12, r.Bottom - 1);
            }

            // Text
            var text = Items[e.Index]?.ToString() ?? "";
            var txtColor = isHover ? _itemTextHoverColor : _itemTextColor;
            var textRect = new Rectangle(r.X + 18, r.Y, r.Width - 28, r.Height);
            using (var brush = new SolidBrush(txtColor))
            using (var sf = new StringFormat { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Near })
            {
                g.DrawString(text, Font, brush, textRect, sf);
            }
        }

        // ── Dropdown popup styling ────────────────────────────────
        private void OnDropDown(object sender, EventArgs e)
        {
            _droppedDown = true;
            Invalidate();
            ApplyDropdownStyle();
        }

        private void OnDropDownClosed(object sender, EventArgs e)
        {
            _droppedDown = false;
            Invalidate();
        }

        private void ApplyDropdownStyle()
        {
            try
            {
                var info = new COMBOBOXINFO();
                info.cbSize = Marshal.SizeOf(info);
                if (!GetComboBoxInfo(Handle, ref info)) return;
                if (info.hwndList == IntPtr.Zero) return;

                var hList = info.hwndList;

                // Drop shadow
                var exStyle = GetWindowLong(hList, GWL_EXSTYLE);
                SetWindowLong(hList, GWL_EXSTYLE, exStyle | WS_EX_DROPSHADOW);

                // Rounded corners
                var listRect = new RECT();
                if (!GetWindowRect(hList, ref listRect))
                {
                    // Fallback: use a timer to apply after layout
                    var t = new Timer { Interval = 50 };
                    t.Tick += (s, args) =>
                    {
                        t.Stop();
                        t.Dispose();
                        ApplyRgnToDropdown(hList);
                    };
                    t.Start();
                    return;
                }
                ApplyRgnToDropdown(hList);
            }
            catch { }
        }

        private void ApplyRgnToDropdown(IntPtr hList)
        {
            try
            {
                var listRect = new RECT();
                if (!GetWindowRect(hList, ref listRect)) return;
                var w = listRect.Right - listRect.Left;
                var h = listRect.Bottom - listRect.Top;
                var rgn = CreateRoundRectRgn(0, 0, w + 1, h + 1, 12, 12);
                if (rgn != IntPtr.Zero)
                {
                    SetWindowRgn(hList, rgn, true);
                }
            }
            catch { }
        }

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, ref RECT rect);

        // ── Focus handling ────────────────────────────────────────
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Invalidate();
        }

        // ── Helper: RoundRect path ────────────────────────────────
        private static GraphicsPath RoundRect(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            var d = radius * 2;
            if (d > r.Width) d = r.Width;
            if (d > r.Height) d = r.Height;
            var r2 = d / 2;

            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
