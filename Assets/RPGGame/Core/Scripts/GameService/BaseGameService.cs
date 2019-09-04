using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseGameService : MonoBehaviour
{
    public const string AUTH_NORMAL = "NORMAL";
    public const string AUTH_GUEST = "GUEST";
    public const ushort BATTLE_RESULT_NONE = 0;
    public const ushort BATTLE_RESULT_LOSE = 1;
    public const ushort BATTLE_RESULT_WIN = 2;
    public UnityEvent onServiceStart;
    public UnityEvent onServiceFinish;
    public long ServiceTimeOffset { get; private set; }
    private float updateTimeCounter;

    private void Update()
    {
        updateTimeCounter -= Time.unscaledDeltaTime;
        if (updateTimeCounter <= 0f)
        {
            // Update time offset every five seconds
            updateTimeCounter = 5f;
            GetServiceTime();
        }
    }

    public void SetPrefsLogin(string playerId, string loginToken)
    {
        string oldPlayerId = PlayerPrefs.GetString(Consts.KeyPlayerId, string.Empty);
        // Delete gameplay settings when difference player login
        if (!oldPlayerId.Equals(playerId))
        {
            PlayerPrefs.DeleteKey(Consts.KeyIsAutoPlay);
            PlayerPrefs.DeleteKey(Consts.KeyIsSpeedMultiply);
        }
        PlayerPrefs.SetString(Consts.KeyPlayerId, playerId);
        PlayerPrefs.SetString(Consts.KeyLoginToken, loginToken);
        PlayerPrefs.Save();
    }

    public string GetPrefsPlayerId()
    {
        return PlayerPrefs.GetString(Consts.KeyPlayerId);
    }

    public string GetPrefsLoginToken()
    {
        return PlayerPrefs.GetString(Consts.KeyLoginToken);
    }

    public void Logout(UnityAction onLogout = null)
    {
        Player.CurrentPlayer = null;
        Player.ClearData();
        PlayerAuth.ClearData();
        PlayerCurrency.ClearData();
        PlayerFormation.ClearData();
        PlayerItem.ClearData();
        PlayerStamina.ClearData();
        PlayerUnlockItem.ClearData();
        SetPrefsLogin("", "");
        onLogout();
    }

    protected void HandleServiceCall()
    {
        onServiceStart.Invoke();
    }

    protected void HandleResult<T>(T result, UnityAction<T> onSuccess, UnityAction<string> onError) where T : GameServiceResult
    {
        onServiceFinish.Invoke();
        if (result.Success)
        {
            if (onSuccess != null)
                onSuccess(result);
        }
        else
        {
            if (onError != null)
                onError(result.error);
        }
    }

    /// <summary>
    /// Register
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void Register(string username, string password, UnityAction<PlayerResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: Register");
        HandleServiceCall();
        DoRegister(username, password, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// Login
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void Login(string username, string password, UnityAction<PlayerResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: Login");
        HandleServiceCall();
        DoLogin(username, password, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// RegisterOrLogin
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void RegisterOrLogin(string username, string password, UnityAction<PlayerResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: RegisterOrLogin");
        HandleServiceCall();
        DoRegisterOrLogin(username, password, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// GuestLogin
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void GuestLogin(string deviceId, UnityAction<PlayerResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GuestLogin");
        HandleServiceCall();
        DoGuestLogin(deviceId, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// ValidateLoginToken
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void ValidateLoginToken(bool refreshToken, UnityAction<PlayerResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: ValidateLoginToken");
        var playerId = GetPrefsPlayerId();
        var loginToken = GetPrefsLoginToken();
        HandleServiceCall();
        DoValidateLoginToken(playerId, loginToken, refreshToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// SetProfileName
    /// </summary>
    /// <param name="profileName"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void SetProfileName(string profileName, UnityAction<PlayerResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: SetProfileName");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoSetProfileName(playerId, loginToken, profileName, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// LevelUpItem
    /// </summary>
    /// <param name="itemId">`playerItem.Id` for item which will levelling up</param>
    /// <param name="materials">This is Key-Value Pair for `playerItem.Id`, `Using Amount`</param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void LevelUpItem(string itemId, Dictionary<string, int> materials, UnityAction<ItemResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: LevelUpItem");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoLevelUpItem(playerId, loginToken, itemId, materials, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// EvolveItem
    /// </summary>
    /// <param name="itemId">`playerItem.Id` for item which will evolving</param>
    /// <param name="materials">This is Key-Value Pair for `playerItem.Id`, `Using Amount`</param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void EvolveItem(string itemId, Dictionary<string, int> materials, UnityAction<ItemResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: EvolveItem");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoEvolveItem(playerId, loginToken, itemId, materials, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// Sell Items
    /// </summary>
    /// <param name="items">List of `playerItem.Id` for items that will selling</param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void SellItems(Dictionary<string, int> items, UnityAction<ItemResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: SellItems");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoSellItems(playerId, loginToken, items, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// StartStage
    /// </summary>
    /// <param name="stageDataId"></param>
    /// <param name="formationName"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void StartStage(string stageDataId, UnityAction<StartStageResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: StartStage");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoStartStage(playerId, loginToken, stageDataId, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// FinishStage
    /// </summary>
    /// <param name="session"></param>
    /// <param name="battleResult"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void FinishStage(string session, ushort battleResult, int deadCharacters, UnityAction<FinishStageResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: FinishStage");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoFinishStage(playerId, loginToken, session, battleResult, deadCharacters, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// ReviveCharacters
    /// </summary>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void ReviveCharacters(UnityAction<CurrencyResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: ReviveCharacters");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoReviveCharacters(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// SelectFormation
    /// </summary>
    /// <param name="formationName"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void SelectFormation(string formationName, UnityAction<PlayerResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: SelectFormation");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoSelectFormation(playerId, loginToken, formationName, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// EquipItem
    /// </summary>
    /// <param name="characterId">`playerItem.Id` for character whom equipping equipment</param>
    /// <param name="equipmentId">`playerItem.Id` for equipment which will be equipped</param>
    /// <param name="equipPosition">Equipping position</param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void EquipItem(string characterId, string equipmentId, string equipPosition, UnityAction<ItemResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: EquipItem");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoEquipItem(playerId, loginToken, characterId, equipmentId, equipPosition, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// UnEquipItem
    /// </summary>
    /// <param name="equipmentId">`playerItem.Id` for equipment which will be unequipped</param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void UnEquipItem(string equipmentId, UnityAction<ItemResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: UnEquipItem");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoUnEquipItem(playerId, loginToken, equipmentId, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// OpenLootBox
    /// </summary>
    /// <param name="lootBoxDataId"></param>
    /// <param name="packIndex"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void OpenLootBox(string lootBoxDataId, int packIndex, UnityAction<ItemResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: OpenLootBox");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoOpenLootBox(playerId, loginToken, lootBoxDataId, packIndex, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// OpenIAPPackage_iOS
    /// </summary>
    /// <param name="receipt"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void OpenIapPackage_iOS(string iapPackageDataId, string receipt, UnityAction<ItemResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: OpenIAPPackage_iOS");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoOpenIapPackage_iOS(playerId, loginToken, iapPackageDataId, receipt, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// OpenIAPPackage_Android
    /// </summary>
    /// <param name="receipt"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void OpenIapPackage_Android(string iapPackageDataId, string data, string signature, UnityAction<ItemResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: DoOpenIAPPackage_Android");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoOpenIapPackage_Android(playerId, loginToken, iapPackageDataId, data, signature, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// GetAuthList
    /// </summary>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void GetAuthList(UnityAction<AuthListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetAuthList");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoGetAuthList(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// GetItemList
    /// </summary>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void GetItemList(UnityAction<ItemListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetItemList");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoGetItemList(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// GetCurrencyList
    /// </summary>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void GetCurrencyList(UnityAction<CurrencyListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetCurrencyList");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoGetCurrencyList(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// GetStaminaList
    /// </summary>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void GetStaminaList(UnityAction<StaminaListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetStaminaList");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoGetStaminaList(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// GetFormationList
    /// </summary>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void GetFormationList(UnityAction<FormationListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetFormationList");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoGetFormationList(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// SetFormation
    /// </summary>
    /// <param name="characterId"></param>
    /// <param name="formationName"></param>
    /// <param name="position"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void SetFormation(string characterId, string formationName, int position, UnityAction<FormationListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: SetFormation");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoSetFormation(playerId, loginToken, characterId, formationName, position, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// GetUnlockItemList
    /// </summary>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void GetUnlockItemList(UnityAction<UnlockItemListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetUnlockItemList");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoGetUnlockItemList(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// GetClearStageList
    /// </summary>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void GetClearStageList(UnityAction<ClearStageListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetClearStageList");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoGetClearStageList(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// GetAvailableLootBoxList
    /// </summary>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void GetAvailableLootBoxList(UnityAction<AvailableLootBoxListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetAvailableLootBoxList");
        HandleServiceCall();
        DoGetAvailableLootBoxList((finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    /// <summary>
    /// GetAvailableIapPackageList
    /// </summary>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    public void GetAvailableIapPackageList(UnityAction<AvailableIapPackageListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetAvailableIapPackageList");
        HandleServiceCall();
        DoGetAvailableIapPackageList((finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void GetHelperList(UnityAction<FriendListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetRandomPlayerList");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoGetHelperList(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void GetFriendList(UnityAction<FriendListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetFriendList");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoGetFriendList(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void GetFriendRequestList(UnityAction<FriendListResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: GetFriendRequestList");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoGetFriendRequestList(playerId, loginToken, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void FriendRequest(string targetPlayerId, UnityAction<GameServiceResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: SendFriendRequest");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoFriendRequest(playerId, loginToken, targetPlayerId, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void FriendAccept(string targetPlayerId, UnityAction<GameServiceResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: SendFriendAccept");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoFriendAccept(playerId, loginToken, targetPlayerId, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void FriendDecline(string targetPlayerId, UnityAction<GameServiceResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: SendFriendDecline");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoFriendDecline(playerId, loginToken, targetPlayerId, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void FriendDelete(string targetPlayerId, UnityAction<GameServiceResult> onSuccess = null, UnityAction<string> onError = null)
    {
        Debug.Log("Call Service: SendFriendDelete");
        var player = Player.CurrentPlayer;
        var playerId = player.Id;
        var loginToken = player.LoginToken;
        HandleServiceCall();
        DoFriendDelete(playerId, loginToken, targetPlayerId, (finishResult) => HandleResult(finishResult, onSuccess, onError));
    }

    public void GetServiceTime()
    {
        DoGetServiceTime((finishResult) =>
        {
            ServiceTimeOffset = finishResult.serviceTime - (System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond);
        });
    }
    
    protected abstract void DoRegister(string username, string password, UnityAction<PlayerResult> onFinish);
    protected abstract void DoLogin(string username, string password, UnityAction<PlayerResult> onFinish);
    protected abstract void DoRegisterOrLogin(string username, string password, UnityAction<PlayerResult> onFinish);
    protected abstract void DoGuestLogin(string deviceId, UnityAction<PlayerResult> onFinish);
    protected abstract void DoValidateLoginToken(string playerId, string loginToken, bool refreshToken, UnityAction<PlayerResult> onFinish);
    protected abstract void DoSetProfileName(string playerId, string loginToken, string profileName, UnityAction<PlayerResult> onFinish);
    protected abstract void DoLevelUpItem(string playerId, string loginToken, string itemId, Dictionary<string, int> materials, UnityAction<ItemResult> onFinish);
    protected abstract void DoEvolveItem(string playerId, string loginToken, string itemId, Dictionary<string, int> materials, UnityAction<ItemResult> onFinish);
    protected abstract void DoSellItems(string playerId, string loginToken, Dictionary<string, int> items, UnityAction<ItemResult> onFinish);
    protected abstract void DoStartStage(string playerId, string loginToken, string stageDataId, UnityAction<StartStageResult> onFinish);
    protected abstract void DoFinishStage(string playerId, string loginToken, string session, ushort battleResult, int deadCharacters, UnityAction<FinishStageResult> onFinish);
    protected abstract void DoReviveCharacters(string playerId, string loginToken, UnityAction<CurrencyResult> onFinish);
    protected abstract void DoSelectFormation(string playerId, string loginToken, string formationName, UnityAction<PlayerResult> onFinish);
    protected abstract void DoEquipItem(string playerId, string loginToken, string characterId, string equipmentId, string equipPosition, UnityAction<ItemResult> onFinish);
    protected abstract void DoUnEquipItem(string playerId, string loginToken, string equipmentId, UnityAction<ItemResult> onFinish);
    protected abstract void DoGetAuthList(string playerId, string loginToken, UnityAction<AuthListResult> onFinish);
    protected abstract void DoGetItemList(string playerId, string loginToken, UnityAction<ItemListResult> onFinish);
    protected abstract void DoGetCurrencyList(string playerId, string loginToken, UnityAction<CurrencyListResult> onFinish);
    protected abstract void DoGetStaminaList(string playerId, string loginToken, UnityAction<StaminaListResult> onFinish);
    protected abstract void DoGetFormationList(string playerId, string loginToken, UnityAction<FormationListResult> onFinish);
    protected abstract void DoGetUnlockItemList(string playerId, string loginToken, UnityAction<UnlockItemListResult> onFinish);
    protected abstract void DoGetClearStageList(string playerId, string loginToken, UnityAction<ClearStageListResult> onFinish);
    protected abstract void DoGetAvailableLootBoxList(UnityAction<AvailableLootBoxListResult> onFinish);
    protected abstract void DoGetAvailableIapPackageList(UnityAction<AvailableIapPackageListResult> onFinish);
    protected abstract void DoSetFormation(string playerId, string loginToken, string characterId, string formationName, int position, UnityAction<FormationListResult> onFinish);
    protected abstract void DoOpenLootBox(string playerId, string loginToken, string lootBoxDataId, int packIndex, UnityAction<ItemResult> onFinish);
    protected abstract void DoOpenIapPackage_iOS(string playerId, string loginToken, string iapPackageDataId, string receipt, UnityAction<ItemResult> onFinish);
    protected abstract void DoOpenIapPackage_Android(string playerId, string loginToken, string iapPackageDataId, string data, string signature, UnityAction<ItemResult> onFinish);
    protected abstract void DoGetHelperList(string playerId, string loginToken, UnityAction<FriendListResult> onFinish);
    protected abstract void DoGetFriendList(string playerId, string loginToken, UnityAction<FriendListResult> onFinish);
    protected abstract void DoGetFriendRequestList(string playerId, string loginToken, UnityAction<FriendListResult> onFinish);
    protected abstract void DoFriendRequest(string playerId, string loginToken, string targetPlayerId, UnityAction<GameServiceResult> onFinish);
    protected abstract void DoFriendAccept(string playerId, string loginToken, string targetPlayerId, UnityAction<GameServiceResult> onFinish);
    protected abstract void DoFriendDecline(string playerId, string loginToken, string targetPlayerId, UnityAction<GameServiceResult> onFinish);
    protected abstract void DoFriendDelete(string playerId, string loginToken, string targetPlayerId, UnityAction<GameServiceResult> onFinish);
    protected abstract void DoGetServiceTime(UnityAction<ServiceTimeResult> onFinish);
}
