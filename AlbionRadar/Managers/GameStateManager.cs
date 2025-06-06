using AlbionDataHandlers.Entities;
using BaseUtils.Logger.Impl;
using System.Diagnostics;

namespace AlbionRadar.Managers;

/// <summary>  
/// Manages the game state, including player, mobs, and harvestables.  
/// Handles updates, interpolation, and synchronization of game entities.  
/// </summary>  
public class GameStateManager
{
    private readonly object _stateLock = new();
    private Player _currentPlayer;
    private readonly Dictionary<int, Mob> _mobs = new();
    private readonly Dictionary<int, Harvestable> _harvestables = new();

    // --- Rendering & Timing ---  
    private readonly Stopwatch _stopwatch = new();
    private long _previousTimeTicks = 0;
    private float _flashTime = -1.0f;

    public float InterpolationFactor { get; private set; }
    public float FlashTime => _flashTime;

    /// <summary>  
    /// Gets the current player.  
    /// </summary>  
    public Player CurrentPlayer
    {
        get
        {
            lock (_stateLock)
            {
                return _currentPlayer;
            }
        }
    }

    /// <summary>  
    /// Gets the list of current mobs.  
    /// </summary>  
    public List<Mob> CurrentMobs
    {
        get
        {
            lock (_stateLock)
            {
                return _mobs.Values.ToList();
            }
        }
    }

    /// <summary>  
    /// Gets the list of current harvestables.  
    /// </summary>  
    public List<Harvestable> CurrentHarvestables
    {
        get
        {
            lock (_stateLock)
            {
                return _harvestables.Values.ToList();
            }
        }
    }

    /// <summary>  
    /// Initializes a new instance of the <see cref="GameStateManager"/> class.  
    /// Starts the stopwatch for timing calculations.  
    /// </summary>  
    public GameStateManager()
    {
        _stopwatch.Start();
    }

    /// <summary>  
    /// Updates the player's state.  
    /// </summary>  
    /// <param name="player">The updated player object.</param>  
    public void UpdatePlayerState(Player player)
    {
        lock (_stateLock)
        {
            if (_currentPlayer == null)
            {
                _currentPlayer = player;
            }
            else
            {
                _currentPlayer.PositionX = player.PositionX;
                _currentPlayer.PositionY = player.PositionY;
            }
        }
    }

    /// <summary>  
    /// Updates the state of mobs.  
    /// </summary>  
    /// <param name="mobs">The collection of updated mobs.</param>  
    public void UpdateMobsState(IEnumerable<Mob> mobs)
    {
        lock (_stateLock)
        {
            try
            {
                foreach (var mob in mobs)
                {
                    if (_mobs.ContainsKey(mob.Id))
                    {
                        // Update existing mob  
                        var existingMob = _mobs[mob.Id];
                        existingMob.PositionX = mob.PositionX;
                        existingMob.PositionY = mob.PositionY;
                        existingMob.Experience = mob.Experience;
                        existingMob.EnchantmentLevel = mob.EnchantmentLevel;
                        existingMob.Rarity = mob.Rarity;
                    }
                    else
                    {
                        // Add new mob  
                        _mobs[mob.Id] = mob;
                    }
                }

                // Remove mobs that are no longer present  
                var idsToRemove = _mobs.Keys.Except(mobs.Select(m => m.Id)).ToList();
                foreach (var id in idsToRemove)
                {
                    _mobs.Remove(id);
                }
            }
            catch (Exception ex)
            {
                DLog.I(ex.Message);
            }
        }
    }

    /// <summary>  
    /// Updates the state of harvestables.  
    /// </summary>  
    /// <param name="harvestables">The collection of updated harvestables.</param>  
    public void UpdateHarvestablesState(IEnumerable<Harvestable> harvestables)
    {
        lock (_stateLock)
        {
            try
            {
                foreach (var harvestable in harvestables)
                {
                    if (_harvestables.ContainsKey(harvestable.Id))
                    {
                        // Update existing harvestable  
                        var existingHarvestable = _harvestables[harvestable.Id];
                        existingHarvestable.PositionX = harvestable.PositionX;
                        existingHarvestable.PositionY = harvestable.PositionY;
                        existingHarvestable.EnchantmentLevel = harvestable.EnchantmentLevel;
                        existingHarvestable.Size = harvestable.Size;
                    }
                    else
                    {
                        // Add new harvestable  
                        _harvestables[harvestable.Id] = harvestable;
                    }
                }

                // Remove harvestables that are no longer present  
                var idsToRemove = _harvestables.Keys.Except(harvestables.Select(h => h.Id)).ToList();
                foreach (var id in idsToRemove)
                {
                    _harvestables.Remove(id);
                }
            }
            catch (Exception ex)
            {
                DLog.I(ex.Message);
            }
        }
    }

    /// <summary>  
    /// Updates the game state, including interpolation and timing.  
    /// </summary>  
    public void Update()
    {
        long currentTimeTicks = _stopwatch.ElapsedTicks;
        long deltaTimeTicks = currentTimeTicks - _previousTimeTicks;
        double deltaTimeMs = (double)deltaTimeTicks / Stopwatch.Frequency * 1000.0;
        InterpolationFactor = Math.Min(1.0f, (float)deltaTimeMs / 100.0f);

        lock (_stateLock)
        {
            _currentPlayer?.Interpolate(InterpolationFactor);
            foreach (var mob in _mobs.Values)
            {
                mob.Interpolate(InterpolationFactor);
            }

            foreach (var harvestable in _harvestables.Values)
            {
                harvestable.Interpolate(InterpolationFactor);
            }
        }

        _previousTimeTicks = currentTimeTicks;

        // Handle flash time  
        if (_flashTime >= 0.0f)
        {
            _flashTime -= InterpolationFactor;
            if (_flashTime < 0.0f)
            {
                _flashTime = -1.0f;
            }
        }

        DLog.I($"GameStateManager: Update called. InterpolationFactor: {InterpolationFactor}, FlashTime: {_flashTime}");

        if (_flashTime < 0.0f)
        {
            _flashTime = 0.0f; // Reset flash time if it was negative  
        }
        else if (_flashTime > 0.0f)
        {
            _flashTime = Math.Max(0.0f, _flashTime - InterpolationFactor); // Ensure flash time does not go negative  
        }

        DLog.I($"GameStateManager: Update completed. Current Player Position: ({_currentPlayer?.PositionX}, {_currentPlayer?.PositionY}), Mobs Count: {_mobs.Count}");
    }
}
