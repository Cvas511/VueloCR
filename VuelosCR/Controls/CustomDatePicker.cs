using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace VuelosCRSA.Controls
{
    public class CustomDatePicker : UserControl
    {
        private TextBox txtDate;
        private Panel btnDrop;
        private MonthCalendar calendar;
        private ToolStripDropDown dropDown;

        private Color _borderColor = Color.FromArgb(50, 50, 50);
        private Color _borderFocusColor = Color.FromArgb(16, 163, 127);
        private Color _bgColor = Color.FromArgb(18, 18, 18);
        private Color _textColor = Color.FromArgb(200, 200, 200);
        private Color _btnColor = Color.FromArgb(30, 30, 30);
        private Color _btnHoverColor = Color.FromArgb(50, 50, 50);
        private bool _focused;

        public event EventHandler ValueChanged;

        public DateTime Value
        {
            get
            {
                if (DateTime.TryParse(txtDate.Text, out var dt))
                    return dt;
                return DateTime.Today;
            }
            set
            {
                txtDate.Text = value.ToString("yyyy-MM-dd");
            }
        }

        public CustomDatePicker()
        {
            MinimumSize = new Size(120, 22);
            Height = 22;
            DoubleBuffered = true;

            txtDate = new TextBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = _bgColor,
                ForeColor = _textColor,
                Font = new Font("Segoe UI", 10f),
                Location = new Point(4, 2),
                Size = new Size(Width - 24, 18),
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right,
                Text = DateTime.Today.ToString("yyyy-MM-dd")
            };
            txtDate.TextChanged += (s, e) => ValueChanged?.Invoke(this, EventArgs.Empty);
            txtDate.GotFocus += (s, e) => { _focused = true; Invalidate(); };
            txtDate.LostFocus += (s, e) => { _focused = false; Invalidate(); };

            btnDrop = new Panel
            {
                BackColor = _btnColor,
                Location = new Point(Width - 20, 0),
                Size = new Size(20, 22),
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                Cursor = Cursors.Hand
            };
            btnDrop.Paint += BtnDrop_Paint;
            btnDrop.MouseEnter += (s, e) => { btnDrop.BackColor = _btnHoverColor; btnDrop.Invalidate(); };
            btnDrop.MouseLeave += (s, e) => { btnDrop.BackColor = _btnColor; btnDrop.Invalidate(); };
            btnDrop.MouseClick += BtnDrop_Click;

            Controls.Add(txtDate);
            Controls.Add(btnDrop);

            calendar = new MonthCalendar
            {
                MaxSelectionCount = 1,
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.FromArgb(200, 200, 200),
                TitleBackColor = Color.FromArgb(20, 20, 20),
                TitleForeColor = Color.FromArgb(16, 163, 127),
                TrailingForeColor = Color.FromArgb(80, 80, 80)
            };
            calendar.DateSelected += Calendar_DateSelected;

            dropDown = new ToolStripDropDown
            {
                AutoClose = true,
                Padding = Padding.Empty,
                Margin = Padding.Empty
            };
            var host = new ToolStripControlHost(calendar)
            {
                Padding = Padding.Empty,
                Margin = Padding.Empty,
                AutoSize = false,
                Size = calendar.SingleMonthSize
            };
            dropDown.Items.Add(host);
            dropDown.Opened += (s, e) => calendar.Invalidate();
            dropDown.Closed += (s, e) => btnDrop.Invalidate();
        }

        private void BtnDrop_Click(object sender, MouseEventArgs e)
        {
            var loc = PointToScreen(new Point(0, Height));
            dropDown.Show(loc);
        }

        private void Calendar_DateSelected(object sender, DateRangeEventArgs e)
        {
            txtDate.Text = e.Start.ToString("yyyy-MM-dd");
            dropDown.Close();
        }

        private void BtnDrop_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var midX = btnDrop.Width / 2;
            var midY = btnDrop.Height / 2;
            var arrow = new[] {
                new Point(midX - 4, midY - 1),
                new Point(midX + 4, midY - 1),
                new Point(midX, midY + 3)
            };
            using (var brush = new SolidBrush(Color.FromArgb(130, 130, 130)))
                g.FillPolygon(brush, arrow);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            var borderCol = _focused ? _borderFocusColor : _borderColor;

            using (var path = RoundRect(rect, 4))
            using (var bg = new SolidBrush(_bgColor))
                g.FillPath(bg, path);

            using (var path = RoundRect(rect, 4))
            using (var pen = new Pen(borderCol, 1.5f))
                g.DrawPath(pen, path);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (txtDate != null)
            {
                txtDate.Width = Math.Max(10, Width - 28);
            }
            if (btnDrop != null)
            {
                btnDrop.Location = new Point(Width - 20, 0);
                btnDrop.Size = new Size(20, Height);
            }
            Invalidate();
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
