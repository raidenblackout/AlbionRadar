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

namespace AlbionRadar.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    // --- The Core Components ---  
    private readonly GameStateManager _gameStateManager;
    private readonly DispatcherTimer _uiUpdateTimer;

    // --- Albion Data Handlers ---  
    private readonly MobsHandler _mobsHandler;
    private readonly PlayersHandler _playersHandler;
    private readonly Program _mainProgram;

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

        var albionDataParser = new AlbionDataParser();
        _mainProgram = new Program(albionDataParser);

        albionDataParser.RegisterEventHandler(_mobsHandler);
        albionDataParser.RegisterEventHandler(_playersHandler);

        _mobsHandler.Mobs.Subscribe(_gameStateManager.UpdateMobsState);
        _playersHandler.Player.Subscribe(_gameStateManager.UpdatePlayerState);

        _mainProgram.Start();

        _uiUpdateTimer = new DispatcherTimer();
        _uiUpdateTimer.Interval = TimeSpan.FromMilliseconds(33);
        _uiUpdateTimer.Tick += OnUiTick;
        _uiUpdateTimer.Start();
    }

    private void OnUiTick(object? sender, EventArgs e)
    {
        var playerState = _gameStateManager.CurrentPlayer;
        if (playerState != null)
        {
            MainPlayer.PositionX = playerState.PositionX;
            MainPlayer.PositionY = playerState.PositionY;

            OnPropertyChanged(nameof(MainPlayer));
        }

        var mobState = _gameStateManager.CurrentMobs;

        var newUiEntities = mobState.Select(mob => new RadarEntity
        {
            Id = mob.Id,
            Name = mob.Name,
            PositionX = mob.PositionX,
            PositionY = mob.PositionY,
            TypeId = mob.TypeId,
        });

        RadarEntities = new ObservableCollection<RadarEntity>(newUiEntities);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
