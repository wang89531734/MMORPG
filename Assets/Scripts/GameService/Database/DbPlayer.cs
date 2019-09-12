using System.Collections.Generic;

[System.Serializable]
public class DbPlayer : IPlayer
{
    public string id;
    [LiteDB.BsonId]
    public string Id { get { return id; } set { id = value; } }
    public string profileName;
    public string ProfileName { get { return profileName; } set { profileName = value; } }
    public string loginToken;
    public string LoginToken { get { return loginToken; } set { loginToken = value; } }
    public int exp;
    public int Exp { get { return exp; } set { exp = value; } }
    public string selectedFormation;
    public string SelectedFormation { get { return selectedFormation; } set { selectedFormation = value; } }
    public string mainCharacter;
    public string MainCharacter { get { return mainCharacter; } set { mainCharacter = value; } }
    public int mainCharacterExp;
    public int MainCharacterExp { get { return mainCharacterExp; } set { mainCharacterExp = value; } }

    public static List<Player> CloneList(IEnumerable<DbPlayer> list)
    {
        var result = new List<Player>();
        foreach (var entry in list)
        {
            var newEntry = new Player();
            Player.CloneTo(entry, newEntry);
            result.Add(newEntry);
        }
        return result;
    }
}
