using AlbionDataHandlers;
using AlbionDataHandlers.Entities;
using AlbionDataHandlers.Handlers;
using AlbionRadar.Entities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System;
using AlbionRadar.Managers;
using System.Windows.Threading;
using AlbionDataHandlers.Mappers;
using AlbionRadar.Mappers;

namespace AlbionRadar.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    // --- The Core Components ---  
    private readonly GameStateManager _gameStateManager;

    // --- Albion Data Handlers ---  
    private readonly MobsHandler _mobsHandler;
    private readonly PlayersHandler _playersHandler;
    private readonly HarvestableHandler _harvestableHandler;
    private readonly Program _mainProgram;
    private readonly DispatcherTimer uiUpdateTimer;

    // --- UI-Bound Properties ---  
    private ObservableCollection<RadarEntity> _radarEntities = new ObservableCollection<RadarEntity>();
    public ObservableCollection<RadarEntity> RadarEntities
    {
        get => _radarEntities;
        set { _radarEntities = value; OnPropertyChanged(); }
    }

    private PlayerEntity _mainPlayer = new PlayerEntity(); // Initialize to avoid nullability issues  
    public PlayerEntity MainPlayer
    {
        get => _mainPlayer;
        set { _mainPlayer = value; OnPropertyChanged(); }
    }

    public MainViewModel()
    {
        _gameStateManager = new GameStateManager();

        _mobsHandler = new MobsHandler();
        _playersHandler = new PlayersHandler();
        _harvestableHandler = new HarvestableHandler();

        var albionDataParser = new AlbionDataParser();
        _mainProgram = new Program(albionDataParser);

        albionDataParser.RegisterEventHandler(_mobsHandler);
        albionDataParser.RegisterEventHandler(_playersHandler);
        albionDataParser.RegisterEventHandler(_harvestableHandler);

        _mobsHandler.Mobs.Subscribe(_gameStateManager.UpdateMobsState);
        _playersHandler.Player.Subscribe(_gameStateManager.UpdatePlayerState);
        _harvestableHandler.Harvestables.Subscribe(_gameStateManager.UpdateHarvestablesState);

        _mainProgram.Start();

        uiUpdateTimer = new DispatcherTimer();
        uiUpdateTimer.Tick += OnUiTick;
        uiUpdateTimer.Interval = TimeSpan.FromMilliseconds(33);
        uiUpdateTimer.Start();
    }

    private readonly object _uiLock = new object();
    private void OnUiTick(object? sender, EventArgs e)
    {
        lock (_uiLock)
        {
            _gameStateManager.Update();

            var playerState = _gameStateManager.CurrentPlayer;
            if (playerState != null)
            {
                MainPlayer.PositionX = playerState.CurrentLerpedX;
                MainPlayer.PositionY = playerState.CurrentLerpedY;

                OnPropertyChanged(nameof(MainPlayer));
            }

            var mobState = _gameStateManager.CurrentMobs;

            var newUiEntities = mobState.Select(mob => mob.ToRadarEntity()).OfType<RadarEntity>();

            var harvestableState = _gameStateManager.CurrentHarvestables;
            newUiEntities = newUiEntities.Concat(harvestableState.Select(harvestable => harvestable.ToRadarEntity()).OfType<RadarEntity>());

            RadarEntities = new ObservableCollection<RadarEntity>(newUiEntities);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
