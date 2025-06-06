using AlbionDataHandlers.Entities;

namespace AlbionRadar.Managers;

public class GameStateManager
{
    private readonly object _stateLock = new object();
    private Player _currentPlayer;
    private List<Mob> _mobs = new List<Mob>();

    public Player CurrentPlayer
    {
        get { lock (_stateLock) { return _currentPlayer; } }
    }

    public List<Mob> CurrentMobs
    {
        get { lock (_stateLock) { return _mobs; } }
    }

    public void  UpdatePlayerState(Player player)
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
            _mobs = mobs.ToList();
        }
    }


}
