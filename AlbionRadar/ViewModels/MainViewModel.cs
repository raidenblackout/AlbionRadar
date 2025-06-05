using AlbionDataHandlers;
using AlbionDataHandlers.Entities;
using AlbionDataHandlers.Handlers;
using AlbionRadar.Entities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AlbionRadar.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{

    private MobsHandler _mobsHandler;
    private Program _mainProgram;

    private ObservableCollection<RadarEntity> _radarEntities = new ObservableCollection<RadarEntity>();
    public ObservableCollection<RadarEntity> RadarEntities
    {
        get => _radarEntities;
        set
        {
            _radarEntities = value;
            OnPropertyChanged();
        }
    }

    public MainViewModel()
    {
        _mobsHandler = new MobsHandler();
        var albionDataParser = new AlbionDataParser();
        _mainProgram = new Program(albionDataParser);
        albionDataParser.RegisterEventHandler(_mobsHandler);
        _mainProgram.Start();

        _mobsHandler.Mobs.Subscribe(OnMobsUpdated);
    }

    private void OnMobsUpdated(IEnumerable<Mob> enumerable)
    {
        var radarEntities = enumerable.Select(mob => new RadarEntity
        {
            Id = mob.Id,
            Name = mob.Name,
            PositionX = mob.PositionX,
            PositionY = mob.PositionY,
            TypeId = mob.TypeId,
        });
        RadarEntities = new ObservableCollection<RadarEntity>(radarEntities);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
