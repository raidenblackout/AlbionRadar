using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotonParser;

internal class SegmentedPackage
{
    public int TotalLength;

    public int BytesWritten;

    public byte[] TotalPayload;
}