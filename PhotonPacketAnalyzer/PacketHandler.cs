namespace PhotonPacketAnalyzer;

class PacketHandler : IPhotonPackageHandler
{
    // Log To Elastic
    public PacketHandler() { }


    public void OnEvent(byte code, Dictionary<byte, object> parameters)
    {
        object val;
        parameters.TryGetValue(252, out val);
        if (val == null) return;

        int iCode = 0;
        if (!int.TryParse(val.ToString(), out iCode)) return;

        EventCodes eventCode;

        try
        {
            eventCode = (EventCodes)iCode;
            Console.WriteLine(eventCode);
        }
        catch
        {
            return;
        }

        switch (eventCode)
        {
            case EventCodes.TimeSync:
                PacketHandlerEvents.InvokeTimeSyncEvent(new TimeSyncEventArgs() { time = long.Parse(parameters[0].ToString()) });
                break;
            case EventCodes.ROAInfoUpdate:
                onROAInfoUpdate(parameters);
                break;
            default: break;
        }
    }

    public void OnResponse(byte operationCode, short returnCode, Dictionary<byte, object> parameters)
    {
        //   return;
        int iCode = 0;
        if (!int.TryParse(parameters[253].ToString(), out iCode)) return;
        OperationCodes code = (OperationCodes)iCode;

        switch (code)
        {
            case OperationCodes.Join:
                {
                    onJoinResponse(parameters);
                }
                break;
        }
    }

    private void onROAInfoUpdate(Dictionary<byte, object> parameters)
    {
        try
        {
            ROAInfoEventArgs args = new ROAInfoEventArgs();

            // If there's no destination key, ignore
            if (!parameters.ContainsKey(6) || parameters[6].ToString() == "")
                return;

            // If there was a recent map change, ignore event
            // Todo: Remove this when I can handle fragmented packets
            if (!Settings.RecentlyJoinedNewMap())
                return;



            args.sourceCluster = PlayerState.currentMapID;
            args.sourceClusterDisplayName = WorldMap.GetMapDisplayName(args.sourceCluster);
            args.sourceClusterType = WorldMap.GetClusterByID(args.sourceCluster).Type;

            args.targetCluster = parameters[6].ToString();
            args.targetClusterDisplayName = WorldMap.GetMapDisplayName(args.targetCluster);
            args.targetClusterType = WorldMap.GetClusterByID(args.targetCluster).Type;

            // Param is not present when remaining capacity is 0
            try
            {
                args.remainingCapacity = parameters[9].ToString();
            }
            catch
            {
                args.remainingCapacity = "0";
            }

            args.maxCapacity = parameters[10].ToString();

            var coordinates = (Single[])parameters[1];
            args.posX = coordinates[0].ToString();
            args.posY = coordinates[1].ToString();

            // Tunnel close time using server time
            long tunnelCloseTime = long.Parse(parameters[8].ToString());

            // Tunnel time remaining in milliseconds
            long timeRemaining = tunnelCloseTime - PlayerState.currentTime;

            // Get the epoch time in seconds when gate closes
            long endTime = (DateTimeOffset.Now.ToUnixTimeMilliseconds() + timeRemaining) / 1000;

            args.endTime = endTime.ToString();


            // Hack solution to get clientID in the POST. Needs a refactor.
            args.clientID = Settings.clientID;

            PacketHandlerEvents.InvokeNewROAUpdateEvent(args);

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            DumpParameters(parameters);

        }

    }

    // Join response is received on map switch. Contains local player information.
    private void onJoinResponse(Dictionary<byte, object> parameters)
    {
        try
        {
            PlayerJoinEventArgs e = new PlayerJoinEventArgs();
            e.playerName = parameters[2].ToString();
            e.guild = parameters[51].ToString();
            e.alliance = parameters[69].ToString();
            e.mapID = parameters[8].ToString();
            e.homeLocation = parameters[59].ToString();
            PacketHandlerEvents.InvokePlayerJoinEvent(e);


        }
        catch
        {
            Console.WriteLine("Error Handling Join Event. Check Event Codes.");
        }
    }

    public void OnRequest(byte operationCode, Dictionary<byte, object> parameters)
    {
    }

    // Ugly function to dump as much info as possible from a game event
    public void DumpParameters(Dictionary<byte, object> parameters)
    {
        foreach (KeyValuePair<byte, object> kvp in parameters)
        {
            Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            try
            {
                foreach (var foo in (string[])kvp.Value)
                {
                    Console.WriteLine(foo);
                }
            }
            catch
            {
                try
                {
                    foreach (var foo in (byte[])kvp.Value)
                    {
                        Console.WriteLine(foo);
                    }
                }
                catch
                {
                    try
                    {
                        var listOfLists = (byte[][])kvp.Value;
                        for (int i = 0; i < listOfLists.Length; i++)
                        {
                            Console.WriteLine("i: " + i);
                            for (int j = 0; j < listOfLists[i].Length; j++)
                            {
                                Console.WriteLine("j: " + j);
                                Console.WriteLine(listOfLists[i][j]);
                            }

                        }

                    }

                    catch
                    {

                        try
                        {
                            var listOfLists = (string[][])kvp.Value;
                            for (int i = 0; i < listOfLists.Length; i++)
                            {
                                Console.WriteLine("i: " + i);
                                for (int j = 0; j < listOfLists[i].Length; j++)
                                {
                                    Console.WriteLine("j: " + j);
                                    Console.WriteLine(listOfLists[i][j]);
                                }

                            }
                        }

                        catch { }


                    }
                }
            }
        }
    }

}
