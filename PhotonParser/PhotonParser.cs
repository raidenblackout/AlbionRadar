using PhotonParser.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace PhotonParser;

public abstract class PhotonParser
{
    private const int CommandHeaderLength = 12;

    private const int PhotonHeaderLength = 12;

    private readonly Dictionary<int, SegmentedPackage> _pendingSegments = new Dictionary<int, SegmentedPackage>();

    public void ReceivePacket(byte[] payload)
    {
        if (payload.Length < 12)
        {
            return;
        }

        int offset = 0;
        NumberDeserializer.Deserialize(out short _, payload, ref offset);
        ReadByte(out var value2, payload, ref offset);
        ReadByte(out var value3, payload, ref offset);
        NumberDeserializer.Deserialize(out int _, payload, ref offset);
        NumberDeserializer.Deserialize(out int _, payload, ref offset);
        bool num = value2 == 1;
        bool flag = value2 == 204;
        if (num)
        {
            return;
        }

        if (flag)
        {
            int offset2 = 0;
            NumberDeserializer.Deserialize(out int value6, payload, ref offset2);
            NumberSerializer.Serialize(0, payload, ref offset);
            if (value6 != CrcCalculator.Calculate(payload, payload.Length))
            {
                return;
            }
        }

        for (int i = 0; i < value3; i++)
        {
            HandleCommand(payload, ref offset);
        }
    }

    protected abstract void OnRequest(byte operationCode, Dictionary<byte, object> parameters);

    protected abstract void OnResponse(byte operationCode, short returnCode, string debugMessage, Dictionary<byte, object> parameters);

    protected abstract void OnEvent(byte code, Dictionary<byte, object> parameters);

    private void HandleCommand(byte[] source, ref int offset)
    {
        ReadByte(out var value, source, ref offset);
        ReadByte(out var _, source, ref offset);
        ReadByte(out var _, source, ref offset);
        offset++;
        NumberDeserializer.Deserialize(out int value4, source, ref offset);
        NumberDeserializer.Deserialize(out int _, source, ref offset);
        value4 -= 12;
        switch ((CommandType)value)
        {
            case CommandType.Disconnect:
                break;
            case CommandType.SendUnreliable:
                offset += 4;
                value4 -= 4;
                goto case CommandType.SendReliable;
            case CommandType.SendReliable:
                HandleSendReliable(source, ref offset, ref value4);
                break;
            case CommandType.SendFragment:
                HandleSendFragment(source, ref offset, ref value4);
                break;
            default:
                offset += value4;
                break;
        }
    }

    private void HandleSendReliable(byte[] source, ref int offset, ref int commandLength)
    {
        offset++;
        commandLength--;
        ReadByte(out var value, source, ref offset);
        commandLength--;
        int num = commandLength;
        Protocol16Stream protocol16Stream = new Protocol16Stream(num);
        protocol16Stream.Write(source, offset, num);
        protocol16Stream.Seek(0L, SeekOrigin.Begin);
        offset += num;
        switch ((MessageType)value)
        {
            case MessageType.OperationRequest:
                {
                    OperationRequest operationRequest = Protocol16Deserializer.DeserializeOperationRequest(protocol16Stream);
                    OnRequest(operationRequest.OperationCode, operationRequest.Parameters);
                    break;
                }
            case MessageType.OperationResponse:
                {
                    OperationResponse operationResponse = Protocol16Deserializer.DeserializeOperationResponse(protocol16Stream);
                    OnResponse(operationResponse.OperationCode, operationResponse.ReturnCode, operationResponse.DebugMessage, operationResponse.Parameters);
                    break;
                }
            case MessageType.Event:
                {
                    EventData eventData = Protocol16Deserializer.DeserializeEventData(protocol16Stream);
                    OnEvent(eventData.Code, eventData.Parameters);
                    break;
                }
        }
    }

    private void HandleSendFragment(byte[] source, ref int offset, ref int commandLength)
    {
        NumberDeserializer.Deserialize(out int value, source, ref offset);
        commandLength -= 4;
        NumberDeserializer.Deserialize(out int _, source, ref offset);
        commandLength -= 4;
        NumberDeserializer.Deserialize(out int _, source, ref offset);
        commandLength -= 4;
        NumberDeserializer.Deserialize(out int value4, source, ref offset);
        commandLength -= 4;
        NumberDeserializer.Deserialize(out int value5, source, ref offset);
        commandLength -= 4;
        int fragmentLength = commandLength;
        HandleSegmentedPayload(value, value4, fragmentLength, value5, source, ref offset);
    }

    private void HandleFinishedSegmentedPackage(byte[] totalPayload)
    {
        int offset = 0;
        int commandLength = totalPayload.Length;
        HandleSendReliable(totalPayload, ref offset, ref commandLength);
    }

    private void HandleSegmentedPayload(int startSequenceNumber, int totalLength, int fragmentLength, int fragmentOffset, byte[] source, ref int offset)
    {
        SegmentedPackage segmentedPackage = GetSegmentedPackage(startSequenceNumber, totalLength);
        Buffer.BlockCopy(source, offset, segmentedPackage.TotalPayload, fragmentOffset, fragmentLength);
        offset += fragmentLength;
        segmentedPackage.BytesWritten += fragmentLength;
        if (segmentedPackage.BytesWritten >= segmentedPackage.TotalLength)
        {
            _pendingSegments.Remove(startSequenceNumber);
            HandleFinishedSegmentedPackage(segmentedPackage.TotalPayload);
        }
    }

    private SegmentedPackage GetSegmentedPackage(int startSequenceNumber, int totalLength)
    {
        if (_pendingSegments.TryGetValue(startSequenceNumber, out var value))
        {
            return value;
        }

        value = new SegmentedPackage
        {
            TotalLength = totalLength,
            TotalPayload = new byte[totalLength]
        };
        _pendingSegments.Add(startSequenceNumber, value);
        return value;
    }

    private static void ReadByte(out byte value, byte[] source, ref int offset)
    {
        value = source[offset++];
    }
}
