using System.Collections;
using System.Collections.Generic;

namespace AlbionDataHandlers.Entities;

public class Player : InterpolatableEntity, IEqualityComparer<Player>
{   public int Id = 0;
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

    public bool Equals(Player x, Player y)
    {
        return x.Id == y.Id;
    }

    public int GetHashCode(Player obj)
    {
        unchecked
        {
            return obj.Id.GetHashCode() * 397 ^ obj.PositionX.GetHashCode() * 397 ^ obj.PositionY.GetHashCode();
        }
    }
}
