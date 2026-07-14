using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VuelosCRSA.Controls
{
    public class DarkMessageBox : Form
    {
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        private const int DWMWA_MICA = 1029;

        private const int WM_NCHITTEST = 0x0084;
        private const int HTCLIENT = 1;
        private const int HTCAPTION = 2;

        private readonly string _message;
        private readonly string _title;
        private readonly MessageBoxButtons _buttons;
        private readonly MessageBoxIcon _icon;
        private readonly int _borderRadius = 14;

        public DialogResult Result { get; private set; } = DialogResult.None;

        private DarkMessageBox(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            _message = message;
            _title = title;
            _buttons = buttons;
            _icon = icon;

            Text = "";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.FromArgb(25, 25, 25);
            ForeColor = Color.FromArgb(200, 200, 200);
            ShowInTaskbar = false;
            TopMost = true;
            Size = new Size(420, 200);
            MinimumSize = new Size(380, 180);

            Load += (s, e) =>
            {
                var dark = 1;
                DwmSetWindowAttribute(Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref dark, sizeof(int));
                RoundCorners();
            };

            Paint += (s, e) => DrawForm(e.Graphics);

            BuildLayout();
        }

        private void RoundCorners()
        {
            var rect = new Rectangle(0, 0, Width, Height);
            using (var path = RoundRect(rect, _borderRadius))
            {
                Region = new Region(path);
            }
        }

        private void DrawForm(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using (var path = RoundRect(rect, _borderRadius))
            {
                using (var bg = new SolidBrush(BackColor))
                    g.FillPath(bg, path);
                using (var pen = new Pen(Color.FromArgb(50, 50, 50), 1))
                    g.DrawPath(pen, path);
            }
        }

        private void BuildLayout()
        {
            // Title bar (draggable)
            var titleLabel = new Label
            {
                Text = _title,
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Location = new Point(20, 18),
                AutoSize = true
            };
            Controls.Add(titleLabel);

            // Close button (X)
            var closeBtn = new Button
            {
                Text = "×",
                Font = new Font("Segoe UI", 16f),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0, MouseOverBackColor = Color.FromArgb(60, 60, 60) },
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(150, 150, 150),
                Size = new Size(36, 36),
                Location = new Point(Width - 46, 8),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                TabStop = false
            };
            closeBtn.Click += (s, e) => { Result = DialogResult.Cancel; Close(); };
            closeBtn.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var b = new SolidBrush(closeBtn.ForeColor))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    e.Graphics.DrawString("×", closeBtn.Font, b, closeBtn.ClientRectangle, sf);
                    sf.Dispose();
                }
            };
            Controls.Add(closeBtn);

            // Message
            var msgLabel = new Label
            {
                Text = _message,
                Font = new Font("Segoe UI", 10f),
                ForeColor = Color.FromArgb(180, 180, 180),
                BackColor = Color.Transparent,
                Location = new Point(24, 55),
                Size = new Size(Width - 48, 70),
                AutoSize = false,
                TextAlign = ContentAlignment.TopLeft
            };
            Controls.Add(msgLabel);

            // Buttons
            var btnY = Height - 58;
            var btnList = GetButtons();
            var btnWidth = 100;
            var btnHeight = 34;
            var totalW = btnList.Length * (btnWidth + 10) - 10;
            var startX = (Width - totalW) / 2;

            for (int i = 0; i < btnList.Length; i++)
            {
                var isPrimary = i == btnList.Length - 1;
                var btn = new Button
                {
                    Text = btnList[i].text,
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = { BorderSize = 0 },
                    BackColor = isPrimary ? Color.FromArgb(16, 163, 127) : Color.FromArgb(50, 50, 50),
                    ForeColor = Color.White,
                    Size = new Size(btnWidth, btnHeight),
                    Location = new Point(startX + i * (btnWidth + 10), btnY),
                    Tag = btnList[i].result
                };
                btn.FlatAppearance.MouseOverBackColor = isPrimary ? Color.FromArgb(19, 185, 145) : Color.FromArgb(65, 65, 65);
                btn.Click += (s, e) =>
                {
                    var b = s as Button;
                    if (b?.Tag != null) Result = (DialogResult)b.Tag;
                    Close();
                };
                RoundButton(btn, 8);
                Controls.Add(btn);
            }

            // Adjust height based on content
            var msgHeight = TextRenderer.MeasureText(_message, msgLabel.Font, new Size(Width - 48, int.MaxValue),
                TextFormatFlags.WordBreak).Height + 110;
            Height = Math.Max(190, Math.Min(400, msgHeight));
            msgLabel.Size = new Size(Width - 48, Height - 140);

            // Re-position buttons
            foreach (Control c in Controls)
            {
                if (c is Button && c != closeBtn && c.Tag is DialogResult)
                {
                    c.Location = new Point(c.Location.X, Height - 58);
                }
            }

            closeBtn.Location = new Point(Width - 46, 8);
        }

        private (string text, DialogResult result)[] GetButtons()
        {
            switch (_buttons)
            {
                case MessageBoxButtons.OK:
                    return new[] { ("OK", DialogResult.OK) };
                case MessageBoxButtons.OKCancel:
                    return new[] { ("Cancel", DialogResult.Cancel), ("OK", DialogResult.OK) };
                case MessageBoxButtons.YesNo:
                    return new[] { ("No", DialogResult.No), ("Yes", DialogResult.Yes) };
                case MessageBoxButtons.YesNoCancel:
                    return new[] { ("Cancel", DialogResult.Cancel), ("No", DialogResult.No), ("Yes", DialogResult.Yes) };
                case MessageBoxButtons.RetryCancel:
                    return new[] { ("Cancel", DialogResult.Cancel), ("Retry", DialogResult.Retry) };
                default:
                    return new[] { ("OK", DialogResult.OK) };
            }
        }

        private static void RoundButton(Button btn, int radius)
        {
            var rect = new Rectangle(0, 0, btn.Width, btn.Height);
            using (var path = RoundRect(rect, radius))
                btn.Region = new Region(path);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RoundCorners();
            Invalidate();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_NCHITTEST)
            {
                base.WndProc(ref m);
                if (m.Result == (IntPtr)HTCLIENT)
                    m.Result = (IntPtr)HTCAPTION;
                return;
            }
            base.WndProc(ref m);
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

        // ── Static helpers ────────────────────────────────────────
        public static DialogResult Show(string message, string title = "",
            MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            using (var mb = new DarkMessageBox(message, title, buttons, icon))
            {
                mb.ShowDialog();
                return mb.Result;
            }
        }

        public static DialogResult Show(IWin32Window owner, string message, string title = "",
            MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            using (var mb = new DarkMessageBox(message, title, buttons, icon))
            {
                mb.StartPosition = FormStartPosition.CenterParent;
                mb.ShowDialog(owner);
                return mb.Result;
            }
        }
    }
}
