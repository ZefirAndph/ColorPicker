using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Point = System.Windows.Point;
using Color = System.Drawing.Color;
using System.Diagnostics;
namespace ColorPicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out POINT lpPoint);
        
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);
        
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        Color actual;

        public void UpdateInfo()
        {
            POINT p;// = Mouse.GetPosition(null);
            GetCursorPos(out p);
            actual = PickColor(p);
            Barva.Content = $"{ColorTranslator.ToHtml(actual)}";
            this.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(actual.R, actual.G, actual.B));
            Barva.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)(255 - actual.R), (byte)(255 - actual.G), (byte)(255 - actual.B)));
        }

        private Color PickColor(POINT p)
        {

            // Získání souřadnic
            int x = p.X;
            int y = p.Y;

            // Získání handle hlavního okna
            IntPtr desktopDC = GetDC(IntPtr.Zero);

            // Získání barvy pixelu na souřadnicích kurzoru myši
            uint pixelColor = GetPixel(desktopDC, x, y);

            // Uvolnění device context
            ReleaseDC(IntPtr.Zero, desktopDC);

            // Extrahování složek barvy
            byte r = (byte)(pixelColor & 0x000000FF);
            byte g = (byte)((pixelColor & 0x0000FF00) >> 8);
            byte b = (byte)((pixelColor & 0x00FF0000) >> 16);

            return Color.FromArgb(r, g, b);
        }

        private void KeyPress(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Space)
            {
                UpdateInfo();
            }
        }

        private void Copy(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(ColorTranslator.ToHtml(actual));
        }
    }
}