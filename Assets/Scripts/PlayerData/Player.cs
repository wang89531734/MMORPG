using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Player : BasePlayerData, ILevel, IPlayer
{
    public static string CurrentPlayerId { get; private set; }
    public static readonly Dictionary<string, Player> DataMap = new Dictionary<string, Player>();
    public static Player CurrentPlayer
    {
        get
        {
            if (!string.IsNullOrEmpty(CurrentPlayerId) && DataMap.ContainsKey(CurrentPlayerId))
                return DataMap[CurrentPlayerId];
            return null;
        }
        set
        {
            if (value == null || string.IsNullOrEmpty(value.Id))
            {
                CurrentPlayerId = string.Empty;
                return;
            }
            SetData(value);
            CurrentPlayerId = value.Id;
        }
    }
    public string id;
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
    public bool isFriend;

    private int level = -1;
    private int collectExp = -1;
    private int dirtyExp = -1;  // Exp for dirty check to calculate `Level` and `CollectExp` fields

    public Player()
    {
        Id = "";
        ProfileName = "";
        LoginToken = "";
        Exp = 0;
    }

    public Player Clone()
    {
        var result = new Player();
        CloneTo(this, result);
        return result;
    }

    public static void CloneTo(IPlayer from, IPlayer to)
    {
        to.Id = from.Id;
        to.ProfileName = from.ProfileName;
        to.LoginToken = from.LoginToken;
        to.Exp = from.Exp;
        to.SelectedFormation = from.SelectedFormation;
        to.MainCharacter = from.MainCharacter;
        to.MainCharacterExp = from.MainCharacterExp;
    }

    #region Non Serialize Fields
    public int Level
    {
        get
        {
            CalculateLevelAndRemainExp();
            return level;
        }
    }
    
    public int CollectExp
    {
        get
        {
            CalculateLevelAndRemainExp();
            return collectExp;
        }
    }
    
    public int MaxLevel
    {
        get { return GameDatabase == null ? 1 : GameDatabase.playerMaxLevel; }
    }
    
    public int NextExp
    {
        get { return GameDatabase == null ? 0 : GameDatabase.playerExpTable.Calculate(Level, GameDatabase.playerMaxLevel); }
    }
    #endregion

    private void CalculateLevelAndRemainExp()
    {
        if (GameDatabase == null)
        {
            level = 1;
            collectExp = 0;
            return;
        }
        if (dirtyExp == -1 || dirtyExp != Exp)
        {
            dirtyExp = Exp;
            var remainExp = Exp;
            var maxLevel = GameDatabase.playerMaxLevel;
            for (level = 1; level < maxLevel; ++level)
            {
                var nextExp = GameDatabase.playerExpTable.Calculate(level, maxLevel);
                if (remainExp - nextExp < 0)
                    break;
                remainExp -= nextExp;
            }
            collectExp = remainExp;
        }
    }

    public static void SetData(Player data)
    {
        if (data == null || string.IsNullOrEmpty(data.Id))
            return;
        DataMap[data.Id] = data;
    }

    public static bool RemoveData(string id)
    {
        return DataMap.Remove(id);
    }

    public static void ClearData()
    {
        DataMap.Clear();
    }

    public static void SetDataRange(IEnumerable<Player> list)
    {
        foreach (var data in list)
        {
            SetData(data);
        }
    }

    public static void RemoveDataRange(IEnumerable<string> ids)
    {
        foreach (var id in ids)
        {
            RemoveData(id);
        }
    }
}
