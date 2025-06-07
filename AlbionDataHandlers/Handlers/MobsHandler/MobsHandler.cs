using AlbionDataHandlers.Entities;
using AlbionDataHandlers.Enums;
using AlbionDataHandlers.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;

namespace AlbionDataHandlers.Handlers;

public class MobsHandler : IEventHandler
{
    private readonly object _lockObject = new();
    private readonly IList<Mob> _mobs = new List<Mob>();
    public ISubject<IEnumerable<Mob>> Mobs { get; } = new Subject<IEnumerable<Mob>>();

    public void OnEvent(EventCodes eventCode, Dictionary<byte, object> parameters)
    {
        switch (eventCode)
        {
            case EventCodes.Leave:
                HandleLeave(parameters);
                break;

            case EventCodes.Move:
                HandleMove(parameters);
                break;

            case EventCodes.NewMob:
                HandleNewMob(parameters);
                break;
            case EventCodes.MobChangeState:
                HandleMobChangeState(parameters);
                break;
        }
    }

    private void HandleMobChangeState(Dictionary<byte, object> parameters)
    {
        var id = EventHandlerUtils.ExtractValue<int>(parameters, 0);
        var enchantmentLevel = EventHandlerUtils.ExtractValue<int>(parameters, 1);

        lock (_lockObject)
        {
            var existingMob = _mobs.FirstOrDefault(m => m.Id == id);
            if (existingMob != null)
            {
                existingMob.EnchantmentLevel = enchantmentLevel;
                Mobs.OnNext(_mobs);
            }
        }
    }

    private void HandleNewMob(Dictionary<byte, object> parameters)
    {
        int id = int.Parse(parameters[0].ToString());
        int typeId = EventHandlerUtils.ExtractValue<int>(parameters, 1);
        var location = parameters[7] as Array;
        float posX = float.Parse(location.GetValue(0).ToString());
        float posY = float.Parse(location.GetValue(1).ToString());

        float experience = EventHandlerUtils.ExtractValue<float>(parameters, 13, 0);
        string name = EventHandlerUtils.ExtractValue<string>(parameters, 32)
                      ?? EventHandlerUtils.ExtractValue<string>(parameters, 31);
        int enchantmentLevel = EventHandlerUtils.ExtractValue<int>(parameters, 33, 0);
        int rarity = EventHandlerUtils.ExtractValue<int>(parameters, 34, 0);

        var mob = new Mob
        {
            Id = id,
            TypeId = typeId,
            Experience = experience,
            Name = name,
            EnchantmentLevel = enchantmentLevel,
            Rarity = rarity,
            PositionX = posX,
            PositionY = posY
        };

        lock (_lockObject)
        {
            var existingMob = _mobs.FirstOrDefault(m => m.Id == mob.Id);
            if (existingMob != null)
            {
                _mobs.Remove(existingMob);
            }

            _mobs.Add(mob);
            Mobs.OnNext(_mobs);
        }
    }

    private void HandleMove(Dictionary<byte, object> parameters)
    {
        int id = int.Parse(parameters[0].ToString());
        float posX = EventHandlerUtils.ExtractValue<float>(parameters, 4);
        float posY = EventHandlerUtils.ExtractValue<float>(parameters, 5);

        lock (_lockObject)
        {
            var mobToUpdate = _mobs.FirstOrDefault(m => m.Id == id);
            if (mobToUpdate != null)
            {
                mobToUpdate.PositionX = posX;
                mobToUpdate.PositionY = posY;
                Mobs.OnNext(_mobs);
            }
        }
    }

    private void HandleLeave(Dictionary<byte, object> parameters)
    {
        int id = int.Parse(parameters[0].ToString());

        lock (_lockObject)
        {
            var mobToRemove = _mobs.FirstOrDefault(m => m.Id == id);
            if (mobToRemove != null)
            {
                _mobs.Remove(mobToRemove);
                Mobs.OnNext(_mobs);
            }
        }
    }

    public void OnRequest(RequestCodes requestCode, Dictionary<byte, object> parameters)
    {
        // No implementation required for OnRequest in the current context
    }

    public void OnResponse(ResponseCodes responseCode, Dictionary<byte, object> parameters)
    {
        if (responseCode == ResponseCodes.PlayerJoiningMap)
        {
            HandlePlayerJoiningMap(parameters);
        }
    }

    private void HandlePlayerJoiningMap(Dictionary<byte, object> parameters)
    {
        lock (_lockObject)
        {
            _mobs.Clear();
            Mobs.OnNext(_mobs);
        }
    }
}
