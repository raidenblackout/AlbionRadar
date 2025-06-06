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

        device.OnPacketArrival += PacketHandler;

        device.Open(DeviceModes.Promiscuous, 1000);

        device.StartCapture();
    }
 
    private void PacketHandler(object sender, PacketCapture e)
    {
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
}
