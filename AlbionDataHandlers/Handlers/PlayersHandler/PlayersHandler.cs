using AlbionDataHandlers.Entities;
using AlbionDataHandlers.Enums;
using AlbionDataHandlers.Utils;
using BaseUtils.Logger.Impl;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace AlbionDataHandlers.Handlers;

public class PlayersHandler : IEventHandler
{
    private Player _player = new Player();
    public ISubject<Player> Player { get; } = new Subject<Player>();

    public void OnEvent(EventCodes eventCode, Dictionary<byte, object> parameters)
    {
        switch(eventCode)
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
        }
    }

    private void HandleMounted(Dictionary<byte, object> parameters)
    {

    }

    private void HandleCharacterEquipmentChanged(Dictionary<byte, object> parameters)
    {

    }

    private void HandleHealthUpdate(Dictionary<byte, object> parameters)
    {

    }

    private void HandleRegenerationHealthChanged(Dictionary<byte, object> parameters)
    {

    }

    private void HandleNewCharacter(Dictionary<byte, object> parameters)
    {
        
    }

    private void HandleLeave(Dictionary<byte, object> parameters)
    {
        if (!int.TryParse(parameters[0].ToString(), out int playerId)) return;
    }

    private bool Remove(string id)
    {
        return true;
    }

    public void OnRequest(RequestCodes requestCode, Dictionary<byte, object> parameters)
    {
        switch(requestCode)
        {
            case RequestCodes.PlayerMoving:
                HandlePlayerMoving(parameters);
                break;

        }
    }

    private void HandlePlayerMoving(Dictionary<byte, object> parameters)
    {
        var location = parameters[1] as Array;
        float posX = float.Parse(location.GetValue(0).ToString());
        float posY = float.Parse(location.GetValue(1).ToString());

        _player.PositionX = posX;
        _player.PositionY = posY;

        DLog.I($"Player moved to position: ({posX}, {posY})");

        Player.OnNext(_player);
    }
}
