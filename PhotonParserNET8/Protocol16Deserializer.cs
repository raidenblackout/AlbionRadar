using PhotonParser.Models;
using System.Collections;
using System.Text;
using BaseUtils.Logger.Impl;

namespace PhotonParser
{
    internal enum Protocol16Type : byte
    {
        Unknown = 0,
        Null = 42,
        Dictionary = 68,
        StringArray = 97,
        Byte = 98,
        Double = 100,
        EventData = 101,
        Float = 102,
        Integer = 105,
        Hashtable = 104,
        Short = 107,
        Long = 108,
        IntegerArray = 110,
        Boolean = 111,
        OperationResponse = 112,
        OperationRequest = 113,
        String = 115,
        ByteArray = 120,
        Array = 121,
        ObjectArray = 122
    }

    public static class Protocol16Deserializer
    {
        private static readonly ThreadLocal<byte[]> _byteBuffer = new ThreadLocal<byte[]>(() => new byte[8]);

        public static object Deserialize(Protocol16Stream input)
        {
            return Deserialize(input, (byte)input.ReadByte());
        }

        public static object Deserialize(Protocol16Stream input, byte typeCode)
        {
            switch ((Protocol16Type)typeCode)
            {
                case Protocol16Type.Unknown:
                case Protocol16Type.Null:
                    return null;
                case Protocol16Type.Dictionary:
                    return DeserializeDictionary(input);
                case Protocol16Type.StringArray:
                    return DeserializeStringArray(input);
                case Protocol16Type.Byte:
                    return DeserializeByte(input);
                case Protocol16Type.Double:
                    return DeserializeDouble(input);
                case Protocol16Type.EventData:
                    return DeserializeEventData(input);
                case Protocol16Type.Float:
                    return DeserializeFloat(input);
                case Protocol16Type.Integer:
                    return DeserializeInteger(input);
                case Protocol16Type.Hashtable:
                    return DeserializeHashtable(input);
                case Protocol16Type.Short:
                    return DeserializeShort(input);
                case Protocol16Type.Long:
                    return DeserializeLong(input);
                case Protocol16Type.IntegerArray:
                    return DeserializeIntArray(input);
                case Protocol16Type.Boolean:
                    return DeserializeBoolean(input);
                case Protocol16Type.OperationResponse:
                    return DeserializeOperationResponse(input);
                case Protocol16Type.OperationRequest:
                    return DeserializeOperationRequest(input);
                case Protocol16Type.String:
                    return DeserializeString(input);
                case Protocol16Type.ByteArray:
                    return DeserializeByteArray(input);
                case Protocol16Type.Array:
                    return DeserializeArray(input);
                case Protocol16Type.ObjectArray:
                    return DeserializeObjectArray(input);
                default:
                    throw new ArgumentException($"Type code: {typeCode} not implemented.");
            }
        }

        public static OperationRequest DeserializeOperationRequest(Protocol16Stream input)
        {
            byte operationCode = DeserializeByte(input);
            Dictionary<byte, object> parameters = DeserializeParameterTable(input);
            return new OperationRequest(operationCode, parameters);
        }

        public static OperationResponse DeserializeOperationResponse(Protocol16Stream input)
        {
            byte operationCode = DeserializeByte(input);
            short returnCode = DeserializeShort(input);
            string debugMessage = Deserialize(input, DeserializeByte(input)) as string;
            Dictionary<byte, object> parameters = DeserializeParameterTable(input);
            return new OperationResponse(operationCode, returnCode, debugMessage, parameters);
        }

        public static EventData DeserializeEventData(Protocol16Stream input)
        {
            byte code = DeserializeByte(input);
            Dictionary<byte, object> parameters = DeserializeParameterTable(input);

            if (code == 3 && parameters.ContainsKey(1) && parameters[1] is byte[] eventBytes)
            {
                try
                {
                    parameters[4] = BitConverter.ToSingle(eventBytes, 9);
                    parameters[5] = BitConverter.ToSingle(eventBytes, 13);

                    parameters[252] = 3;
                }
                catch (Exception ex)
                {
                    DLog.E($"Failed to deserialize event data: {ex.Message}");
                }
            }

            return new EventData(code, parameters);
        }

        private static Type GetTypeOfCode(byte typeCode)
        {
            switch ((Protocol16Type)typeCode)
            {
                case Protocol16Type.Unknown:
                case Protocol16Type.Null:
                    return typeof(object);
                case Protocol16Type.Dictionary:
                    return typeof(IDictionary);
                case Protocol16Type.StringArray:
                    return typeof(string[]);
                case Protocol16Type.Byte:
                    return typeof(byte);
                case Protocol16Type.Double:
                    return typeof(double);
                case Protocol16Type.EventData:
                    return typeof(EventData);
                case Protocol16Type.Float:
                    return typeof(float);
                case Protocol16Type.Integer:
                    return typeof(int);
                case Protocol16Type.Short:
                    return typeof(short);
                case Protocol16Type.Long:
                    return typeof(long);
                case Protocol16Type.IntegerArray:
                    return typeof(int[]);
                case Protocol16Type.Boolean:
                    return typeof(bool);
                case Protocol16Type.OperationResponse:
                    return typeof(OperationResponse);
                case Protocol16Type.OperationRequest:
                    return typeof(OperationRequest);
                case Protocol16Type.String:
                    return typeof(string);
                case Protocol16Type.ByteArray:
                    return typeof(byte[]);
                case Protocol16Type.Array:
                    return typeof(Array);
                case Protocol16Type.ObjectArray:
                    return typeof(object[]);
                default:
                    throw new ArgumentException($"Type code: {typeCode} not implemented.");
            }
        }

        private static byte DeserializeByte(Protocol16Stream stream)
        {
            return (byte)stream.ReadByte();
        }

        private static bool DeserializeBoolean(Protocol16Stream input)
        {
            return input.ReadByte() != 0;
        }

        public static short DeserializeShort(Protocol16Stream input)
        {
            byte[] value = _byteBuffer.Value;
            input.Read(value, 0, 2);
            return (short)((value[0] << 8) | value[1]);
        }

        private static int DeserializeInteger(Protocol16Stream input)
        {
            byte[] value = _byteBuffer.Value;
            input.Read(value, 0, 4);
            return (value[0] << 24) | (value[1] << 16) | (value[2] << 8) | value[3];
        }

        private static object DeserializeHashtable(Protocol16Stream input)
        {
            int num = DeserializeShort(input);
            Hashtable hashtable = new Hashtable(num);
            DeserializeDictionaryElements(input, hashtable, num, 0, 0);
            return hashtable;
        }

        private static long DeserializeLong(Protocol16Stream input)
        {
            byte[] value = _byteBuffer.Value;
            input.Read(value, 0, 8);
            if (BitConverter.IsLittleEndian)
            {
                return (long)(((ulong)value[0] << 56) | ((ulong)value[1] << 48) | ((ulong)value[2] << 40) | ((ulong)value[3] << 32) | ((ulong)value[4] << 24) | ((ulong)value[5] << 16) | ((ulong)value[6] << 8) | value[7]);
            }

            return BitConverter.ToInt64(value, 0);
        }

        private static float DeserializeFloat(Protocol16Stream input)
        {
            byte[] value = _byteBuffer.Value;
            input.Read(value, 0, 4);
            if (BitConverter.IsLittleEndian)
            {
                byte b = value[0];
                byte b2 = value[1];
                value[0] = value[3];
                value[1] = value[2];
                value[2] = b2;
                value[3] = b;
            }

            return BitConverter.ToSingle(value, 0);
        }

        private static double DeserializeDouble(Protocol16Stream input)
        {
            byte[] value = _byteBuffer.Value;
            input.Read(value, 0, 8);
            if (BitConverter.IsLittleEndian)
            {
                byte b = value[0];
                byte b2 = value[1];
                byte b3 = value[2];
                byte b4 = value[3];
                value[0] = value[7];
                value[1] = value[6];
                value[2] = value[5];
                value[3] = value[4];
                value[4] = b4;
                value[5] = b3;
                value[6] = b2;
                value[7] = b;
            }

            return BitConverter.ToDouble(value, 0);
        }

        private static string DeserializeString(Protocol16Stream input)
        {
            int num = DeserializeShort(input);
            if (num == 0)
            {
                return string.Empty;
            }

            byte[] array = new byte[num];
            input.Read(array, 0, num);
            return Encoding.UTF8.GetString(array, 0, num);
        }

        private static byte[] DeserializeByteArray(Protocol16Stream input)
        {
            int num = DeserializeInteger(input);
            byte[] array = new byte[num];
            input.Read(array, 0, num);
            return array;
        }

        private static int[] DeserializeIntArray(Protocol16Stream input)
        {
            int num = DeserializeInteger(input);
            int[] array = new int[num];
            for (int i = 0; i < num; i++)
            {
                array[i] = DeserializeInteger(input);
            }

            return array;
        }

        private static string[] DeserializeStringArray(Protocol16Stream input)
        {
            int num = DeserializeShort(input);
            string[] array = new string[num];
            for (int i = 0; i < num; i++)
            {
                array[i] = DeserializeString(input);
            }

            return array;
        }

        private static object[] DeserializeObjectArray(Protocol16Stream input)
        {
            int num = DeserializeShort(input);
            object[] array = new object[num];
            for (int i = 0; i < num; i++)
            {
                byte typeCode = (byte)input.ReadByte();
                array[i] = Deserialize(input, typeCode);
            }

            return array;
        }

        private static IDictionary DeserializeDictionary(Protocol16Stream input)
        {
            byte b = (byte)input.ReadByte();
            byte b2 = (byte)input.ReadByte();
            int dictionarySize = DeserializeShort(input);
            Type typeOfCode = GetTypeOfCode(b);
            Type typeOfCode2 = GetTypeOfCode(b2);
            IDictionary dictionary = Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(typeOfCode, typeOfCode2)) as IDictionary;
            DeserializeDictionaryElements(input, dictionary, dictionarySize, b, b2);
            return dictionary;
        }

        private static void DeserializeDictionaryElements(Protocol16Stream input, IDictionary output, int dictionarySize, byte keyTypeCode, byte valueTypeCode)
        {
            for (int i = 0; i < dictionarySize; i++)
            {
                object key = Deserialize(input, (keyTypeCode == 0 || keyTypeCode == 42) ? ((byte)input.ReadByte()) : keyTypeCode);
                object value = Deserialize(input, (valueTypeCode == 0 || valueTypeCode == 42) ? ((byte)input.ReadByte()) : valueTypeCode);
                output.Add(key, value);
            }
        }

        private static bool DeserializeDictionaryArray(Protocol16Stream input, short size, out Array result)
        {
            byte keyTypeCode;
            byte valueTypeCode;
            Type type = DeserializeDictionaryType(input, out keyTypeCode, out valueTypeCode);
            result = Array.CreateInstance(type, size);
            for (short num = 0; num < size; num++)
            {
                if (!(Activator.CreateInstance(type) is IDictionary dictionary))
                {
                    return false;
                }

                short num2 = DeserializeShort(input);
                for (int i = 0; i < num2; i++)
                {
                    object key;
                    if (keyTypeCode > 0)
                    {
                        key = Deserialize(input, keyTypeCode);
                    }
                    else
                    {
                        byte typeCode = (byte)input.ReadByte();
                        key = Deserialize(input, typeCode);
                    }

                    object value;
                    if (valueTypeCode > 0)
                    {
                        value = Deserialize(input, valueTypeCode);
                    }
                    else
                    {
                        byte typeCode2 = (byte)input.ReadByte();
                        value = Deserialize(input, typeCode2);
                    }

                    dictionary.Add(key, value);
                }

                result.SetValue(dictionary, num);
            }

            return true;
        }

        private static Array DeserializeArray(Protocol16Stream input)
        {
            short num = DeserializeShort(input);
            byte b = (byte)input.ReadByte();
            switch ((Protocol16Type)b)
            {
                case Protocol16Type.Array:
                    {
                        Array array3 = DeserializeArray(input);
                        Array array4 = Array.CreateInstance(array3.GetType(), num);
                        array4.SetValue(array3, 0);
                        for (short num4 = 1; num4 < num; num4++)
                        {
                            array3 = DeserializeArray(input);
                            array4.SetValue(array3, num4);
                        }

                        return array4;
                    }
                case Protocol16Type.ByteArray:
                    {
                        byte[][] array2 = new byte[num][];
                        for (short num3 = 0; num3 < num; num3++)
                        {
                            array2[num3] = DeserializeByteArray(input);
                        }

                        return array2;
                    }
                case Protocol16Type.Dictionary:
                    {
                        DeserializeDictionaryArray(input, num, out var result);
                        return result;
                    }
                default:
                    {
                        Array array = Array.CreateInstance(GetTypeOfCode(b), num);
                        for (short num2 = 0; num2 < num; num2++)
                        {
                            array.SetValue(Deserialize(input, b), num2);
                        }

                        return array;
                    }
            }
        }

        private static Type DeserializeDictionaryType(Protocol16Stream input, out byte keyTypeCode, out byte valueTypeCode)
        {
            keyTypeCode = (byte)input.ReadByte();
            valueTypeCode = (byte)input.ReadByte();
            Type typeOfCode = GetTypeOfCode(keyTypeCode);
            Type typeOfCode2 = GetTypeOfCode(valueTypeCode);
            return typeof(Dictionary<,>).MakeGenericType(typeOfCode, typeOfCode2);
        }

        private static Dictionary<byte, object> DeserializeParameterTable(Protocol16Stream input)
        {
            int num = DeserializeShort(input);
            Dictionary<byte, object> dictionary = new Dictionary<byte, object>(num);
            for (int i = 0; i < num; i++)
            {
                byte key = (byte)input.ReadByte();
                byte typeCode = (byte)input.ReadByte();
                object value = Deserialize(input, typeCode);
                dictionary[key] = value;
            }

            return dictionary;
        }
    }
}
