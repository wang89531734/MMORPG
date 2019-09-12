[System.Serializable]
public class FakePlayer
{
    public string profileName;
    public int level;
    public CharacterItem mainCharacter;
    public int mainCharacterLevel;

    public int GetExp()
    {
        var exp = 0;
        var gameDb = GameInstance.GameDatabase;
        for (var i = 0; i < level - 1; ++i)
        {
            exp += gameDb.playerExpTable.Calculate(i + 1, gameDb.playerMaxLevel);
        }
        return exp;
    }

    public int GetMainCharacterExp()
    {
        if (mainCharacter == null)
            return 0;
        var exp = 0;
        var itemTier = mainCharacter.itemTier;
        for (var i = 0; i < mainCharacterLevel - 1; ++i)
        {
            exp += itemTier.expTable.Calculate(i + 1, itemTier.maxLevel);
        }
        return exp;
    }
}
