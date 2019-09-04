using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;

public partial class SQLiteGameService
{
    private bool IsPlayerWithUsernameFound(string type, string username)
    {
        var count = ExecuteScalar(@"SELECT COUNT(*) FROM playerAuth WHERE type=@type AND username=@username",
            new SqliteParameter("@type", type),
            new SqliteParameter("@username", username));
        return count != null && (long)count > 0;
    }

    private Player SetNewPlayerData(Player player)
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

        ExecuteNonQuery(@"UPDATE playerCurrency SET amount=@amount WHERE id=@id",
            new SqliteParameter("@amount", softCurrency.Amount),
            new SqliteParameter("@id", softCurrency.Id));
        ExecuteNonQuery(@"UPDATE playerCurrency SET amount=@amount WHERE id=@id",
            new SqliteParameter("@amount", hardCurrency.Amount),
            new SqliteParameter("@id", hardCurrency.Id));

        ExecuteNonQuery(@"DELETE FROM playerClearStage WHERE playerId=@playerId",
            new SqliteParameter("@playerId", player.Id));
        ExecuteNonQuery(@"DELETE FROM playerFormation WHERE playerId=@playerId",
            new SqliteParameter("@playerId", player.Id));
        ExecuteNonQuery(@"DELETE FROM playerItem WHERE playerId=@playerId",
            new SqliteParameter("@playerId", player.Id));
        ExecuteNonQuery(@"DELETE FROM playerStamina WHERE playerId=@playerId",
            new SqliteParameter("@playerId", player.Id));
        ExecuteNonQuery(@"DELETE FROM playerUnlockItem WHERE playerId=@playerId",
            new SqliteParameter("@playerId", player.Id));

        for (var i = 0; i < gameDb.startItems.Count; ++i)
        {
            var startItem = gameDb.startItems[i];
            if (startItem == null || startItem.item == null)
                continue;
            var createItems = new List<PlayerItem>();
            var updateItems = new List<PlayerItem>();
            if (AddItems(player.Id, startItem.Id, startItem.amount, out createItems, out updateItems))
            {
                foreach (var createEntry in createItems)
                {
                    createEntry.Id = System.Guid.NewGuid().ToString();
                    ExecuteNonQuery(@"INSERT INTO playerItem (id, playerId, dataId, amount, exp, equipItemId, equipPosition) VALUES (@id, @playerId, @dataId, @amount, @exp, @equipItemId, @equipPosition)",
                        new SqliteParameter("@id", createEntry.Id),
                        new SqliteParameter("@playerId", createEntry.PlayerId),
                        new SqliteParameter("@dataId", createEntry.DataId),
                        new SqliteParameter("@amount", createEntry.Amount),
                        new SqliteParameter("@exp", createEntry.Exp),
                        new SqliteParameter("@equipItemId", createEntry.EquipItemId),
                        new SqliteParameter("@equipPosition", createEntry.EquipPosition));
                    HelperUnlockItem(player.Id, startItem.Id);
                }
                foreach (var updateEntry in updateItems)
                {
                    ExecuteNonQuery(@"UPDATE playerItem SET playerId=@playerId, dataId=@dataId, amount=@amount, exp=@exp, equipItemId=@equipItemId, equipPosition=@equipPosition WHERE id=@id",
                        new SqliteParameter("@playerId", updateEntry.PlayerId),
                        new SqliteParameter("@dataId", updateEntry.DataId),
                        new SqliteParameter("@amount", updateEntry.Amount),
                        new SqliteParameter("@exp", updateEntry.Exp),
                        new SqliteParameter("@equipItemId", updateEntry.EquipItemId),
                        new SqliteParameter("@equipPosition", updateEntry.EquipPosition),
                        new SqliteParameter("@id", updateEntry.Id));
                }
            }
        }
        for (var i = 0; i < gameDb.startCharacters.Count; ++i)
        {
            var startCharacter = gameDb.startCharacters[i];
            if (startCharacter == null)
                continue;
            var createItems = new List<PlayerItem>();
            var updateItems = new List<PlayerItem>();
            if (AddItems(player.Id, startCharacter.Id, 1, out createItems, out updateItems))
            {
                foreach (var createEntry in createItems)
                {
                    createEntry.Id = System.Guid.NewGuid().ToString();
                    ExecuteNonQuery(@"INSERT INTO playerItem (id, playerId, dataId, amount, exp, equipItemId, equipPosition) VALUES (@id, @playerId, @dataId, @amount, @exp, @equipItemId, @equipPosition)",
                        new SqliteParameter("@id", createEntry.Id),
                        new SqliteParameter("@playerId", createEntry.PlayerId),
                        new SqliteParameter("@dataId", createEntry.DataId),
                        new SqliteParameter("@amount", createEntry.Amount),
                        new SqliteParameter("@exp", createEntry.Exp),
                        new SqliteParameter("@equipItemId", createEntry.EquipItemId),
                        new SqliteParameter("@equipPosition", createEntry.EquipPosition));
                    HelperUnlockItem(player.Id, startCharacter.Id);
                    HelperSetFormation(player.Id, createEntry.Id, formationName, i);
                }
                foreach (var updateEntry in updateItems)
                {
                    ExecuteNonQuery(@"UPDATE playerItem SET playerId=@playerId, dataId=@dataId, amount=@amount, exp=@exp, equipItemId=@equipItemId, equipPosition=@equipPosition WHERE id=@id",
                        new SqliteParameter("@playerId", updateEntry.PlayerId),
                        new SqliteParameter("@dataId", updateEntry.DataId),
                        new SqliteParameter("@amount", updateEntry.Amount),
                        new SqliteParameter("@exp", updateEntry.Exp),
                        new SqliteParameter("@equipItemId", updateEntry.EquipItemId),
                        new SqliteParameter("@equipPosition", updateEntry.EquipPosition),
                        new SqliteParameter("@id", updateEntry.Id));
                }
            }
        }
        ExecuteNonQuery(@"UPDATE player SET profileName=@profileName, loginToken=@loginToken, exp=@exp, selectedFormation=@selectedFormation WHERE id=@id",
            new SqliteParameter("@profileName", player.ProfileName),
            new SqliteParameter("@loginToken", player.LoginToken),
            new SqliteParameter("@exp", player.Exp),
            new SqliteParameter("@selectedFormation", player.SelectedFormation),
            new SqliteParameter("@id", player.Id));
        return player;
    }

    private Player InsertNewPlayer(string type, string username, string password)
    {
        var playerId = System.Guid.NewGuid().ToString();
        var playerAuth = new PlayerAuth();
        playerAuth.Id = PlayerAuth.GetId(playerId, type);
        playerAuth.PlayerId = playerId;
        playerAuth.Type = type;
        playerAuth.Username = username;
        playerAuth.Password = password;
        ExecuteNonQuery(@"INSERT INTO playerAuth (id, playerId, type, username, password) VALUES (@id, @playerId, @type, @username, @password)",
            new SqliteParameter("@id", playerAuth.Id),
            new SqliteParameter("@playerId", playerAuth.PlayerId),
            new SqliteParameter("@type", playerAuth.Type),
            new SqliteParameter("@username", playerAuth.Username),
            new SqliteParameter("@password", playerAuth.Password));
        var player = new Player();
        player.Id = playerId;
        player = SetNewPlayerData(player);
        UpdatePlayerStamina(player);
        ExecuteNonQuery(@"INSERT INTO player (id, profileName, loginToken, exp, selectedFormation) VALUES (@id, @profileName, @loginToken, @exp, @selectedFormation)",
            new SqliteParameter("@id", player.Id),
            new SqliteParameter("@profileName", player.ProfileName),
            new SqliteParameter("@loginToken", player.LoginToken),
            new SqliteParameter("@exp", player.Exp),
            new SqliteParameter("@selectedFormation", player.SelectedFormation));
        return player;
    }

    private bool TryGetPlayer(string type, string username, string password, out Player player)
    {
        player = null;
        var playerAuths = ExecuteReader(@"SELECT * FROM playerAuth WHERE type=@type AND username=@username AND password=@password",
            new SqliteParameter("@type", type),
            new SqliteParameter("@username", username),
            new SqliteParameter("@password", password));
        if (!playerAuths.Read())
            return false;
        var playerAuth = new PlayerAuth();
        playerAuth.Id = playerAuths.GetString(0);
        playerAuth.PlayerId = playerAuths.GetString(1);
        playerAuth.Type = playerAuths.GetString(2);
        playerAuth.Username = playerAuths.GetString(3);
        playerAuth.Password = playerAuths.GetString(4);
        var players = ExecuteReader(@"SELECT * FROM player WHERE id=@id",
            new SqliteParameter("@id", playerAuth.PlayerId));
        if (players.Read())
        {
            player = new Player();
            player.Id = players.GetString(0);
            player.ProfileName = players.GetString(1);
            player.LoginToken = players.GetString(2);
            player.Exp = players.GetInt32(3);
            player.SelectedFormation = players.GetString(4);
        }
        if (player == null)
            return false;
        return true;
    }

    private Player UpdatePlayerLoginToken(Player player)
    {
        player.LoginToken = System.Guid.NewGuid().ToString();
        ExecuteNonQuery(@"UPDATE player SET loginToken=@loginToken WHERE id=@id",
            new SqliteParameter("@loginToken", player.loginToken),
            new SqliteParameter("@id", player.Id));
        return player;
    }

    private bool DecreasePlayerStamina(Player player, Stamina staminaTable, int decreaseAmount)
    {
        var gameDb = GameInstance.GameDatabase;
        var stamina = GetStamina(player.Id, staminaTable.id);
        var maxStamina = staminaTable.maxAmountTable.Calculate(player.Level, gameDb.playerMaxLevel);
        if (stamina.Amount >= decreaseAmount)
        {
            if (stamina.Amount == maxStamina && stamina.Amount - decreaseAmount < maxStamina)
                stamina.RecoveredTime = System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;
            stamina.Amount -= decreaseAmount;
            ExecuteNonQuery(@"UPDATE playerStamina SET amount=@amount, recoveredTime=@recoveredTime WHERE id=@id",
                new SqliteParameter("@amount", stamina.Amount),
                new SqliteParameter("@recoveredTime", stamina.RecoveredTime),
                new SqliteParameter("@id", stamina.Id));
            UpdatePlayerStamina(player, staminaTable);
            return true;
        }
        return false;
    }

    private void UpdatePlayerStamina(Player player, Stamina staminaTable)
    {
        var gameDb = GameInstance.GameDatabase;
        var stamina = GetStamina(player.Id, staminaTable.id);
        var maxStamina = staminaTable.maxAmountTable.Calculate(player.Level, gameDb.playerMaxLevel);
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
                ExecuteNonQuery(@"UPDATE playerStamina SET amount=@amount, recoveredTime=@recoveredTime WHERE id=@id",
                    new SqliteParameter("@amount", stamina.Amount),
                    new SqliteParameter("@recoveredTime", stamina.RecoveredTime),
                    new SqliteParameter("@id", stamina.Id));
            }
        }
    }

    private void UpdatePlayerStamina(Player player)
    {
        var gameDb = GameInstance.GameDatabase;
        var stageStaminaTable = gameDb.stageStamina;
        UpdatePlayerStamina(player, stageStaminaTable);
    }

    private PlayerCurrency GetCurrency(string playerId, string dataId)
    {
        var currencies = ExecuteReader(@"SELECT * FROM playerCurrency WHERE playerId=@playerId AND dataId=@dataId LIMIT 1",
            new SqliteParameter("@playerId", playerId),
            new SqliteParameter("@dataId", dataId));
        PlayerCurrency currency = null;
        if (!currencies.Read())
        {
            currency = new PlayerCurrency();
            currency.Id = PlayerCurrency.GetId(playerId, dataId);
            currency.PlayerId = playerId;
            currency.DataId = dataId;
            ExecuteNonQuery(@"INSERT INTO playerCurrency (id, playerId, dataId, amount, purchasedAmount) VALUES (@id, @playerId, @dataId, @amount, @purchasedAmount)",
                new SqliteParameter("@id", currency.Id),
                new SqliteParameter("@playerId", currency.PlayerId),
                new SqliteParameter("@dataId", currency.DataId),
                new SqliteParameter("@amount", currency.Amount),
                new SqliteParameter("@purchasedAmount", currency.PurchasedAmount));
        }
        else
        {
            currency = new PlayerCurrency();
            currency.Id = currencies.GetString(0);
            currency.PlayerId = currencies.GetString(1);
            currency.DataId = currencies.GetString(2);
            currency.Amount = currencies.GetInt32(3);
            currency.PurchasedAmount = currencies.GetInt32(4);
        }
        return currency;
    }

    private PlayerStamina GetStamina(string playerId, string dataId)
    {
        var staminas = ExecuteReader(@"SELECT * FROM playerStamina WHERE playerId=@playerId AND dataId=@dataId LIMIT 1",
            new SqliteParameter("@playerId", playerId),
            new SqliteParameter("@dataId", dataId));
        PlayerStamina stamina = null;
        if (!staminas.Read())
        {
            stamina = new PlayerStamina();
            stamina.Id = PlayerStamina.GetId(playerId, dataId);
            stamina.PlayerId = playerId;
            stamina.DataId = dataId;
            ExecuteNonQuery(@"INSERT INTO playerStamina (id, playerId, dataId, amount, recoveredTime) VALUES (@id, @playerId, @dataId, @amount, @recoveredTime)",
                new SqliteParameter("@id", stamina.Id),
                new SqliteParameter("@playerId", stamina.PlayerId),
                new SqliteParameter("@dataId", stamina.DataId),
                new SqliteParameter("@amount", stamina.Amount),
                new SqliteParameter("@recoveredTime", stamina.RecoveredTime));
        }
        else
        {
            stamina = new PlayerStamina();
            stamina.Id = staminas.GetString(0);
            stamina.PlayerId = staminas.GetString(1);
            stamina.DataId = staminas.GetString(2);
            stamina.Amount = staminas.GetInt32(3);
            stamina.RecoveredTime = staminas.GetInt64(4);
        }
        return stamina;
    }

    private bool AddItems(string playerId,
        string dataId,
        int amount,
        out List<PlayerItem> createItems,
        out List<PlayerItem> updateItems)
    {
        createItems = new List<PlayerItem>();
        updateItems = new List<PlayerItem>();
        BaseItem itemData = null;
        if (!GameInstance.GameDatabase.Items.TryGetValue(dataId, out itemData))
            return false;
        var maxStack = itemData.MaxStack;
        var oldEntries = ExecuteReader(@"SELECT * FROM playerItem WHERE playerId=@playerId AND dataId=@dataId AND amount<@amount",
            new SqliteParameter("@playerId", playerId),
            new SqliteParameter("@dataId", dataId),
            new SqliteParameter("@amount", maxStack));
        while (oldEntries.Read())
        {
            var entry = new PlayerItem();
            entry.Id = oldEntries.GetString(0);
            entry.PlayerId = oldEntries.GetString(1);
            entry.DataId = oldEntries.GetString(2);
            entry.Amount = oldEntries.GetInt32(3);
            entry.Exp = oldEntries.GetInt32(4);
            entry.EquipItemId = oldEntries.GetString(5);
            entry.EquipPosition = oldEntries.GetString(6);
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
            var newEntry = new PlayerItem();
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
        out List<PlayerItem> updateItem,
        out List<string> deleteItemIds,
        bool conditionCanLevelUp = false,
        bool conditionCanEvolve = false,
        bool conditionCanSell = false,
        bool conditionCanBeMaterial = false,
        bool conditionCanBeEquipped = false)
    {
        updateItem = new List<PlayerItem>();
        deleteItemIds = new List<string>();
        if (!GameInstance.GameDatabase.Items.ContainsKey(dataId))
            return false;
        var materials = ExecuteReader(@"SELECT * FROM playerItem WHERE playerId=@playerId AND dataId=@dataId",
            new SqliteParameter("@playerId", playerId),
            new SqliteParameter("@dataId", dataId));
        while (materials.Read())
        {
            var material = new PlayerItem();
            material.Id = materials.GetString(0);
            material.PlayerId = materials.GetString(1);
            material.DataId = materials.GetString(2);
            material.Amount = materials.GetInt32(3);
            material.Exp = materials.GetInt32(4);
            material.EquipItemId = materials.GetString(5);
            material.EquipPosition = materials.GetString(6);

            if ((!conditionCanLevelUp || material.CanLevelUp) &&
                (!conditionCanEvolve || material.CanEvolve) &&
                (!conditionCanSell || material.CanSell) &&
                (!conditionCanBeMaterial || material.CanBeMaterial) &&
                (!conditionCanBeEquipped || material.CanBeEquipped))
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
        PlayerFormation oldFormation = null;
            if (!string.IsNullOrEmpty(characterId))
        {
            var oldFormations = ExecuteReader(@"SELECT * FROM playerFormation WHERE playerId=@playerId AND dataId=@dataId AND itemId=@itemId",
                new SqliteParameter("@playerId", playerId),
                new SqliteParameter("@dataId", formationName),
                new SqliteParameter("@itemId", characterId));
            while (oldFormations.Read())
            {
                oldFormation = new PlayerFormation();
                oldFormation.Id = oldFormations.GetString(0);
                oldFormation.PlayerId = oldFormations.GetString(1);
                oldFormation.DataId = oldFormations.GetString(2);
                oldFormation.Position = oldFormations.GetInt32(3);
                oldFormation.ItemId = oldFormations.GetString(4);
            }
            if (oldFormation != null)
            {
                ExecuteNonQuery(@"UPDATE playerFormation SET itemId=@itemId WHERE id=@id",
                    new SqliteParameter("@itemId", ""),
                    new SqliteParameter("@id", oldFormation.Id));
            }
        }
        PlayerFormation formation = null;
        var targetFormations = ExecuteReader(@"SELECT * FROM playerFormation WHERE playerId=@playerId AND dataId=@dataId AND position=@position LIMIT 1",
            new SqliteParameter("@playerId", playerId),
            new SqliteParameter("@dataId", formationName),
            new SqliteParameter("@position", position));
        if (targetFormations.Read())
        {
            formation = new PlayerFormation();
            formation.Id = targetFormations.GetString(0);
            formation.PlayerId = targetFormations.GetString(1);
            formation.DataId = targetFormations.GetString(2);
            formation.Position = targetFormations.GetInt32(3);
            formation.ItemId = targetFormations.GetString(4);
        }
        if (formation == null)
        {
            formation = new PlayerFormation();
            formation.Id = PlayerFormation.GetId(playerId, formationName, position);
            formation.PlayerId = playerId;
            formation.DataId = formationName;
            formation.Position = position;
            formation.ItemId = characterId;
            ExecuteNonQuery(@"INSERT INTO playerFormation (id, playerId, dataId, position, itemId)
                VALUES (@id, @playerId, @dataId, @position, @itemId)",
                new SqliteParameter("@id", formation.Id),
                new SqliteParameter("@playerId", formation.PlayerId),
                new SqliteParameter("@dataId", formation.DataId),
                new SqliteParameter("@position", formation.Position),
                new SqliteParameter("@itemId", formation.ItemId));
        }
        else
        {
            if (oldFormation != null)
            {
                oldFormation.ItemId = formation.ItemId;
                ExecuteNonQuery(@"UPDATE playerFormation SET itemId=@itemId WHERE id=@id",
                    new SqliteParameter("@itemId", oldFormation.ItemId),
                    new SqliteParameter("@id", oldFormation.Id));
            }
            formation.ItemId = characterId;
            ExecuteNonQuery(@"UPDATE playerFormation SET itemId=@itemId WHERE id=@id",
                new SqliteParameter("@itemId", formation.ItemId),
                new SqliteParameter("@id", formation.Id));
        }
    }

    private void HelperUnlockItem(string playerId, string dataId)
    {
        PlayerUnlockItem unlockItem = null;
        var oldUnlockItems = ExecuteReader(@"SELECT * FROM playerUnlockItem WHERE playerId=@playerId AND dataId=@dataId LIMIT 1",
            new SqliteParameter("@playerId", playerId),
            new SqliteParameter("@dataId", dataId));
        if (!oldUnlockItems.Read())
        {
            unlockItem = new PlayerUnlockItem();
            unlockItem.Id = PlayerUnlockItem.GetId(playerId, dataId);
            unlockItem.PlayerId = playerId;
            unlockItem.DataId = dataId;
            unlockItem.Amount = 0;
            ExecuteNonQuery(@"INSERT INTO playerUnlockItem (id, playerId, dataId, amount)
                VALUES (@id, @playerId, @dataId, @amount)",
                new SqliteParameter("@id", unlockItem.Id),
                new SqliteParameter("@playerId", unlockItem.PlayerId),
                new SqliteParameter("@dataId", unlockItem.DataId),
                new SqliteParameter("@amount", unlockItem.Amount));
        }
    }

    private PlayerClearStage HelperClearStage(string playerId, string dataId, int grade)
    {
        PlayerClearStage clearStage = null;
        var clearStages = ExecuteReader(@"SELECT * FROM playerClearStage WHERE playerId=@playerId AND dataId=@dataId LIMIT 1",
            new SqliteParameter("@playerId", playerId),
            new SqliteParameter("@dataId", dataId));
        if (!clearStages.Read())
        {
            clearStage = new PlayerClearStage();
            clearStage.Id = PlayerClearStage.GetId(playerId, dataId);
            clearStage.PlayerId = playerId;
            clearStage.DataId = dataId;
            clearStage.BestRating = grade;
            ExecuteNonQuery(@"INSERT INTO playerClearStage (id, playerId, dataId, bestRating)
                VALUES (@id, @playerId, @dataId, @bestRating)",
                new SqliteParameter("@id", clearStage.Id),
                new SqliteParameter("@playerId", clearStage.PlayerId),
                new SqliteParameter("@dataId", clearStage.DataId),
                new SqliteParameter("@bestRating", clearStage.BestRating));
        }
        else
        {
            clearStage = new PlayerClearStage();
            clearStage.Id = clearStages.GetString(0);
            clearStage.PlayerId = clearStages.GetString(1);
            clearStage.DataId = clearStages.GetString(2);
            clearStage.BestRating = clearStages.GetInt32(3);
            if (clearStage.BestRating < grade)
            {
                clearStage.BestRating = grade;
                ExecuteNonQuery(@"UPDATE playerClearStage SET bestRating=@bestRating WHERE id=@id",
                    new SqliteParameter("@bestRating", clearStage.BestRating),
                    new SqliteParameter("@id", clearStage.Id));
            }
        }
        return clearStage;
    }

    private Player GetPlayerByLoginToken(string playerId, string loginToken)
    {
        Player player = null;
        var players = ExecuteReader(@"SELECT * FROM player WHERE id=@playerId AND loginToken=@loginToken",
            new SqliteParameter("@playerId", playerId),
            new SqliteParameter("@loginToken", loginToken));
        if (players.Read())
        {
            player = new Player();
            player.Id = players.GetString(0);
            player.ProfileName = players.GetString(1);
            player.LoginToken = players.GetString(2);
            player.Exp = players.GetInt32(3);
            player.SelectedFormation = players.GetString(4);
        }
        return player;
    }

    private PlayerBattle GetPlayerBattleBySession(string playerId, string session)
    {
        PlayerBattle playerBattle = null;
        var playerBattles = ExecuteReader(@"SELECT * FROM playerBattle WHERE playerId=@playerId AND session=@session",
            new SqliteParameter("@playerId", playerId),
            new SqliteParameter("@session", session));
        if (playerBattles.Read())
        {
            playerBattle = new PlayerBattle();
            playerBattle.Id = playerBattles.GetString(0);
            playerBattle.PlayerId = playerBattles.GetString(1);
            playerBattle.DataId = playerBattles.GetString(2);
            playerBattle.Session = playerBattles.GetString(3);
            playerBattle.BattleResult = (uint)playerBattles.GetInt32(4);
            playerBattle.Rating = playerBattles.GetInt32(5);
        }
        return playerBattle;
    }

    private PlayerItem GetPlayerItemById(string id)
    {
        PlayerItem playerItem = null;
        var playerItems = ExecuteReader(@"SELECT * FROM playerItem WHERE id=@id",
            new SqliteParameter("@id", id));
        if (playerItems.Read())
        {
            playerItem = new PlayerItem();
            playerItem.Id = playerItems.GetString(0);
            playerItem.PlayerId = playerItems.GetString(1);
            playerItem.DataId = playerItems.GetString(2);
            playerItem.Amount = playerItems.GetInt32(3);
            playerItem.Exp = playerItems.GetInt32(4);
            playerItem.EquipItemId = playerItems.GetString(5);
            playerItem.EquipPosition = playerItems.GetString(6);
        }
        return playerItem;
    }

    private PlayerItem GetPlayerItemByEquipper(string playerId, string equipItemId, string equipPosition)
    {
        PlayerItem playerItem = null;
        var playerItems = ExecuteReader(@"SELECT * FROM playerItem WHERE playerId=@playerId AND equipItemId=@equipItemId AND equipPosition=@equipPosition",
            new SqliteParameter("@playerId", playerId),
            new SqliteParameter("@equipItemId", equipItemId),
            new SqliteParameter("@equipPosition", equipPosition));
        if (playerItems.Read())
        {
            playerItem = new PlayerItem();
            playerItem.Id = playerItems.GetString(0);
            playerItem.PlayerId = playerItems.GetString(1);
            playerItem.DataId = playerItems.GetString(2);
            playerItem.Amount = playerItems.GetInt32(3);
            playerItem.Exp = playerItems.GetInt32(4);
            playerItem.EquipItemId = playerItems.GetString(5);
            playerItem.EquipPosition = playerItems.GetString(6);
        }
        return playerItem;
    }
}
