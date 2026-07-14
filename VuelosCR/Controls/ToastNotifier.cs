using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace VuelosCRSA.Controls
{
    public static class ToastNotifier
    {
        private static Timer _timer;
        private static Panel _toast;
        private static Label _label;
        private static Form _owner;
        private static float _opacity = 1f;
        private static int _step;

        public static void Show(Form owner, string message, int durationMs = 3000, bool isError = false)
        {
            Hide();

            _owner = owner;
            _toast = new Panel
            {
                BackColor = Color.FromArgb(35, 35, 35),
                Height = 48,
                AutoSize = true,
                MinimumSize = new Size(280, 48),
                MaximumSize = new Size(500, 48),
                Anchor = AnchorStyles.None
            };

            _label = new Label
            {
                Text = message,
                Font = new Font("Segoe UI", 10f),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(0),
                Location = new Point(16, 0),
                Size = new Size(_toast.MinimumSize.Width - 32, 48),
                MaximumSize = new Size(500 - 32, 48)
            };
            _label.SizeChanged += (s, e) =>
            {
                _toast.Width = _label.Width + 48;
            };

            // Accent stripe on the left
            var stripe = new Panel
            {
                BackColor = isError ? Color.FromArgb(200, 50, 50) : Color.FromArgb(16, 163, 127),
                Width = 4,
                Height = 48,
                Location = new Point(0, 0),
                Dock = DockStyle.Left
            };
            _toast.Controls.Add(stripe);
            _toast.Controls.Add(_label);

            // Round corners
            RoundControl(_toast, 10);

            // Position at bottom-center
            _toast.Location = new Point(
                (owner.ClientSize.Width - _toast.Width) / 2,
                owner.ClientSize.Height - _toast.Height - 30
            );

            owner.Controls.Add(_toast);
            _toast.BringToFront();

            _opacity = 1f;
            _step = 0;

            _timer = new Timer { Interval = 30 };
            _timer.Tick += (s, e) =>
            {
                _step++;
                if (_step < durationMs / 30 - 20) return;

                _opacity -= 0.05f;
                if (_opacity <= 0)
                {
                    Hide();
                    return;
                }
                _toast.BackColor = Color.FromArgb((int)(_opacity * 35), 35, 35, 35);
                _label.ForeColor = Color.FromArgb((int)(_opacity * 255), 255, 255, 255);
                foreach (Control c in _toast.Controls)
                {
                    if (c.BackColor != Color.Transparent)
                        c.BackColor = Color.FromArgb((int)(_opacity * c.BackColor.A), c.BackColor);
                }
            };
            _timer.Start();
        }

        public static void ShowError(Form owner, string message, int durationMs = 4000)
        {
            Show(owner, message, durationMs, true);
        }

        public static void Hide()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
            if (_toast != null)
            {
                if (_owner != null && !_owner.IsDisposed && _owner.Controls.Contains(_toast))
                    _owner.Controls.Remove(_toast);
                _toast.Dispose();
                _toast = null;
            }
            _label = null;
            _owner = null;
            _opacity = 1f;
        }

        private static void RoundControl(Control ctrl, int radius)
        {
            var rect = new Rectangle(0, 0, ctrl.Width, ctrl.Height);
            using (var path = new GraphicsPath())
            {
                var d = radius * 2;
                path.AddArc(rect.X, rect.Y, d, d, 180, 90);
                path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
                path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
                path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
                path.CloseFigure();
                ctrl.Region = new Region(path);
            }
        }
    }
}
