using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using VuelosCRSA;

namespace VuelosCRSA.Controls
{
    public class PurchaseSummaryCard : UserControl
    {
        private int _borderRadius = 12;
        private Color _cardBg = Color.FromArgb(28, 28, 28);
        private Color _accent = Color.FromArgb(16, 163, 127);
        private Color _textPrimary = Color.FromArgb(200, 200, 200);
        private Color _textSecondary = Color.FromArgb(130, 130, 130);

        private Label lblTitle, lblVoucher, lblPassenger, lblDestination, lblOrigin, lblAirline, lblClase, lblTripType, lblDate, lblReturnDate;
        private Label lblBaseFare, lblService, lblVAT, lblTotal;

        public PurchaseSummaryCard()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            MinimumSize = new Size(220, 280);
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
                RowCount = 17
            };
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));

            for (int i = 0; i < 17; i++)
                tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / 17));

            var titleFont = new Font("Segoe UI", 10f, FontStyle.Bold);
            var labelFont = new Font("Segoe UI", 8.5f);
            var valueFont = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            var totalFont = new Font("Segoe UI", 10f, FontStyle.Bold);
            var voucherFont = new Font("Segoe UI", 8f, FontStyle.Bold);

            lblTitle = new Label
            {
                Text = "Purchase Summary",
                Font = titleFont,
                ForeColor = _accent,
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false
            };
            tbl.Controls.Add(lblTitle, 0, 0);
            tbl.SetColumnSpan(lblTitle, 2);

            lblVoucher = new Label
            {
                Text = "Voucher: -",
                Font = voucherFont,
                ForeColor = _textSecondary,
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false
            };
            tbl.Controls.Add(lblVoucher, 0, 1);
            tbl.SetColumnSpan(lblVoucher, 2);

            // Separator 1
            tbl.Controls.Add(MakeSeparator(), 0, 2);
            tbl.SetColumnSpan(tbl.GetControlFromPosition(0, 2), 2);

            AddRow(tbl, "Passenger", "—", 3, out lblPassenger, out _);
            AddRow(tbl, "Destination", "—", 4, out lblDestination, out _);
            AddRow(tbl, "Origin", "—", 5, out lblOrigin, out _);
            AddRow(tbl, "Airline", "—", 6, out lblAirline, out _);
            AddRow(tbl, "Class", "—", 7, out lblClase, out _);
            AddRow(tbl, "Trip", "—", 8, out lblTripType, out _);
            AddRow(tbl, "Date", "—", 9, out lblDate, out _);
            AddRow(tbl, "Return", "—", 10, out lblReturnDate, out _);

            // Separator 2
            tbl.Controls.Add(MakeSeparator(), 0, 11);
            tbl.SetColumnSpan(tbl.GetControlFromPosition(0, 11), 2);

            AddRow(tbl, "Base Fare", "—", 12, out lblBaseFare, out _);
            AddRow(tbl, "Service Fee", "—", 13, out lblService, out _);
            AddRow(tbl, "VAT", "—", 14, out lblVAT, out _);

            // Separator 3
            tbl.Controls.Add(MakeSeparator(), 0, 15);
            tbl.SetColumnSpan(tbl.GetControlFromPosition(0, 15), 2);

            // Total row
            var lblTotalLabel = new Label
            {
                Text = "Total",
                Font = totalFont,
                ForeColor = _textPrimary,
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false
            };
            tbl.Controls.Add(lblTotalLabel, 0, 16);

            lblTotal = new Label
            {
                Text = "—",
                Font = totalFont,
                ForeColor = _accent,
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                AutoSize = false
            };
            tbl.Controls.Add(lblTotal, 1, 16);

            Controls.Add(tbl);
            ResumeLayout();
        }

        private static Label MakeSeparator()
        {
            var sep = new Label { Dock = DockStyle.Fill, BackColor = Color.Transparent, AutoSize = false };
            sep.Paint += (s, e) =>
            {
                var c = s as Label;
                if (c == null) return;
                using (var pen = new Pen(Color.FromArgb(50, 50, 50)))
                    e.Graphics.DrawLine(pen, c.Padding.Left, c.Height / 2, c.Width, c.Height / 2);
            };
            return sep;
        }

        private static void AddRow(TableLayoutPanel tbl, string label, string value, int row,
            out Label labelCtrl, out Label valueCtrl)
        {
            labelCtrl = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(130, 130, 130),
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false
            };
            tbl.Controls.Add(labelCtrl, 0, row);

            valueCtrl = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(200, 200, 200),
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                AutoSize = false
            };
            tbl.Controls.Add(valueCtrl, 1, row);
        }

        // ── Public API ──────────────────────────────────────────────

        public void UpdateData(string voucher, string passenger, string destination, string origin, string airline, string clase, string tripType, string date, string returnDate,
            string baseFare, string serviceFee, string vat, string total)
        {
            lblVoucher.Text = voucher;
            lblPassenger.Text = passenger;
            lblDestination.Text = destination;
            lblOrigin.Text = origin;
            lblAirline.Text = airline;
            lblClase.Text = clase;
            lblTripType.Text = tripType;
            lblDate.Text = date;
            lblReturnDate.Text = returnDate;
            lblBaseFare.Text = baseFare;
            lblService.Text = serviceFee;
            lblVAT.Text = vat;
            lblTotal.Text = total;
            Invalidate();
        }

        public void ApplyLanguage()
        {
            var lang = Program.SelectedLanguage ?? "es";
            if (lang == "en")
            {
                lblTitle.Text = "Purchase Summary";
            }
            else
            {
                lblTitle.Text = "Resumen de Compra";
            }
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
