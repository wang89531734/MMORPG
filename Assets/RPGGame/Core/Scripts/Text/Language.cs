using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameText
{
    private const string TEXT_PREFIX = "TEXT_";
    private const string VALUE_PREFIX = "VALUE_";
    private const string FORMAT_PREFIX = "FORMAT_";
    public const string TITLE_INFO_DIALOG = TEXT_PREFIX + "TITLE_INFO_DIALOG";
    public const string TITLE_ERROR_DIALOG = TEXT_PREFIX + "TITLE_ERROR_DIALOG";
    public const string TITLE_PROFILE_NAME_DIALOG = TEXT_PREFIX + "TITLE_PROFILE_NAME_DIALOG";
    public const string CONTENT_PROFILE_NAME_DIALOG = TEXT_PREFIX + "CONTENT_PROFILE_NAME_DIALOG";
    public const string PLACE_HOLDER_PROFILE_NAME = TEXT_PREFIX + "PLACE_HOLDER_PROFILE_NAME";
    public const string TITLE_SOFT_CURRENCY = TEXT_PREFIX + "TITLE_SOFT_CURRENCY";
    public const string TITLE_HARD_CURRENCY = TEXT_PREFIX + "TITLE_HARD_CURRENCY";
    public const string TITLE_STAGE_STAMINA = TEXT_PREFIX + "TITLE_STAGE_STAMINA";
    // Combat
    public const string COMBAT_MISS = TEXT_PREFIX + "COMBAT_MISS";
    // Item
    public const string TITLE_LEVEL = TEXT_PREFIX + "TITLE_LEVEL";
    public const string TITLE_COLLECT_EXP = TEXT_PREFIX + "TITLE_COLLECT_EXP";
    public const string TITLE_EVOLVE_PRICE = TEXT_PREFIX + "TITLE_EVOLVE_PRICE";
    public const string TITLE_NEXT_EXP = TEXT_PREFIX + "TITLE_NEXT_EXP";
    public const string TITLE_REQUIRE_EXP = TEXT_PREFIX + "TITLE_REQUIRE_EXP";
    public const string TITLE_PRICE = TEXT_PREFIX + "TITLE_PRICE";
    public const string TITLE_REWARD_EXP = TEXT_PREFIX + "TITLE_REWARD_EXP";
    public const string TITLE_LEVEL_UP_PRICE = TEXT_PREFIX + "TITLE_LEVEL_UP_PRICE";
    // Attribute titles
    public const string TITLE_EXP_MAX = TEXT_PREFIX + "TITLE_EXP_MAX";
    public const string TITLE_ATTRIBUTE_HP = TEXT_PREFIX + "TITLE_ATTRIBUTE_HP";
    public const string TITLE_ATTRIBUTE_PATK = TEXT_PREFIX + "TITLE_ATTRIBUTE_PATK";
    public const string TITLE_ATTRIBUTE_PDEF = TEXT_PREFIX + "TITLE_ATTRIBUTE_PDEF";
    public const string TITLE_ATTRIBUTE_MATK = TEXT_PREFIX + "TITLE_ATTRIBUTE_MATK";
    public const string TITLE_ATTRIBUTE_MDEF = TEXT_PREFIX + "TITLE_ATTRIBUTE_MDEF";
    public const string TITLE_ATTRIBUTE_SPD = TEXT_PREFIX + "TITLE_ATTRIBUTE_SPD";
    public const string TITLE_ATTRIBUTE_EVA = TEXT_PREFIX + "TITLE_ATTRIBUTE_EVA";
    public const string TITLE_ATTRIBUTE_ACC = TEXT_PREFIX + "TITLE_ATTRIBUTE_ACC";
    public const string TITLE_ATTRIBUTE_HP_RATE = TEXT_PREFIX + "TITLE_ATTRIBUTE_HP_RATE";
    public const string TITLE_ATTRIBUTE_PATK_RATE = TEXT_PREFIX + "TITLE_ATTRIBUTE_PATK_RATE";
    public const string TITLE_ATTRIBUTE_PDEF_RATE = TEXT_PREFIX + "TITLE_ATTRIBUTE_PDEF_RATE";
    public const string TITLE_ATTRIBUTE_MATK_RATE = TEXT_PREFIX + "TITLE_ATTRIBUTE_MATK_RATE";
    public const string TITLE_ATTRIBUTE_MDEF_RATE = TEXT_PREFIX + "TITLE_ATTRIBUTE_MDEF_RATE";
    public const string TITLE_ATTRIBUTE_SPD_RATE = TEXT_PREFIX + "TITLE_ATTRIBUTE_SPD_RATE";
    public const string TITLE_ATTRIBUTE_EVA_RATE = TEXT_PREFIX + "TITLE_ATTRIBUTE_EVA_RATE";
    public const string TITLE_ATTRIBUTE_ACC_RATE = TEXT_PREFIX + "TITLE_ATTRIBUTE_ACC_RATE";
    public const string TITLE_ATTRIBUTE_CRIT_CHANCE = TEXT_PREFIX + "TITLE_ATTRIBUTE_CRIT_CHANCE";
    public const string TITLE_ATTRIBUTE_CRIT_DAMAGE_RATE = TEXT_PREFIX + "TITLE_ATTRIBUTE_CRIT_DAMAGE_RATE";
    public const string TITLE_ATTRIBUTE_BLOCK_CHANCE = TEXT_PREFIX + "TITLE_ATTRIBUTE_BLOCK_CHANCE";
    public const string TITLE_ATTRIBUTE_BLOCK_DAMAGE_RATE = TEXT_PREFIX + "TITLE_ATTRIBUTE_BLOCK_DAMAGE_RATE";
    // Formats
    public const string FORMAT_INFO = FORMAT_PREFIX + "FORMAT_INFO";
    public const string FORMAT_ATTRIBUTE = FORMAT_PREFIX + "FORMAT_ATTRIBUTE";
    public const string FORMAT_BONUS = FORMAT_PREFIX + "FORMAT_BONUS";
}

public static class DefaultLocale
{
    public static readonly Dictionary<string, string> Texts = new Dictionary<string, string>();
    static DefaultLocale()
    {
        Texts.Add(GameText.TITLE_INFO_DIALOG, "Info");
        Texts.Add(GameText.TITLE_ERROR_DIALOG, "Error");
        Texts.Add(GameText.TITLE_PROFILE_NAME_DIALOG, "Name");
        Texts.Add(GameText.CONTENT_PROFILE_NAME_DIALOG, "Enter your name");
        Texts.Add(GameText.PLACE_HOLDER_PROFILE_NAME, "Enter your name...");
        Texts.Add(GameText.TITLE_SOFT_CURRENCY, "Gold(s)");
        Texts.Add(GameText.TITLE_HARD_CURRENCY, "Gem(s)");
        Texts.Add(GameText.TITLE_STAGE_STAMINA, "Stamina");
        // Combat
        Texts.Add(GameText.COMBAT_MISS, "Miss");
        // Item
        Texts.Add(GameText.TITLE_LEVEL, "Level");
        Texts.Add(GameText.TITLE_COLLECT_EXP, "Collect Exp");
        Texts.Add(GameText.TITLE_EVOLVE_PRICE, "Evolve Price");
        Texts.Add(GameText.TITLE_NEXT_EXP, "Next Exp");
        Texts.Add(GameText.TITLE_REQUIRE_EXP, "Require Exp");
        Texts.Add(GameText.TITLE_PRICE, "Price");
        Texts.Add(GameText.TITLE_REWARD_EXP, "Reward Exp");
        Texts.Add(GameText.TITLE_LEVEL_UP_PRICE, "Level Up Price");
        // Attributes
        Texts.Add(GameText.TITLE_EXP_MAX, "Max");
        Texts.Add(GameText.TITLE_ATTRIBUTE_HP, "Hp");
        Texts.Add(GameText.TITLE_ATTRIBUTE_PATK, "P.Atk");
        Texts.Add(GameText.TITLE_ATTRIBUTE_PDEF, "P.Def");
        Texts.Add(GameText.TITLE_ATTRIBUTE_MATK, "M.Atk");
        Texts.Add(GameText.TITLE_ATTRIBUTE_MDEF, "M.Def");
        Texts.Add(GameText.TITLE_ATTRIBUTE_SPD, "Speed");
        Texts.Add(GameText.TITLE_ATTRIBUTE_EVA, "Evasion");
        Texts.Add(GameText.TITLE_ATTRIBUTE_ACC, "Accuracy");
        Texts.Add(GameText.TITLE_ATTRIBUTE_HP_RATE, "Hp Rate");
        Texts.Add(GameText.TITLE_ATTRIBUTE_PATK_RATE, "P.Atk Rate");
        Texts.Add(GameText.TITLE_ATTRIBUTE_PDEF_RATE, "P.Def Rate");
        Texts.Add(GameText.TITLE_ATTRIBUTE_MATK_RATE, "M.Atk Rate");
        Texts.Add(GameText.TITLE_ATTRIBUTE_MDEF_RATE, "M.Def Rate");
        Texts.Add(GameText.TITLE_ATTRIBUTE_SPD_RATE, "Speed Rate");
        Texts.Add(GameText.TITLE_ATTRIBUTE_EVA_RATE, "Evasion Rate");
        Texts.Add(GameText.TITLE_ATTRIBUTE_ACC_RATE, "Accuracy Rate");
        Texts.Add(GameText.TITLE_ATTRIBUTE_CRIT_CHANCE, "Critical Chance");
        Texts.Add(GameText.TITLE_ATTRIBUTE_CRIT_DAMAGE_RATE, "Critical Damage Rate");
        Texts.Add(GameText.TITLE_ATTRIBUTE_BLOCK_CHANCE, "Block Chance");
        Texts.Add(GameText.TITLE_ATTRIBUTE_BLOCK_DAMAGE_RATE, "Block Damage Rate");
        // Format
        Texts.Add(GameText.FORMAT_INFO, "{0}{1}");
        Texts.Add(GameText.FORMAT_ATTRIBUTE, "{0}: {1}{2}");
        Texts.Add(GameText.FORMAT_BONUS, "{0}{1}");
        // Error texts
        Texts.Add(GameServiceErrorCode.EMPTY_USERNAME_OR_PASSWORD, "Username or password is empty");
        Texts.Add(GameServiceErrorCode.EXISTED_USERNAME, "Username is already used");
        Texts.Add(GameServiceErrorCode.EMPTY_PROFILE_NAME, "Name is empty");
        Texts.Add(GameServiceErrorCode.EXISTED_PROFILE_NAME, "Name is already used");
        Texts.Add(GameServiceErrorCode.INVALID_USERNAME_OR_PASSWORD, "Username or password is invalid");
        Texts.Add(GameServiceErrorCode.INVALID_LOGIN_TOKEN, "Invalid login token");
        Texts.Add(GameServiceErrorCode.INVALID_PLAYER_DATA, "Invalid player data");
        Texts.Add(GameServiceErrorCode.INVALID_PLAYER_ITEM_DATA, "Invalid player item data");
        Texts.Add(GameServiceErrorCode.INVALID_ITEM_DATA, "Invalid item data");
        Texts.Add(GameServiceErrorCode.INVALID_FORMATION_DATA, "Invalid formation data");
        Texts.Add(GameServiceErrorCode.INVALID_STAGE_DATA, "Invalid stage data");
        Texts.Add(GameServiceErrorCode.INVALID_LOOT_BOX_DATA, "Invalid loot box data");
        Texts.Add(GameServiceErrorCode.INVALID_IAP_PACKAGE_DATA, "Invalid IAP package data");
        Texts.Add(GameServiceErrorCode.INVALID_EQUIP_POSITION, "Invalid equip position");
        Texts.Add(GameServiceErrorCode.INVALID_BATTLE_SESSION, "Invalid battle session");
        Texts.Add(GameServiceErrorCode.NOT_ENOUGH_SOFT_CURRENCY, "Not enough " + Texts[GameText.TITLE_SOFT_CURRENCY]);
        Texts.Add(GameServiceErrorCode.NOT_ENOUGH_HARD_CURRENCY, "Not enough " + Texts[GameText.TITLE_HARD_CURRENCY]);
        Texts.Add(GameServiceErrorCode.NOT_ENOUGH_STAGE_STAMINA, "Not enough " + Texts[GameText.TITLE_STAGE_STAMINA]);
        Texts.Add(GameServiceErrorCode.NOT_ENOUGH_ITEMS, "Not enough items");
        Texts.Add(GameServiceErrorCode.CANNOT_EVOLVE, "Cannot evolve");
        Texts.Add(GameServiceErrorCode.NOT_AVAILABLE, "Services not available");
        Texts.Add(GameServiceErrorCode.UNKNOW, "Unknow Error");
    }
}

[System.Serializable]
public class Language
{
    public string languageKey;
    public List<LanguageData> dataList = new List<LanguageData>();

    public bool ContainKey(string key)
    {
        foreach (var entry in dataList)
        {
            if (entry.key == key)
                return true;
        }
        return false;
    }
}

[System.Serializable]
public struct LanguageData
{
    public string key;
    [TextArea(1, 30)]
    public string value;
}