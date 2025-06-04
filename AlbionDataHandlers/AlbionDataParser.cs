using AlbionDataHandlers.Enums;
using BaseUtils.Logger;
using BaseUtils.Logger.Impl;
using PhotonPackageParser;
using System.Collections.Generic;

namespace AlbionDataHandlers;

public class AlbionDataParser : PhotonParser
{
    public AlbionDataParser()
    {

    }
    protected override void OnEvent(byte code, Dictionary<byte, object> parameters)
    {
        if (!parameters.TryGetValue(252, out var val) || val == null) return;

        if(!int.TryParse(val.ToString(), out int integerCode)) return;

        EventCodes eventCode;
        try
        {
            eventCode = (EventCodes)integerCode;
            DLog.I($"Event Code: {eventCode}");
        }
        catch
        {
            return;
        }

        switch (eventCode)
        {
            case EventCodes.Leave:

        }
    }

    protected override void OnRequest(byte operationCode, Dictionary<byte, object> parameters)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnResponse(byte operationCode, short returnCode, string debugMessage, Dictionary<byte, object> parameters)
    {
        throw new System.NotImplementedException();
    }
}
