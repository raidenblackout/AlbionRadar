namespace AlbionRadar.Mappers;

public static class AlbionMapMapper
{
    public static Tuple<float, float> RotateWithCenter(float x, float y, float centerX, float centerY, float angle)
    {
        float radians = angle * (MathF.PI / 180f);
        float cos = MathF.Cos(radians);
        float sin = MathF.Sin(radians);
        float translatedX = x - centerX;
        float translatedY = y - centerY;
        return new Tuple<float, float>(
            translatedX * cos - translatedY * sin + centerX,
            translatedX * sin + translatedY * cos + centerY
        );
    }
}
