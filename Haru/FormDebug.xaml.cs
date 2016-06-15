using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Haru
{
    /// <summary>
    /// Interaction logic for FormDebug.xaml
    /// </summary>
    public partial class FormDebug : Window
    {
        public FormDebug()
        {
            InitializeComponent();
            Loaded += FormDebug_Loaded;
        }

        private void FormDebug_Loaded(object sender, RoutedEventArgs e)
        {
            Timer t1 = new Timer();
            t1.Interval = 50;
            t1.Elapsed += T1_Elapsed;
            t1.Enabled = true;
        }

        private void T1_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                boxMouseX.Text = System.Windows.Forms.Control.MousePosition.X.ToString();
            }));
            
            //boxMouseY.Text = GetMousePosition().Y.ToString();
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
        public static Point GetMousePosition()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }
    }
}
