using AlbionDataHandlers.Enums;
using AlbionRadar.ViewModels;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace AlbionRadar.Entities;

/// <summary>  
/// Represents an entity in the radar system with properties such as position, type, and other metadata.  
/// Implements INotifyPropertyChanged for data binding and IEqualityComparer for comparison.  
/// </summary>  
public class RadarEntity : MVVMBase, IEqualityComparer<RadarEntity>
{
    /// <summary>  
    /// Compares two RadarEntity objects for equality based on their hash codes.  
    /// </summary>  
    public bool Equals(RadarEntity? x, RadarEntity? y)
    {
        return x?.GetHashCode() == y?.GetHashCode();
    }

    /// <summary>  
    /// Generates a hash code for the RadarEntity object based on its properties.  
    /// </summary>  
    public int GetHashCode([DisallowNull] RadarEntity obj)
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + Id.GetHashCode();
            hash = hash * 23 + TypeId.GetHashCode();
            hash = hash * 23 + PositionX.GetHashCode();
            hash = hash * 23 + PositionY.GetHashCode();
            hash = hash * 23 + (Name?.GetHashCode() ?? 0);
            return hash;
        }
    }

    // Backing fields for properties.  
    private string _name;
    private float _positionX;
    private float _positionY;
    private int _id;
    private int _typeId;
    private string? _imageUrl = string.Empty;
    private int _enchantmentLevel = 0;
    private EntityTypes _type = EntityTypes.Player;

    /// <summary>  
    /// Gets or sets the name of the entity.  
    /// </summary>  
    public string Name
    {
        get => _name;
        set => SetField(ref _name, value);
    }

    /// <summary>  
    /// Gets or sets the X-coordinate of the entity's position.  
    /// </summary>  
    public float PositionX
    {
        get => _positionX;
        set => SetField(ref _positionX, value);
    }

    /// <summary>  
    /// Gets or sets the Y-coordinate of the entity's position.  
    /// </summary>  
    public float PositionY
    {
        get => _positionY;
        set => SetField(ref _positionY, value);
    }

    /// <summary>  
    /// Gets or sets the unique identifier of the entity.  
    /// </summary>  
    public int Id
    {
        get => _id;
        set => SetField(ref _id, value);
    }

    /// <summary>  
    /// Gets or sets the type identifier of the entity.  
    /// </summary>  
    public int TypeId
    {
        get => _typeId;
        set => SetField(ref _typeId, value);
    }

    /// <summary>  
    /// Gets or sets the image URL associated with the entity.  
    /// </summary>  
    public string? ImageUrl
    {
        get => _imageUrl;
        set => SetField(ref _imageUrl, value);
    }

    /// <summary>  
    /// Gets or sets the enchantment level of the entity.  
    /// </summary>  
    public int EnchantmentLevel
    {
        get => _enchantmentLevel;
        set => SetField(ref _enchantmentLevel, value);
    }

    /// <summary>  
    /// Gets or sets the type of the entity.  
    /// </summary>  
    public EntityTypes Type
    {
        get => _type;
        set => SetField(ref _type, value);
    }
}
