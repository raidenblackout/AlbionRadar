using AlbionRadar.Entities;
using SharpPcap;
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

namespace AlbionRadar.Dialogues
{
    /// <summary>  
    /// Interaction logic for PacketDeviceSelectorDialog.xaml  
    /// </summary>  
    public partial class PacketDeviceSelectorDialog : Window
    {
        // The selected capture device  
        public ICaptureDevice SelectedDevice { get; private set; }

        // List of displayable capture devices for the UI  
        private List<DisplayableCaptureDevice> _devices = new();

        /// <summary>  
        /// Initializes the dialog and loads available devices.  
        /// </summary>  
        public PacketDeviceSelectorDialog()
        {
            InitializeComponent();
            LoadDevices();
        }

        /// <summary>  
        /// Loads the list of available capture devices and populates the UI.  
        /// </summary>  
        private void LoadDevices()
        {
            // Retrieve the list of available capture devices  
            CaptureDeviceList devices = CaptureDeviceList.Instance;

            // If no devices are found, show an error message and close the dialog  
            if (devices.Count == 0)
            {
                MessageBox.Show(
                    "No interfaces found! Make sure WinPcap/Npcap is installed.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                DialogResult = false;
                Close();
                return;
            }

            // Map devices to displayable objects and bind them to the ListBox  
            _devices = devices
                .Select((dev, index) => new DisplayableCaptureDevice(dev, index))
                .ToList();
            DeviceListBox.ItemsSource = _devices;

            // Select the first device by default if any are available  
            if (_devices.Any())
            {
                DeviceListBox.SelectedIndex = 0;
            }
        }

        /// <summary>  
        /// Handles the Select button click event.  
        /// Sets the selected device and closes the dialog.  
        /// </summary>  
        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (DeviceListBox.SelectedItem is DisplayableCaptureDevice selectedDisplayableDevice)
            {
                SelectedDevice = selectedDisplayableDevice.Device;
                DialogResult = true; // Indicates successful selection  
                Close();
            }
            else
            {
                MessageBox.Show(
                    "Please select a device.",
                    "Selection Required",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
            }
        }

        /// <summary>  
        /// Handles the Cancel button click event.  
        /// Closes the dialog without selecting a device.  
        /// </summary>  
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Indicates cancellation  
            Close();
        }
    }
}
