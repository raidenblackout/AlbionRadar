using AlbionDataHandlers.Entities;
using AlbionDataHandlers.Enums;
using AlbionRadar.Entities;

namespace AlbionRadar.Mappers;

/// <summary>  
/// Provides mapping functionality to convert Harvestable objects into RadarEntity objects.  
/// </summary>  
public static class HarvestableMapper
{
    /// <summary>  
    /// Converts a Harvestable object to a RadarEntity object.  
    /// </summary>  
    /// <param name="harvestable">The Harvestable object to convert.</param>  
    /// <returns>A RadarEntity object or null if the input is null.</returns>  
    public static RadarEntity? ToRadarEntity(this Harvestable harvestable)
    {
        if (harvestable == null) return null;

        // Create and populate a RadarEntity object based on the Harvestable object.  
        var entity = new RadarEntity
        {
            Id = harvestable.Id,
            TypeId = harvestable.Size,
            Name = string.Empty, // Name is intentionally left empty.  
            PositionX = harvestable.CurrentLerpedX,
            PositionY = harvestable.CurrentLerpedY,
            ImageUrl = GetImageUrl(harvestable),
            EnchantmentLevel = harvestable.EnchantmentLevel,
            Type = EntityTypes.Harvestable,
        };

        return entity;
    }

    /// <summary>  
    /// Generates the image URL for a given Harvestable object based on its type, tier, and enchantment level.  
    /// </summary>  
    /// <param name="harvestable">The Harvestable object for which to generate the image URL.</param>  
    /// <returns>A string representing the image URL, or null if the type is invalid.</returns>  
    private static string? GetImageUrl(Harvestable harvestable)
    {
        string? prefix = harvestable.Type switch
        {
            >= 0 and <= 5 => "Logs_",
            >= 6 and <= 10 => "rock_",
            >= 11 and <= 15 => "fiber_",
            >= 16 and <= 22 => "hide_",
            >= 23 and <= 27 => "ore_",
            _ => null // Return null if the type does not match any known category.  
        };

        // If no valid prefix is found, return null.  
        if (prefix is null) return null;

        // Construct and return the image URL.  
        return $"Resources/{prefix}{harvestable.Tier}_{harvestable.EnchantmentLevel}.png";
    }
}
