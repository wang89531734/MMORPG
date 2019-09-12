using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LiteDB;

public partial class LiteDbGameService : BaseGameService
{
    public string dbPath = "./tbRpgDb.db";
    private LiteDatabase db;
    private LiteCollection<DbPlayer> colPlayer;
    private LiteCollection<DbPlayerItem> colPlayerItem;
    private LiteCollection<DbPlayerAuth> colPlayerAuth;
    private LiteCollection<DbPlayerCurrency> colPlayerCurrency;
    private LiteCollection<DbPlayerStamina> colPlayerStamina;
    private LiteCollection<DbPlayerFormation> colPlayerFormation;
    private LiteCollection<DbPlayerUnlockItem> colPlayerUnlockItem;
    private LiteCollection<DbPlayerClearStage> colPlayerClearStage;
    private LiteCollection<DbPlayerBattle> colPlayerBattle;

    private void Awake()
    {
        if (Application.isMobilePlatform)
        {
            if (dbPath.StartsWith("./"))
                dbPath = dbPath.Substring(1);
            if (!dbPath.StartsWith("/"))
                dbPath = "/" + dbPath;
            dbPath = Application.persistentDataPath + dbPath;
        }
        db = new LiteDatabase(dbPath);
        colPlayer = db.GetCollection<DbPlayer>("player");
        colPlayerItem = db.GetCollection<DbPlayerItem>("playerItem");
        colPlayerAuth = db.GetCollection<DbPlayerAuth>("playerAuth");
        colPlayerCurrency = db.GetCollection<DbPlayerCurrency>("playerCurrency");
        colPlayerStamina = db.GetCollection<DbPlayerStamina>("playerStamina");
        colPlayerFormation = db.GetCollection<DbPlayerFormation>("playerFormation");
        colPlayerUnlockItem = db.GetCollection<DbPlayerUnlockItem>("playerUnlockItem");
        colPlayerClearStage = db.GetCollection<DbPlayerClearStage>("playerClearStage");
        colPlayerBattle = db.GetCollection<DbPlayerBattle>("playerBattle");
    }

    protected override void DoGetAuthList(string playerId, string loginToken, UnityAction<AuthListResult> onFinish)
    {
        var result = new AuthListResult();
        var player = colPlayer.FindOne(a => a.Id == playerId && a.LoginToken == loginToken);
        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else
            result.list.AddRange(DbPlayerAuth.CloneList(colPlayerAuth.Find(a => a.PlayerId == playerId)));
        onFinish(result);
    }

    protected override void DoGetItemList(string playerId, string loginToken, UnityAction<ItemListResult> onFinish)
    {
        var result = new ItemListResult();
        var player = colPlayer.FindOne(a => a.Id == playerId && a.LoginToken == loginToken);
        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else
            result.list.AddRange(DbPlayerItem.CloneList(colPlayerItem.Find(a => a.PlayerId == playerId)));
        onFinish(result);
    }

    protected override void DoGetCurrencyList(string playerId, string loginToken, UnityAction<CurrencyListResult> onFinish)
    {
        var result = new CurrencyListResult();
        var player = colPlayer.FindOne(a => a.Id == playerId && a.LoginToken == loginToken);
        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else
            result.list.AddRange(DbPlayerCurrency.CloneList(colPlayerCurrency.Find(a => a.PlayerId == playerId)));
        onFinish(result);
    }

    protected override void DoGetStaminaList(string playerId, string loginToken, UnityAction<StaminaListResult> onFinish)
    {
        var result = new StaminaListResult();
        var player = colPlayer.FindOne(a => a.Id == playerId && a.LoginToken == loginToken);
        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else
            result.list.AddRange(DbPlayerStamina.CloneList(colPlayerStamina.Find(a => a.PlayerId == playerId)));
        onFinish(result);
    }

    protected override void DoGetFormationList(string playerId, string loginToken, UnityAction<FormationListResult> onFinish)
    {
        var result = new FormationListResult();
        var player = colPlayer.FindOne(a => a.Id == playerId && a.LoginToken == loginToken);
        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else
            result.list.AddRange(DbPlayerFormation.CloneList(colPlayerFormation.Find(a => a.PlayerId == playerId)));
        onFinish(result);
    }

    protected override void DoGetUnlockItemList(string playerId, string loginToken, UnityAction<UnlockItemListResult> onFinish)
    {
        var result = new UnlockItemListResult();
        var player = colPlayer.FindOne(a => a.Id == playerId && a.LoginToken == loginToken);
        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else
            result.list.AddRange(DbPlayerUnlockItem.CloneList(colPlayerUnlockItem.Find(a => a.PlayerId == playerId)));
        onFinish(result);
    }

    protected override void DoGetClearStageList(string playerId, string loginToken, UnityAction<ClearStageListResult> onFinish)
    {
        var result = new ClearStageListResult();
        var player = colPlayer.FindOne(a => a.Id == playerId && a.LoginToken == loginToken);
        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else
            result.list.AddRange(DbPlayerClearStage.CloneList(colPlayerClearStage.Find(a => a.PlayerId == playerId)));
        onFinish(result);
    }

    protected override void DoGetServiceTime(UnityAction<ServiceTimeResult> onFinish)
    {
        var result = new ServiceTimeResult();
        result.serviceTime = System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;
        onFinish(result);
    }
}
