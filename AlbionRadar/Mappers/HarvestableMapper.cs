using AlbionDataHandlers.Entities;
using AlbionDataHandlers.Enums;
using AlbionRadar.Entities;

namespace AlbionRadar.Mappers;

public static class HarvestableMapper
{
    public static RadarEntity? ToRadarEntity(this Harvestable harvestable)
    {
        if (harvestable == null) return null;
        var entity = new RadarEntity
        {
            Id = harvestable.Id,
            TypeId = harvestable.Size,
            Name = "",
            PositionX = harvestable.CurrentLerpedX,
            PositionY = harvestable.CurrentLerpedY,
            ImageUrl = GetImageUrl(harvestable),
            EnchantmentLevel = harvestable.EnchantmentLevel,
            Type = EntityTypes.Harvestable,
        };

        return entity;
    }

    private static string? GetImageUrl(Harvestable harvestable)
    {
        string? prefix = null;
        if (harvestable.Type >= 0 && harvestable.Type <= 5) prefix = "Logs_";
        else if (harvestable.Type >= 6 && harvestable.Type <= 10) prefix = "rock_";
        else if (harvestable.Type >= 11 && harvestable.Type <= 15) prefix = "fiber_";
        else if (harvestable.Type >= 16 && harvestable.Type <= 22) prefix = "hide_";
        else if (harvestable.Type >= 23 && harvestable.Type <= 27) prefix = "ore_";

        if (prefix is null) return null;

        return $"Resources/{prefix}{harvestable.Tier}_{harvestable.EnchantmentLevel}.png";
    }
}
