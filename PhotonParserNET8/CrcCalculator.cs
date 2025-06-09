namespace PhotonParser;

public static class CrcCalculator
{
    public static uint Calculate(byte[] bytes, int length)
    {
        uint num = uint.MaxValue;
        uint num2 = 3988292384u;
        for (int i = 0; i < length; i++)
        {
            num ^= bytes[i];
            for (int j = 0; j < 8; j++)
            {
                num = (((num & 1) == 0) ? (num >> 1) : ((num >> 1) ^ num2));
            }
        }

        return num;
    }
}