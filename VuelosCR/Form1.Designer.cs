namespace VuelosCRSA
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 540);
            this.MinimumSize = new System.Drawing.Size(900, 560);
            this.Text = "Venta de Tiquetes - VuelosCR.SA";
            this.MaximizeBox = true;
            // Modernize: base font and background
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);

            // Controles
            this.lblCedula = new System.Windows.Forms.Label();
            this.txtCedula = new VuelosCRSA.Controls.CustomTextBox();
            this.lblNombre = new System.Windows.Forms.Label();
            this.txtNombre = new VuelosCRSA.Controls.CustomTextBox();
            this.lblDestino = new System.Windows.Forms.Label();
            this.cmbDestino = new VuelosCRSA.Controls.CustomComboBox();
            this.lblAerolinea = new System.Windows.Forms.Label();
            this.cmbAerolinea = new VuelosCRSA.Controls.CustomComboBox();
            this.lblClase = new System.Windows.Forms.Label();
            this.cmbClase = new VuelosCRSA.Controls.CustomComboBox();
            this.lblTipoViaje = new System.Windows.Forms.Label();
            this.cmbTipoViaje = new VuelosCRSA.Controls.CustomComboBox();
            this.dtpFecha = new VuelosCRSA.Controls.CustomDatePicker();
            this.dtpFechaVuelta = new VuelosCRSA.Controls.CustomDatePicker();
            this.lblOrigen = new System.Windows.Forms.Label();
            this.cmbOrigen = new VuelosCRSA.Controls.CustomComboBox();
            this.btnTarifa = new System.Windows.Forms.Button();
            this.btnComprar = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.lblPrecio = new System.Windows.Forms.Label();
            this.lblVoucher = new System.Windows.Forms.Label();
            this.dgvTiquetes = new VuelosCRSA.Controls.CustomDataGridView();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.btnEliminarGuardados = new System.Windows.Forms.Button();
            this.btnEliminarReciente = new System.Windows.Forms.Button();
            this.btnIdioma = new System.Windows.Forms.Button();
            this.btnCerrarPrograma = new System.Windows.Forms.Button();
            this.grpDetalleRegistro = new System.Windows.Forms.GroupBox();
            this.txtDetalleRegistro = new VuelosCRSA.Controls.CustomTextBox();
            this.txtFiltro = new VuelosCRSA.Controls.CustomTextBox();

            // lblCedula
            this.lblCedula.AutoSize = true;
            this.lblCedula.Location = new System.Drawing.Point(20, 20);
            this.lblCedula.Name = "lblCedula";
            this.lblCedula.Size = new System.Drawing.Size(44, 13);
            this.lblCedula.TabIndex = 0;
            this.lblCedula.Text = "Cédula:";

            // txtCedula
            this.txtCedula.Location = new System.Drawing.Point(100, 17);
            this.txtCedula.Name = "txtCedula";
            this.txtCedula.Size = new System.Drawing.Size(200, 20);
            this.txtCedula.TabIndex = 1;
            this.txtCedula.TextChanged += new System.EventHandler(this.txtCedula_TextChanged);

            // lblNombre
            this.lblNombre.AutoSize = true;
            this.lblNombre.Location = new System.Drawing.Point(20, 55);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(47, 13);
            this.lblNombre.TabIndex = 2;
            this.lblNombre.Text = "Nombre:";

            // txtNombre
            this.txtNombre.Location = new System.Drawing.Point(100, 52);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(200, 20);
            this.txtNombre.TabIndex = 3;
            this.txtNombre.TextChanged += new System.EventHandler(this.txtNombre_TextChanged);

            // lblDestino
            this.lblDestino.AutoSize = true;
            this.lblDestino.Location = new System.Drawing.Point(20, 125);
            this.lblDestino.Name = "lblDestino";
            this.lblDestino.Size = new System.Drawing.Size(50, 13);
            this.lblDestino.TabIndex = 6;
            this.lblDestino.Text = "Destino:";

            // cmbDestino
            this.cmbDestino.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDestino.Location = new System.Drawing.Point(100, 122);
            this.cmbDestino.Name = "cmbDestino";
            this.cmbDestino.Size = new System.Drawing.Size(200, 21);
            this.cmbDestino.TabIndex = 7;
            this.cmbDestino.SelectedIndexChanged += new System.EventHandler(this.cmbDestino_SelectedIndexChanged);

            // lblAerolinea
            this.lblAerolinea.AutoSize = true;
            this.lblAerolinea.Location = new System.Drawing.Point(20, 160);
            this.lblAerolinea.Name = "lblAerolinea";
            this.lblAerolinea.Size = new System.Drawing.Size(60, 13);
            this.lblAerolinea.TabIndex = 8;
            this.lblAerolinea.Text = "Aerolínea:";

            // cmbAerolinea
            this.cmbAerolinea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAerolinea.Location = new System.Drawing.Point(100, 157);
            this.cmbAerolinea.Name = "cmbAerolinea";
            this.cmbAerolinea.Size = new System.Drawing.Size(200, 21);
            this.cmbAerolinea.TabIndex = 9;
            this.cmbAerolinea.SelectedIndexChanged += new System.EventHandler(this.cmbAerolinea_SelectedIndexChanged);

            // lblClase
            this.lblClase.AutoSize = true;
            this.lblClase.Location = new System.Drawing.Point(20, 195);
            this.lblClase.Name = "lblClase";
            this.lblClase.Size = new System.Drawing.Size(39, 13);
            this.lblClase.TabIndex = 22;
            this.lblClase.Text = "Clase:";

            // cmbClase
            this.cmbClase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbClase.Location = new System.Drawing.Point(100, 192);
            this.cmbClase.Name = "cmbClase";
            this.cmbClase.Size = new System.Drawing.Size(200, 21);
            this.cmbClase.TabIndex = 23;
            this.cmbClase.SelectedIndexChanged += new System.EventHandler(this.cmbClase_SelectedIndexChanged);

            // lblTipoViaje
            this.lblTipoViaje.AutoSize = true;
            this.lblTipoViaje.Location = new System.Drawing.Point(20, 230);
            this.lblTipoViaje.Name = "lblTipoViaje";
            this.lblTipoViaje.Size = new System.Drawing.Size(67, 13);
            this.lblTipoViaje.TabIndex = 24;
            this.lblTipoViaje.Text = "Tipo Viaje:";

            // cmbTipoViaje
            this.cmbTipoViaje.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipoViaje.Location = new System.Drawing.Point(100, 227);
            this.cmbTipoViaje.Name = "cmbTipoViaje";
            this.cmbTipoViaje.Size = new System.Drawing.Size(120, 21);
            this.cmbTipoViaje.TabIndex = 25;
            this.cmbTipoViaje.SelectedIndexChanged += new System.EventHandler(this.cmbTipoViaje_SelectedIndexChanged);

            // dtpFecha
            this.dtpFecha.Location = new System.Drawing.Point(2, 22);
            this.dtpFecha.Name = "dtpFecha";
            this.dtpFecha.Size = new System.Drawing.Size(126, 22);
            this.dtpFecha.TabIndex = 26;
            this.dtpFecha.Value = System.DateTime.Today;
            this.dtpFecha.ValueChanged += new System.EventHandler(this.dtpFecha_ValueChanged);

            // dtpFechaVuelta
            this.dtpFechaVuelta.Location = new System.Drawing.Point(132, 22);
            this.dtpFechaVuelta.Name = "dtpFechaVuelta";
            this.dtpFechaVuelta.Size = new System.Drawing.Size(126, 22);
            this.dtpFechaVuelta.TabIndex = 27;
            this.dtpFechaVuelta.Value = System.DateTime.Today.AddDays(7);
            this.dtpFechaVuelta.ValueChanged += new System.EventHandler(this.dtpFechaVuelta_ValueChanged);
            this.dtpFechaVuelta.Visible = false;

            // lblIda
            this.lblIda = new System.Windows.Forms.Label();
            this.lblIda.AutoSize = false;
            this.lblIda.Location = new System.Drawing.Point(0, 0);
            this.lblIda.Name = "lblIda";
            this.lblIda.Size = new System.Drawing.Size(130, 18);
            this.lblIda.TabIndex = 28;
            this.lblIda.Text = "Ida:";
            this.lblIda.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // lblVuelta
            this.lblVuelta = new System.Windows.Forms.Label();
            this.lblVuelta.AutoSize = false;
            this.lblVuelta.Location = new System.Drawing.Point(130, 0);
            this.lblVuelta.Name = "lblVuelta";
            this.lblVuelta.Size = new System.Drawing.Size(130, 18);
            this.lblVuelta.TabIndex = 29;
            this.lblVuelta.Text = "Vuelta:";
            this.lblVuelta.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblVuelta.Visible = false;

            // dateRowPanel
            this.dateRowPanel = new System.Windows.Forms.Panel();
            this.dateRowPanel.Height = 46;
            this.dateRowPanel.Width = 260;
            this.dateRowPanel.Margin = new System.Windows.Forms.Padding(6, 3, 6, 3);
            this.dateRowPanel.Name = "dateRowPanel";
            this.dateRowPanel.Controls.Add(this.lblIda);
            this.dateRowPanel.Controls.Add(this.dtpFecha);
            this.dateRowPanel.Controls.Add(this.lblVuelta);
            this.dateRowPanel.Controls.Add(this.dtpFechaVuelta);

            // lblOrigen
            this.lblOrigen.AutoSize = true;
            this.lblOrigen.Location = new System.Drawing.Point(20, 90);
            this.lblOrigen.Name = "lblOrigen";
            this.lblOrigen.Size = new System.Drawing.Size(47, 13);
            this.lblOrigen.TabIndex = 4;
            this.lblOrigen.Text = "Origen:";

            // cmbOrigen
            this.cmbOrigen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOrigen.Location = new System.Drawing.Point(100, 87);
            this.cmbOrigen.Name = "cmbOrigen";
            this.cmbOrigen.Size = new System.Drawing.Size(200, 21);
            this.cmbOrigen.TabIndex = 5;
            this.cmbOrigen.SelectedIndexChanged += new System.EventHandler(this.cmbOrigen_SelectedIndexChanged);

            // btnTarifa
            this.btnTarifa.Location = new System.Drawing.Point(20, 170);
            this.btnTarifa.Name = "btnTarifa";
            this.btnTarifa.Size = new System.Drawing.Size(110, 30);
            this.btnTarifa.TabIndex = 10;
            this.btnTarifa.Text = "💲 Tarifa";
            this.btnTarifa.UseVisualStyleBackColor = true;
            this.btnTarifa.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTarifa.BackColor = System.Drawing.Color.FromArgb(33, 150, 243);
            this.btnTarifa.ForeColor = System.Drawing.Color.White;
            this.btnTarifa.FlatAppearance.BorderSize = 0;
            this.btnTarifa.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.btnTarifa.Click += new System.EventHandler(this.btnTarifa_Click);

            // btnComprar
            this.btnComprar.Location = new System.Drawing.Point(180, 170);
            this.btnComprar.Name = "btnComprar";
            this.btnComprar.Size = new System.Drawing.Size(110, 30);
            this.btnComprar.TabIndex = 11;
            this.btnComprar.Text = "✈ Comprar";
            this.btnComprar.UseVisualStyleBackColor = true;
            this.btnComprar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnComprar.BackColor = System.Drawing.Color.FromArgb(76, 175, 80);
            this.btnComprar.ForeColor = System.Drawing.Color.White;
            this.btnComprar.FlatAppearance.BorderSize = 0;
            this.btnComprar.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.btnComprar.Click += new System.EventHandler(this.btnComprar_Click);

            // txtFiltro
            this.txtFiltro.Location = new System.Drawing.Point(330, 10);
            this.txtFiltro.Name = "txtFiltro";
            this.txtFiltro.Size = new System.Drawing.Size(370, 20);
            this.txtFiltro.TabIndex = 26;
            this.txtFiltro.TextChanged += new System.EventHandler(this.txtFiltro_TextChanged);

            // dgvTiquetes
            this.dgvTiquetes.Location = new System.Drawing.Point(330, 10);
            this.dgvTiquetes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvTiquetes.Name = "dgvTiquetes";
            this.dgvTiquetes.Size = new System.Drawing.Size(370, 420);
            this.dgvTiquetes.TabIndex = 12;
            this.dgvTiquetes.AllowUserToAddRows = false;
            this.dgvTiquetes.AllowUserToDeleteRows = false;
            this.dgvTiquetes.ReadOnly = true;
            this.dgvTiquetes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTiquetes.RowHeadersVisible = false;
            this.dgvTiquetes.AllowUserToResizeColumns = true;
            this.dgvTiquetes.AllowUserToResizeRows = false;
            this.dgvTiquetes.AllowUserToOrderColumns = false;
            this.dgvTiquetes.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.dgvTiquetes.SelectionChanged += new System.EventHandler(this.dgvTiquetes_SelectionChanged);
            this.dgvTiquetes.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTiquetes_CellDoubleClick);
            this.dgvTiquetes.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dgvTiquetes_MouseMove);
            this.dgvTiquetes.MouseLeave += new System.EventHandler(this.dgvTiquetes_MouseLeave);

            // Agregar columnas a dgv
            this.dgvTiquetes.Columns.Add("Cedula", "Cédula");
            this.dgvTiquetes.Columns.Add("Nombre", "Nombre");
            this.dgvTiquetes.Columns.Add("Origen", "Origen");
            this.dgvTiquetes.Columns.Add("Destino", "Destino");
            this.dgvTiquetes.Columns.Add("Aerolinea", "Aerolínea");
            this.dgvTiquetes.Columns.Add("Clase", "Clase");
            this.dgvTiquetes.Columns.Add("TipoViaje", "Tipo");
            this.dgvTiquetes.Columns.Add("Fecha", "Fecha");
            this.dgvTiquetes.Columns.Add("MontoIVA", "MontoIVA");
            this.dgvTiquetes.Columns.Add("PrecioFinal", "PrecioFinal");
            this.dgvTiquetes.Columns.Add("PrecioBase", "PrecioBase");

            // btnEliminar
            this.btnEliminar.Location = new System.Drawing.Point(320, 470);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(150, 25);
            this.btnEliminar.TabIndex = 13;
            this.btnEliminar.Text = "🗑 Eliminar";
            this.btnEliminar.UseVisualStyleBackColor = true;
            this.btnEliminar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEliminar.BackColor = System.Drawing.Color.FromArgb(244, 67, 54);
            this.btnEliminar.ForeColor = System.Drawing.Color.White;
            this.btnEliminar.FlatAppearance.BorderSize = 0;
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);

            // btnLimpiar
            this.btnLimpiar.Location = new System.Drawing.Point(480, 470);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(120, 25);
            this.btnLimpiar.TabIndex = 14;
            this.btnLimpiar.Text = "🧹 Eliminar todos";
            this.btnLimpiar.UseVisualStyleBackColor = true;
            this.btnLimpiar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLimpiar.BackColor = System.Drawing.Color.FromArgb(158, 158, 158);
            this.btnLimpiar.ForeColor = System.Drawing.Color.White;
            this.btnLimpiar.FlatAppearance.BorderSize = 0;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);

            // btnEliminarGuardados
            this.btnEliminarGuardados.Location = new System.Drawing.Point(20, 470);
            this.btnEliminarGuardados.Name = "btnEliminarGuardados";
            this.btnEliminarGuardados.Size = new System.Drawing.Size(140, 25);
            this.btnEliminarGuardados.TabIndex = 15;
            this.btnEliminarGuardados.Text = "🗄 Eliminar guardados";
            this.btnEliminarGuardados.UseVisualStyleBackColor = true;
            this.btnEliminarGuardados.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEliminarGuardados.BackColor = System.Drawing.Color.FromArgb(158, 158, 158);
            this.btnEliminarGuardados.ForeColor = System.Drawing.Color.White;
            this.btnEliminarGuardados.FlatAppearance.BorderSize = 0;
            this.btnEliminarGuardados.Click += new System.EventHandler(this.btnEliminarGuardados_Click);

            // btnEliminarReciente
            this.btnEliminarReciente.Location = new System.Drawing.Point(630, 470);
            this.btnEliminarReciente.Name = "btnEliminarReciente";
            this.btnEliminarReciente.Size = new System.Drawing.Size(140, 25);
            this.btnEliminarReciente.TabIndex = 16;
            this.btnEliminarReciente.Text = "🕒 Eliminar reciente";
            this.btnEliminarReciente.UseVisualStyleBackColor = true;
            this.btnEliminarReciente.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEliminarReciente.BackColor = System.Drawing.Color.FromArgb(158, 158, 158);
            this.btnEliminarReciente.ForeColor = System.Drawing.Color.White;
            this.btnEliminarReciente.FlatAppearance.BorderSize = 0;
            this.btnEliminarReciente.Click += new System.EventHandler(this.btnEliminarReciente_Click);

            // btnIdioma
            this.btnIdioma.Location = new System.Drawing.Point(780, 470);
            this.btnIdioma.Name = "btnIdioma";
            this.btnIdioma.Size = new System.Drawing.Size(95, 25);
            this.btnIdioma.TabIndex = 18;
            this.btnIdioma.Text = "🌎 Idioma";
            this.btnIdioma.UseVisualStyleBackColor = true;
            this.btnIdioma.Click += new System.EventHandler(this.btnIdioma_Click);

            // btnCerrarPrograma
            this.btnCerrarPrograma.Location = new System.Drawing.Point(880, 470);
            this.btnCerrarPrograma.Name = "btnCerrarPrograma";
            this.btnCerrarPrograma.Size = new System.Drawing.Size(100, 25);
            this.btnCerrarPrograma.TabIndex = 19;
            this.btnCerrarPrograma.Text = "❌ Cerrar";
            this.btnCerrarPrograma.UseVisualStyleBackColor = true;
            this.btnCerrarPrograma.Click += new System.EventHandler(this.btnCerrarPrograma_Click);

            // btnExportPdf
            this.btnExportPdf = new System.Windows.Forms.Button();
            this.btnExportPdf.Location = new System.Drawing.Point(520, 500);
            this.btnExportPdf.Name = "btnExportPdf";
            this.btnExportPdf.Size = new System.Drawing.Size(140, 25);
            this.btnExportPdf.TabIndex = 20;
            this.btnExportPdf.Text = "Exportar PDF";
            this.btnExportPdf.UseVisualStyleBackColor = true;
            this.btnExportPdf.Click += new System.EventHandler(this.btnExportPdf_Click);

            // pictureLogo
            this.pictureLogo = new System.Windows.Forms.PictureBox();
            this.pictureLogo.Location = new System.Drawing.Point(10, 10);
            this.pictureLogo.Name = "pictureLogo";
            this.pictureLogo.Size = new System.Drawing.Size(56, 56);
            this.pictureLogo.TabIndex = 21;
            this.pictureLogo.TabStop = false;

            // grpPurchaseSummary → purchaseSummaryCard
            this.purchaseSummaryCard = new VuelosCRSA.Controls.PurchaseSummaryCard();
            this.purchaseSummaryCard.Location = new System.Drawing.Point(710, 10);
            this.purchaseSummaryCard.Name = "purchaseSummaryCard";
            this.purchaseSummaryCard.Size = new System.Drawing.Size(270, 320);

            // grpDetalleRegistro removed (user requested)

            // lblPrecio (adjusted position after removing grpDetalleRegistro)
            this.lblPrecio.AutoSize = true;
            this.lblPrecio.Location = new System.Drawing.Point(20, 220);
            this.lblPrecio.Name = "lblPrecio";
            this.lblPrecio.Size = new System.Drawing.Size(40, 13);
            this.lblPrecio.TabIndex = 10;
            this.lblPrecio.Text = "Precio:";

            // lblVoucher
            // lblVoucher (adjusted position after removing grpDetalleRegistro)
            this.lblVoucher.AutoSize = false;
            this.lblVoucher.Location = new System.Drawing.Point(20, 250);
            this.lblVoucher.Name = "lblVoucher";
            this.lblVoucher.Size = new System.Drawing.Size(210, 95);
            this.lblVoucher.TabIndex = 11;
            this.lblVoucher.Text = "Voucher:";

            // pictureBoxFlag (espacio para mostrar la bandera del país seleccionado)
            this.pictureBoxFlag = new System.Windows.Forms.PictureBox();
            // moved up inside left panel to avoid overlapping the purchase summary
            this.pictureBoxFlag.Location = new System.Drawing.Point(20, 190);
            this.pictureBoxFlag.Name = "pictureBoxFlag";
            this.pictureBoxFlag.Size = new System.Drawing.Size(80, 48);
            this.pictureBoxFlag.TabIndex = 15;
            this.pictureBoxFlag.TabStop = false;
            this.pictureBoxFlag.BackColor = System.Drawing.Color.White;
            this.pictureBoxFlag.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxFlag.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxFlag_Paint);

            // copyrightLabel
            this.copyrightLabel = new System.Windows.Forms.Label();
            this.copyrightLabel.AutoSize = true;
            this.copyrightLabel.Location = new System.Drawing.Point(20, 510);
            this.copyrightLabel.Name = "copyrightLabel";
            this.copyrightLabel.Size = new System.Drawing.Size(350, 13);
            this.copyrightLabel.TabIndex = 17;
            this.copyrightLabel.Text = "© VuelosCR.SA - Desarrollado por Sebastian Mendez Vargas";


            // left panel to group input controls and avoid overlap
            this.leftPanel = new System.Windows.Forms.Panel();
            this.leftPanel.Location = new System.Drawing.Point(10, 10);
            this.leftPanel.Name = "leftPanel";
            this.leftPanel.Size = new System.Drawing.Size(300, 520);
            this.leftPanel.BackColor = System.Drawing.Color.Transparent;

            // create a FlowLayoutPanel inside leftPanel to layout controls vertically and avoid overlaps
            this.leftFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.leftFlowPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftFlowPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.leftFlowPanel.WrapContents = false;
            this.leftFlowPanel.AutoScroll = true;
            this.leftFlowPanel.Padding = new System.Windows.Forms.Padding(8);
            this.leftFlowPanel.Name = "leftFlowPanel";

            // normalize sizes for inputs when placed inside the flow panel
            this.txtCedula.Width = 260;
            this.txtNombre.Width = 260;
            this.cmbDestino.Width = 260;
            this.cmbAerolinea.Width = 260;
            this.cmbOrigen.Width = 260;
            this.cmbClase.Width = 260;
            this.pictureBoxFlag.Size = new System.Drawing.Size(80, 48);

            // Add left-side controls into the inner flow panel
            this.leftFlowPanel.Controls.Add(this.lblCedula);
            this.leftFlowPanel.Controls.Add(this.txtCedula);
            this.leftFlowPanel.Controls.Add(this.lblNombre);
            this.leftFlowPanel.Controls.Add(this.txtNombre);
            this.leftFlowPanel.Controls.Add(this.lblOrigen);
            this.leftFlowPanel.Controls.Add(this.cmbOrigen);
            this.leftFlowPanel.Controls.Add(this.lblDestino);
            this.leftFlowPanel.Controls.Add(this.cmbDestino);
            this.leftFlowPanel.Controls.Add(this.lblAerolinea);
            this.leftFlowPanel.Controls.Add(this.cmbAerolinea);
            this.leftFlowPanel.Controls.Add(this.lblClase);
            this.leftFlowPanel.Controls.Add(this.cmbClase);
            this.leftFlowPanel.Controls.Add(this.lblTipoViaje);
            this.leftFlowPanel.Controls.Add(this.cmbTipoViaje);
            this.leftFlowPanel.Controls.Add(this.dateRowPanel);
            // buttons in a small FlowLayoutPanel horizontal
            this.buttonsFlow = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonsFlow.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.buttonsFlow.WrapContents = false;
            this.buttonsFlow.AutoSize = true;
            this.buttonsFlow.Padding = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.buttonsFlow.Controls.Add(this.btnTarifa);
            this.buttonsFlow.Controls.Add(this.btnComprar);
            this.leftFlowPanel.Controls.Add(this.buttonsFlow);
            this.leftFlowPanel.Controls.Add(this.lblPrecio);
            this.leftFlowPanel.Controls.Add(this.lblVoucher);
            // spacer to prevent buttons/controls from touching the panel border
            this.leftFlowPanel.Controls.Add(new System.Windows.Forms.Panel { Height = 14, Width = 1, BackColor = System.Drawing.Color.Transparent });
            this.leftPanel.Controls.Add(this.leftFlowPanel);

            // Flight Preview Card
            this.flightPreview = new VuelosCRSA.Controls.FlightPreviewCard();
            this.flightPreview.Name = "flightPreview";
            this.flightPreview.Visible = false;

            // ToolStrip — ghost style with icons
            var toolStrip = new System.Windows.Forms.ToolStrip();
            toolStrip.Name = "toolStrip";
            toolStrip.BackColor = System.Drawing.Color.FromArgb(25, 25, 25);
            toolStrip.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            toolStrip.Dock = System.Windows.Forms.DockStyle.Top;
            toolStrip.Renderer = new VuelosCRSA.Controls.GhostToolStripRenderer();

            var btnStripEliminar = new System.Windows.Forms.ToolStripButton(VuelosCRSA.Controls.ToolbarIcons.Delete);
            btnStripEliminar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            btnStripEliminar.ToolTipText = "Delete selected";
            btnStripEliminar.Click += btnEliminar_Click;

            var btnStripLimpiar = new System.Windows.Forms.ToolStripButton(VuelosCRSA.Controls.ToolbarIcons.ClearAll);
            btnStripLimpiar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            btnStripLimpiar.ToolTipText = "Clear all";
            btnStripLimpiar.Click += btnLimpiar_Click;

            var btnStripExport = new System.Windows.Forms.ToolStripButton(VuelosCRSA.Controls.ToolbarIcons.Export);
            btnStripExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            btnStripExport.ToolTipText = "Export PDF";
            btnStripExport.Click += btnExportPdf_Click;

            var sep1 = new System.Windows.Forms.ToolStripSeparator();
            var sep2 = new System.Windows.Forms.ToolStripSeparator();

            var moreDropdown = new System.Windows.Forms.ToolStripDropDownButton(VuelosCRSA.Controls.ToolbarIcons.More);
            moreDropdown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            moreDropdown.ToolTipText = "More options";
            moreDropdown.DropDownItems.Add("Delete saved data", null, btnEliminarGuardados_Click);
            moreDropdown.DropDownItems.Add("Delete latest", null, btnEliminarReciente_Click);
            var sepDropdown = new System.Windows.Forms.ToolStripSeparator();
            moreDropdown.DropDownItems.Add(sepDropdown);
            moreDropdown.DropDownItems.Add("Language", null, btnIdioma_Click);
            moreDropdown.DropDownItems.Add("Exit", null, btnCerrarPrograma_Click);

            toolStrip.Items.Add(btnStripEliminar);
            toolStrip.Items.Add(btnStripLimpiar);
            toolStrip.Items.Add(sep1);
            toolStrip.Items.Add(btnStripExport);
            toolStrip.Items.Add(sep2);
            toolStrip.Items.Add(moreDropdown);

            // Add toolbar TOP of form
            this.Controls.Add(toolStrip);

            // Add panels and main controls to form
            this.Controls.Add(this.pictureLogo);
            this.Controls.Add(this.leftPanel);
            this.Controls.Add(this.dgvTiquetes);
            this.Controls.Add(this.txtFiltro);
            this.Controls.SetChildIndex(this.txtFiltro, 0);
            this.Controls.Add(this.purchaseSummaryCard);
            this.Controls.Add(this.flightPreview);
            this.Controls.Add(this.pictureBoxFlag);
            this.Controls.Add(this.copyrightLabel);

            // Bottom buttons are now removed — replaced by toolbar above
        }

        #endregion

        private System.Windows.Forms.Label lblCedula;
        private VuelosCRSA.Controls.CustomTextBox txtCedula;
        private System.Windows.Forms.Label lblNombre;
        private VuelosCRSA.Controls.CustomTextBox txtNombre;
        private System.Windows.Forms.Label lblDestino;
        private VuelosCRSA.Controls.CustomComboBox cmbDestino;
        private System.Windows.Forms.Label lblAerolinea;
        private VuelosCRSA.Controls.CustomComboBox cmbAerolinea;
        private System.Windows.Forms.Label lblOrigen;
        private VuelosCRSA.Controls.CustomComboBox cmbOrigen;
        private System.Windows.Forms.Label lblClase;
        private VuelosCRSA.Controls.CustomComboBox cmbClase;
        private System.Windows.Forms.Label lblTipoViaje;
        private VuelosCRSA.Controls.CustomComboBox cmbTipoViaje;
        private VuelosCRSA.Controls.CustomDatePicker dtpFecha;
        private VuelosCRSA.Controls.CustomDatePicker dtpFechaVuelta;
        private System.Windows.Forms.Label lblIda;
        private System.Windows.Forms.Label lblVuelta;
        private System.Windows.Forms.Panel dateRowPanel;
        private System.Windows.Forms.Button btnTarifa;
        private System.Windows.Forms.Button btnComprar;
        private VuelosCRSA.Controls.CustomDataGridView dgvTiquetes;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.Button btnEliminarGuardados;
        private System.Windows.Forms.Button btnEliminarReciente;
        private System.Windows.Forms.Button btnIdioma;
        private System.Windows.Forms.Button btnCerrarPrograma;
        private System.Windows.Forms.Button btnExportPdf;
        private System.Windows.Forms.Panel leftPanel;
        private System.Windows.Forms.FlowLayoutPanel leftFlowPanel;
        private System.Windows.Forms.FlowLayoutPanel buttonsFlow;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.PictureBox pictureLogo;
        private VuelosCRSA.Controls.PurchaseSummaryCard purchaseSummaryCard;
        private System.Windows.Forms.GroupBox grpDetalleRegistro;
        private VuelosCRSA.Controls.CustomTextBox txtDetalleRegistro;
        private System.Windows.Forms.Label lblPrecio;
        private System.Windows.Forms.Label lblVoucher;
        private System.Windows.Forms.PictureBox pictureBoxFlag;
        private VuelosCRSA.Controls.FlightPreviewCard flightPreview;
        private VuelosCRSA.Controls.CustomTextBox txtFiltro;
        private System.Windows.Forms.Label copyrightLabel;
    }
}
