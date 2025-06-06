using AlbionDataHandlers.Entities;
using BaseUtils.Logger.Impl;
using System.Diagnostics;

namespace AlbionRadar.Managers;

public class GameStateManager
{
    private readonly object _stateLock = new object();
    private Player _currentPlayer;
    private Dictionary<int,Mob> _mobs = new Dictionary<int,Mob>();
    private Dictionary<int, Harvestable> _harvestables= new Dictionary<int, Harvestable>();

    // --- Rendering & Timing ---
    private readonly Stopwatch _stopwatch = new();
    private long _previousTimeTicks = 0;
    private float _flashTime = -1.0f;

    public float InterpolationFactor { get; private set; }
    public float FlashTime => _flashTime;

    public Player CurrentPlayer
    {
        get { lock (_stateLock) { return _currentPlayer; } }
    }

    public List<Mob> CurrentMobs
    {
        get { lock (_stateLock) { return _mobs.Values.ToList(); } }
    }

    public List<Harvestable> CurrentHarvestables
    {
        get { lock (_stateLock) { return _harvestables.Values.ToList(); } }
    }

    public GameStateManager()
    {
        _stopwatch.Start();
    }

    public void UpdatePlayerState(Player player)
    {
        lock (_stateLock)
        {
            if(_currentPlayer == null)
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

    public void UpdateMobsState(IEnumerable<Mob> mobs)
    {
        lock (_stateLock)
        {
            try
            {
                foreach(var mob in mobs)
                {
                    if (_mobs.ContainsKey(mob.Id))
                    {
                        // Update existing mob
                        _mobs[mob.Id].PositionX = mob.PositionX;
                        _mobs[mob.Id].PositionY = mob.PositionY;
                        _mobs[mob.Id].Experience = mob.Experience;
                        _mobs[mob.Id].EnchantmentLevel = mob.EnchantmentLevel;
                        _mobs[mob.Id].Rarity = mob.Rarity;
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
                        _harvestables[harvestable.Id].PositionX = harvestable.PositionX;
                        _harvestables[harvestable.Id].PositionY = harvestable.PositionY;
                        _harvestables[harvestable.Id].EnchantmentLevel = harvestable.EnchantmentLevel;
                        _harvestables[harvestable.Id].Size = harvestable.Size;
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
