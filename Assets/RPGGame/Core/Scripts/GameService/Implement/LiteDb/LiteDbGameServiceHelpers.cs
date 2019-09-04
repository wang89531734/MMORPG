using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class LiteDbGameService
{
    private bool IsPlayerWithUsernameFound(string type, string username)
    {
        var playerAuth = colPlayerAuth.FindOne(a => a.Type == type && a.Username == username);
        return playerAuth != null;
    }

    private DbPlayer SetNewPlayerData(DbPlayer player)
    {
        player.LoginToken = System.Guid.NewGuid().ToString();
        player.Exp = 0;

        var gameDb = GameInstance.GameDatabase;
        var softCurrencyTable = gameDb.softCurrency;
        var hardCurrencyTable = gameDb.hardCurrency;

        var formationName = gameDb.stageFormations[0].id;
        player.SelectedFormation = formationName;

        var softCurrency = GetCurrency(player.Id, softCurrencyTable.id);
        var hardCurrency = GetCurrency(player.Id, hardCurrencyTable.id);
        softCurrency.Amount = softCurrencyTable.startAmount + softCurrency.PurchasedAmount;
        hardCurrency.Amount = hardCurrencyTable.startAmount + hardCurrency.PurchasedAmount;
        colPlayerCurrency.Update(softCurrency);
        colPlayerCurrency.Update(hardCurrency);

        colPlayerClearStage.Delete(a => a.PlayerId == player.Id);
        colPlayerFormation.Delete(a => a.PlayerId == player.Id);
        colPlayerItem.Delete(a => a.PlayerId == player.Id);
        colPlayerStamina.Delete(a => a.PlayerId == player.Id);
        colPlayerUnlockItem.Delete(a => a.PlayerId == player.Id);

        for (var i = 0; i < gameDb.startItems.Count; ++i)
        {
            var startItem = gameDb.startItems[i];
            if (startItem == null || startItem.item == null)
                continue;
            var createItems = new List<DbPlayerItem>();
            var updateItems = new List<DbPlayerItem>();
            if (AddItems(player.Id, startItem.Id, startItem.amount, out createItems, out updateItems))
            {
                foreach (var createEntry in createItems)
                {
                    createEntry.Id = System.Guid.NewGuid().ToString();
                    colPlayerItem.Insert(createEntry);
                    HelperUnlockItem(player.Id, startItem.Id);
                }
                foreach (var updateEntry in updateItems)
                {
                    colPlayerItem.Update(updateEntry);
                }
            }
        }
        for (var i = 0; i < gameDb.startCharacters.Count; ++i)
        {
            var startCharacter = gameDb.startCharacters[i];
            if (startCharacter == null)
                continue;
            var createItems = new List<DbPlayerItem>();
            var updateItems = new List<DbPlayerItem>();
            if (AddItems(player.Id, startCharacter.Id, 1, out createItems, out updateItems))
            {
                foreach (var createEntry in createItems)
                {
                    createEntry.Id = System.Guid.NewGuid().ToString();
                    colPlayerItem.Insert(createEntry);
                    HelperUnlockItem(player.Id, startCharacter.Id);
                    HelperSetFormation(player.Id, createEntry.Id, formationName, i);
                }
                foreach (var updateEntry in updateItems)
                {
                    colPlayerItem.Update(updateEntry);
                }
            }
        }
        colPlayer.Update(player);
        return player;
    }

    private DbPlayer InsertNewPlayer(string type, string username, string password)
    {
        var playerId = System.Guid.NewGuid().ToString();
        var playerAuth = new DbPlayerAuth();
        playerAuth.Id = PlayerAuth.GetId(playerId, type);
        playerAuth.PlayerId = playerId;
        playerAuth.Type = type;
        playerAuth.Username = username;
        playerAuth.Password = password;
        colPlayerAuth.Insert(playerAuth);
        var player = new DbPlayer();
        player.Id = playerId;
        player = SetNewPlayerData(player);
        UpdatePlayerStamina(player);
        colPlayer.Insert(player);
        return player;
    }

    private bool TryGetPlayer(string type, string username, string password, out DbPlayer player)
    {
        player = null;
        var playerAuth = colPlayerAuth.FindOne(a => a.Type == type && a.Username == username && a.Password == password);
        if (playerAuth == null)
            return false;
        player = colPlayer.FindOne(a => a.Id == playerAuth.PlayerId);
        if (player == null)
            return false;
        return true;
    }

    private DbPlayer UpdatePlayerLoginToken(DbPlayer player)
    {
        player.LoginToken = System.Guid.NewGuid().ToString();
        colPlayer.Update(player);
        return player;
    }

    private bool DecreasePlayerStamina(DbPlayer player, Stamina staminaTable, int decreaseAmount)
    {
        var gameDb = GameInstance.GameDatabase;
        var stamina = GetStamina(player.Id, staminaTable.id);
        var gamePlayer = new Player();
        Player.CloneTo(player, gamePlayer);
        var maxStamina = staminaTable.maxAmountTable.Calculate(gamePlayer.Level, gameDb.playerMaxLevel);
        if (stamina.Amount >= decreaseAmount)
        {
            if (stamina.Amount == maxStamina && stamina.Amount - decreaseAmount < maxStamina)
                stamina.RecoveredTime = System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;
            stamina.Amount -= decreaseAmount;
            colPlayerStamina.Update(stamina);
            UpdatePlayerStamina(player, staminaTable);
            return true;
        }
        return false;
    }

    private void UpdatePlayerStamina(DbPlayer player, Stamina staminaTable)
    {
        var gameDb = GameInstance.GameDatabase;

        var stamina = GetStamina(player.Id, staminaTable.id);
        var gamePlayer = new Player();
        Player.CloneTo(player, gamePlayer);
        var maxStamina = staminaTable.maxAmountTable.Calculate(gamePlayer.Level, gameDb.playerMaxLevel);
        if (stamina.Amount < maxStamina)
        {
            var currentTimeInMillisecond = System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;
            var diffTimeInMillisecond = currentTimeInMillisecond - stamina.RecoveredTime;
            var devideAmount = 1;
            switch (staminaTable.recoverUnit)
            {
                case StaminaUnit.Days:
                    devideAmount = 1000 * 60 * 60 * 24;
                    break;
                case StaminaUnit.Hours:
                    devideAmount = 1000 * 60 * 60;
                    break;
                case StaminaUnit.Minutes:
                    devideAmount = 1000 * 60;
                    break;
                case StaminaUnit.Seconds:
                    devideAmount = 1000;
                    break;
            }
            var recoveryAmount = (int)(diffTimeInMillisecond / devideAmount) / staminaTable.recoverDuration;
            if (recoveryAmount > 0)
            {
                stamina.Amount += recoveryAmount;
                if (stamina.Amount > maxStamina)
                    stamina.Amount = maxStamina;
                stamina.RecoveredTime = currentTimeInMillisecond;
                colPlayerStamina.Update(stamina);
            }
        }
    }

    private void UpdatePlayerStamina(DbPlayer player)
    {
        var gameDb = GameInstance.GameDatabase;
        var stageStaminaTable = gameDb.stageStamina;
        UpdatePlayerStamina(player, stageStaminaTable);
    }

    private DbPlayerCurrency GetCurrency(string playerId, string dataId)
    {
        var currency = colPlayerCurrency.FindOne(a => a.PlayerId == playerId && a.DataId == dataId);
        if (currency == null)
        {
            currency = new DbPlayerCurrency();
            currency.Id = PlayerCurrency.GetId(playerId, dataId);
            currency.PlayerId = playerId;
            currency.DataId = dataId;
            colPlayerCurrency.Insert(currency);
        }
        return currency;
    }

    private DbPlayerStamina GetStamina(string playerId, string dataId)
    {
        var stamina = colPlayerStamina.FindOne(a => a.PlayerId == playerId && a.DataId == dataId);
        if (stamina == null)
        {
            stamina = new DbPlayerStamina();
            stamina.Id = PlayerStamina.GetId(playerId, dataId);
            stamina.PlayerId = playerId;
            stamina.DataId = dataId;
            colPlayerStamina.Insert(stamina);
        }
        return stamina;
    }

    private bool AddItems(string playerId,
        string dataId,
        int amount,
        out List<DbPlayerItem> createItems,
        out List<DbPlayerItem> updateItems)
    {
        createItems = new List<DbPlayerItem>();
        updateItems = new List<DbPlayerItem>();
        BaseItem itemData = null;
        if (!GameInstance.GameDatabase.Items.TryGetValue(dataId, out itemData))
            return false;
        var maxStack = itemData.MaxStack;
        var oldEntries = colPlayerItem.Find(a => a.DataId == dataId && a.PlayerId == playerId && a.Amount < maxStack);
        foreach (var entry in oldEntries)
        {
            var sumAmount = entry.Amount + amount;
            if (sumAmount > maxStack)
            {
                entry.Amount = maxStack;
                amount = sumAmount - maxStack;
            }
            else
            {
                entry.Amount += amount;
                amount = 0;
            }
            updateItems.Add(entry);

            if (amount == 0)
                break;
        }
        while (amount > 0)
        {
            var newEntry = new DbPlayerItem();
            newEntry.PlayerId = playerId;
            newEntry.DataId = dataId;
            if (amount > maxStack)
            {
                newEntry.Amount = maxStack;
                amount -= maxStack;
            }
            else
            {
                newEntry.Amount = amount;
                amount = 0;
            }
            createItems.Add(newEntry);
        }
        return true;
    }

    private bool UseItems(string playerId,
        string dataId,
        int amount,
        out List<DbPlayerItem> updateItem,
        out List<string> deleteItemIds,
        bool conditionCanLevelUp = false,
        bool conditionCanEvolve = false,
        bool conditionCanSell = false,
        bool conditionCanBeMaterial = false,
        bool conditionCanBeEquipped = false)
    {
        updateItem = new List<DbPlayerItem>();
        deleteItemIds = new List<string>();
        if (!GameInstance.GameDatabase.Items.ContainsKey(dataId))
            return false;
        var materials = colPlayerItem.Find(a => a.DataId == dataId && a.PlayerId == playerId);
        foreach (var material in materials)
        {
            var gamePlayerItem = new PlayerItem();
            PlayerItem.CloneTo(material, gamePlayerItem);

            if ((!conditionCanLevelUp || gamePlayerItem.CanLevelUp) &&
                (!conditionCanEvolve || gamePlayerItem.CanEvolve) &&
                (!conditionCanSell || gamePlayerItem.CanSell) &&
                (!conditionCanBeMaterial || gamePlayerItem.CanBeMaterial) &&
                (!conditionCanBeEquipped || gamePlayerItem.CanBeEquipped))
            {
                if (material.Amount >= amount)
                {
                    material.Amount -= amount;
                    amount = 0;
                }
                else
                {
                    amount -= material.Amount;
                    material.Amount = 0;
                }

                if (material.Amount > 0)
                    updateItem.Add(material);
                else
                    deleteItemIds.Add(material.Id);

                if (amount == 0)
                    break;
            }
        }
        if (amount > 0)
            return false;
        return true;
    }

    private void HelperSetFormation(string playerId, string characterId, string formationName, int position)
    {
        DbPlayerFormation oldFormation = null;
        if (!string.IsNullOrEmpty(characterId))
        {
            oldFormation = colPlayerFormation.FindOne(a => a.PlayerId == playerId && a.DataId == formationName && a.ItemId == characterId);
            if (oldFormation != null)
            {
                oldFormation.ItemId = string.Empty;
                colPlayerFormation.Update(oldFormation);
            }
        }
        var formation = colPlayerFormation.FindOne(a => a.PlayerId == playerId && a.DataId == formationName && a.Position == position);
        if (formation == null)
        {
            formation = new DbPlayerFormation();
            formation.Id = PlayerFormation.GetId(playerId, formationName, position);
            formation.PlayerId = playerId;
            formation.DataId = formationName;
            formation.Position = position;
            formation.ItemId = characterId;
            colPlayerFormation.Insert(formation);
        }
        else
        {
            if (oldFormation != null)
            {
                oldFormation.ItemId = formation.ItemId;
                colPlayerFormation.Update(oldFormation);
            }
            formation.ItemId = characterId;
            colPlayerFormation.Update(formation);
        }
    }

    private DbPlayerUnlockItem HelperUnlockItem(string playerId, string dataId)
    {
        var unlockItem = colPlayerUnlockItem.FindById(PlayerUnlockItem.GetId(playerId, dataId));
        if (unlockItem == null)
        {
            unlockItem = new DbPlayerUnlockItem();
            unlockItem.Id = PlayerUnlockItem.GetId(playerId, dataId);
            unlockItem.PlayerId = playerId;
            unlockItem.DataId = dataId;
            unlockItem.Amount = 0;
            colPlayerUnlockItem.Insert(unlockItem);
        }
        return unlockItem;
    }

    private DbPlayerClearStage HelperClearStage(string playerId, string dataId, int grade)
    {
        var clearStage = colPlayerClearStage.FindById(PlayerClearStage.GetId(playerId, dataId));
        if (clearStage == null)
        {
            clearStage = new DbPlayerClearStage();
            clearStage.Id = PlayerClearStage.GetId(playerId, dataId);
            clearStage.PlayerId = playerId;
            clearStage.DataId = dataId;
            clearStage.BestRating = grade;
            colPlayerClearStage.Insert(clearStage);
        }
        else
        {
            if (clearStage.BestRating < grade)
            {
                clearStage.BestRating = grade;
                colPlayerClearStage.Update(clearStage);
            }
        }
        return clearStage;
    }
}
