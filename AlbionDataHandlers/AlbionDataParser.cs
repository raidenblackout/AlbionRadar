using AlbionDataHandlers.Enums;
using AlbionDataHandlers.Handlers;
using BaseUtils.Logger.Impl;
using PhotonPackageParser;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlbionDataHandlers;

public class AlbionDataParser : PhotonParser
{
    private List<IEventHandler> _eventHandlers = new List<IEventHandler>();

    public void RegisterEventHandler(IEventHandler handler)
    {
        if (handler == null) return;
        _eventHandlers.Add(handler);
    }

    public void UnregisterEventHandler(IEventHandler handler)
    {
        if (handler == null) return;
        _eventHandlers.Remove(handler);
    }

    protected override void OnEvent(byte code, Dictionary<byte, object> parameters)
    {
        if (!parameters.TryGetValue(252, out var val) || val == null) return;

        if (!int.TryParse(val.ToString(), out int integerCode)) return;

        EventCodes eventCode;
        try
        {
            eventCode = (EventCodes)integerCode;
        }
        catch
        {
            return; // Ignore invalid event codes
        }

        DLog.I($"Received event code: {eventCode} with parameters: {string.Join(", ", parameters)}");

        _eventHandlers.ForEach(handler =>
        {
            Task.Run(() => handler.OnEvent(eventCode, parameters));
        });
    }

    protected override void OnRequest(byte operationCode, Dictionary<byte, object> parameters)
    {
        //throw new System.NotImplementedException();
    }

    protected override void OnResponse(byte operationCode, short returnCode, string debugMessage, Dictionary<byte, object> parameters)
    {
        //throw new System.NotImplementedException();
    }
}
