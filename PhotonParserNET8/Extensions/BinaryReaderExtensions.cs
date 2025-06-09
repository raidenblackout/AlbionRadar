using System.Text;

namespace PhotonParser.Extensions
{
    public static class BinaryReaderExtensions
    {
        public static ushort ReadUInt16BE(this BinaryReader reader)
        {
            var bytes = reader.ReadBytes(2);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return BitConverter.ToUInt16(bytes, 0);
        }

        public static short ReadInt16BE(this BinaryReader reader)
        {
            var bytes = reader.ReadBytes(2);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return BitConverter.ToInt16(bytes, 0);
        }

        public static uint ReadUInt32BE(this BinaryReader reader)
        {
            var bytes = reader.ReadBytes(4);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }

        public static int ReadInt32BE(this BinaryReader reader)
        {
            var bytes = reader.ReadBytes(4);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        public static long ReadInt64BE(this BinaryReader reader) // For Photon's Long
        {
            var bytes = reader.ReadBytes(8);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return BitConverter.ToInt64(bytes, 0);
        }

        public static float ReadSingleBE(this BinaryReader reader) // For Photon's Float
        {
            var bytes = reader.ReadBytes(4);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return BitConverter.ToSingle(bytes, 0);
        }

        public static double ReadDoubleBE(this BinaryReader reader) // For Photon's Double
        {
            var bytes = reader.ReadBytes(8);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return BitConverter.ToDouble(bytes, 0);
        }

        public static string ReadString(this BinaryReader reader) // Photon's string serialization
        {
            short stringSize = reader.ReadInt16BE();
            if (stringSize == 0) return "";
            byte[] stringBytes = reader.ReadBytes(stringSize);
            return Encoding.UTF8.GetString(stringBytes);
        }
    }

    // Protocol16Type.cs (replace with your actual values from Protocol16Type.json)
    public static class Protocol16Type
    {
        public const byte Unknown = 0; // Or whatever '*' maps to
        public const byte Null = (byte)'*'; // Example, check your JSON
        public const byte Dictionary = (byte)'D';
        public const byte StringArray = (byte)'a';
        public const byte Byte = (byte)'b';
        public const byte Custom = (byte)'c'; // Not in your JS example, but common
        public const byte Double = (byte)'d';
        public const byte EventData = (byte)'e';
        public const byte Float = (byte)'f';
        public const byte Hashtable = (byte)'h';
        public const byte Integer = (byte)'i';
        public const byte IntegerArray = (byte)'I'; // Custom type in JS, usually not a distinct type code
        public const byte Short = (byte)'k';
        public const byte Long = (byte)'l';
        public const byte Boolean = (byte)'o';
        public const byte OperationResponse = (byte)'p';
        public const byte OperationRequest = (byte)'q';
        public const byte String = (byte)'s';
        public const byte ByteArray = (byte)'x';
        public const byte Array = (byte)'y'; // For object[]
        public const byte ObjectArray = (byte)'z'; // Often same as Array 'y'

        // Add all types from your JSON file
    }

    // PhotonMessageTypes.cs
    public enum PhotonMessageType : byte
    {
        OperationRequest = 2,
        OperationResponse = 3,
        Event = 4,
        InternalOperationRequest = 6, // Not in your JS, but exists
        InternalOperationResponse = 7, // Not in your JS, but exists
    }

    // PhotonCommandTypes.cs
    public enum PhotonCommandType : byte
    {
        Ack = 1,
        Connect = 2,
        VerifyConnect = 3,
        Disconnect = 4,
        Ping = 5,
        SendReliable = 6,
        SendUnreliable = 7,
        SendFragment = 8,
        // EG_SENDFRAGMENT = 8, // Older name
        // EG_ACK_UNRELIABLE = 9, // Not standard Photon UDP
        // EG_SEND_UNSEQUENCED = 10 // Not standard Photon UDP
    }
}
