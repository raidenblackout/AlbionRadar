using AlbionDataHandlers;
using BaseUtils.Logger.Impl;
using PacketDotNet;
using SharpPcap;
using System.Threading.Tasks;

namespace AlbionRadar;

/// <summary>  
/// Main program class for AlbionRadar.  
/// Handles packet capture and processing.  
/// </summary>  
public class Program
{
    private readonly AlbionDataParser _albionDataParser;

    /// <summary>  
    /// Constructor to initialize the AlbionDataParser.  
    /// </summary>  
    /// <param name="albionDataParser">Instance of AlbionDataParser.</param>  
    public Program(AlbionDataParser albionDataParser)
    {
        _albionDataParser = albionDataParser;
    }

    /// <summary>  
    /// Starts the packet capture process.  
    /// </summary>  
    public void Start()
    {
        // Prompt the user to select a network device for packet capture.  
        ICaptureDevice device = PacketDeviceSelector.AskForPacketDevice();

        // Attach the packet handler to process incoming packets.  
        device.OnPacketArrival += PacketHandler;

        // Open the device in MaxResponsiveness mode with a read timeout of 1000ms.  
        device.Open(DeviceModes.MaxResponsiveness, 1000);

        // Start capturing packets.  
        device.StartCapture();
    }

    /// <summary>  
    /// Handles incoming packets and processes UDP packets on port 5056.  
    /// </summary>  
    /// <param name="sender">The source of the event.</param>  
    /// <param name="e">Packet capture event arguments.</param>  
    private void PacketHandler(object sender, PacketCapture e)
    {
        // Extract the raw packet data.  
        RawCapture rawCapture = e.GetPacket();

        // Parse the packet and extract the UDP layer.  
        UdpPacket? packet = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data).Extract<UdpPacket>();

        // Check if the packet is a UDP packet and matches the target port (5056).  
        if (packet != null && (packet.SourcePort == 5056 || packet.DestinationPort == 5056))
        {
            // Process the packet asynchronously to avoid blocking the main thread.  
            Task.Run(() =>
            {
                try
                {
                    // Pass the packet payload to the AlbionDataParser for processing.  
                    _albionDataParser.ReceivePacket(packet.PayloadData);
                }
                catch (Exception ex)
                {
                    // Log any errors that occur during packet processing.  
                    DLog.I($"Error: {ex}");
                }
            });
        }
    }
}
