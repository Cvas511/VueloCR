using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VuelosCRSA.Controls
{
    public class CustomDataGridView : DataGridView
    {
        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, ref RECT rect);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT { public int Left, Top, Right, Bottom; }

        private Color _gridBg = Color.FromArgb(18, 18, 18);
        private Color _headerBg = Color.FromArgb(28, 28, 28);
        private Color _rowBg = Color.FromArgb(22, 22, 22);
        private Color _altRowBg = Color.FromArgb(28, 28, 28);
        private Color _accent = Color.FromArgb(16, 163, 127);
        private Color _textPrimary = Color.FromArgb(200, 200, 200);
        private Color _gridLine = Color.FromArgb(45, 45, 45);
        private bool _themed;
        private int _activeHeaderColumn = -1;

        private readonly Pen _selectionBorderPen;
        private readonly SolidBrush _headerActiveBg;
        private readonly SolidBrush _headerActiveFg;

        public CustomDataGridView()
        {
            DoubleBuffered = true;
            BackgroundColor = _gridBg;
            BorderStyle = BorderStyle.None;
            GridColor = _gridLine;
            RowTemplate.Height = 32;
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            ColumnHeadersHeight = 38;
            EnableHeadersVisualStyles = false;
            SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            MultiSelect = false;

            // Default header style (dark)
            ColumnHeadersDefaultCellStyle.BackColor = _headerBg;
            ColumnHeadersDefaultCellStyle.ForeColor = _textPrimary;
            ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ColumnHeadersDefaultCellStyle.SelectionBackColor = _headerBg;
            ColumnHeadersDefaultCellStyle.SelectionForeColor = _textPrimary;

            // Data rows — solid black background, no alternating
            var font = new Font("Segoe UI", 10F);
            RowsDefaultCellStyle.BackColor = _rowBg;
            RowsDefaultCellStyle.ForeColor = _textPrimary;
            RowsDefaultCellStyle.Font = font;
            RowsDefaultCellStyle.SelectionBackColor = _rowBg;
            RowsDefaultCellStyle.SelectionForeColor = _textPrimary;

            AlternatingRowsDefaultCellStyle.BackColor = _rowBg;
            AlternatingRowsDefaultCellStyle.ForeColor = _textPrimary;
            AlternatingRowsDefaultCellStyle.Font = font;
            AlternatingRowsDefaultCellStyle.SelectionBackColor = _rowBg;
            AlternatingRowsDefaultCellStyle.SelectionForeColor = _textPrimary;

            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            _selectionBorderPen = new Pen(_accent, 3f);
            _headerActiveBg = new SolidBrush(_accent);
            _headerActiveFg = new SolidBrush(Color.Black);

            // Events
            CellFormatting += OnCellFormatting;
            CellPainting += OnCellPainting;
            ColumnHeaderMouseClick += (s, e) =>
            {
                _activeHeaderColumn = e.ColumnIndex;
                Invalidate();
            };
            RowsAdded += (s, e) => ApplyScrollbarTheme();
            ColumnAdded += (s, e) => ApplyScrollbarTheme();
            Resize += (s, e) => ApplyScrollbarTheme();
            Scroll += (s, e) => Invalidate();
        }

        // ── 1. Force black background on every data cell (selected or not) ──
        private void OnCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            e.CellStyle.BackColor = _rowBg;
            e.CellStyle.ForeColor = _textPrimary;
            e.CellStyle.SelectionBackColor = _rowBg;
            e.CellStyle.SelectionForeColor = _textPrimary;
        }

        // ── 2. Column header: paint active column green ────────────
        private void OnCellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex != -1) return;

            if (e.ColumnIndex == _activeHeaderColumn)
            {
                e.CellStyle.BackColor = _accent;
                e.CellStyle.ForeColor = Color.Black;
                e.CellStyle.SelectionBackColor = _accent;
                e.CellStyle.SelectionForeColor = Color.Black;
            }
            else
            {
                e.CellStyle.BackColor = _headerBg;
                e.CellStyle.ForeColor = _textPrimary;
                e.CellStyle.SelectionBackColor = _headerBg;
                e.CellStyle.SelectionForeColor = _textPrimary;
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            var t = new Timer { Interval = 100 };
            t.Tick += (s, args) =>
            {
                t.Stop();
                t.Dispose();
                ApplyScrollbarTheme();
            };
            t.Start();
        }

        private void ApplyScrollbarTheme()
        {
            if (_themed) return;
            try
            {
                foreach (Control c in Controls)
                {
                    var type = c.GetType();
                    if (type.Name.Contains("ScrollBar"))
                    {
                        SetWindowTheme(c.Handle, "", "");
                        SetWindowTheme(c.Handle, "DarkMode_Explorer", null);
                        c.BackColor = Color.FromArgb(30, 30, 30);
                        c.ForeColor = Color.FromArgb(100, 100, 100);
                    }
                }
                _themed = true;
            }
            catch { }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (!_themed) ApplyScrollbarTheme();

            if (SelectedRows.Count == 0) return;

            var row = SelectedRows[0];
            if (row == null || row.Index < 0 || row.IsNewRow) return;

            var rowRect = GetRowDisplayRectangle(row.Index, false);
            if (rowRect.IsEmpty || rowRect.Height <= 0) return;

            int left = 2;
            int right = ClientSize.Width - 2;
            int top = rowRect.Y + 1;
            int bottom = rowRect.Bottom - 1;

            var r = new Rectangle(left, top, right - left, bottom - top);
            if (r.Width > 12 && r.Height > 12)
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                var radius = 6;
                var d = radius * 2;
                using (var gp = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    gp.AddArc(r.X, r.Y, d, d, 180, 90);
                    gp.AddArc(r.Right - d, r.Y, d, d, 270, 90);
                    gp.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
                    gp.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
                    gp.CloseFigure();
                    e.Graphics.DrawPath(_selectionBorderPen, gp);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _selectionBorderPen.Dispose();
                _headerActiveBg.Dispose();
                _headerActiveFg.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
