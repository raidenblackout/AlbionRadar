using System.Collections.Generic;

namespace PhotonAnalyzer.Entities;

public interface IPhotonPackageHandler
{
    void OnEvent(byte code, Dictionary<byte, object> parameters);

    void OnResponse(byte operationCode, short returnCode, Dictionary<byte, object> parameters);

    void OnRequest(byte operationCode, Dictionary<byte, object> parameters);
}