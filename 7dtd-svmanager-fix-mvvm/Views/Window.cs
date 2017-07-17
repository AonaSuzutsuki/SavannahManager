using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace _7dtd_svmanager_fix_mvvm.Views
{
    [SuppressUnmanagedCodeSecurity]
    public static class WindowCommands
    {
        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public static void ShowSystemMenu(Window w, int x, int y)
        {
            IntPtr lParam = new IntPtr((x & 0xffff) | (y & 0xffff) << 16);
            
            IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(w).Handle;
            PostMessage(hwnd, 0x313, IntPtr.Zero, lParam);
        }
    }
}
