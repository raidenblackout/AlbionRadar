using AlbionDataHandlers;
using AlbionDataHandlers.Entities; // Required for Mob and Harvestable types
using AlbionDataHandlers.Handlers;
using AlbionRadar.Entities;
using AlbionRadar.Managers;
using AlbionRadar.Mappers;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows.Media;

namespace AlbionRadar.ViewModels
{
    /// <summary>  
    /// ViewModel for managing the main radar functionality.  
    /// Uses an efficient diff-and-update strategy and zero-allocation  
    /// data retrieval from the GameStateManager to minimize UI churn and GC pressure.  
    /// </summary>  
    public class MainViewModel : MVVMBase
    {
        private readonly GameStateManager _gameStateManager;
        private readonly Program _mainProgram;

        // Pre-allocated buffers to avoid memory allocations every frame.  
        private readonly List<Mob> _mobBuffer = new();
        private readonly List<Harvestable> _harvestableBuffer = new();

        // Cache for fast lookups of existing entities by their ID.  
        private readonly Dictionary<int, RadarEntity> _entityCache = new();

        // Observable collection for radar entities bound to the UI.  
        public ObservableCollection<RadarEntity> RadarEntities { get; } = new();

        // Main player entity for data binding.  
        private PlayerEntity _mainPlayer = new();
        public PlayerEntity MainPlayer
        {
            get => _mainPlayer;
            private set => SetField(ref _mainPlayer, value);
        }

        public MainViewModel()
        {
            _gameStateManager = new GameStateManager();
            var mobsHandler = new MobsHandler();
            var playersHandler = new PlayersHandler();
            var harvestableHandler = new HarvestableHandler();

            var albionDataParser = new AlbionDataParser();
            _mainProgram = new Program(albionDataParser);

            albionDataParser.RegisterEventHandler(mobsHandler);
            albionDataParser.RegisterEventHandler(playersHandler);
            albionDataParser.RegisterEventHandler(harvestableHandler);

            mobsHandler.Mobs.Subscribe(_gameStateManager.UpdateMobsState);
            playersHandler.Player.Subscribe(_gameStateManager.UpdatePlayerState);
            harvestableHandler.Harvestables.Subscribe(_gameStateManager.UpdateHarvestablesState);

            _mainProgram.Start();

            CompositionTarget.Rendering += OnRenderFrame;
        }

        /// <summary>  
        /// Handles per-frame updates by synchronizing game state with the UI.  
        /// </summary>  
        private void OnRenderFrame(object? sender, EventArgs e)
        {
            _gameStateManager.Update();
            UpdateMainPlayer();
            UpdateRadarEntities();
        }

        /// <summary>  
        /// Updates the main player's position based on the current game state.  
        /// </summary>  
        private void UpdateMainPlayer()
        {
            var playerState = _gameStateManager.GetPlayer();
            if (playerState != null)
            {
                MainPlayer.PositionX = playerState.CurrentLerpedX;
                MainPlayer.PositionY = playerState.CurrentLerpedY;
            }
        }

        /// <summary>  
        /// Updates the radar entities collection by adding, removing,  
        /// and updating items without clearing the whole collection.  
        /// </summary>  
        private void UpdateRadarEntities()
        {
            _gameStateManager.GetMobs(_mobBuffer);
            _gameStateManager.GetHarvestables(_harvestableBuffer);

            var currentUiEntities = _mobBuffer
                .Select(mob => mob.ToRadarEntity())
                .Concat(_harvestableBuffer.Select(h => h.ToRadarEntity()))
                .Where(e => e != null);

            var currentIds = new HashSet<int>();

            foreach (var entity in currentUiEntities)
            {
                currentIds.Add(entity.Id);

                if (_entityCache.TryGetValue(entity.Id, out var existingEntity))
                {
                    existingEntity.PositionX = entity.PositionX;
                    existingEntity.PositionY = entity.PositionY;
                    existingEntity.EnchantmentLevel = entity.EnchantmentLevel;
                }
                else
                {
                    _entityCache[entity.Id] = entity;
                    RadarEntities.Add(entity);
                }
            }

            var idsToRemove = _entityCache.Keys.Where(id => !currentIds.Contains(id)).ToList();
            foreach (var id in idsToRemove)
            {
                if (_entityCache.TryGetValue(id, out var entityToRemove))
                {
                    RadarEntities.Remove(entityToRemove);
                    _entityCache.Remove(id);
                }
            }
        }
    }
}