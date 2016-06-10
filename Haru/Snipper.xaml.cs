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
using System.Media;

namespace Haru
{

    public partial class Snipper : Window
    {
        private System.Windows.Point p1;
        private System.Windows.Point p2;
        private BitmapSource screenshot;
        private State state = State.START;

        enum State {START,DRAG}

        public Snipper()
        {
            InitializeComponent();
            screenshot = CopyScreen();
            snipcanvas.Background = new ImageBrush(screenshot);
            MouseDown += Snipper_MouseDown;
            MouseUp += Snipper_MouseUp;
            MouseMove += Snipper_MouseMove;
        }

        private void Snipper_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (state == State.START)
            {
                p1 = e.GetPosition(this);
                state = State.DRAG;
            }
        }

        private void Snipper_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //var notificationSound = new SoundPlayer(Haru.Properties.Resources.blip);
            //notificationSound.Play();
            p2 = e.GetPosition(this);
            if (p1 != p2)
            {
                System.Windows.Clipboard.SetImage(new CroppedBitmap(screenshot, new Int32Rect((int)p1.X, (int)p1.Y, (int)Math.Abs(p2.X - p1.X), (int)Math.Abs(p2.Y - p1.Y))));
                DialogResult = true;
            }
            else
            {
                state = State.START;
            }
        }

        private void Snipper_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (state == State.DRAG) {
                this.innerRect.Rect = new Rect(p1, e.GetPosition(this));
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            Left = Screen.AllScreens.Min(screen => screen.Bounds.X);
            Top = Screen.AllScreens.Min(screen => screen.Bounds.Y);
            Width = Screen.AllScreens.Max(screen => screen.Bounds.X + screen.Bounds.Width);
            Height = Screen.AllScreens.Max(screen => screen.Bounds.Y + screen.Bounds.Height);
            outerRect.Rect = new Rect(0, 0, Width, Height);
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

        public static void SaveClipboardImageToFile(string filePath)
        {
            var image = System.Windows.Clipboard.GetImage();
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(fileStream);
            }
        }
    }
}
