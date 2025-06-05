using System.Collections.Generic;

namespace AlbionDataHandlers.Handlers;

public interface IRequestHandler
{
    public void OnRequest(int requestCode, Dictionary<byte, object> parameters);
}
