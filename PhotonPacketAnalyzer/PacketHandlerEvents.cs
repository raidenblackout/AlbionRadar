namespace PhotonPacketAnalyzer;


static class PacketHandlerEvents
{
    public delegate void JoinHandler(object sender, PlayerJoinEventArgs e);
    public static event JoinHandler PlayerJoinEvent;

    public delegate void TimeSyncHandler(object sender, TimeSyncEventArgs e);
    public static event TimeSyncHandler TimeSyncEvent;

    public delegate void ROAInfoUpdateHandler(object sender, ROAInfoEventArgs e);
    public static event ROAInfoUpdateHandler ROAInfoUpdateEvent;


    public static void InvokePlayerJoinEvent(PlayerJoinEventArgs e)
    {
        PlayerJoinEvent?.Invoke(null, e);
    }

    public static void InvokeNewROAUpdateEvent(ROAInfoEventArgs e)
    {
        ROAInfoUpdateEvent?.Invoke(null, e);
    }

    public static void InvokeTimeSyncEvent(TimeSyncEventArgs e)
    {
        TimeSyncEvent?.Invoke(null, e);
    }
}


public class PlayerJoinEventArgs : EventArgs
{
    public string playerName;
    public string mapID;
    public string guild;
    public string alliance;
    public string homeLocation;
}

public class TimeSyncEventArgs : EventArgs
{
    public long time;
}

public class ROAInfoEventArgs : EventArgs
{
    public string sourceCluster;
    public string sourceClusterDisplayName;
    public string sourceClusterType;
    public string targetCluster;
    public string targetClusterDisplayName;
    public string targetClusterType;
    public string maxCapacity;
    public string remainingCapacity;
    public string posX;
    public string posY;

    // Epoch time that wormhole closes
    public string endTime;

    // Hack solution to get clientID in the web post
    public string clientID;
}
