using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotonParser;

internal enum CommandType
{
    Disconnect = 4,
    SendReliable = 6,
    SendUnreliable = 7,
    SendFragment = 8
}
