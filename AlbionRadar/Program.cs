using AlbionDataHandlers;
using BaseUtils.Logger.Impl;
using PacketDotNet;
using PhotonPackageParser;
using SharpPcap;

namespace AlbionRadar;

public class Program
{
    private readonly AlbionDataParser photonParser;

    public Program(AlbionDataParser photonParser)
    {
        this.photonParser = photonParser ?? throw new ArgumentNullException(nameof(photonParser));
    }

    public void Start()
    {
        ICaptureDevice device = PacketDeviceSelector.AskForPacketDevice();

        // Fix for CS0149: Use a lambda expression to specify the method name  
        device.OnPacketArrival += (sender, e) => PacketHandler(sender, e);

        // Fix for CS0103: Define DeviceMode explicitly  
        device.Open(DeviceModes.Promiscuous, 1000);

        device.StartCapture();
    }

    // Fix for CS0246 and IDE0060: Ensure CaptureEventArgs is properly referenced and remove unused parameters  
    private void PacketHandler(object sender, PacketCapture e)
    {
        // Fix for CS1061: Use the GetPacket() method to retrieve the RawCapture object,  
        // which contains the necessary information to parse the packet.  
        RawCapture rawCapture = e.GetPacket();
        UdpPacket? packet = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data).Extract<UdpPacket>();
        if (packet != null && (packet.SourcePort == 5056 || packet.DestinationPort == 5056))
        {
            try
            {
                photonParser.ReceivePacket(packet.PayloadData);
            }catch(Exception ex)
            {
                DLog.I($"Error: {ex}");
            }
        }
    }
}
