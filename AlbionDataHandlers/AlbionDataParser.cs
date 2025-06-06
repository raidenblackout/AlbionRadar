using AlbionDataHandlers.Enums;
using AlbionDataHandlers.Handlers;
using BaseUtils.Logger.Impl;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlbionDataHandlers;

public class AlbionDataParser : PhotonParser.PhotonParser
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
        if (!int.TryParse(val.ToString(), out int integerCode))
        {
            DLog.I($"Failed with Value {val}");
            return;
        }

        EventCodes eventCode;
        try
        {
            eventCode = (EventCodes)integerCode;
        }
        catch
        {
            DLog.I($"Failed with {integerCode}");
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
        if (!parameters.TryGetValue(253, out var val) || val == null) return;
        if (!int.TryParse(val.ToString(), out int integerCode)) return;

        RequestCodes requestCode;
        try
        {
            requestCode = (RequestCodes)integerCode;
        }
        catch
        {
            return; // Ignore invalid request codes
        }

        DLog.I($"Received request code: {requestCode} with parameters: {string.Join(", ", parameters)}");

        _eventHandlers.ForEach(handler =>
        {
            Task.Run(() => handler.OnRequest(requestCode, parameters));
        });
    }

    protected override void OnResponse(byte operationCode, short returnCode, string debugMessage, Dictionary<byte, object> parameters)
    {
        //throw new System.NotImplementedException();
        if (!parameters.TryGetValue(253, out var val) || val == null)
        {
            DLog.I($"Failed To get Value {string.Join(", ", parameters)}");
            return;
        }
        if (!int.TryParse(val.ToString(), out int integerCode))
        {
            DLog.I($"Failed with Value {val}");
            return;
        }

        ResponseCodes eventCode;
        try
        {
            eventCode = (ResponseCodes)integerCode;
        }
        catch
        {
            DLog.I($"Failed with {integerCode}");
            return; // Ignore invalid response codes
        }

        DLog.I($"Received event code: {eventCode} with parameters: {string.Join(", ", parameters)}");

        _eventHandlers.ForEach(handler =>
        {
            Task.Run(() => handler.OnResponse(eventCode, parameters));
        });
    }
}
