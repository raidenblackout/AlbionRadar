using AlbionDataHandlers;
using AlbionDataHandlers.Entities;
using AlbionDataHandlers.Handlers;
using AlbionRadar.Entities;
using PhotonParser;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace AlbionRadar.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{

    private MobsHandler _mobsHandler;
    private PlayersHandler _playersHandler;
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

    private PlayerEntity _mainPlayer;
    public PlayerEntity MainPlayer
    {
        get => _mainPlayer;
        set
        {
            _mainPlayer = value;
            OnPropertyChanged();
        }
    }

    public MainViewModel()
    {
        _mobsHandler = new MobsHandler();
        _playersHandler = new PlayersHandler();
        var albionDataParser = new AlbionDataParser();
        _mainProgram = new Program(albionDataParser);
        albionDataParser.RegisterEventHandler(_mobsHandler);
        albionDataParser.RegisterEventHandler(_playersHandler);
        _mainProgram.Start();

        _mobsHandler.Mobs.Subscribe(OnMobsUpdated);
        _playersHandler.Player.Subscribe(OnPlayerUpdated);
    }

    private void OnPlayerUpdated(Player player)
    {
        MainPlayer = new PlayerEntity
        {
            PositionX = player.PositionX,
            PositionY = player.PositionY,
        };
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
        }).ToList();
        Application.Current.Dispatcher.Invoke(() =>
        {
            RadarEntities.Clear();
            foreach (var entity in radarEntities)
            {
                RadarEntities.Add(entity);
            }
        });
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
