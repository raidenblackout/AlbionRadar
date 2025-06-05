using AlbionDataHandlers.Enums;
using BaseUtils.Logger.Impl;
using System;
using System.Collections.Generic;

namespace AlbionDataHandlers.Handlers;

public class PlayersHandler : IEventHandler
{
    private static object lockObject = new object();

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
}
