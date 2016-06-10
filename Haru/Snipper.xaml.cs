using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Windows.Interop;
using System.IO;

namespace Haru
{

    public partial class Snipper : Window
    {
        private System.Windows.Point p1;
        private System.Windows.Point p2;

        private BitmapSource screenshot;

        public Snipper()
        {
            InitializeComponent();
            ImageBrush ib = new ImageBrush();
            screenshot = CopyScreen();
            ib.ImageSource = screenshot;
            snipcanvas.Background = ib;
            MouseDown += Snipper_MouseDown;
            MouseUp += Snipper_MouseUp;
            MouseMove += Snipper_MouseMove;
        }

        private void Snipper_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.innerRect.Rect = new Rect(p1, e.GetPosition(this));
        }

        private void Snipper_MouseDown(object sender, MouseButtonEventArgs e)
        {
            p1 = e.GetPosition(this);
            //throw new NotImplementedException();
        }

        private void Snipper_MouseUp(object sender, MouseButtonEventArgs e)
        {
            p2 = e.GetPosition(this);
            System.Windows.Clipboard.SetImage(new CroppedBitmap(screenshot, new Int32Rect((int)p1.X, (int)p1.Y, (int)Math.Abs(p2.X - p1.X), (int)Math.Abs(p2.Y - p1.Y))));
            DialogResult = true;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            Left = Screen.AllScreens.Min(screen => screen.Bounds.X);
            Top = Screen.AllScreens.Min(screen => screen.Bounds.Y);
            Width = Screen.AllScreens.Max(screen => screen.Bounds.X + screen.Bounds.Width);
            Height = Screen.AllScreens.Max(screen => screen.Bounds.Y + screen.Bounds.Height);
            outerRect.Rect = new Rect(0, 0, Width, Height);
            innerRect.Rect = new Rect(Width / 4, Height / 4, Width/2, Height/2);
        }

        private static BitmapSource CopyScreen()
        {
            var left = Screen.AllScreens.Min(screen => screen.Bounds.X);
            var top = Screen.AllScreens.Min(screen => screen.Bounds.Y);
            var right = Screen.AllScreens.Max(screen => screen.Bounds.X + screen.Bounds.Width);
            var bottom = Screen.AllScreens.Max(screen => screen.Bounds.Y + screen.Bounds.Height);
            var width = right - left;
            var height = bottom - top;

            using (var screenBmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (var bmpGraphics = Graphics.FromImage(screenBmp))
                {
                    bmpGraphics.CopyFromScreen(left, top, 0, 0, new System.Drawing.Size(width, height));
                    return Imaging.CreateBitmapSourceFromHBitmap(
                        screenBmp.GetHbitmap(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                }
            }
        }
    }
}
