using AlbionDataHandlers;
using BaseUtils.Logger.Impl;
using PacketDotNet;
using PacketDotNet.Ieee80211;
using PhotonParser;
using SharpPcap;

namespace AlbionRadar;

public class Program
{
    private readonly AlbionDataParser albionDataParser;

    public Program(AlbionDataParser albionDataParser)
    {
        this.albionDataParser = albionDataParser;
    }

    public void Start()
    {
        ICaptureDevice device = PacketDeviceSelector.AskForPacketDevice();

        // Fix for CS0149: Use a lambda expression to specify the method name  
        //device.OnPacketArrival += (sender, e) => Device_OnPacketArrival_ManualStyle(sender, e);
        device.OnPacketArrival += PacketHandler;

        // Fix for CS0103: Define DeviceMode explicitly  
        device.Open(DeviceModes.MaxResponsiveness, 1000);

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
                albionDataParser.ReceivePacket(packet.PayloadData);
            }
            catch (Exception ex)
            {
                DLog.I($"Error: {ex}");
            }
        }
    }

    private void Device_OnPacketArrival_ManualStyle(object sender, PacketCapture e)
    {
        try
        {
            RawCapture rawCapture = e.GetPacket();
            byte[] buffer = rawCapture.Data; // This is the full raw packet buffer  
            int nbytes = rawCapture.Data.Length; // Equivalent to JS nbytes  

            var packet = Packet.ParsePacket(rawCapture.LinkLayerType, buffer);
            if (packet == null)
            {
                return;
            }

            IPPacket ipPacket = packet.Extract<IPPacket>();
            if (ipPacket == null)
            {
                return;
            }

            UdpPacket udpPacket = ipPacket.Extract<UdpPacket>();
            if (udpPacket == null)
            {
                return;
            }

            byte[] payload = udpPacket.PayloadData;

            if (payload != null && payload.Length > 0)
            {
                //string hexPayload = BitConverter.ToString(payload).Replace("-", " "); // Convert byte[] to hex string  
                //DLog.I($"Payload (Hex): {hexPayload}");
                albionDataParser.ReceivePacket(payload);
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error processing packet (manual style): {ex.Message}");
        }
    }
}
