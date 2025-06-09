namespace PhotonParser;

public class NumberDeserializer
{
    public static void Deserialize(out int value, byte[] source, ref int offset)
    {
        int num = source[offset] << 24;
        offset++;
        int num2 = num | (source[offset] << 16);
        offset++;
        int num3 = num2 | (source[offset] << 8);
        offset++;
        value = num3 | source[offset];
        offset++;
    }

    public static void Deserialize(out short value, byte[] source, ref int offset)
    {
        short num = (short)(source[offset] << 8);
        offset++;
        value = (short)(num | source[offset]);
        offset++;
    }
}