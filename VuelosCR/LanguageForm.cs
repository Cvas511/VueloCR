using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using VuelosCRSA.Controls;

namespace VuelosCRSA
{
    public class LanguageForm : Form
    {
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        private CustomComboBox cmbLanguages;
        private Button btnOk;
        private Button btnCancel;

        public LanguageForm()
        {
            InitializeComponent();
            ApplyDarkTheme();
        }

        private void ApplyDarkTheme()
        {
            if (Environment.OSVersion.Version.Major >= 10)
            {
                int value = 1;
                DwmSetWindowAttribute(this.Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref value, sizeof(int));
            }

            var bg = Color.FromArgb(25, 25, 25);
            var surface = Color.FromArgb(35, 35, 35);
            var accent = Color.FromArgb(16, 163, 127);
            var accentHover = Color.FromArgb(20, 185, 145);
            var textPrimary = Color.FromArgb(236, 236, 236);
            var textSecondary = Color.FromArgb(160, 160, 160);

            BackColor = bg;

            cmbLanguages.BackColor = surface;
            cmbLanguages.ForeColor = textPrimary;
            cmbLanguages.BorderHoverColor = accent;
            cmbLanguages.BorderFocusColor = accent;
            cmbLanguages.ArrowHoverColor = accent;
            cmbLanguages.ItemHoverColor = accent;
            cmbLanguages.DropItemHeight = 34;

            Action<Button, Color, Color> styleBtn = (b, c, h) =>
            {
                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderSize = 0;
                b.BackColor = c;
                b.ForeColor = Color.White;
                b.FlatAppearance.MouseOverBackColor = h;
                b.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                b.Padding = new Padding(6, 4, 6, 6);
                using (var gp = RoundRect(new Rectangle(0, 0, b.Width, b.Height), 8))
                    b.Region = new Region(gp);
            };
            styleBtn(btnOk, accent, accentHover);
            styleBtn(btnCancel, Color.FromArgb(60, 60, 60), Color.FromArgb(80, 80, 80));

            foreach (var lbl in new[] { lblTitle })
                if (lbl != null) lbl.ForeColor = textSecondary;
        }

        private void InitializeComponent()
        {
            Text = "Select language";
            ClientSize = new Size(340, 170);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;
            MinimizeBox = false;
            Font = new Font("Segoe UI", 10F);

            lblTitle = new Label
            {
                Text = "Language / Idioma",
                ForeColor = Color.FromArgb(160, 160, 160),
                Font = new Font("Segoe UI", 9F),
                Location = new Point(20, 16),
                Size = new Size(300, 20)
            };

            cmbLanguages = new CustomComboBox();
            cmbLanguages.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLanguages.Items.Add(new LanguageItem("es", "Español"));
            cmbLanguages.Items.Add(new LanguageItem("en", "English"));
            cmbLanguages.SelectedIndex = 0;
            cmbLanguages.Location = new Point(20, 40);
            cmbLanguages.Size = new Size(300, 32);

            btnOk = new Button();
            btnOk.Text = "OK";
            btnOk.Location = new Point(160, 90);
            btnOk.Size = new Size(75, 32);
            btnOk.Click += BtnOk_Click;

            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Location = new Point(245, 90);
            btnCancel.Size = new Size(75, 32);
            btnCancel.Click += BtnCancel_Click;

            Controls.Add(lblTitle);
            Controls.Add(cmbLanguages);
            Controls.Add(btnOk);
            Controls.Add(btnCancel);
        }

        private Label lblTitle;

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (cmbLanguages.SelectedItem is LanguageItem li)
            {
                Program.SelectedLanguage = li.Code;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private static System.Drawing.Drawing2D.GraphicsPath RoundRect(Rectangle r, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            var d = radius * 2;
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private class LanguageItem
        {
            public string Code { get; }
            public string Name { get; }
            public LanguageItem(string code, string name)
            {
                Code = code;
                Name = name;
            }
            public override string ToString() => Name;
        }
    }
}
