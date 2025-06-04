using PcapDotNet.Packets;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;
using System.IO;
using System.Net;

namespace PhotonAnalyzer.Entities;

class PhotonPacketHandler
{
    class Fragment
    {
        public byte[] buffer;
        public int totalFragments;
        public int readFragments;
    }

    private Dictionary<int, Fragment> fragmentMap = new Dictionary<int, Fragment>();

    PacketHandler _eventHandler;
    public PhotonPacketHandler(PacketHandler p)
    {
        this._eventHandler = p;
    }
    public void PacketHandler(Packet packet)
    {
        try
        {
            // Make this static or at least dont create a new Protocol16 for every package
            Protocol16 protocol16 = new Protocol16();

            IpV4Datagram ip = packet.Ethernet.IpV4;
            UdpDatagram udp = ip.Udp;

            // Not technically necesasry as the bpf limits to only udp 5055
            if (udp.SourcePort != 5056 && udp.DestinationPort != 5056 && udp.SourcePort != 5055 && udp.DestinationPort != 5055)
                return;


            var ms = udp.Payload.ToMemoryStream();
            var p = new BinaryReader(ms);

            var peerId = IPAddress.NetworkToHostOrder(p.ReadUInt16());
            var crcEnabled = p.ReadByte();
            var commandCount = p.ReadByte();
            var timestamp = IPAddress.NetworkToHostOrder(p.ReadInt32());
            var challenge = IPAddress.NetworkToHostOrder(p.ReadInt32());

            var commandHeaderLength = 12;
            var signifierByteLength = 1;

            for (int commandIdx = 0; commandIdx < commandCount; commandIdx++)
            {

                var commandType = p.ReadByte();
                var channelId = p.ReadByte();
                var commandFlags = p.ReadByte();
                var unkBytes = p.ReadByte();
                var commandLength = IPAddress.NetworkToHostOrder(p.ReadInt32());
                var sequenceNumber = IPAddress.NetworkToHostOrder(p.ReadInt32());

                void ParseOperation(byte messageType, StreamBuffer payload, int operationLength)
                {
                    switch (messageType)
                    {
                        case 2: //Operation Request
                            var requestData = protocol16.DeserializeOperationRequest(payload);
                            _eventHandler.OnRequest(requestData.OperationCode, requestData.Parameters);
                            break;
                        case 3: //Operation Response
                            var responseData = protocol16.DeserializeOperationResponse(payload);
                            _eventHandler.OnResponse(responseData.OperationCode, responseData.ReturnCode, responseData.Parameters);
                            break;
                        case 4: //Event
                            var eventData = protocol16.DeserializeEventData(payload);
                            _eventHandler.OnEvent(eventData.Code, eventData.Parameters);
                            break;
                        default:
                            p.BaseStream.Position += operationLength;
                            break;
                    }
                }

                switch (commandType)
                {
                    case 4://Disconnect
                        break;
                    case 7://Send unreliable
                        p.BaseStream.Position += 4;
                        commandLength -= 4;
                        goto case 6;
                    case 6://Send reliable
                        {
                            p.BaseStream.Position += signifierByteLength;
                            var messageType = p.ReadByte();

                            var operationLength = commandLength - commandHeaderLength - 2;
                            var payload = new StreamBuffer(p.ReadBytes(operationLength));
                            ParseOperation(messageType, payload, operationLength);
                            break;
                        }
                    case 8: // Reliable Fragment
                        {
                            // Each fragment contains a starting sequence number, its sequence number,
                            // and how many fragments total there are.
                            var startSequenceNumber = IPAddress.NetworkToHostOrder(p.ReadInt32());
                            var fragmentCount = IPAddress.NetworkToHostOrder(p.ReadInt32());
                            var fragmentNumber = IPAddress.NetworkToHostOrder(p.ReadInt32());
                            var totalLength = IPAddress.NetworkToHostOrder(p.ReadInt32());
                            var fragmentOffset = IPAddress.NetworkToHostOrder(p.ReadInt32());
                            var operationLength = commandLength - 5 * 4 - commandHeaderLength; // 5 4-byte 
                            var payload = p.ReadBytes(operationLength);

                            if (!fragmentMap.TryGetValue(startSequenceNumber, out var fragment))
                            {
                                fragment = new Fragment
                                {
                                    buffer = new byte[totalLength],
                                    totalFragments = fragmentCount,
                                    readFragments = 0
                                };
                                fragmentMap.Add(startSequenceNumber, fragment);
                            }

                            fragment.readFragments++;
                            Array.Copy(payload, 0, fragment.buffer, fragmentOffset, payload.Length);

                            if (fragment.readFragments == fragment.totalFragments)
                            {
                                Console.WriteLine("Fragment Complete");
                                var stream = new StreamBuffer(fragment.buffer);
                                stream.Position += signifierByteLength;

                                Console.WriteLine("Fragment Stream Read");
                                var messageType = stream.ReadByte();
                                Console.WriteLine("Fragment MessageType: " + messageType);
                                ParseOperation(messageType, stream, operationLength);
                                Console.WriteLine("Fragment Successfully Handled");
                                fragmentMap.Remove(startSequenceNumber);
                            }
                            break;
                        }

                    default:
                        if (commandType != 1 && commandType != 5)
                        {
                            // Console.WriteLine("Received Unhandled Command Type: " + commandType);
                        }
                        p.BaseStream.Position += commandLength - commandHeaderLength;
                        break;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("PacketHandler Exception: " + e.ToString());
        }
    }
}