using AlbionRadar.ViewModels;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using Application = System.Windows.Application;

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
        private DispatcherTimer _pollTimer;
        private IntPtr _targetHwnd = IntPtr.Zero;
        private const string TARGET_WINDOW_TITLE = "Albion Online Client";

        public MainViewModel ViewModel { get; set; }

        private void StartTracking()
        {
            _pollTimer = new DispatcherTimer();
            _pollTimer.Interval = TimeSpan.FromMilliseconds(500);
            _pollTimer.Tick += PollTargetWindow;
            _pollTimer.Start();
        }

        private void PollTargetWindow(object sender, EventArgs e)
        {

            _targetHwnd = FindWindow(null, TARGET_WINDOW_TITLE);

            if (_targetHwnd == IntPtr.Zero || !IsWindowVisible(_targetHwnd) || IsIconic(_targetHwnd))
            {
                this.Hide();
                return;
            }

            IntPtr foregroundWindow = GetForegroundWindow();
            if (foregroundWindow != _targetHwnd)
            {
                this.Hide();
                return;
            }

            if (GetWindowRect(_targetHwnd, out RECT rect))
            {
                this.Left = rect.Left;
                this.Top = rect.Top + 200;
                this.Width = 400;
                this.Height = 400;

                if (!this.IsVisible)
                    this.Show();
            }
            else
            {
                this.Hide();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            var hwnd = new WindowInteropHelper(this).Handle;
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_LAYERED | WS_EX_TRANSPARENT);

            // Optional: Track external window and move overlay to match  
            StartTracking();

            ViewModel = new MainViewModel();
            this.DataContext = ViewModel;
        }

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;
        }

        private void MyNotifyIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.WindowState = WindowState.Normal;
                this.Show();
                this.Activate();
            }
            else
            {
                this.Hide();
            }
        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}