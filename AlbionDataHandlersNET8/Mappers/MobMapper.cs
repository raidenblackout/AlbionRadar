using AlbionDataHandlers.Entities;
using AlbionDataHandlers.Enums;
using System.Text.Json.Nodes;

namespace AlbionDataHandlers.Mappers;

public class MobMapper
{
    public static MobMapper Instance { get; } = new MobMapper("Assets/mob_info.json");
    private static Dictionary<int, MobTypeInfo> TypeMap;

    public MobMapper(string filePath)
    {
        TypeMap = new Dictionary<int, MobTypeInfo>();
        using (FileStream file = File.OpenRead(filePath))
        using (StreamReader reader = new StreamReader(file))
        {
            string json = reader.ReadToEnd();
            JsonNode? jsonNode = JsonNode.Parse(json);
            if (jsonNode is JsonObject mobsObject)
            {
                foreach (var kvp in mobsObject)
                {
                    if (int.TryParse(kvp.Key, out int typeId) && kvp.Value is JsonArray mobInfo)
                    {
                        var mobTypeInfo = new MobTypeInfo
                        {
                            TypeId = typeId,
                            Tier = (TierLevels)mobInfo[0].GetValue<int>(),
                            Type = (MobTypes)mobInfo[1].GetValue<int>(),
                            Name = mobInfo[2].GetValue<string>()
                        };
                        TypeMap[typeId] = mobTypeInfo;
                    }
                }
            }
        }
    }

    public MobTypeInfo? GetMobInfo(int typeId)
    {
        return TypeMap.TryGetValue(typeId, out var mobInfo) ? mobInfo : null;
    }
}
