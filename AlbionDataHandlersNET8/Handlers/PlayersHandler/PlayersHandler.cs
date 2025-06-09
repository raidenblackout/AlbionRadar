using AlbionDataHandlers.Entities;
using AlbionDataHandlers.Enums;
using BaseUtils.Logger.Impl;
using System.Reactive.Subjects;

namespace AlbionDataHandlers.Handlers;

public class PlayersHandler : IEventHandler
{
    private readonly Player _player = new Player();
    public ISubject<Player> Player { get; } = new Subject<Player>();

    public void OnEvent(EventCodes eventCode, Dictionary<byte, object> parameters)
    {
        switch (eventCode)
        {
            case EventCodes.Leave:
                HandleLeave(parameters);
                break;

            case EventCodes.NewCharacter:
                HandleNewCharacter(parameters);
                break;

            case EventCodes.RegenerationHealthChanged:
                HandleRegenerationHealthChanged(parameters);
                break;

            case EventCodes.HealthUpdate:
                HandleHealthUpdate(parameters);
                break;

            case EventCodes.CharacterEquipmentChanged:
                HandleCharacterEquipmentChanged(parameters);
                break;

            case EventCodes.Mounted:
                HandleMounted(parameters);
                break;

            default:
                DLog.I($"Unhandled event code: {eventCode}");
                break;
        }
    }

    private void HandleMounted(Dictionary<byte, object> parameters)
    {
        // Implementation for handling mounted event
    }

    private void HandleCharacterEquipmentChanged(Dictionary<byte, object> parameters)
    {
        // Implementation for handling character equipment change
    }

    private void HandleHealthUpdate(Dictionary<byte, object> parameters)
    {
        // Implementation for handling health update
    }

    private void HandleRegenerationHealthChanged(Dictionary<byte, object> parameters)
    {
        // Implementation for handling regeneration health change
    }

    private void HandleNewCharacter(Dictionary<byte, object> parameters)
    {
        // Implementation for handling new character creation
    }

    private void HandleLeave(Dictionary<byte, object> parameters)
    {
        if (parameters.TryGetValue(0, out var playerIdObj) && int.TryParse(playerIdObj.ToString(), out int playerId))
        {
            DLog.I($"Player with ID {playerId} has left.");
            // Additional logic for handling player leave
        }
        else
        {
            DLog.I("Invalid player ID in Leave event.");
        }
    }

    private bool Remove(string id)
    {
        // Implementation for removing a player by ID
        return true;
    }

    public void OnRequest(RequestCodes requestCode, Dictionary<byte, object> parameters)
    {
        switch (requestCode)
        {
            case RequestCodes.PlayerMoving:
                HandlePlayerMoving(parameters);
                break;

            default:
                DLog.I($"Unhandled request code: {requestCode}");
                break;
        }
    }

    private void HandlePlayerMoving(Dictionary<byte, object> parameters)
    {
        if (parameters.TryGetValue(1, out var locationObj) && locationObj is Array location)
        {
            if (float.TryParse(location.GetValue(0)?.ToString(), out float posX) &&
                float.TryParse(location.GetValue(1)?.ToString(), out float posY))
            {
                _player.PositionX = posX;
                _player.PositionY = posY;

                DLog.I($"Player moved to position: ({posX}, {posY})");
                Player.OnNext(_player);
            }
            else
            {
                DLog.I("Invalid position data in PlayerMoving request.");
            }
        }
        else
        {
            DLog.I("Invalid location data in PlayerMoving request.");
        }
    }

    public void OnResponse(ResponseCodes responseCode, Dictionary<byte, object> parameters)
    {
        switch (responseCode)
        {
            case ResponseCodes.PlayerJoiningMap:
                HandlePlayerJoiningMap(parameters);
                break;

            default:
                DLog.I($"Unhandled response code: {responseCode}");
                break;
        }
    }

    private void HandlePlayerJoiningMap(Dictionary<byte, object> parameters)
    {
        // Implementation for handling player joining map
    }
}
