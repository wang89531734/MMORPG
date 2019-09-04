using System.Collections;
using System.Collections.Generic;

public class GameServiceErrorCode
{
    private const string ERROR_PREFIX = "ERROR_";
    public const string UNKNOW = ERROR_PREFIX + "UNKNOW";
    public const string EMPTY_USERNAME_OR_PASSWORD = ERROR_PREFIX + "EMPTY_USERNAME_OR_PASSWORD";
    public const string EXISTED_USERNAME = ERROR_PREFIX + "EXISTED_USERNAME";
    public const string EMPTY_PROFILE_NAME = ERROR_PREFIX + "EMPTY_PROFILE_NAME";
    public const string EXISTED_PROFILE_NAME = ERROR_PREFIX + "EXISTED_PROFILE_NAME";
    public const string INVALID_USERNAME_OR_PASSWORD = ERROR_PREFIX + "INVALID_USERNAME_OR_PASSWORD";
    public const string INVALID_LOGIN_TOKEN = ERROR_PREFIX + "INVALID_LOGIN_TOKEN";
    public const string INVALID_PLAYER_DATA = ERROR_PREFIX + "INVALID_PLAYER_DATA";
    public const string INVALID_PLAYER_ITEM_DATA = ERROR_PREFIX + "INVALID_PLAYER_ITEM_DATA";
    public const string INVALID_ITEM_DATA = ERROR_PREFIX + "INVALID_ITEM_DATA";
    public const string INVALID_FORMATION_DATA = ERROR_PREFIX + "INVALID_FORMATION_DATA";
    public const string INVALID_STAGE_DATA = ERROR_PREFIX + "INVALID_STAGE_DATA";
    public const string INVALID_LOOT_BOX_DATA = ERROR_PREFIX + "INVALID_LOOT_BOX_DATA";
    public const string INVALID_IAP_PACKAGE_DATA = ERROR_PREFIX + "INVALID_IAP_PACKAGE_DATA";
    public const string INVALID_EQUIP_POSITION = ERROR_PREFIX + "INVALID_EQUIP_POSITION";
    public const string INVALID_BATTLE_SESSION = ERROR_PREFIX + "INVALID_BATTLE_SESSION";
    public const string NOT_ENOUGH_SOFT_CURRENCY = ERROR_PREFIX + "NOT_ENOUGH_SOFT_CURRENCY";
    public const string NOT_ENOUGH_HARD_CURRENCY = ERROR_PREFIX + "NOT_ENOUGH_HARD_CURRENCY";
    public const string NOT_ENOUGH_STAGE_STAMINA = ERROR_PREFIX + "NOT_ENOUGH_STAGE_STAMINA";
    public const string NOT_ENOUGH_ITEMS = ERROR_PREFIX + "NOT_ENOUGH_ITEMS";
    public const string CANNOT_EVOLVE = ERROR_PREFIX + "CANNOT_EVOLVE";
    public const string NOT_AVAILABLE = ERROR_PREFIX + "NOT_AVAILABLE";
}

public class GameServiceResult
{
    public string error;
    public bool Success { get { return string.IsNullOrEmpty(error); } }
}

public class ServiceTimeResult : GameServiceResult
{
    public long serviceTime;
}

public class PlayerResult : GameServiceResult
{
    public Player player;
}

public class StartStageResult : GameServiceResult
{
    public PlayerStamina stamina;
    public string session;
}

public class FinishStageResult : PlayerResult
{
    public int rating;
    public int rewardPlayerExp;
    public int rewardCharacterExp;
    public int rewardSoftCurrency;
    public List<PlayerItem> rewardItems = new List<PlayerItem>();
    public List<PlayerCurrency> updateCurrencies = new List<PlayerCurrency>();
    public List<PlayerItem> createItems = new List<PlayerItem>();
    public List<PlayerItem> updateItems = new List<PlayerItem>();
    public List<string> deleteItemIds = new List<string>();
    public PlayerClearStage clearStage;
}

public class CurrencyResult : GameServiceResult
{
    public List<PlayerCurrency> updateCurrencies = new List<PlayerCurrency>();
}

public class ItemResult : GameServiceResult
{
    public List<PlayerCurrency> updateCurrencies = new List<PlayerCurrency>();
    public List<PlayerItem> createItems = new List<PlayerItem>();
    public List<PlayerItem> updateItems = new List<PlayerItem>();
    public List<string> deleteItemIds = new List<string>();
}

public class AuthListResult : GameServiceResult
{
    public List<PlayerAuth> list = new List<PlayerAuth>();
}

public class ItemListResult : GameServiceResult
{
    public List<PlayerItem> list = new List<PlayerItem>();
}

public class CurrencyListResult : GameServiceResult
{
    public List<PlayerCurrency> list = new List<PlayerCurrency>();
}

public class StaminaListResult : GameServiceResult
{
    public List<PlayerStamina> list = new List<PlayerStamina>();
}

public class FormationListResult : GameServiceResult
{
    public List<PlayerFormation> list = new List<PlayerFormation>();
}

public class UnlockItemListResult : GameServiceResult
{
    public List<PlayerUnlockItem> list = new List<PlayerUnlockItem>();
}

public class ClearStageListResult : GameServiceResult
{
    public List<PlayerClearStage> list = new List<PlayerClearStage>();
}

public class AvailableLootBoxListResult : GameServiceResult
{
    public List<string> list = new List<string>();
}

public class AvailableIapPackageListResult : GameServiceResult
{
    public List<string> list = new List<string>();
}

public class FriendListResult : GameServiceResult
{
    public List<Player> list = new List<Player>();
}
