using AlbionDataHandlers.Entities;
using BaseUtils.Logger.Impl;
using System.Diagnostics;

namespace AlbionRadar.Managers
{
    /// <summary>  
    /// Manages the game state by holding entity data and processing per-frame updates.  
    /// Optimized to minimize memory allocations and ensure smooth interpolation.  
    /// </summary>  
    public class GameStateManager
    {
        private readonly object _stateLock = new();
        private Player? _currentPlayer;
        private readonly Dictionary<int, Mob> _mobs = new();
        private readonly Dictionary<int, Harvestable> _harvestables = new();

        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        private long _previousTimeTicks;
        private float _flashTimeRemaining;

        public float FlashTimeNormalized => _flashTimeRemaining > 0.0f ? _flashTimeRemaining : 0.0f;

        public GameStateManager()
        {
            _previousTimeTicks = _stopwatch.ElapsedTicks;
        }

        #region State Update Methods  

        /// <summary>  
        /// Updates the player's state using network data.  
        /// </summary>  
        public void UpdatePlayerState(Player player)
        {
            lock (_stateLock)
            {
                _currentPlayer ??= player;
                _currentPlayer.SetTargetPosition(player.PositionX, player.PositionY);
            }
        }

        /// <summary>  
        /// Updates the state of all mobs using network data.  
        /// </summary>  
        public void UpdateMobsState(IEnumerable<Mob> incomingMobs)
        {
            lock (_stateLock)
            {
                try
                {
                    var incomingIds = new HashSet<int>(incomingMobs.Select(m => m.Id));
                    var idsToRemove = _mobs.Keys.Where(id => !incomingIds.Contains(id)).ToList();

                    foreach (var id in idsToRemove)
                    {
                        _mobs.Remove(id);
                    }

                    foreach (var mob in incomingMobs)
                    {
                        if (_mobs.TryGetValue(mob.Id, out var existingMob))
                        {
                            existingMob.SetTargetPosition(mob.PositionX, mob.PositionY);
                            existingMob.Experience = mob.Experience;
                            existingMob.EnchantmentLevel = mob.EnchantmentLevel;
                            existingMob.Rarity = mob.Rarity;
                        }
                        else
                        {
                            mob.SetTargetPosition(mob.PositionX, mob.PositionY);
                            _mobs[mob.Id] = mob;
                        }
                    }
                }
                catch (Exception ex)
                {
                    DLog.E($"Error updating mobs state {ex}");
                }
            }
        }

        /// <summary>  
        /// Updates the state of all harvestables using network data.  
        /// </summary>  
        public void UpdateHarvestablesState(IEnumerable<Harvestable> incomingHarvestables)
        {
            lock (_stateLock)
            {
                try
                {
                    var incomingIds = new HashSet<int>(incomingHarvestables.Select(h => h.Id));
                    var idsToRemove = _harvestables.Keys.Where(id => !incomingIds.Contains(id)).ToList();

                    foreach (var id in idsToRemove)
                    {
                        _harvestables.Remove(id);
                    }

                    foreach (var harvestable in incomingHarvestables)
                    {
                        if (_harvestables.TryGetValue(harvestable.Id, out var existingHarvestable))
                        {
                            existingHarvestable.SetTargetPosition(harvestable.PositionX, harvestable.PositionY);
                            existingHarvestable.EnchantmentLevel = harvestable.EnchantmentLevel;
                            existingHarvestable.Size = harvestable.Size;
                        }
                        else
                        {
                            harvestable.SetTargetPosition(harvestable.PositionX, harvestable.PositionY);
                            _harvestables[harvestable.Id] = harvestable;
                        }
                    }
                }
                catch (Exception ex)
                {
                    DLog.E($"Error updating harvestables state {ex}");
                }
            }
        }

        #endregion

        #region State Accessor Methods  

        public Player? GetPlayer()
        {
            lock (_stateLock) { return _currentPlayer; }
        }

        public void GetMobs(List<Mob> targetList)
        {
            targetList.Clear();
            lock (_stateLock) { targetList.AddRange(_mobs.Values); }
        }

        public void GetHarvestables(List<Harvestable> targetList)
        {
            targetList.Clear();
            lock (_stateLock) { targetList.AddRange(_harvestables.Values); }
        }

        #endregion

        public void TriggerFlash(float durationSeconds)
        {
            _flashTimeRemaining = Math.Max(_flashTimeRemaining, durationSeconds);
        }

        /// <summary>  
        /// Per-frame update tick. Calculates delta time and updates interpolation for all entities.  
        /// </summary>  
        public void Update()
        {
            long currentTimeTicks = _stopwatch.ElapsedTicks;
            long deltaTimeTicks = currentTimeTicks - _previousTimeTicks;
            _previousTimeTicks = currentTimeTicks;
            float deltaTimeSeconds = (float)deltaTimeTicks / Stopwatch.Frequency;

            if (deltaTimeSeconds > 0.1f)
            {
                deltaTimeSeconds = 0.1f;
            }

            lock (_stateLock)
            {
                _currentPlayer?.Interpolate(deltaTimeSeconds);

                foreach (var mob in _mobs.Values)
                {
                    mob.Interpolate(deltaTimeSeconds);
                }

                foreach (var harvestable in _harvestables.Values)
                {
                    harvestable.Interpolate(deltaTimeSeconds);
                }
            }

            if (_flashTimeRemaining > 0)
            {
                _flashTimeRemaining -= deltaTimeSeconds;
            }

            DLog.D($"GameStateManager Updated. Delta: {deltaTimeSeconds * 1000:F2}ms");
        }
    }
}