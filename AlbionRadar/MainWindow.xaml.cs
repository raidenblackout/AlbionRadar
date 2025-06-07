using AlbionRadar.ViewModels;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Application = System.Windows.Application;

namespace AlbionRadar
{
    /// <summary>  
    /// Interaction logic for MainWindow.xaml  
    /// </summary>  
    public partial class MainWindow : Window
    {
        #region Constants and Fields  

        // Constants for window styles  
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x00080000;
        private const int WS_EX_TRANSPARENT = 0x00000020;

        // Timer for polling the target window  
        private DispatcherTimer _pollTimer;

        // Handle to the target window  
        private IntPtr _targetHwnd = IntPtr.Zero;

        // Title of the target window  
        private const string TARGET_WINDOW_TITLE = "Albion Online Client";

        #endregion

        #region Public Properties  

        /// <summary>  
        /// ViewModel for data binding.  
        /// </summary>  
        public MainViewModel ViewModel { get; set; }

        #endregion

        #region Constructor  

        /// <summary>  
        /// Initializes the MainWindow and sets up tracking.  
        /// </summary>  
        public MainWindow()
        {
            InitializeComponent();

            // Set window styles to make it layered and transparent  
            var hwnd = new WindowInteropHelper(this).Handle;
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_LAYERED | WS_EX_TRANSPARENT);

            // Start tracking the target window  
            StartTracking();

            // Initialize ViewModel and set DataContext  
            ViewModel = new MainViewModel();
            this.DataContext = ViewModel;
        }

        #endregion

        #region Private Methods  

        /// <summary>  
        /// Starts the timer to periodically poll the target window.  
        /// </summary>  
        private void StartTracking()
        {
            CompositionTarget.Rendering += PollTargetWindow;
        }

        /// <summary>  
        /// Polls the target window to check its visibility and position.  
        /// </summary>  
        private void PollTargetWindow(object sender, EventArgs e)
        {
            // Find the target window by its title  
            _targetHwnd = FindWindow(null, TARGET_WINDOW_TITLE);

            // Hide this window if the target window is not found, minimized, or not visible  
            if (_targetHwnd == IntPtr.Zero || !IsWindowVisible(_targetHwnd) || IsIconic(_targetHwnd))
            {
                this.Hide();
                return;
            }

            // Hide this window if the target window is not in the foreground  
            IntPtr foregroundWindow = GetForegroundWindow();
            if (foregroundWindow != _targetHwnd)
            {
                this.Hide();
                return;
            }

            // Update this window's position and size to match the target window  
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

        /// <summary>  
        /// Handles double-click events on the tray icon to toggle window visibility.  
        /// </summary>  
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

        /// <summary>  
        /// Handles the Exit menu item click to close the application.  
        /// </summary>  
        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion

        #region User32.dll Imports  

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left, Top, Right, Bottom;
        }

        #endregion
    }
}