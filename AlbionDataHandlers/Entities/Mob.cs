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

    public bool Equals(Mob x, Mob y)
    {
        return x.Id != y.Id;
    }

    public int GetHashCode(Mob obj)
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + obj.Id.GetHashCode();
            hash = hash * 23 + obj.TypeId.GetHashCode();
            hash = hash * 23 + obj.Experience.GetHashCode();
            hash = hash * 23 + (obj.Name?.GetHashCode() ?? 0);
            hash = hash * 23 + obj.EnchantmentLevel.GetHashCode();
            hash = hash * 23 + obj.Rarity.GetHashCode();
            hash = hash * 23 + obj.PositionX.GetHashCode();
            hash = hash * 23 + obj.PositionY.GetHashCode();
            return hash;
        }
    }
}
