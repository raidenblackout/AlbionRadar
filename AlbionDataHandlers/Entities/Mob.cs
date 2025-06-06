using System;
using System.Collections;
using System.Collections.Generic;

namespace AlbionDataHandlers.Entities;

public record Mob : IEqualityComparer<Mob>
{
    public int Id = 0;
    public int TypeId = 0;
    public float Experience = 0;
    public string Name;
    public int EnchantmentLevel = 0;
    public int Rarity = 0;
    public float PositionX = 0;
    public float PositionY = 0;
    public float HX = 0;
    public float HY = 0;

    public bool Equals(Mob x, Mob y)
    {
        return x.GetHashCode() == y.GetHashCode();
    }

    public int GetHashCode(Mob obj)
    {
        unchecked
        {
            return (obj.Id * 397) ^ obj.TypeId.GetHashCode() ^ obj.Experience.GetHashCode() ^
                   (obj.Name?.GetHashCode() ?? 0) ^ obj.EnchantmentLevel.GetHashCode() ^
                   obj.Rarity.GetHashCode() ^ obj.PositionX.GetHashCode() ^ obj.PositionY.GetHashCode();
        }
    }
}
