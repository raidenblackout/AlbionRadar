using AlbionDataHandlers.Entities;
using AlbionDataHandlers.Enums;
using AlbionDataHandlers.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace AlbionDataHandlers.Handlers;

public class HarvestableHandler : IEventHandler
{
    private object _lockObject = new object();
    private readonly IList<Harvestable> _harvestables = new List<Harvestable>();

    public ISubject<IEnumerable<Harvestable>> Harvestables { get; } = new Subject<IEnumerable<Harvestable>>();

    public void OnEvent(EventCodes eventCode, Dictionary<byte, object> parameters)
    {
        switch (eventCode)
        {
            case EventCodes.NewHarvestableObject:
                HandleNewHarvestable(parameters);
                break;
            case EventCodes.HarvestableChangeState:
                HandleHarvestableChangeState(parameters);
                break;
        }
    }

    private void HandleHarvestableChangeState(Dictionary<byte, object> parameters)
    {
        var id = EventHandlerUtils.ExtractValue<int>(parameters, 0);

        if(!parameters.TryGetValue(1, out var val))
        {
            lock (_lockObject)
            {
                _harvestables.Remove(_harvestables.FirstOrDefault(h => h.Id == id));
                Harvestables.OnNext(_harvestables);
            }
            return;
        }

        var harvestable = _harvestables.FirstOrDefault(h => h.Id == id);

        if(harvestable == null)
        {
            return; // Harvestable not found, nothing to update
        }

        var size = EventHandlerUtils.ExtractValue<int>(parameters, 1, 0);

        harvestable.Size = size;

        lock (_lockObject)
        {
            Harvestables.OnNext(_harvestables);
        }
    }


    private void HandleNewHarvestable(Dictionary<byte, object> parameters)
    {
        var id = EventHandlerUtils.ExtractValue<int>(parameters, 0);
        var type = EventHandlerUtils.ExtractValue<int>(parameters, 5);
        var tier = EventHandlerUtils.ExtractValue<int>(parameters, 7);
        var location = parameters[8] as Array;
        float posX = float.Parse(location.GetValue(0).ToString());
        float posY = float.Parse(location.GetValue(1).ToString());

        var enchantmentLevel = EventHandlerUtils.ExtractValue<int>(parameters, 11, 0);
        var size = EventHandlerUtils.ExtractValue<int>(parameters, 10, 0);
        var harvestable = new Harvestable
        {
            Id = id,
            Type = type,
            Tier = tier,
            PositionX = posX,
            PositionY = posY,
            EnchantmentLevel = enchantmentLevel,
            Size = size,
        };

        lock (_lockObject)
        {
            if (!_harvestables.Any(h => h.Id == harvestable.Id))
            {
                _harvestables.Add(harvestable);
                Harvestables.OnNext(_harvestables);
            }
            else
            {
                var existingHarvestable = _harvestables.FirstOrDefault(h => h.Id == harvestable.Id);
                if (existingHarvestable != null)
                {
                    _harvestables.Remove(existingHarvestable);
                }
                _harvestables.Add(harvestable);
                Harvestables.OnNext(_harvestables);
            }
        }
    }

    public void OnRequest(RequestCodes requestCode, Dictionary<byte, object> parameters)
    {

    }
}
