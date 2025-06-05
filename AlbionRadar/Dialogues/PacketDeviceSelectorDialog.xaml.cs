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
        public ICaptureDevice SelectedDevice { get; private set; }
        private List<DisplayableCaptureDevice> _devices=new();

        public PacketDeviceSelectorDialog()
        {
            InitializeComponent();
            LoadDevices();
        }

        private void LoadDevices()
        {
            CaptureDeviceList devices = CaptureDeviceList.Instance;

            if (devices.Count == 0)
            {
                MessageBox.Show("No interfaces found! Make sure WinPcap/Npcap is installed.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                DialogResult = false; // Close dialog
                Close();
                return;
            }

            _devices = devices.Select((dev, index) => new DisplayableCaptureDevice(dev, index)).ToList();
            DeviceListBox.ItemsSource = _devices;
            if (_devices.Any())
            {
                DeviceListBox.SelectedIndex = 0; // Select first by default
            }
        }

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
                MessageBox.Show("Please select a device.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Indicates cancellation
            Close();
        }
    }
}
