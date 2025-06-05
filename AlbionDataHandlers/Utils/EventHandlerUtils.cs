using System.Collections.Generic;

namespace AlbionDataHandlers.Utils;

public static class EventHandlerUtils
{
    public static T ExtractValue<T>(Dictionary<byte, object> parameters, byte key, T defaultValue = default)
    {
        if (parameters.TryGetValue(key, out var value) && value is T typedValue)
        {
            return typedValue;
        }
        return defaultValue;
    }
}
