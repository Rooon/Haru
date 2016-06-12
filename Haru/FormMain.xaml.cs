using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Haru
{
    public class Hotkeying
    {
        [DllImport("User32.dll")]
        private static extern bool RegisterHotKey(
            [In] IntPtr hWnd,
            [In] int id,
            [In] uint fsModifiers,
            [In] uint vk);

        [DllImport("User32.dll")]
        private static extern bool UnregisterHotKey(
            [In] IntPtr hWnd,
            [In] int id);

        private Window win;
        private HwndSource _source;

        public Hotkeying()
        {
            this.win = Application.Current.MainWindow;
            var helper = new WindowInteropHelper(win);
            _source = HwndSource.FromHwnd(helper.Handle);
            _source.AddHook(HwndHook);
            Register();
        }

        public void Register()
        {
            var helper = new WindowInteropHelper(win);
            if (!RegisterHotKey(helper.Handle, 9001, 0x0006, 0x34))
            {
                // handle error
                System.Windows.Forms.MessageBox.Show("Hotkey 9001 in use.");
            }
            if (!RegisterHotKey(helper.Handle, 9002, 0x0006, 0x35))
            {
                // handle error
                System.Windows.Forms.MessageBox.Show("Hotkey 9002 in use.");
            }
            if (!RegisterHotKey(helper.Handle, 9003, 0x0006, 0x36))
            {
                // handle error
                System.Windows.Forms.MessageBox.Show("Hotkey 9003 in use.");
            }
        }

        public void Unregister()
        {
            var helper = new WindowInteropHelper(win);
            UnregisterHotKey(helper.Handle, 9001);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case 9001:
                            Snipper.Snip();
                        break;

                        case 9002:
                            Window haru = Application.Current.Windows.OfType<Window>().Where(x => x.Name == "windowHaru").FirstOrDefault();
                            if (haru==null)
                            {
                                new FormHaru().Show();
                            }
                            else
                            {
                                haru.Close();
                            }
                           break;

                        case 9003:
                            new FormDebug().Show();
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }
    }

    public partial class FormMain : Window
    {

        private Hotkeying hk;

        public FormMain()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            //Append code
            hk = new Hotkeying();
            WindowInteropHelper wndHelper = new WindowInteropHelper(this);

            int exStyle = (int)GetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE);

            exStyle |= (int)ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            SetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);
        }

        protected override void OnClosed(EventArgs e)
        {
            hk.Unregister();
            base.OnClosed(e);
        }

        [Flags]
        public enum ExtendedWindowStyles
        {
            // ...
            WS_EX_TOOLWINDOW = 0x00000080,
            // ...
        }

        public enum GetWindowLongFields
        {
            // ...
            GWL_EXSTYLE = (-20),
            // ...
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            int error = 0;
            IntPtr result = IntPtr.Zero;
            // Win32 SetWindowLong doesn't clear error on success
            SetLastError(0);

            if (IntPtr.Size == 4)
            {
                // use SetWindowLong
                Int32 tempResult = IntSetWindowLong(hWnd, nIndex, IntPtrToInt32(dwNewLong));
                error = Marshal.GetLastWin32Error();
                result = new IntPtr(tempResult);
            }
            else
            {
                // use SetWindowLongPtr
                result = IntSetWindowLongPtr(hWnd, nIndex, dwNewLong);
                error = Marshal.GetLastWin32Error();
            }

            if ((result == IntPtr.Zero) && (error != 0))
            {
                throw new System.ComponentModel.Win32Exception(error);
            }

            return result;
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr IntSetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        private static extern Int32 IntSetWindowLong(IntPtr hWnd, int nIndex, Int32 dwNewLong);

        private static int IntPtrToInt32(IntPtr intPtr)
        {
            return unchecked((int)intPtr.ToInt64());
        }

        [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
        public static extern void SetLastError(int dwErrorCode);

    }
}
