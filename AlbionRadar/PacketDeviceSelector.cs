using AlbionRadar.Dialogues;
using SharpPcap;
using System.Windows;

namespace AlbionRadar;

/// <summary>
/// Provides functionality to select a network packet capture device.
/// </summary>
public static class PacketDeviceSelector
{
    /// <summary>
    /// Prompts the user to select a network packet capture device.
    /// </summary>
    /// <returns>
    /// The selected <see cref="ICaptureDevice"/> if a device is chosen, or null if no device is selected or an error occurs.
    /// </returns>
    public static ICaptureDevice AskForPacketDevice()
    {
        try
        {
            // Attempt to retrieve the list of available capture devices.
            // This may throw an exception if Npcap/WinPcap is not properly installed or accessible.
            var availableDevices = CaptureDeviceList.Instance;

            // Check if no devices are found and the OS is Windows 10 or newer.
            if (availableDevices.Count == 0 && System.Environment.OSVersion.Version.Major >= 10)
            {
                // Display a message box with troubleshooting steps for Npcap installation issues.
                MessageBox.Show(
                    "No network interfaces found. \n\n" +
                    "If you have Npcap installed, try reinstalling it and ensure you check:\n" +
                    "1. 'Install Npcap in WinPcap API-compatible Mode'.\n" +
                    "2. 'Support raw 802.11 traffic (and monitor mode) for wireless adapters' (if applicable).\n" +
                    "3. Consider if you need to run this application as Administrator.\n\n" +
                    "SharpPcap might also require the Npcap OEM (Silent) installation for non-admin users " +
                    "or for services, or Npcap installed for 'All Users'.",
                    "Interface Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return null;
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions that occur during device initialization and provide troubleshooting guidance.
            MessageBox.Show(
                $"Error initializing packet capture: {ex.Message}\n\n" +
                "Please ensure WinPcap or Npcap is installed correctly. " +
                "If using Npcap, ensure 'WinPcap API-compatible Mode' was selected during installation.",
                "Initialization Error", MessageBoxButton.OK, MessageBoxImage.Error);

            return null;
        }

        // Open the packet device selection dialog.
        var deviceSelectionDialog = new PacketDeviceSelectorDialog();
        bool? dialogResult = deviceSelectionDialog.ShowDialog(); // ShowDialog is blocking.

        // Return the selected device if the user confirms their selection.
        if (dialogResult == true)
        {
            return deviceSelectionDialog.SelectedDevice;
        }

        // Return null if the user cancels the dialog or no device is selected.
        return null;
    }
}
