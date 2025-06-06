using AlbionRadar.ViewModels;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace AlbionRadar
{
    /// <summary>  
    /// Interaction logic for MainWindow.xaml  
    /// </summary>  
    public partial class MainWindow : Window
    {
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x00080000;
        private const int WS_EX_TRANSPARENT = 0x00000020;

        public MainViewModel ViewModel { get; set; }

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public MainWindow()
        {
            InitializeComponent();
            var hwnd = new WindowInteropHelper(this).Handle;
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_LAYERED | WS_EX_TRANSPARENT);

            // Optional: Track external window and move overlay to match  
            TrackAndPositionOverExternalWindow("Albion Online Client");

            ViewModel = new MainViewModel();
            this.DataContext = ViewModel;
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;
        }

        private void TrackAndPositionOverExternalWindow(string windowTitle)
        {
            var hwnd = FindWindow(null, windowTitle);
            if (hwnd == IntPtr.Zero)
            {
                MessageBox.Show("Target window not found!");
                return;
            }

            if (GetWindowRect(hwnd, out RECT rect))
            {
                Left = rect.Left;
                Top = rect.Top;
                Width = rect.Right - rect.Left;
                Height = rect.Bottom - rect.Top;
            }
        }
    }
}