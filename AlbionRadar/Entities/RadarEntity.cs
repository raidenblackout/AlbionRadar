using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace AlbionRadar.Entities;

public class RadarEntity : INotifyPropertyChanged, IEqualityComparer<RadarEntity>
{
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public bool Equals(RadarEntity? x, RadarEntity? y)
    {
        return x.GetHashCode() == y.GetHashCode();
    }

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

    private string _name;
    public string Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged();
            }
        }
    }

    private float _positionX;
    public float PositionX
    {
        get => _positionX;
        set
        {
            if (_positionX != value)
            {
                _positionX = value;
                OnPropertyChanged();
            }
        }
    }

    private float _positionY;
    public float PositionY
    {
        get => _positionY;
        set
        {
            if (_positionY != value)
            {
                _positionY = value;
                OnPropertyChanged();
            }
        }
    }

    private int _id;
    public int Id
    {
        get => _id;
        set
        {
            if (_id != value)
            {
                _id = value;
                OnPropertyChanged();
            }
        }
    }

    private int _typeId;
    public int TypeId
    {
        get => _typeId;
        set
        {
            if (_typeId != value)
            {
                _typeId = value;
                OnPropertyChanged();
            }
        }
    }

    private string? _imageUrl = string.Empty;
    public string? ImageUrl
    {
        get => _imageUrl;
        set
        {
            if (_imageUrl != value)
            {
                _imageUrl = value;
                OnPropertyChanged();
            }
        }
    }
}
