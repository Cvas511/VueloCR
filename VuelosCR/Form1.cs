using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using VuelosCRSA.Data;
using VuelosCRSA.Models;
using VuelosCRSA.Services;
using VuelosCRSA.Controls;

namespace VuelosCRSA
{
    public partial class Form1 : Form
    {
        private readonly ConfigService config;
        private readonly ITiqueteRepository repo;
        private readonly TicketService ticketService = new TicketService();
        private readonly VoucherService voucherService = new VoucherService();
        private readonly FlagService flagService = new FlagService();
        private readonly VouchersPdfService vouchersPdfService = new VouchersPdfService();
        private string _lastTicketId;

        public Form1()
        {
            InitializeComponent();
            var cfgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            config = new ConfigService(cfgPath);
            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "vuelos.sqlite");
            repo = new SQLiteTiqueteRepository(dbPath);
            ApplyLanguage();
            LoadDataToControls();
            RefreshTicketGrid();
            ApplyModernStyle();
            LoadLogo();
            this.MinimumSize = new System.Drawing.Size(880, 600);
            AdjustLayout();
            this.Resize += (s, e) => AdjustLayout();
            this.Shown += (s, e) => AdjustLayout();
        }

        // Adjust layout dynamically so controls don't overlap and use available space
        private void AdjustLayout()
        {
            try
            {
                var margin = 10;
                var cw = this.ClientSize.Width;
                var tbHeight = 0;
                var tb = this.Controls["toolStrip"] as System.Windows.Forms.ToolStrip;
                if (tb != null && tb.Visible) tbHeight = tb.Height;

                // Responsive: collapse right panel if too narrow
                var showRightPanel = cw >= 880;
                var leftW = showRightPanel ? 300 : Math.Max(200, cw - 460);
                var rightW = showRightPanel ? Math.Min(280, Math.Max(240, (cw - leftW) / 3)) : 0;
                var logoHeight = 90;

                // Logo at top, spanning left column width
                if (pictureLogo != null)
                {
                    pictureLogo.Location = new System.Drawing.Point(margin, tbHeight + margin);
                    pictureLogo.Size = new System.Drawing.Size(leftW, logoHeight);
                    RoundControl(pictureLogo, 10);
                }

                if (leftPanel != null)
                {
                    leftPanel.Location = new System.Drawing.Point(margin, tbHeight + margin + logoHeight + margin);
                    leftPanel.Size = new System.Drawing.Size(leftW, this.ClientSize.Height - 120 - logoHeight - margin - tbHeight);
                    RoundControl(leftPanel, 10);
                }

                // ensure the inner flow panel fits and its children do not trigger horizontal scroll
                if (leftFlowPanel != null && leftPanel != null)
                {
                    leftFlowPanel.SuspendLayout();
                    try
                    {
                        leftFlowPanel.Width = leftPanel.ClientSize.Width;
                        leftFlowPanel.Height = leftPanel.ClientSize.Height;
                        int childAvailable = Math.Max(80, leftFlowPanel.ClientSize.Width - leftFlowPanel.Padding.Horizontal - 12);

                        foreach (System.Windows.Forms.Control c in leftFlowPanel.Controls)
                        {
                            if (c is System.Windows.Forms.TextBox || c is System.Windows.Forms.ComboBox)
                            {
                                c.Width = childAvailable;
                                c.Margin = new System.Windows.Forms.Padding(6, 3, 6, 3);
                            }
                        }

                        // resize dateRowPanel to fill available width
                        if (dateRowPanel != null)
                        {
                            dateRowPanel.Width = childAvailable;
                            dateRowPanel.Margin = new System.Windows.Forms.Padding(6, 3, 6, 3);
                            var tipo = cmbTipoViaje.SelectedItem?.ToString() ?? "Ida";
                            if (tipo == "Ida y Vuelta")
                            {
                                dateRowPanel.Height = 46;
                                var halfW = childAvailable / 2;
                                lblIda.Size = new System.Drawing.Size(halfW, 18);
                                lblVuelta.Size = new System.Drawing.Size(halfW, 18);
                                lblVuelta.Location = new System.Drawing.Point(halfW, 0);
                                dtpFecha.Size = new System.Drawing.Size(halfW - 4, 22);
                                dtpFecha.Location = new System.Drawing.Point(2, 22);
                                dtpFechaVuelta.Size = new System.Drawing.Size(halfW - 4, 22);
                                dtpFechaVuelta.Location = new System.Drawing.Point(halfW + 2, 22);
                            }
                            else
                            {
                                dateRowPanel.Height = 46;
                                lblIda.Size = new System.Drawing.Size(childAvailable, 18);
                                lblIda.Location = new System.Drawing.Point(0, 0);
                                dtpFecha.Size = new System.Drawing.Size(childAvailable - 4, 22);
                                dtpFecha.Location = new System.Drawing.Point(2, 22);
                            }
                        }

                        // center buttonsFlow horizontally
                        if (buttonsFlow != null)
                        {
                            var bw = buttonsFlow.PreferredSize.Width;
                            var contentW = leftFlowPanel.ClientSize.Width - leftFlowPanel.Padding.Horizontal;
                            var leftM = Math.Max(0, (contentW - bw) / 2);
                            buttonsFlow.Margin = new System.Windows.Forms.Padding(leftM, 3, leftM, 3);
                        }
                    }
                    finally { leftFlowPanel.ResumeLayout(); }
                }

                // Right panel: group + flight preview + flag
                if (purchaseSummaryCard != null)
                {
                    var rX = cw - rightW - margin;
                    var rVis = showRightPanel && rightW >= 220;
                    purchaseSummaryCard.Visible = rVis;
                    if (rVis)
                    {
                        purchaseSummaryCard.Location = new System.Drawing.Point(rX, tbHeight + margin);
                        purchaseSummaryCard.Size = new System.Drawing.Size(rightW, Math.Max(240, this.ClientSize.Height / 3));
                    }
                }

                var btnY = this.ClientSize.Height - 20;

                // Flight Preview Card
                if (flightPreview != null)
                {
                    flightPreview.Visible = showRightPanel && purchaseSummaryCard.Visible;
                    if (flightPreview.Visible && purchaseSummaryCard != null)
                    {
                        flightPreview.Location = new System.Drawing.Point(
                            purchaseSummaryCard.Left,
                            purchaseSummaryCard.Bottom + margin);
                        flightPreview.Width = purchaseSummaryCard.Width;
                        var previewMaxH = (btnY - 160) - flightPreview.Top;
                        flightPreview.Height = Math.Max(100, Math.Min(
                            this.ClientSize.Height - purchaseSummaryCard.Bottom - margin * 3 - 140,
                            previewMaxH));
                    }
                }

                // Flag — right panel bottom, always above copyright
                if (pictureBoxFlag != null)
                {
                    var flagMargin = 6;
                    var flagW = purchaseSummaryCard != null && purchaseSummaryCard.Visible
                        ? purchaseSummaryCard.Width - flagMargin * 2
                        : 200;
                    var flagH = Math.Min(90, btnY - margin - 60);
                    var flagX = purchaseSummaryCard != null && purchaseSummaryCard.Visible
                        ? purchaseSummaryCard.Left + flagMargin
                        : cw - flagW - margin;
                    var flagY = Math.Max(tbHeight + margin, btnY - flagH - 60);
                    pictureBoxFlag.Location = new System.Drawing.Point(flagX, flagY);
                    pictureBoxFlag.Size = new System.Drawing.Size(flagW, flagH);
                    pictureBoxFlag.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
                    RoundControl(pictureBoxFlag, 8);
                }

                // Filter + DataGridView: fills available center space
                var gridX = leftPanel.Right + margin;
                var gridInnerGap = 8;
                var gridRightLimit = purchaseSummaryCard != null && purchaseSummaryCard.Visible
                    ? purchaseSummaryCard.Left
                    : cw - margin;
                var gridW = Math.Max(320, gridRightLimit - gridX - gridInnerGap);

                if (txtFiltro != null)
                {
                    txtFiltro.Location = new System.Drawing.Point(gridX, tbHeight + margin + 10);
                    txtFiltro.Width = gridW;
                    txtFiltro.Height = 24;
                }

                if (dgvTiquetes != null)
                {
                    var filterH = txtFiltro != null ? txtFiltro.Height + 6 : 0;
                    dgvTiquetes.Location = new System.Drawing.Point(gridX, tbHeight + margin + 10 + filterH);
                    dgvTiquetes.Size = new System.Drawing.Size(
                        gridW,
                        Math.Max(180, Math.Min(this.ClientSize.Height - 105, (pictureBoxFlag?.Top ?? this.ClientSize.Height) - margin) - dgvTiquetes.Top));
                    RoundControl(dgvTiquetes, 8);
                }

                // Copyright at bottom-left
                if (copyrightLabel != null)
                {
                    copyrightLabel.Location = new System.Drawing.Point(margin, this.ClientSize.Height - 30);
                }
            }
            catch { }
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        private void EnableDarkTitleBar()
        {
            if (Environment.OSVersion.Version.Major < 10) return;
            int value = 1;
            DwmSetWindowAttribute(this.Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref value, sizeof(int));
        }

        // ChatGPT-inspired modern dark theme
        private void ApplyModernStyle()
        {
            try { this.Font = new System.Drawing.Font("Segoe UI", 10F); } catch { }
            this.DoubleBuffered = true;
            EnableDarkTitleBar();

            var bg = System.Drawing.Color.FromArgb(18, 18, 18);
            var surface = System.Drawing.Color.FromArgb(32, 32, 32);
            var card = System.Drawing.Color.FromArgb(42, 42, 42);
            var border = System.Drawing.Color.FromArgb(55, 55, 55);
            var textPrimary = System.Drawing.Color.FromArgb(236, 236, 236);
            var textSecondary = System.Drawing.Color.FromArgb(180, 180, 180);
            var accent = System.Drawing.Color.FromArgb(16, 163, 127);
            var accentHover = System.Drawing.Color.FromArgb(20, 180, 140);
            var danger = System.Drawing.Color.FromArgb(239, 68, 68);
            var neutral = System.Drawing.Color.FromArgb(60, 60, 60);
            var gridBg = System.Drawing.Color.FromArgb(24, 24, 24);
            var gridAlt = System.Drawing.Color.FromArgb(32, 32, 32);
            var gridHeader = System.Drawing.Color.FromArgb(36, 36, 36);

            this.BackColor = bg;

            // Left panel
            if (leftPanel != null)
            {
                leftPanel.BackColor = surface;
                RoundControl(leftPanel, 10);
            }
            if (leftFlowPanel != null)
            {
                leftFlowPanel.BackColor = surface;
            }

            // Labels
            Action<System.Windows.Forms.Label, System.Drawing.Color> styleLabel = (l, c) =>
            {
                if (l == null) return;
                l.ForeColor = c;
            };
            foreach (var lbl in new[] { lblCedula, lblNombre, lblDestino, lblAerolinea, lblOrigen, lblClase, lblTipoViaje, lblIda, lblVuelta, lblPrecio, lblVoucher })
                styleLabel(lbl, textSecondary);
            foreach (var lbl in new[] { copyrightLabel })
                if (lbl != null) lbl.ForeColor = textPrimary;

            // TextBoxes
            foreach (var tb in new[] { txtCedula, txtNombre, txtDetalleRegistro, txtFiltro })
            {
                if (tb == null) continue;
                tb.CustomBackColor = card;
                tb.ForeColor = textPrimary;
                tb.BorderFocusColor = accent;
            }

            // DateTimePicker
            if (dtpFecha != null)
            {
                dtpFecha.BackColor = card;
                dtpFecha.ForeColor = textPrimary;
            }

            // ComboBoxes: custom modern
            foreach (var cb in new[] { cmbDestino, cmbAerolinea, cmbOrigen, cmbClase })
            {
                if (cb == null) continue;
                cb.BorderHoverColor = accent;
                cb.BorderFocusColor = accent;
                cb.ArrowHoverColor = accent;
                cb.ItemHoverColor = accent;
                cb.DropItemHeight = 32;
            }

            // Buttons: ChatGPT-style
            Action<System.Windows.Forms.Button, System.Drawing.Color, System.Drawing.Color, bool> styleBtn = (b, bgColor, hoverColor, fullStyle) =>
            {
                if (b == null) return;
                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderSize = 0;
                b.BackColor = bgColor;
                b.ForeColor = System.Drawing.Color.White;
                b.FlatAppearance.MouseOverBackColor = hoverColor;
                if (fullStyle)
                {
                    b.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                    b.Padding = new Padding(6, 4, 6, 6);
                    RoundControl(b, 8);
                }
            };

            styleBtn(btnComprar, accent, accentHover, true);
            styleBtn(btnTarifa, accent, accentHover, true);
            styleBtn(btnEliminar, danger, System.Drawing.Color.FromArgb(200, 50, 50), false);
            foreach (var b in new[] { btnLimpiar, btnEliminarGuardados, btnEliminarReciente, btnIdioma, btnCerrarPrograma, btnExportPdf })
                styleBtn(b, neutral, System.Drawing.Color.FromArgb(75, 75, 75), false);

            // DataGridView: custom dark
            if (dgvTiquetes != null)
            {
                dgvTiquetes.RowsDefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(22, 22, 22);
                dgvTiquetes.AlternatingRowsDefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(22, 22, 22);
                RoundControl(dgvTiquetes, 8);
            }

            // Flag picture
            if (pictureBoxFlag != null)
            {
                pictureBoxFlag.BackColor = card;
                pictureBoxFlag.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            }
        }

        private void LoadLogo()
        {
            try
            {
                var logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo.png");
                if (File.Exists(logoPath))
                {
                    pictureLogo.Image = System.Drawing.Image.FromFile(logoPath);
                    pictureLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
                    pictureLogo.BackColor = System.Drawing.Color.White;
                }
            }
            catch { }
        }

        private void RoundControl(System.Windows.Forms.Control ctrl, int radius)
        {
            if (ctrl == null) return;
            try
            {
                var rect = new System.Drawing.Rectangle(0, 0, ctrl.Width, ctrl.Height);
                using (var gp = new GraphicsPath())
                {
                    var d = radius * 2;
                    gp.AddArc(rect.X, rect.Y, d, d, 180, 90);
                    gp.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
                    gp.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
                    gp.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
                    gp.CloseFigure();
                    ctrl.Region = new System.Drawing.Region(gp);
                }
            }
            catch
            {
                // ignore region errors
            }
        }

        private void DrawChileFlag(System.Drawing.Graphics g, System.Drawing.Bitmap bmp)
        {
            int w = bmp.Width, h = bmp.Height;
            // left square blue
            int sqW = (int)(w * 0.33f);
            using (var b = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(0, 56, 168))) g.FillRectangle(b, 0, 0, sqW, h);
            // top and bottom stripes white/red
            using (var b = new System.Drawing.SolidBrush(System.Drawing.Color.White)) g.FillRectangle(b, sqW, 0, w - sqW, h/2);
            using (var b = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(206,17,38))) g.FillRectangle(b, sqW, h/2, w - sqW, h/2);
            // white star in blue square
            using (var star = new System.Drawing.SolidBrush(System.Drawing.Color.White))
            {
                g.FillEllipse(star, sqW*0.25f - 5, h*0.4f - 10, 20, 20);
            }
        }

        private void DrawColombiaFlag(System.Drawing.Graphics g, System.Drawing.Bitmap bmp)
        {
            // yellow (50%), blue, red
            int w = bmp.Width, h = bmp.Height;
            using (var b1 = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(255, 209, 0))) g.FillRectangle(b1, 0, 0, w, h*50/100);
            using (var b2 = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(0, 56, 168))) g.FillRectangle(b2, 0, h*50/100, w, h*25/100);
            using (var b3 = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(206,17,38))) g.FillRectangle(b3, 0, h*75/100, w, h*25/100);
        }

        private void DrawGuyanaFlag(System.Drawing.Graphics g, System.Drawing.Bitmap bmp)
        {
            int w = bmp.Width, h = bmp.Height;
            g.Clear(System.Drawing.Color.FromArgb(0, 153, 51));
            // yellow triangle
            var points = new System.Drawing.PointF[] { new System.Drawing.PointF(0,h*0.1f), new System.Drawing.PointF(w*0.6f,h*0.5f), new System.Drawing.PointF(0,h*0.9f)};
            using (var b = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(255,204,0))) g.FillPolygon(b, points);
            using (var b = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(206,17,38)))
            {
                var inner = new System.Drawing.PointF[] { new System.Drawing.PointF(0,h*0.2f), new System.Drawing.PointF(w*0.5f,h*0.5f), new System.Drawing.PointF(0,h*0.8f)};
                g.FillPolygon(b, inner);
            }
        }

        private void DrawVerticalBands(System.Drawing.Graphics g, System.Drawing.Bitmap bmp, System.Drawing.Color left, System.Drawing.Color middle, System.Drawing.Color right)
        {
            int w = bmp.Width, h = bmp.Height;
            using (var b1 = new System.Drawing.SolidBrush(left)) g.FillRectangle(b1, 0, 0, w/3, h);
            using (var b2 = new System.Drawing.SolidBrush(middle)) g.FillRectangle(b2, w/3, 0, w/3, h);
            using (var b3 = new System.Drawing.SolidBrush(right)) g.FillRectangle(b3, 2*w/3, 0, w/3, h);
        }

        private void DrawSwissCross(System.Drawing.Graphics g, System.Drawing.Bitmap bmp)
        {
            int w = bmp.Width, h = bmp.Height;
            using (var b = new System.Drawing.SolidBrush(System.Drawing.Color.White))
            {
                float bar = Math.Min(w,h)/5f;
                g.FillRectangle(b, w*0.4f - bar/2, h*0.15f, bar, h*0.7f);
                g.FillRectangle(b, w*0.15f, h*0.4f - bar/2, w*0.7f, bar);
            }
        }

        // (Revertido) El owner-draw con emojis puede ser inconsistente en WinForms/GDI+.
        // Por compatibilidad dejamos el comportamiento por defecto del ComboBox.

        private void ApplyLanguage()
        {
            // minimal localization using Program.SelectedLanguage
            var lang = Program.SelectedLanguage ?? "es";
            if (lang == "en")
            {
                this.Text = "Ticket Sales - FlightsCR.SA";
                lblCedula.Text = "ID:";
                lblNombre.Text = "Name:";
                lblDestino.Text = "Destination:";
                lblAerolinea.Text = "Airline:";
                lblOrigen.Text = "Origin:";
                lblClase.Text = "Class:";
                lblTipoViaje.Text = "Trip:";
                lblIda.Text = "Out:";
                lblVuelta.Text = "Return:";
                btnTarifa.Text = "Fare";
                btnComprar.Text = "Buy";
                lblPrecio.Text = "Price:";
                lblVoucher.Text = "Voucher:";
                grpDetalleRegistro.Text = "Record details";
                copyrightLabel.Text = "© VuelosCR.SA - Developed by Sebastian Mendez Vargas";
                if (dgvTiquetes != null && dgvTiquetes.Columns.Count >= 11)
                {
                    dgvTiquetes.Columns["Cedula"].HeaderText = "ID";
                    dgvTiquetes.Columns["Nombre"].HeaderText = "Name";
                    dgvTiquetes.Columns["Origen"].HeaderText = "Origin";
                    dgvTiquetes.Columns["Destino"].HeaderText = "Destination";
                    dgvTiquetes.Columns["Aerolinea"].HeaderText = "Airline";
                    dgvTiquetes.Columns["Clase"].HeaderText = "Class";
                    dgvTiquetes.Columns["TipoViaje"].HeaderText = "Type";
                    dgvTiquetes.Columns["Fecha"].HeaderText = "Date";
                    dgvTiquetes.Columns["MontoIVA"].HeaderText = "VATAmount";
                    dgvTiquetes.Columns["PrecioFinal"].HeaderText = "FinalPrice";
                    ConfigureTicketGridColumnPresentation();
                }
            }
            else
            {
                this.Text = "Venta de Tiquetes - VuelosCR.SA";
                lblCedula.Text = "Cédula:";
                lblNombre.Text = "Nombre:";
                lblDestino.Text = "Destino:";
                lblAerolinea.Text = "Aerolínea:";
                lblOrigen.Text = "Origen:";
                lblClase.Text = "Clase:";
                lblTipoViaje.Text = "Tipo:";
                lblIda.Text = "Ida:";
                lblVuelta.Text = "Vuelta:";
                btnTarifa.Text = "Tarifa";
                btnComprar.Text = "Comprar";
                lblPrecio.Text = "Precio:";
                lblVoucher.Text = "Voucher:";
                grpDetalleRegistro.Text = "Detalle del registro";
                copyrightLabel.Text = "© VuelosCR.SA - Desarrollado por Sebastian Mendez Vargas";
                if (dgvTiquetes != null && dgvTiquetes.Columns.Count >= 11)
                {
                    dgvTiquetes.Columns["Cedula"].HeaderText = "C\u00e9dula";
                    dgvTiquetes.Columns["Nombre"].HeaderText = "Nombre";
                    dgvTiquetes.Columns["Origen"].HeaderText = "Origen";
                    dgvTiquetes.Columns["Destino"].HeaderText = "Destino";
                    dgvTiquetes.Columns["Aerolinea"].HeaderText = "Aerol\u00ednea";
                    dgvTiquetes.Columns["Clase"].HeaderText = "Clase";
                    dgvTiquetes.Columns["TipoViaje"].HeaderText = "Tipo";
                    dgvTiquetes.Columns["Fecha"].HeaderText = "Fecha";
                    dgvTiquetes.Columns["MontoIVA"].HeaderText = "MontoIVA";
                    dgvTiquetes.Columns["PrecioFinal"].HeaderText = "PrecioFinal";
                    ConfigureTicketGridColumnPresentation();
                }
            }

            // Flight preview card
            if (flightPreview != null) flightPreview.ApplyLanguage();

            // Purchase summary card
            if (purchaseSummaryCard != null) purchaseSummaryCard.ApplyLanguage();

            // Toolbar tooltips and dropdown items
            var ts = this.Controls["toolStrip"] as ToolStrip;
            if (ts != null)
            {
                if (ts.Items[0] is ToolStripButton b0) b0.ToolTipText = lang == "en" ? "Delete selected" : "Eliminar seleccionado";
                if (ts.Items[1] is ToolStripButton b1) b1.ToolTipText = lang == "en" ? "Clear all" : "Limpiar todo";
                if (ts.Items[3] is ToolStripButton b3) b3.ToolTipText = lang == "en" ? "Export PDF" : "Exportar PDF";
                if (ts.Items[5] is ToolStripDropDownButton dd)
                {
                    dd.ToolTipText = lang == "en" ? "More options" : "Más opciones";
                    if (dd.DropDownItems.Count >= 6)
                    {
                        dd.DropDownItems[0].Text = lang == "en" ? "Delete saved data" : "Eliminar guardados";
                        dd.DropDownItems[1].Text = lang == "en" ? "Delete latest" : "Eliminar reciente";
                        dd.DropDownItems[3].Text = lang == "en" ? "Language" : "Idioma";
                        dd.DropDownItems[4].Text = lang == "en" ? "Exit" : "Cerrar";
                    }
                }
            }
        }

        private void ConfigureTicketGridColumnPresentation()
        {
            var columns = dgvTiquetes.Columns;
            columns["Cedula"].MinimumWidth = 90;
            columns["Nombre"].MinimumWidth = 135;
            columns["Origen"].MinimumWidth = 90;
            columns["Destino"].MinimumWidth = 110;
            columns["Aerolinea"].MinimumWidth = 120;
            columns["Clase"].MinimumWidth = 75;
            columns["TipoViaje"].MinimumWidth = 55;
            columns["Fecha"].MinimumWidth = 175;
            columns["MontoIVA"].MinimumWidth = 90;
            columns["PrecioFinal"].MinimumWidth = 105;

            columns["Cedula"].FillWeight = 85;
            columns["Nombre"].FillWeight = 135;
            columns["Origen"].FillWeight = 90;
            columns["Destino"].FillWeight = 110;
            columns["Aerolinea"].FillWeight = 125;
            columns["Clase"].FillWeight = 75;
            columns["TipoViaje"].FillWeight = 55;
            columns["Fecha"].FillWeight = 175;
            columns["MontoIVA"].FillWeight = 90;
            columns["PrecioFinal"].FillWeight = 105;

            columns["Cedula"].ToolTipText = Program.SelectedLanguage == "en" ? "Customer identification" : "Cédula del cliente";
            columns["Nombre"].ToolTipText = Program.SelectedLanguage == "en" ? "Passenger name" : "Nombre del pasajero";
            columns["Origen"].ToolTipText = Program.SelectedLanguage == "en" ? "Departure origin" : "Origen de salida";
            columns["Destino"].ToolTipText = Program.SelectedLanguage == "en" ? "Flight destination" : "Destino del vuelo";
            columns["Aerolinea"].ToolTipText = Program.SelectedLanguage == "en" ? "Selected airline" : "Aerolínea seleccionada";
            columns["Clase"].ToolTipText = Program.SelectedLanguage == "en" ? "Service class" : "Clase de servicio";
            columns["TipoViaje"].ToolTipText = Program.SelectedLanguage == "en" ? "Trip type" : "Tipo de viaje";
            columns["Fecha"].ToolTipText = Program.SelectedLanguage == "en" ? "Flight date(s)" : "Fecha(s) del vuelo";
            columns["MontoIVA"].ToolTipText = Program.SelectedLanguage == "en" ? "VAT amount (13%)" : "Monto de IVA (13%)";
            columns["PrecioFinal"].ToolTipText = Program.SelectedLanguage == "en" ? "Final ticket price" : "Precio final del tiquete";

            columns["PrecioBase"].Visible = false;
        }

        private void dgvTiquetes_MouseMove(object sender, MouseEventArgs e)
        {
            var hit = dgvTiquetes.HitTest(e.X, e.Y);
            if (hit.Type == DataGridViewHitTestType.ColumnHeader && hit.ColumnIndex >= 0)
            {
                var text = dgvTiquetes.Columns[hit.ColumnIndex].ToolTipText;
                toolTip1.Show(text, dgvTiquetes, e.X + 12, e.Y + 18, 4000);
            }
            else
            {
                toolTip1.Hide(dgvTiquetes);
            }
        }

        private void dgvTiquetes_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Hide(dgvTiquetes);
        }

        private void LoadDataToControls()
        {
            cmbDestino.Items.Clear();
            foreach (var country in config.DestinosNombres)
            {
                cmbDestino.Items.Add(new DestinationItem(country, country));
            }
            if (cmbDestino.Items.Count > 0) cmbDestino.SelectedIndex = 0;

            cmbAerolinea.Items.Clear();
            foreach (var a in config.AerolineasNombres) cmbAerolinea.Items.Add(a);
            if (cmbAerolinea.Items.Count > 0) cmbAerolinea.SelectedIndex = 0;

            cmbOrigen.Items.Clear();
            foreach (var o in config.OrigenesNombres) cmbOrigen.Items.Add(o);
            if (cmbOrigen.Items.Count > 0) cmbOrigen.SelectedIndex = 0;

            UpdateClaseCombo();

            cmbTipoViaje.Items.Clear();
            cmbTipoViaje.Items.Add("Ida");
            cmbTipoViaje.Items.Add("Ida y Vuelta");
            if (cmbTipoViaje.Items.Count > 0) cmbTipoViaje.SelectedIndex = 0;

            // inicializar pictureBox vacío (color de fondo marcado para que destaque)
            try
            {
                pictureBoxFlag.BackColor = System.Drawing.Color.FromArgb(255, 240, 240);
            }
            catch
            {
                // ignorar si no está creado aún
            }
        }

        private void cmbDestino_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string country = null;
                var di = cmbDestino.SelectedItem as DestinationItem;
                if (di != null) country = di.Value;
                else country = cmbDestino.SelectedItem?.ToString();

                if (string.IsNullOrEmpty(country))
                {
                    pictureBoxFlag.Image = null;
                    pictureBoxFlag.Invalidate();
                    return;
                }

                // intentar cargar recurso por nombre mapeado
                var resName = GetResourceNameForCountry(country);
                if (!string.IsNullOrEmpty(resName))
                {
                    try
                    {
                        var img = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject(resName);
                        if (img != null)
                        {
                            pictureBoxFlag.Image = img;
                            pictureBoxFlag.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
                            return;
                        }
                    }
                    catch
                    {
                        // no encontrado o error, seguir
                    }
                }
                // intentar cargar desde cache o descargar de internet
                var imgFromWeb = TryLoadFlagFromCacheOrDownload(country);
                if (imgFromWeb != null)
                {
                    pictureBoxFlag.Image = imgFromWeb;
                    pictureBoxFlag.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
                    return;
                }

                // fallback: generar una bandera simple programáticamente
                var bmp = CreateFlagBitmap(country, pictureBoxFlag.Width, pictureBoxFlag.Height);
                pictureBoxFlag.Image = bmp;
                pictureBoxFlag.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;

                // update purchase summary
                UpdatePurchaseSummaryFields();
            }
            catch
            {
                // ignorar errores de carga
            }
        }

        private void UpdatePurchaseSummaryFields()
        {
            try
            {
                if (purchaseSummaryCard == null) return;

                var lang = Program.SelectedLanguage ?? "es";
                var destPrefix = lang == "en" ? "Destination" : "Destino";
                var airlinePrefix = lang == "en" ? "Airline" : "Aerol\u00ednea";
                var originPrefix = lang == "en" ? "Origin" : "Origen";
                var clasePrefix = lang == "en" ? "Class" : "Clase";
                var tripPrefix = lang == "en" ? "Trip" : "Tipo";
                var datePrefix = lang == "en" ? "Date" : "Fecha";
                var returnPrefix = lang == "en" ? "Return" : "Vuelta";
                var passengerPrefix = lang == "en" ? "Passenger" : "Pasajero";
                var baseFarePrefix = lang == "en" ? "Base Fare" : "Tarifa Base";
                var serviceFeePrefix = lang == "en" ? "Service Fee" : "Cargo Servicio";
                var vatPrefix = lang == "en" ? "VAT" : "IVA";
                var totalPrefix = lang == "en" ? "Total" : "Total";
                var voucherPrefix = lang == "en" ? "Voucher" : "Voucher";

                var di = cmbDestino.SelectedItem as DestinationItem;
                var dest = di != null ? di.Value : cmbDestino.SelectedItem?.ToString();
                var origin = cmbOrigen.SelectedItem?.ToString() ?? "-";
                var clase = cmbClase.SelectedItem?.ToString() ?? "-";
                var tipo = cmbTipoViaje.SelectedItem?.ToString() ?? "Ida";
                var isRoundTrip = tipo == "Ida y Vuelta";
                var dateText = datePrefix + ": " + dtpFecha.Value.ToString("yyyy-MM-dd");
                var returnDateText = isRoundTrip ? returnPrefix + ": " + dtpFechaVuelta.Value.ToString("yyyy-MM-dd") : "";
                var tripText = tripPrefix + ": " + tipo;
                var voucherText = voucherPrefix + ": " + (_lastTicketId != null ? _lastTicketId.Substring(0, 8) + "..." : "-");
                var passengerText = passengerPrefix + ": " + (txtNombre.Text ?? "-");
                var destText = destPrefix + ": " + (dest ?? "-");
                var originText = originPrefix + ": " + origin;
                var airlineText = airlinePrefix + ": " + (cmbAerolinea.SelectedItem?.ToString() ?? "-");
                var claseText = clasePrefix + ": " + clase;

                if (TryCreateTicketFromInputs(out var ti, out var _))
                {
                    purchaseSummaryCard.UpdateData(
                        voucherText, passengerText, destText, originText, airlineText, claseText, tripText, dateText, returnDateText,
                        baseFarePrefix + ": $" + ti.PrecioBase.ToString("N2"),
                        serviceFeePrefix + ": $" + ti.CalcularMontoServicio().ToString("N2"),
                        vatPrefix + ": $" + ti.CalcularMontoIVA().ToString("N2"),
                        totalPrefix + ": $" + ti.CalcularPrecioFinalTiquete().ToString("N2")
                    );
                }
                else
                {
                    purchaseSummaryCard.UpdateData(
                        voucherText, passengerText, destText, originText, airlineText, claseText, tripText, dateText, returnDateText,
                        baseFarePrefix + ": -",
                        serviceFeePrefix + ": -",
                        vatPrefix + ": -",
                        totalPrefix + ": -"
                    );
                }
            }
            catch { }
        }

        // simple logo paint: draw a small plane icon
        private void txtNombre_TextChanged(object sender, EventArgs e)
        {
            UpdatePurchaseSummaryFields();
        }

        private void cmbAerolinea_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateClaseCombo();
            UpdatePurchaseSummaryFields();
        }

        private void UpdateClaseCombo()
        {
            var aerolinea = cmbAerolinea.SelectedItem?.ToString();
            cmbClase.Items.Clear();
            if (!string.IsNullOrEmpty(aerolinea))
            {
                var clases = config.GetClasesForAirline(aerolinea);
                foreach (var c in clases)
                    cmbClase.Items.Add(c);
            }
            if (cmbClase.Items.Count > 0)
                cmbClase.SelectedIndex = 0;
        }

        private void cmbClase_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePurchaseSummaryFields();
        }

        private void cmbOrigen_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePurchaseSummaryFields();
        }

        private void dtpFecha_ValueChanged(object sender, EventArgs e)
        {
            UpdatePurchaseSummaryFields();
        }

        private void dtpFechaVuelta_ValueChanged(object sender, EventArgs e)
        {
            UpdatePurchaseSummaryFields();
        }

        private void cmbTipoViaje_SelectedIndexChanged(object sender, EventArgs e)
        {
            var isRoundTrip = cmbTipoViaje.SelectedItem?.ToString() == "Ida y Vuelta";
            dtpFechaVuelta.Visible = isRoundTrip;
            lblVuelta.Visible = isRoundTrip;
            UpdatePurchaseSummaryFields();
            AdjustLayout();
        }

        private void txtFiltro_TextChanged(object sender, EventArgs e)
        {
            var filter = txtFiltro.Text?.Trim().ToLower() ?? "";
            var ticketsEnum = repo.GetAll();
            var tickets = new System.Collections.Generic.List<Tiquete>(ticketsEnum);

            dgvTiquetes.SelectionChanged -= dgvTiquetes_SelectionChanged;
            dgvTiquetes.Rows.Clear();

            foreach (var ti in tickets)
            {
                if (!string.IsNullOrEmpty(filter))
                {
                    var match = (ti.Cliente?.Cedula?.ToLower().Contains(filter) ?? false)
                        || (ti.Cliente?.Nombre?.ToLower().Contains(filter) ?? false)
                        || (ti.Origen?.ToLower().Contains(filter) ?? false)
                        || (ti.Destino?.ToLower().Contains(filter) ?? false)
                        || (ti.Aerolinea?.ToLower().Contains(filter) ?? false)
                        || (ti.Clase?.ToLower().Contains(filter) ?? false)
                        || (ti.TipoViaje?.ToLower().Contains(filter) ?? false)
                        || ti.FechaVuelo.ToString("yyyy-MM-dd").Contains(filter);
                    if (!match) continue;
                }

                var rowIndex = dgvTiquetes.Rows.Add(
                    ti.Cliente?.Cedula,
                    ti.Cliente?.Nombre,
                    ti.Origen,
                    ti.Destino,
                    ti.Aerolinea,
                    ti.Clase,
                    ti.TipoViaje,
                    ti.TipoViaje == "Ida y Vuelta"
                        ? ti.FechaVuelo.ToString("yyyy-MM-dd") + " \u2192 " + ti.FechaVuelta.ToString("yyyy-MM-dd")
                        : ti.FechaVuelo.ToString("yyyy-MM-dd"),
                    ti.CalcularMontoIVA(),
                    ti.CalcularPrecioFinalTiquete(),
                    ti.PrecioBase);
                dgvTiquetes.Rows[rowIndex].Tag = ti;
            }

            dgvTiquetes.SelectionChanged += dgvTiquetes_SelectionChanged;
        }

        private void dgvTiquetes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvTiquetes.Rows[e.RowIndex];
            var ticket = row.Tag as Tiquete ?? CreateTicketFromRow(row);
            if (ticket == null) return;

            txtCedula.Text = ticket.Cliente?.Cedula ?? "";
            txtNombre.Text = ticket.Cliente?.Nombre ?? "";

            foreach (var item in cmbOrigen.Items)
            {
                if (item.ToString() == ticket.Origen) { cmbOrigen.SelectedItem = item; break; }
            }
            foreach (var item in cmbDestino.Items)
            {
                var val = item is DestinationItem di ? di.Value : item.ToString();
                if (val == ticket.Destino) { cmbDestino.SelectedItem = item; break; }
            }
            foreach (var item in cmbAerolinea.Items)
            {
                if (item.ToString() == ticket.Aerolinea) { cmbAerolinea.SelectedItem = item; break; }
            }
            // clase combo depends on airline, will auto-update via SelectedIndexChanged
            foreach (var item in cmbClase.Items)
            {
                if (item.ToString() == ticket.Clase) { cmbClase.SelectedItem = item; break; }
            }
            dtpFecha.Value = ticket.FechaVuelo;

            // update the ticket in db
            if (TryCreateTicketFromInputs(out var updated, out var _))
            {
                updated.Id = ticket.Id;
                repo.Update(updated);
                ToastNotifier.Show(this, Program.SelectedLanguage == "en" ? "Record updated." : "Registro actualizado.", 2000);
                RefreshTicketGrid();
            }
        }

        private System.Drawing.Image TryLoadFlagFromCacheOrDownload(string country)
        {
            try
            {
                var code = CountryToIso(country);
                if (string.IsNullOrEmpty(code)) return null;

                var flagsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "flags");
                if (!Directory.Exists(flagsDir)) Directory.CreateDirectory(flagsDir);

                var fileName = code.ToLowerInvariant() + ".png";
                var filePath = Path.Combine(flagsDir, fileName);
                if (File.Exists(filePath))
                {
                    // load from disk
                    try
                    {
                        using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            return System.Drawing.Image.FromStream(fs);
                        }
                    }
                    catch
                    {
                        // if can't load, delete and attempt download
                        try { File.Delete(filePath); } catch { }
                    }
                }

                // descargar desde flagcdn
                var url = $"https://flagcdn.com/w320/{code.ToLowerInvariant()}.png";
                try
                {
                    using (var wc = new System.Net.WebClient())
                    {
                        wc.Headers.Add("User-Agent", "Mozilla/5.0");
                        wc.DownloadFile(url, filePath);
                    }
                    using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        return System.Drawing.Image.FromStream(fs);
                    }
                }
                catch
                {
                    // descarga fallida
                    try { if (File.Exists(filePath)) File.Delete(filePath); } catch { }
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        private string CountryToIso(string country)
        {
            if (string.IsNullOrEmpty(country)) return null;
            switch (country)
            {
                case "Costa Rica": return "cr";
                case "Argentina": return "ar";
                case "Bolivia": return "bo";
                case "Brasil": return "br";
                case "Chile": return "cl";
                case "Colombia": return "co";
                case "Ecuador": return "ec";
                case "Guyana": return "gy";
                case "Nicaragua": return "ni";
                case "México": return "mx";
                case "Estados Unidos": return "us";
                case "Japón": return "jp";
                case "China": return "cn";
                case "Reino Unido": return "gb";
                case "España": return "es";
                case "Francia": return "fr";
                case "Alemania": return "de";
                case "Italia": return "it";
                case "Países Bajos": return "nl";
                case "Suiza": return "ch";
                default: return null;
            }
        }

        private string GetResourceNameForCountry(string country)
        {
            // nombres simples para recursos (sin tildes y espacios)
            switch (country)
            {
                case "Costa Rica": return "flag_costa_rica";
                case "Argentina": return "flag_argentina";
                case "Bolivia": return "flag_bolivia";
                case "Brasil": return "flag_brasil";
                case "Chile": return "flag_chile";
                case "Colombia": return "flag_colombia";
                case "Ecuador": return "flag_ecuador";
                case "Guyana": return "flag_guyana";
                case "Nicaragua": return "flag_nicaragua";
                case "México": return "flag_mexico";
                case "Estados Unidos": return "flag_usa";
                case "Japón": return "flag_japan";
                case "China": return "flag_china";
                case "Reino Unido": return "flag_uk";
                case "España": return "flag_spain";
                case "Francia": return "flag_france";
                case "Alemania": return "flag_germany";
                case "Italia": return "flag_italy";
                case "Países Bajos": return "flag_netherlands";
                case "Suiza": return "flag_switzerland";
                default: return null;
            }
        }

        // Genera una bandera dibujada programáticamente para algunos países; si no soportado, muestra un placeholder
        private System.Drawing.Bitmap CreateFlagBitmap(string country, int w, int h)
        {
            var bmp = new System.Drawing.Bitmap(Math.Max(1, w), Math.Max(1, h));
            using (var g = System.Drawing.Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                // default background
                g.Clear(System.Drawing.Color.White);

                switch (country)
                {
                    case "Argentina":
                        // tres franjas horizontales celeste-blanco-celeste y disco amarillo
                        DrawHorizontalBands(g, bmp, System.Drawing.Color.FromArgb(116, 172, 223), System.Drawing.Color.White, System.Drawing.Color.FromArgb(116, 172, 223));
                        DrawCenteredCircle(g, bmp, System.Drawing.Color.Gold);
                        break;
                    case "Costa Rica":
                        DrawHorizontalBands(g, bmp, System.Drawing.Color.FromArgb(0, 43, 127), System.Drawing.Color.White, System.Drawing.Color.FromArgb(0, 43, 127));
                        DrawCenteredCircle(g, bmp, System.Drawing.Color.FromArgb(196, 30, 58), 0.25f);
                        break;
                    case "Estados Unidos":
                        DrawUsaFlag(g, bmp);
                        break;
                    case "España":
                        DrawHorizontalBands(g, bmp, System.Drawing.Color.Red, System.Drawing.Color.FromArgb(255, 204, 0), System.Drawing.Color.Red);
                        break;
                    case "Japón":
                        g.Clear(System.Drawing.Color.White);
                        DrawCenteredCircle(g, bmp, System.Drawing.Color.Red, 0.5f);
                        break;
                    case "China":
                        g.Clear(System.Drawing.Color.FromArgb(222, 41, 16));
                        DrawChinaStars(g, bmp);
                        break;
                    case "Reino Unido":
                        DrawUnionJack(g, bmp);
                        break;
                    case "Brasil":
                        // Verde fondo, rombo amarillo y círculo azul
                        g.Clear(System.Drawing.Color.FromArgb(0, 156, 59));
                        DrawDiamond(g, bmp, System.Drawing.Color.Gold);
                        DrawCenteredCircle(g, bmp, System.Drawing.Color.FromArgb(0, 39, 118), 0.45f);
                        break;
                    case "Bolivia":
                        DrawHorizontalBands(g, bmp, System.Drawing.Color.FromArgb(204, 0, 0), System.Drawing.Color.FromArgb(255, 204, 0), System.Drawing.Color.FromArgb(0, 153, 51));
                        break;
                    case "Chile":
                        DrawChileFlag(g, bmp);
                        break;
                    case "Colombia":
                        DrawColombiaFlag(g, bmp);
                        break;
                    case "Ecuador":
                        DrawColombiaFlag(g, bmp); // similar layout
                        break;
                    case "Guyana":
                        DrawGuyanaFlag(g, bmp);
                        break;
                    case "Nicaragua":
                        DrawHorizontalBands(g, bmp, System.Drawing.Color.FromArgb(0, 102, 204), System.Drawing.Color.White, System.Drawing.Color.FromArgb(0, 102, 204));
                        DrawCenteredCircle(g, bmp, System.Drawing.Color.LightBlue, 0.35f);
                        break;
                    case "México":
                        DrawVerticalBands(g, bmp, System.Drawing.Color.FromArgb(0, 104, 71), System.Drawing.Color.White, System.Drawing.Color.FromArgb(206, 17, 38));
                        DrawCenteredCircle(g, bmp, System.Drawing.Color.DarkGreen, 0.25f);
                        break;
                    case "Francia":
                        DrawVerticalBands(g, bmp, System.Drawing.Color.FromArgb(0, 85, 164), System.Drawing.Color.White, System.Drawing.Color.FromArgb(239, 65, 53));
                        break;
                    case "Alemania":
                        DrawHorizontalBands(g, bmp, System.Drawing.Color.Black, System.Drawing.Color.FromArgb(221, 0, 0), System.Drawing.Color.FromArgb(255, 204, 0));
                        break;
                    case "Italia":
                        DrawVerticalBands(g, bmp, System.Drawing.Color.FromArgb(0, 146, 70), System.Drawing.Color.White, System.Drawing.Color.FromArgb(206, 43, 55));
                        break;
                    case "Países Bajos":
                        DrawHorizontalBands(g, bmp, System.Drawing.Color.FromArgb(174, 28, 40), System.Drawing.Color.White, System.Drawing.Color.FromArgb(33, 70, 139));
                        break;
                    case "Suiza":
                        g.Clear(System.Drawing.Color.FromArgb(206, 17, 38));
                        DrawSwissCross(g, bmp);
                        break;
                    default:
                        // placeholder con nombre
                        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.LightGray))
                        {
                            g.FillRectangle(brush, 0, 0, bmp.Width, bmp.Height);
                        }
                        using (var pen = new System.Drawing.Pen(System.Drawing.Color.DarkGray, 2))
                        {
                            g.DrawRectangle(pen, 1, 1, bmp.Width - 3, bmp.Height - 3);
                        }
                        var text = country;
                        using (var font = new System.Drawing.Font("Segoe UI", Math.Max(8, bmp.Width / 15)))
                        using (var sf = new System.Drawing.StringFormat() { Alignment = System.Drawing.StringAlignment.Center, LineAlignment = System.Drawing.StringAlignment.Center })
                        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.Black))
                        {
                            g.DrawString(text, font, brush, new System.Drawing.RectangleF(0, 0, bmp.Width, bmp.Height), sf);
                        }
                        break;
                }
            }
            return bmp;
        }

        private void DrawHorizontalBands(System.Drawing.Graphics g, System.Drawing.Bitmap bmp, System.Drawing.Color top, System.Drawing.Color middle, System.Drawing.Color bottom)
        {
            int h = bmp.Height;
            int w = bmp.Width;
            using (var b1 = new System.Drawing.SolidBrush(top)) g.FillRectangle(b1, 0, 0, w, h / 3);
            using (var b2 = new System.Drawing.SolidBrush(middle)) g.FillRectangle(b2, 0, h / 3, w, h / 3);
            using (var b3 = new System.Drawing.SolidBrush(bottom)) g.FillRectangle(b3, 0, 2 * h / 3, w, h / 3);
        }

        private void DrawCenteredCircle(System.Drawing.Graphics g, System.Drawing.Bitmap bmp, System.Drawing.Color color, float relativeSize = 0.3f)
        {
            int w = bmp.Width; int h = bmp.Height;
            float diameter = Math.Min(w, h) * relativeSize;
            var rect = new System.Drawing.RectangleF((w - diameter) / 2f, (h - diameter) / 2f, diameter, diameter);
            using (var b = new System.Drawing.SolidBrush(color)) g.FillEllipse(b, rect);
        }

        private void DrawDiamond(System.Drawing.Graphics g, System.Drawing.Bitmap bmp, System.Drawing.Color color)
        {
            int w = bmp.Width; int h = bmp.Height;
            var points = new System.Drawing.PointF[]
            {
                new System.Drawing.PointF(w * 0.5f, h * 0.1f),
                new System.Drawing.PointF(w * 0.9f, h * 0.5f),
                new System.Drawing.PointF(w * 0.5f, h * 0.9f),
                new System.Drawing.PointF(w * 0.1f, h * 0.5f)
            };
            using (var b = new System.Drawing.SolidBrush(color)) g.FillPolygon(b, points);
        }

        private void DrawUsaFlag(System.Drawing.Graphics g, System.Drawing.Bitmap bmp)
        {
            int w = bmp.Width; int h = bmp.Height;
            // 13 stripes
            for (int i = 0; i < 13; i++)
            {
                var color = (i % 2 == 0) ? System.Drawing.Color.FromArgb(178, 34, 52) : System.Drawing.Color.White;
                using (var b = new System.Drawing.SolidBrush(color))
                    g.FillRectangle(b, 0, i * (h / 13f), w, h / 13f + 1);
            }
            // canton
            var cantonH = (int)(h * 7f / 13f);
            var cantonW = (int)(w * 0.4f);
            using (var b = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(10, 49, 97))) g.FillRectangle(b, 0, 0, cantonW, cantonH);
            // stars simplified as small white dots
            int rows = 5; int cols = 6;
            float padX = cantonW / (cols + 1f);
            float padY = cantonH / (rows + 1f);
            using (var starBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White))
            {
                for (int r = 1; r <= rows; r++)
                for (int c = 1; c <= cols; c++)
                {
                    float cx = c * padX - padX/2;
                    float cy = r * padY - padY/2;
                    g.FillEllipse(starBrush, cx - 3, cy - 3, 6, 6);
                }
            }
        }

        private void DrawChinaStars(System.Drawing.Graphics g, System.Drawing.Bitmap bmp)
        {
            int w = bmp.Width; int h = bmp.Height;
            // big star + 4 small
            using (var b = new System.Drawing.SolidBrush(System.Drawing.Color.Gold))
            {
                // big star as circle for simplicity
                g.FillEllipse(b, w * 0.08f, h * 0.08f, w * 0.18f, h * 0.18f);
                float sx = w * 0.6f, sy = h * 0.12f, s = Math.Min(w,h)*0.06f;
                g.FillEllipse(b, sx, sy, s, s);
                g.FillEllipse(b, sx + s*1.6f, sy + s*0.8f, s, s);
                g.FillEllipse(b, sx + s*1.6f, sy + s*2.4f, s, s);
                g.FillEllipse(b, sx, sy + s*3.2f, s, s);
            }
        }

        private void DrawUnionJack(System.Drawing.Graphics g, System.Drawing.Bitmap bmp)
        {
            int w = bmp.Width; int h = bmp.Height;
            // blue background
            g.Clear(System.Drawing.Color.FromArgb(1, 33, 105));
            using (var white = new System.Drawing.SolidBrush(System.Drawing.Color.White))
            using (var red = new System.Drawing.SolidBrush(System.Drawing.Color.Red))
            {
                // white diagonals
                var penW = new System.Drawing.Pen(white, Math.Max(6, Math.Min(w,h)/10));
                var penR = new System.Drawing.Pen(red, Math.Max(3, Math.Min(w,h)/18));
                g.DrawLine(penW, 0, 0, w, h);
                g.DrawLine(penW, w, 0, 0, h);
                // red thinner diagonals
                g.DrawLine(penR, 0, 0, w*0.6f, h*0.4f);
                g.DrawLine(penR, w, 0, w*0.4f, h*0.4f);
                g.DrawLine(penR, 0, h, w*0.4f, h*0.6f);
                g.DrawLine(penR, w, h, w*0.6f, h*0.6f);
                // central cross
                g.FillRectangle(white, w*0.45f, 0, w*0.1f, h);
                g.FillRectangle(white, 0, h*0.45f, w, h*0.1f);
                g.FillRectangle(red, w*0.475f, 0, w*0.05f, h);
                g.FillRectangle(red, 0, h*0.475f, w, h*0.05f);
            }
        }

        // El PictureBox de la bandera ya no muestra recuadro ni texto guía.

        private void btnTarifa_Click(object sender, EventArgs e)
        {
            if (!TryCreateTicketFromInputs(out var tiquete, out var errorMessage))
            {
                DarkMessageBox.Show(errorMessage);
                return;
            }

            var precioFinal = tiquete.CalcularPrecioFinalTiquete();
            lblPrecio.Text = Program.SelectedLanguage == "en" ? $"Price: ${precioFinal:N2}" : $"Precio: ${precioFinal:N2}";
            UpdateFlightPreview(tiquete);
        }

        private bool TryCreateTicketFromInputs(out Tiquete tiquete, out string errorMessage)
        {
            var aerolinea = cmbAerolinea.SelectedItem?.ToString();
            var origen = GetSelectedOriginValue();
            var destino = GetSelectedDestinationValue();
            var clase = GetSelectedClaseValue();
            var tipoViaje = cmbTipoViaje.SelectedItem?.ToString() ?? "Ida";
            var porcentaje = config.GetPorcentajeServicio(aerolinea);
            var origenTarifa = config.GetOrigenTarifa(origen);
            var airlineFactor = config.GetFactorPrecio(aerolinea);
            var claseMultiplier = config.GetClaseMultiplier(aerolinea, clase);
            return ticketService.TryCreateTicket(
                txtCedula.Text,
                txtNombre.Text,
                destino,
                aerolinea,
                origen,
                clase,
                tipoViaje,
                dtpFecha.Value,
                dtpFechaVuelta.Value,
                config.Destinos,
                porcentaje,
                origenTarifa,
                airlineFactor,
                claseMultiplier,
                out tiquete,
                out errorMessage,
                Program.SelectedLanguage);
        }

        private class DestinationItem
        {
            public string Value { get; private set; }
            public string Display { get; private set; }
            public DestinationItem(string value, string display)
            {
                this.Value = value;
                this.Display = display;
            }
            public override string ToString()
            {
                return this.Display;
            }
        }

        private void btnComprar_Click(object sender, EventArgs e)
        {
            btnComprar.Enabled = false;
            var originalText = btnComprar.Text;
            btnComprar.Text = Program.SelectedLanguage == "en" ? "Processing..." : "Procesando...";
            Application.DoEvents();

            if (!TryCreateTicketFromInputs(out var ti, out var errorMessage))
            {
                btnComprar.Enabled = true;
                btnComprar.Text = originalText;
                DarkMessageBox.Show(errorMessage);
                return;
            }

            repo.Insert(ti);
            _lastTicketId = ti.Id;
            lblVoucher.Text = voucherService.BuildVoucherText(ti, Program.SelectedLanguage);
            ToastNotifier.Show(this, Program.SelectedLanguage == "en" ? "Purchase completed!" : "¡Compra realizada!", 2500);
            DarkMessageBox.Show(voucherService.BuildPurchaseConfirmation(Program.SelectedLanguage));

            RefreshTicketGrid();
            UpdatePurchaseSummaryFields();
            UpdateSelectedTicketDetails(ti);

            btnComprar.Enabled = true;
            btnComprar.Text = originalText;
        }

        private string GetSelectedDestinationValue()
        {
            if (cmbDestino.SelectedItem is DestinationItem di)
                return di.Value;
            return cmbDestino.SelectedItem?.ToString() ?? "";
        }

        private string GetSelectedOriginValue()
        {
            return cmbOrigen.SelectedItem?.ToString() ?? "";
        }

        private string GetSelectedClaseValue()
        {
            return cmbClase.SelectedItem?.ToString() ?? "";
        }

        private void UpdateFlightPreview(Tiquete ticket)
        {
            if (flightPreview == null) return;
            if (ticket == null)
            {
                flightPreview.Clear();
                flightPreview.Visible = false;
                return;
            }
            flightPreview.UpdateData(
                ticket.Origen,
                ticket.Destino,
                ticket.Aerolinea,
                ticket.PrecioBase,
                ticket.CalcularMontoServicio(),
                ticket.CalcularMontoIVA(),
                ticket.CalcularPrecioFinalTiquete()
            );
            flightPreview.Visible = true;
        }

        private void RefreshTicketGrid()
        {
            dgvTiquetes.SelectionChanged -= dgvTiquetes_SelectionChanged;
            dgvTiquetes.Rows.Clear();
            var ticketsEnum = repo.GetAll();
            var tickets = new System.Collections.Generic.List<Tiquete>(ticketsEnum);
            for (int i = 0; i < tickets.Count; i++)
            {
                var ti = tickets[i];
            var rowIndex = dgvTiquetes.Rows.Add(
                ti.Cliente?.Cedula,
                ti.Cliente?.Nombre,
                ti.Origen,
                ti.Destino,
                ti.Aerolinea,
                ti.Clase,
                ti.TipoViaje,
                ti.TipoViaje == "Ida y Vuelta"
                    ? ti.FechaVuelo.ToString("yyyy-MM-dd") + " \u2192 " + ti.FechaVuelta.ToString("yyyy-MM-dd")
                    : ti.FechaVuelo.ToString("yyyy-MM-dd"),
                ti.CalcularMontoIVA(),
                ti.CalcularPrecioFinalTiquete(),
                ti.PrecioBase);
                dgvTiquetes.Rows[rowIndex].Tag = ti;
            }

            if (dgvTiquetes.Rows.Count > 0)
            {
                // select most recent (last) entry to reflect latest purchase
                dgvTiquetes.ClearSelection();
                var lastIndex = dgvTiquetes.Rows.Count - 1;
                dgvTiquetes.Rows[lastIndex].Selected = true;
                dgvTiquetes.CurrentCell = dgvTiquetes.Rows[lastIndex].Cells[0];
                UpdateSelectedTicketDetails(dgvTiquetes.Rows[lastIndex].Tag as Tiquete);
            }
            else
            {
                UpdateSelectedTicketDetails(null);
            }

            dgvTiquetes.SelectionChanged += dgvTiquetes_SelectionChanged;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvTiquetes.SelectedRows.Count == 0)
            {
                DarkMessageBox.Show(Program.SelectedLanguage == "en" ? "Select a row to delete." : "Seleccione una fila para eliminar.");
                return;
            }

            var row = dgvTiquetes.SelectedRows[0];
            var ticket = row.Tag as Tiquete;
            if (ticket == null)
            {
                ticket = CreateTicketFromRow(row);
            }

            repo.Delete(ticket);
            ToastNotifier.Show(this, Program.SelectedLanguage == "en" ? "Record deleted." : "Registro eliminado.", 2000);
            RefreshTicketGrid();
        }

        private void dgvTiquetes_SelectionChanged(object sender, EventArgs e)
        {
            var row = dgvTiquetes.CurrentRow;
            if (row == null)
            {
                UpdateSelectedTicketDetails(null);
                return;
            }

            var ticket = row.Tag as Tiquete;
            if (ticket == null && row.Cells.Count >= 5)
            {
                ticket = CreateTicketFromRow(row);
            }

            UpdateSelectedTicketDetails(ticket);
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            var r = DarkMessageBox.Show(Program.SelectedLanguage == "en" ? "Do you want to delete all records?" : "¿Desea eliminar todos los registros?", Program.SelectedLanguage == "en" ? "Confirm" : "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (r != DialogResult.Yes) return;

            repo.Clear();
            ToastNotifier.Show(this, Program.SelectedLanguage == "en" ? "All records deleted." : "Registros eliminados.", 2000);
            RefreshTicketGrid();
        }

        private void btnEliminarGuardados_Click(object sender, EventArgs e)
        {
            var r = DarkMessageBox.Show(Program.SelectedLanguage == "en" ? "Do you want to delete the saved data file?" : "¿Desea eliminar el archivo de datos guardados?", Program.SelectedLanguage == "en" ? "Confirm" : "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (r != DialogResult.Yes) return;

            repo.DeleteSavedData();
            ToastNotifier.Show(this, Program.SelectedLanguage == "en" ? "Saved data deleted." : "Archivo eliminado.", 2000);
            RefreshTicketGrid();
        }

        private void btnEliminarReciente_Click(object sender, EventArgs e)
        {
            var r = DarkMessageBox.Show(Program.SelectedLanguage == "en" ? "Do you want to delete the latest record?" : "¿Desea eliminar el registro más reciente?", Program.SelectedLanguage == "en" ? "Confirm" : "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (r != DialogResult.Yes) return;

            repo.DeleteLast();
            ToastNotifier.Show(this, Program.SelectedLanguage == "en" ? "Latest record deleted." : "Último registro eliminado.", 2000);
            RefreshTicketGrid();
        }

        private void btnIdioma_Click(object sender, EventArgs e)
        {
            this.Hide();
            try
            {
                using (var lf = new LanguageForm())
                {
                    if (lf.ShowDialog() == DialogResult.OK)
                    {
                        ApplyLanguage();
                        LoadDataToControls();
                        RefreshTicketGrid();
                    }
                }
            }
            finally
            {
                this.Show();
                this.Activate();
            }
        }

        private void btnCerrarPrograma_Click(object sender, EventArgs e)
        {
            var isEnglish = Program.SelectedLanguage == "en";
            var result = DarkMessageBox.Show(
                isEnglish ? "Do you want to close the program?" : "¿Desea cerrar el programa?",
                isEnglish ? "Confirm" : "Confirmar",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btnExportPdf_Click(object sender, EventArgs e)
        {
            // Exporta un comprobante PDF real del registro seleccionado.
            var row = dgvTiquetes.CurrentRow;
            if (row == null)
            {
                DarkMessageBox.Show(Program.SelectedLanguage == "en" ? "Select a record first." : "Seleccione un registro primero.");
                return;
            }

            var ticket = row.Tag as Tiquete ?? CreateTicketFromRow(row);
            var outDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Exports");
            var nextNumber = DateTime.Now.Ticks % 1000000; // simple unique number
            var pdfPath = vouchersPdfService.SaveVoucherPdf(ticket, outDir, (int)nextNumber, Program.SelectedLanguage);
            DarkMessageBox.Show(Program.SelectedLanguage == "en" ? $"Voucher PDF saved: {pdfPath}" : $"PDF del comprobante guardado: {pdfPath}");
        }

        private void UpdateSelectedTicketDetails(Tiquete ticket)
        {
            if (ticket == null)
            {
                txtDetalleRegistro.Text = Program.SelectedLanguage == "en"
                    ? "No record selected."
                    : "No hay un registro seleccionado.";
                return;
            }

            txtDetalleRegistro.Text = Program.SelectedLanguage == "en"
                ? $"ID: {ticket.Cliente?.Cedula}\r\nName: {ticket.Cliente?.Nombre}\r\nOrigin: {ticket.Origen}\r\nDestination: {ticket.Destino}\r\nAirline: {ticket.Aerolinea}\r\nClass: {ticket.Clase}\r\nDate: {ticket.FechaVuelo:yyyy-MM-dd}\r\nBase price: ${ticket.PrecioBase:N2}\r\nService: ${ticket.CalcularMontoServicio():N2}\r\nVAT: ${ticket.CalcularMontoIVA():N2}\r\nTotal: ${ticket.CalcularPrecioFinalTiquete():N2}"
                : $"Cédula: {ticket.Cliente?.Cedula}\r\nNombre: {ticket.Cliente?.Nombre}\r\nOrigen: {ticket.Origen}\r\nDestino: {ticket.Destino}\r\nAerolínea: {ticket.Aerolinea}\r\nClase: {ticket.Clase}\r\nFecha: {ticket.FechaVuelo:yyyy-MM-dd}\r\nPrecio base: ${ticket.PrecioBase:N2}\r\nServicio: ${ticket.CalcularMontoServicio():N2}\r\nIVA: ${ticket.CalcularMontoIVA():N2}\r\nTotal: ${ticket.CalcularPrecioFinalTiquete():N2}";
        }

        private Tiquete CreateTicketFromRow(DataGridViewRow row)
        {
            var fechaCell = row.Cells.Count > 7 ? row.Cells[7].Value?.ToString() ?? "" : "";
            var fechaParts = fechaCell.Contains("\u2192")
                ? fechaCell.Split(new[] { " \u2192 " }, StringSplitOptions.None)
                : new[] { fechaCell };
            DateTime.TryParse(fechaParts[0]?.Trim(), out var fechaVuelo);
            var fechaVuelta = fechaParts.Length > 1 && DateTime.TryParse(fechaParts[1]?.Trim(), out var fv) ? fv : fechaVuelo;

            return new Tiquete
            {
                Cliente = new Cliente
                {
                    Cedula = row.Cells[0].Value?.ToString(),
                    Nombre = row.Cells[1].Value?.ToString()
                },
                Origen = row.Cells.Count > 2 ? row.Cells[2].Value?.ToString() : "",
                Destino = row.Cells[3].Value?.ToString(),
                Aerolinea = row.Cells.Count > 4 ? row.Cells[4].Value?.ToString() : "",
                Clase = row.Cells.Count > 5 ? row.Cells[5].Value?.ToString() : "",
                TipoViaje = row.Cells.Count > 6 ? (row.Cells[6].Value?.ToString() ?? "Ida") : "Ida",
                FechaVuelo = fechaVuelo,
                FechaVuelta = fechaVuelta,
                PrecioBase = row.Cells.Count > 11 && decimal.TryParse(row.Cells[11].Value?.ToString(), out var precioBase) ? precioBase : 0m
            };
        }

        private string GetCountryFromDisplay(string display)
        {
            if (string.IsNullOrWhiteSpace(display)) return string.Empty;
            var value = display.Trim();
            var spaceIndex = value.IndexOf(' ');
            return spaceIndex > 0 ? value.Substring(spaceIndex + 1).Trim() : value;
        }

        // Handler added to satisfy designer event wiring for pictureBoxFlag.Paint
        private void pictureBoxFlag_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            try
            {
                var pb = sender as System.Windows.Forms.PictureBox;
                if (pb == null) return;

                if (pb.Image != null) return;

                var g = e.Graphics;
                using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(240, 240, 240)))
                {
                    g.FillRectangle(brush, 0, 0, pb.Width, pb.Height);
                }
                using (var pen = new System.Drawing.Pen(System.Drawing.Color.DarkGray, 2))
                {
                    g.DrawRectangle(pen, 1, 1, Math.Max(0, pb.Width - 3), Math.Max(0, pb.Height - 3));
                }
                using (var font = new System.Drawing.Font("Segoe UI", Math.Max(8, pb.Width / 15)))
                using (var sf = new System.Drawing.StringFormat() { Alignment = System.Drawing.StringAlignment.Center, LineAlignment = System.Drawing.StringAlignment.Center })
                using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.Gray))
                {
                    g.DrawString("Bandera", font, brush, new System.Drawing.RectangleF(0, 0, pb.Width, pb.Height), sf);
                }
            }
            catch { }
        }

        // ── 2.1 Validación en tiempo real (Cédula) ─────────────────
        private void txtCedula_TextChanged(object sender, EventArgs e)
        {
            var text = txtCedula.Text.Trim();
            var isValid = string.IsNullOrEmpty(text) || System.Text.RegularExpressions.Regex.IsMatch(text, @"^\d{1,12}$");
            txtCedula.BorderFocusColor = isValid ? ThemeColors.Accent : ThemeColors.Error;
            txtCedula.Tag = isValid ? null : "invalid";
        }

    }
}
