using AlbionDataHandlers.Enums;
using System.Collections.Generic;

namespace AlbionDataHandlers.Handlers;

public interface IEventHandler
{
    public void OnEvent(EventCodes eventCode, Dictionary<byte, object> parameters);

    public void OnRequest(RequestCodes requestCode, Dictionary<byte, object> parameters);
}
