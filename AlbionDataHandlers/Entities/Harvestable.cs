using System;
using System.Collections.Generic;

namespace AlbionDataHandlers.Entities;

public class Harvestable : InterpolatableEntity, IEqualityComparer<Harvestable>
{
    public int Id = 0;
    public int Type = 0;
    public int Tier = 0;
    public int EnchantmentLevel = 0;
    public int Size = 0;
    public float PositionX
    {
        set
        {
            FromX = ToX;
            ToX = value;
        }
        get => ToX;
    }

    public float PositionY
    {
        set
        {
            FromY = ToY;
            ToY = value;
        }
        get => ToY;
    }

    public bool Equals(Harvestable x, Harvestable y)
    {
        return x.GetHashCode() == y.GetHashCode();
    }

    public int GetHashCode(Harvestable obj)
    {
        unchecked
        {
            return (obj.Id * 397) ^ obj.Type.GetHashCode() ^ obj.Tier.GetHashCode() ^
                   obj.EnchantmentLevel.GetHashCode() ^ obj.Size.GetHashCode() ^
                   obj.PositionX.GetHashCode() ^ obj.PositionY.GetHashCode();
        }
    }
}
