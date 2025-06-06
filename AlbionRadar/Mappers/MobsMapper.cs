using AlbionRadar.Entities;

namespace AlbionRadar.Mappers;

public static class MobsMapper
{
    private static float Lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }
}
