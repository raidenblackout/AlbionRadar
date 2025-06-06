using AlbionDataHandlers;
using AlbionDataHandlers.Handlers;
using AlbionRadar.Entities;
using AlbionRadar.Managers;
using AlbionRadar.Mappers;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows.Threading;

namespace AlbionRadar.ViewModels;

/// <summary>  
/// ViewModel for managing the main radar functionality in the application.  
/// Handles game state updates, UI updates, and data binding for radar entities and the main player.  
/// </summary>  
public class MainViewModel : MVVMBase
{
    // Dependencies and managers  
    private readonly GameStateManager _gameStateManager;
    private readonly MobsHandler _mobsHandler;
    private readonly PlayersHandler _playersHandler;
    private readonly HarvestableHandler _harvestableHandler;
    private readonly Program _mainProgram;
    private readonly DispatcherTimer _uiUpdateTimer;

    // Observable collection for radar entities displayed in the UI  
    private ObservableCollection<RadarEntity> _radarEntities = new ObservableCollection<RadarEntity>();
    public ObservableCollection<RadarEntity> RadarEntities
    {
        get => _radarEntities;
        set => SetField(ref _radarEntities, value);
    }

    // Main player entity for data binding  
    private PlayerEntity _mainPlayer = new PlayerEntity(); // Initialize to avoid nullability issues  
    public PlayerEntity MainPlayer
    {
        get => _mainPlayer;
        set => SetField(ref _mainPlayer, value);
    }

    // Lock object for thread safety during UI updates  
    private readonly object _uiLock = new object();

    /// <summary>  
    /// Constructor initializes dependencies, sets up event handlers, and starts the main program and UI update timer.  
    /// </summary>  
    public MainViewModel()
    {
        // Initialize game state manager  
        _gameStateManager = new GameStateManager();

        // Initialize handlers for mobs, players, and harvestables  
        _mobsHandler = new MobsHandler();
        _playersHandler = new PlayersHandler();
        _harvestableHandler = new HarvestableHandler();

        // Initialize Albion data parser and program  
        var albionDataParser = new AlbionDataParser();
        _mainProgram = new Program(albionDataParser);

        // Register event handlers with the data parser  
        albionDataParser.RegisterEventHandler(_mobsHandler);
        albionDataParser.RegisterEventHandler(_playersHandler);
        albionDataParser.RegisterEventHandler(_harvestableHandler);

        // Subscribe to handler events to update game state  
        _mobsHandler.Mobs.Subscribe(_gameStateManager.UpdateMobsState);
        _playersHandler.Player.Subscribe(_gameStateManager.UpdatePlayerState);
        _harvestableHandler.Harvestables.Subscribe(_gameStateManager.UpdateHarvestablesState);

        // Start the main program  
        _mainProgram.Start();

        // Set up and start the UI update timer  
        _uiUpdateTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(33) // Approximately 30 FPS  
        };
        _uiUpdateTimer.Tick += OnUiTick;
        _uiUpdateTimer.Start();
    }

    /// <summary>  
    /// Handles periodic UI updates by synchronizing game state with the UI.  
    /// </summary>  
    private void OnUiTick(object? sender, EventArgs e)
    {
        lock (_uiLock)
        {
            // Update game state  
            _gameStateManager.Update();

            // Update main player and radar entities in the UI  
            UpdateMainPlayer();
            UpdateRadarEntities();
        }
    }

    /// <summary>  
    /// Updates the main player's position based on the current game state.  
    /// </summary>  
    private void UpdateMainPlayer()
    {
        var playerState = _gameStateManager.CurrentPlayer;
        if (playerState != null)
        {
            MainPlayer.PositionX = playerState.CurrentLerpedX;
            MainPlayer.PositionY = playerState.CurrentLerpedY;

            // Notify UI of changes to the MainPlayer property  
            OnPropertyChanged(nameof(MainPlayer));
        }
    }

    /// <summary>  
    /// Updates the radar entities displayed in the UI based on the current game state.  
    /// </summary>  
    private void UpdateRadarEntities()
    {
        // Get the current mob state and map to radar entities  
        var mobState = _gameStateManager.CurrentMobs;
        var newUiEntities = mobState
            .Select(mob => mob.ToRadarEntity())
            .OfType<RadarEntity>();

        // Get the current harvestable state and map to radar entities  
        var harvestableState = _gameStateManager.CurrentHarvestables;
        newUiEntities = newUiEntities.Concat(
            harvestableState
                .Select(harvestable => harvestable.ToRadarEntity())
                .OfType<RadarEntity>()
        );

        // Update the RadarEntities collection with the new entities  
        RadarEntities = new ObservableCollection<RadarEntity>(newUiEntities);
    }
}
