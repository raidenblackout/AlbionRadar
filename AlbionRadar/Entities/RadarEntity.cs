using System.ComponentModel;

namespace AlbionRadar.Entities;

public class RadarEntity : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
}
