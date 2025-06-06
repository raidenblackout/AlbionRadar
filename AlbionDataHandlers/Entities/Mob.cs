using AlbionDataHandlers.Enums;
using AlbionDataHandlers.Mappers;
using System.Collections.Generic;

namespace AlbionDataHandlers.Entities;

public class Mob : InterpolatableEntity, IEqualityComparer<Mob>
{
    public int Id = 0;
    public int TypeId = 0;
    public float Experience = 0;
    private string _name = string.Empty;
    public string Name
    {
        get => string.IsNullOrEmpty(_name) ? MobMapper.Instance.GetMobInfo(TypeId)?.Name ?? _name : _name;
        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                _name = value;
            }
        }
    }

    public int EnchantmentLevel = 0;
    public int Rarity = 0;
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

    public MobTypes Type => MobMapper.Instance.GetMobInfo(TypeId)?.Type ?? MobTypes.Enemy;
    public TierLevels Tier => MobMapper.Instance.GetMobInfo(TypeId)?.Tier ?? TierLevels.Tier1;

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
                   obj.Rarity.GetHashCode() ^ obj.CurrentLerpedX.GetHashCode() ^ obj.CurrentLerpedY.GetHashCode();
        }
    }
}
