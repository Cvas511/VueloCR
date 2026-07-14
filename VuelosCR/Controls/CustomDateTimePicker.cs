using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VuelosCRSA.Controls
{
    public class CustomDateTimePicker : DateTimePicker
    {
        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateSolidBrush(int crColor);

        [DllImport("user32.dll")]
        private static extern int InvalidateRect(IntPtr hWnd, IntPtr lpRect, int bErase);

        private const int WM_CTLCOLOREDIT = 0x0133;
        private const int WM_ERASEBKGND = 0x0014;
        private const int EM_SETMARGINS = 0x00D3;
        private const int EC_LEFTMARGIN = 0x0001;
        private const int EC_RIGHTMARGIN = 0x0002;

        private static readonly IntPtr _blackBrush = CreateSolidBrush(Color.Black.ToArgb());

        public CustomDateTimePicker()
        {
            BackColor = Color.Black;
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 10f);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            SetWindowTheme(Handle, "", "");

            CalendarMonthBackground = Color.FromArgb(30, 30, 30);
            CalendarTitleBackColor = Color.FromArgb(20, 20, 20);
            CalendarTitleForeColor = Color.FromArgb(16, 163, 127);
            CalendarTrailingForeColor = Color.FromArgb(80, 80, 80);
            CalendarForeColor = Color.FromArgb(200, 200, 200);

            var margin = (IntPtr)((6 << 16) | 6);
            SendMessage(Handle, EM_SETMARGINS, (IntPtr)(EC_LEFTMARGIN | EC_RIGHTMARGIN), margin);

            var editHwnd = FindWindowEx(Handle, IntPtr.Zero, "Edit", null);
            if (editHwnd != IntPtr.Zero)
            {
                SetWindowTheme(editHwnd, "", "");
                InvalidateRect(editHwnd, IntPtr.Zero, 1);
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_CTLCOLOREDIT)
            {
                m.Result = _blackBrush;
            }
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            Invalidate();
        }
    }
}
