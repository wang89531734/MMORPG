using System.Collections.Generic;

[System.Serializable]
public class DbPlayerPair : IPlayerPair
{
    public string id;
    [LiteDB.BsonId]
    public string Id { get { return id; } set { id = value; } }
    public string playerId;
    public string PlayerId { get { return playerId; } set { playerId = value; } }
    public string pairPlayerId;
    public string PairPlayerId { get { return pairPlayerId; } set { pairPlayerId = value; } }

    public static List<PlayerPair> CloneList(IEnumerable<DbPlayerPair> list)
    {
        var result = new List<PlayerPair>();
        foreach (var entry in list)
        {
            var newEntry = new PlayerPair();
            PlayerPair.CloneTo(entry, newEntry);
            result.Add(newEntry);
        }
        return result;
    }
}
