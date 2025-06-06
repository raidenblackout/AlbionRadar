using System.Collections;
using System.Collections.Generic;

namespace AlbionDataHandlers.Entities;

public record Player : IEqualityComparer<Player>
{   public int Id = 0;
    public float PositionX = 0;
    public float PositionY = 0;
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
