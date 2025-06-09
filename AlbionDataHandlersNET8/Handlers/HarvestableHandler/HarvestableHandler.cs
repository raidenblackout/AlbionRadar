using AlbionDataHandlers.Entities;
using AlbionDataHandlers.Enums;
using AlbionDataHandlers.Utils;
using System.Reactive.Subjects;

namespace AlbionDataHandlers.Handlers;

public class HarvestableHandler : IEventHandler
{
    private readonly object _lockObject = new();
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

        if (!parameters.TryGetValue(1, out _))
        {
            lock (_lockObject)
            {
                var harvestableToRemove = _harvestables.FirstOrDefault(h => h.Id == id);
                if (harvestableToRemove != null)
                {
                    _harvestables.Remove(harvestableToRemove);
                    Harvestables.OnNext(_harvestables);
                }
            }
            return;
        }

        var harvestable = _harvestables.FirstOrDefault(h => h.Id == id);
        if (harvestable == null) return;

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

        if (location == null || location.Length < 2) return;

        var posX = float.Parse(location.GetValue(0)?.ToString() ?? "0");
        var posY = float.Parse(location.GetValue(1)?.ToString() ?? "0");

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
            var existingHarvestable = _harvestables.FirstOrDefault(h => h.Id == harvestable.Id);
            if (existingHarvestable != null)
            {
                _harvestables.Remove(existingHarvestable);
            }

            _harvestables.Add(harvestable);
            Harvestables.OnNext(_harvestables);
        }
    }

    public void OnRequest(RequestCodes requestCode, Dictionary<byte, object> parameters)
    {
        // No implementation needed for now
    }

    public void OnResponse(ResponseCodes responseCode, Dictionary<byte, object> parameters)
    {
        if (responseCode == ResponseCodes.PlayerJoiningMap)
        {
            HandlePlayerJoiningMap();
        }
    }

    private void HandlePlayerJoiningMap()
    {
        lock (_lockObject)
        {
            _harvestables.Clear();
            Harvestables.OnNext(_harvestables);
        }
    }
}
