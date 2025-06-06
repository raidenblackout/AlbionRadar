using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotonParser.Models
{

    public class EventData
    {
        public byte Code { get; }

        public Dictionary<byte, object> Parameters { get; }

        public EventData(byte code, Dictionary<byte, object> parameters)
        {
            Code = code;
            Parameters = parameters;
        }
    }
}
