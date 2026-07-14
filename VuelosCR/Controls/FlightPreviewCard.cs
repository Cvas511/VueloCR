using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using VuelosCRSA;

namespace VuelosCRSA.Controls
{
    public class FlightPreviewCard : UserControl
    {
        private int _borderRadius = 12;
        private Color _cardBg = Color.FromArgb(28, 28, 28);
        private Color _accent = Color.FromArgb(16, 163, 127);
        private Color _textPrimary = Color.FromArgb(200, 200, 200);
        private Color _textSecondary = Color.FromArgb(130, 130, 130);
        private bool _hasData;

        private Label lblTitle, lblRouteLabel, lblRouteValue, lblAirlineLabel, lblAirlineValue;
        private Label lblBaseLabel, lblBaseValue, lblServiceLabel, lblServiceValue;
        private Label lblVatLabel, lblVatValue, lblTotalLabel, lblTotalValue;

        public FlightPreviewCard()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            MinimumSize = new Size(260, 100);
            BackColor = Color.Transparent;
            BuildLayout();
        }

        private void BuildLayout()
        {
            SuspendLayout();

            var tbl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(14, 10, 14, 10),
                ColumnCount = 2,
                RowCount = 9
            };
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            for (int i = 0; i < 9; i++)
                tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / 9));

            Font titleFont = new Font("Segoe UI", 10f, FontStyle.Bold);
            Font labelFont = new Font("Segoe UI", 8.5f);
            Font valueFont = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            Font totalFont = new Font("Segoe UI", 10f, FontStyle.Bold);

            lblTitle = new Label
            {
                Text = "Flight Preview",
                Font = titleFont,
                ForeColor = _accent,
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false
            };
            tbl.Controls.Add(lblTitle, 0, 0);
            tbl.SetColumnSpan(lblTitle, 2);

            StyleRow(tbl, "Route", "San José → —", 1);
            lblRouteLabel = (Label)tbl.GetControlFromPosition(0, 1);
            lblRouteValue = (Label)tbl.GetControlFromPosition(1, 1);

            StyleRow(tbl, "Airline", "—", 2);
            lblAirlineLabel = (Label)tbl.GetControlFromPosition(0, 2);
            lblAirlineValue = (Label)tbl.GetControlFromPosition(1, 2);

            // Separator row
            var sep1 = new Label { Dock = DockStyle.Fill, BackColor = Color.Transparent, AutoSize = false };
            sep1.Paint += (s, e) =>
            {
                var c = s as Label;
                if (c == null) return;
                using (var pen = new Pen(Color.FromArgb(50, 50, 50)))
                    e.Graphics.DrawLine(pen, c.Padding.Left, c.Height / 2, c.Width, c.Height / 2);
            };
            tbl.Controls.Add(sep1, 0, 3);
            tbl.SetColumnSpan(sep1, 2);

            StyleRow(tbl, "Base fare", "—", 4);
            lblBaseLabel = (Label)tbl.GetControlFromPosition(0, 4);
            lblBaseValue = (Label)tbl.GetControlFromPosition(1, 4);

            StyleRow(tbl, "Service", "—", 5);
            lblServiceLabel = (Label)tbl.GetControlFromPosition(0, 5);
            lblServiceValue = (Label)tbl.GetControlFromPosition(1, 5);

            StyleRow(tbl, "VAT (13%)", "—", 6);
            lblVatLabel = (Label)tbl.GetControlFromPosition(0, 6);
            lblVatValue = (Label)tbl.GetControlFromPosition(1, 6);

            // Separator row
            var sep2 = new Label { Dock = DockStyle.Fill, BackColor = Color.Transparent, AutoSize = false };
            sep2.Paint += (s, e) =>
            {
                var c = s as Label;
                if (c == null) return;
                using (var pen = new Pen(Color.FromArgb(50, 50, 50)))
                    e.Graphics.DrawLine(pen, c.Padding.Left, c.Height / 2, c.Width, c.Height / 2);
            };
            tbl.Controls.Add(sep2, 0, 7);
            tbl.SetColumnSpan(sep2, 2);

            lblTotalLabel = new Label
            {
                Text = "Total",
                Font = totalFont,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false
            };
            tbl.Controls.Add(lblTotalLabel, 0, 8);

            lblTotalValue = new Label
            {
                Text = "—",
                Font = totalFont,
                ForeColor = _accent,
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                AutoSize = false
            };
            tbl.Controls.Add(lblTotalValue, 1, 8);

            Controls.Add(tbl);
            ResumeLayout();
        }

        private void StyleRow(TableLayoutPanel tbl, string label, string value, int row)
        {
            var lbl = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = _textSecondary,
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false
            };
            tbl.Controls.Add(lbl, 0, row);

            var val = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = _textPrimary,
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                AutoSize = false
            };
            tbl.Controls.Add(val, 1, row);
        }

        public void UpdateData(string origin, string destination, string airline, decimal basePrice,
            decimal serviceCharge, decimal vat, decimal total)
        {
            var lang = Program.SelectedLanguage ?? "es";
            _hasData = !string.IsNullOrEmpty(destination);
            lblRouteValue.Text = (origin ?? "—") + " → " + (destination ?? "—");
            lblAirlineValue.Text = airline;
            lblBaseValue.Text = $"${basePrice:F2}";
            lblServiceValue.Text = $"${serviceCharge:F2}";
            lblVatValue.Text = $"${vat:F2}";
            lblTotalValue.Text = $"${total:F2}";
            Invalidate();
        }

        public void ApplyLanguage()
        {
            var lang = Program.SelectedLanguage ?? "es";
            if (lang == "en")
            {
                lblTitle.Text = "Flight Preview";
                lblRouteLabel.Text = "Route";
                lblAirlineLabel.Text = "Airline";
                lblBaseLabel.Text = "Base fare";
                lblServiceLabel.Text = "Service fee";
                lblVatLabel.Text = "VAT (13%)";
                lblTotalLabel.Text = "Total";
            }
            else
            {
                lblTitle.Text = "Vista Previa";
                lblRouteLabel.Text = "Ruta";
                lblAirlineLabel.Text = "Aerolínea";
                lblBaseLabel.Text = "Tarifa base";
                lblServiceLabel.Text = "Cargo servicio";
                lblVatLabel.Text = "IVA (13%)";
                lblTotalLabel.Text = "Total";
            }
        }

        public void Clear()
        {
            _hasData = false;
            lblRouteValue.Text = "—";
            lblAirlineValue.Text = "—";
            lblBaseValue.Text = "—";
            lblServiceValue.Text = "—";
            lblVatValue.Text = "—";
            lblTotalValue.Text = "—";
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(0, 0, Width - 1, Height - 1);

            using (var path = RoundRect(rect, _borderRadius))
            using (var bg = new SolidBrush(_cardBg))
                g.FillPath(bg, path);

            using (var path = RoundRect(rect, _borderRadius))
            using (var pen = new Pen(Color.FromArgb(50, 50, 50), 1))
                g.DrawPath(pen, path);

            using (var accentPen = new Pen(_accent, 3))
                g.DrawLine(accentPen, 20, 0, 80, 0);

            base.OnPaint(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
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
