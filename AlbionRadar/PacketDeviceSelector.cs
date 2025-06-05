using AlbionRadar.Dialogues;
using SharpPcap;
using System.Windows;

namespace AlbionRadar;

public static class PacketDeviceSelector
{
    public static ICaptureDevice AskForPacketDevice()
    {
        try
        {
            // Test if devices can be listed. This might throw if Npcap/WinPcap isn't properly installed or accessible.
            var testDevices = CaptureDeviceList.Instance;
            if (testDevices.Count == 0 && System.Environment.OSVersion.Version.Major >= 10) // Windows 10 or newer
            {
                // A common issue with Npcap is needing admin rights or correct installation scope
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
            MessageBox.Show($"Error initializing packet capture: {ex.Message}\n\n" +
                            "Please ensure WinPcap or Npcap is installed correctly. " +
                            "If using Npcap, ensure 'WinPcap API-compatible Mode' was selected during installation.",
                            "Initialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return null;
        }


        var dialog = new PacketDeviceSelectorDialog();
        bool? result = dialog.ShowDialog(); // ShowDialog is blocking

        if (result == true)
        {
            return dialog.SelectedDevice;
        }
        return null; // User cancelled or no device selected
    }
}
