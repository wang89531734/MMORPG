using LiteDB;

public abstract class BasePlayerData
{
    public static GameDatabase GameDatabase
    {
        get { return GameInstance.GameDatabase; }
    }
}
