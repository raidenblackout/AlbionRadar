using AlbionDataHandlers.Entities;
using AlbionRadar.Entities;

namespace AlbionRadar.Mappers;

public static class MobsMapper
{
    public static RadarEntity? ToRadarEntity(this Mob mob)
    {
        if (mob == null) return null;
        var entity = new RadarEntity
        {
            Id = mob.Id,
            TypeId = mob.TypeId,
            Name = mob.Name,
            PositionX = mob.CurrentLerpedX,
            PositionY = mob.CurrentLerpedY,
            ImageUrl = GetImageUrl(mob),
            EnchantmentLevel = mob.EnchantmentLevel,
            Type = AlbionDataHandlers.Enums.EntityTypes.Mob,
        };
        return entity;
    }

    private static string? GetImageUrl(Mob mob)
    {
        if (mob.Type == AlbionDataHandlers.Enums.MobTypes.LivingSkinnable || mob.Type == AlbionDataHandlers.Enums.MobTypes.LivingHarvestable)
        {
            return $"Resources/{mob.Name}_{(int)mob.Tier}_{mob.EnchantmentLevel}.png";
        }
        return null;
    }
}
