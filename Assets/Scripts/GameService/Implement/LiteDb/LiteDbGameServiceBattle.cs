using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public partial class LiteDbGameService
{
    protected override void DoStartStage(string playerId, string loginToken, string stageDataId, UnityAction<StartStageResult> onFinish)
    {
        var result = new StartStageResult();
        var gameDb = GameInstance.GameDatabase;
        var player = colPlayer.FindOne(a => a.Id == playerId && a.LoginToken == loginToken);
        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else if (!gameDb.Stages.ContainsKey(stageDataId))
            result.error = GameServiceErrorCode.INVALID_STAGE_DATA;
        else
        {
            colPlayerBattle.Delete(a => a.PlayerId == playerId && a.BattleResult == BATTLE_RESULT_NONE);
            var stage = gameDb.Stages[stageDataId];
            var stageStaminaTable = gameDb.stageStamina;
            if (!DecreasePlayerStamina(player, stageStaminaTable, stage.requireStamina))
                result.error = GameServiceErrorCode.NOT_ENOUGH_STAGE_STAMINA;
            else
            {
                var playerBattle = new DbPlayerBattle();
                playerBattle.Id = System.Guid.NewGuid().ToString();
                playerBattle.PlayerId = playerId;
                playerBattle.DataId = stageDataId;
                playerBattle.Session = System.Guid.NewGuid().ToString();
                playerBattle.BattleResult = BATTLE_RESULT_NONE;
                colPlayerBattle.Insert(playerBattle);

                var stamina = GetStamina(player.Id, stageStaminaTable.id);
                var resultStamina = new PlayerStamina();
                PlayerStamina.CloneTo(stamina, resultStamina);
                result.stamina = resultStamina;
                result.session = playerBattle.Session;
            }
        }
        onFinish(result);
    }

    protected override void DoFinishStage(string playerId, string loginToken, string session, ushort battleResult, int deadCharacters, UnityAction<FinishStageResult> onFinish)
    {
        var result = new FinishStageResult();
        var gameDb = GameInstance.GameDatabase;
        var player = colPlayer.FindOne(a => a.Id == playerId && a.LoginToken == loginToken);
        var battle = colPlayerBattle.FindOne(a => a.PlayerId == playerId && a.Session == session);
        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else if (battle == null)
            result.error = GameServiceErrorCode.INVALID_BATTLE_SESSION;
        else
        {
            var rating = 0;
            battle.BattleResult = battleResult;
            if (battleResult == BATTLE_RESULT_WIN)
            {
                rating = 3 - deadCharacters;
                if (rating <= 0)
                    rating = 1;
            }
            battle.Rating = rating;
            result.rating = rating;
            colPlayerBattle.Update(battle);
            if (battleResult == BATTLE_RESULT_WIN)
            {
                var stage = gameDb.Stages[battle.DataId];
                var rewardPlayerExp = stage.rewardPlayerExp;
                result.rewardPlayerExp = rewardPlayerExp;
                // Player exp
                player.Exp += rewardPlayerExp;
                colPlayer.Update(player);
                var resultPlayer = new Player();
                Player.CloneTo(player, resultPlayer);
                result.player = resultPlayer;
                // Character exp
                var formations = new List<DbPlayerFormation>(colPlayerFormation.Find(a => a.PlayerId == playerId && a.DataId == player.SelectedFormation));
                var countFormation = 0;
                foreach (var formation in formations)
                {
                    if (!string.IsNullOrEmpty(formation.ItemId))
                        ++countFormation;
                }
                if (countFormation > 0)
                {
                    var devivedExp = stage.rewardCharacterExp / countFormation;
                    result.rewardCharacterExp = devivedExp;
                    foreach (var formation in formations)
                    {
                        var character = colPlayerItem.FindById(formation.ItemId);
                        if (character != null)
                        {
                            character.Exp += devivedExp;
                            colPlayerItem.Update(character);
                            var resultCharacter = new PlayerItem();
                            PlayerItem.CloneTo(character, resultCharacter);
                            result.updateItems.Add(resultCharacter);
                        }
                    }
                }
                // Soft currency
                var softCurrency = GetCurrency(playerId, gameDb.softCurrency.id);
                var rewardSoftCurrency = Random.Range(stage.randomSoftCurrencyMinAmount, stage.randomSoftCurrencyMaxAmount);
                result.rewardSoftCurrency = rewardSoftCurrency;
                softCurrency.Amount += rewardSoftCurrency;
                colPlayerCurrency.Update(softCurrency);
                var resultSoftCurrency = new PlayerCurrency();
                PlayerCurrency.CloneTo(softCurrency, resultSoftCurrency);
                result.updateCurrencies.Add(resultSoftCurrency);
                // Items
                for (var i = 0; i < stage.rewardItems.Length; ++i)
                {
                    var rewardItem = stage.rewardItems[i];
                    if (rewardItem == null || rewardItem.item == null || Random.value > rewardItem.randomRate)
                        continue;
                    var createItems = new List<DbPlayerItem>();
                    var updateItems = new List<DbPlayerItem>();
                    if (AddItems(player.Id, rewardItem.Id, rewardItem.amount, out createItems, out updateItems))
                    {
                        foreach (var createEntry in createItems)
                        {
                            createEntry.Id = System.Guid.NewGuid().ToString();
                            colPlayerItem.Insert(createEntry);
                            var resultItem = new PlayerItem();
                            PlayerItem.CloneTo(createEntry, resultItem);
                            result.rewardItems.Add(resultItem);
                            result.createItems.Add(resultItem);
                            HelperUnlockItem(player.Id, rewardItem.Id);
                        }
                        foreach (var updateEntry in updateItems)
                        {
                            colPlayerItem.Update(updateEntry);
                            var resultItem = new PlayerItem();
                            PlayerItem.CloneTo(updateEntry, resultItem);
                            result.rewardItems.Add(resultItem);
                            result.updateItems.Add(resultItem);
                        }
                    }
                    // End add item condition
                }
                // End reward items loop
                var clearedStage = HelperClearStage(playerId, stage.Id, rating);
                var resultClearedStage = new PlayerClearStage();
                PlayerClearStage.CloneTo(clearedStage, resultClearedStage);
                result.clearStage = resultClearedStage;
            }
        }
        onFinish(result);
    }

    protected override void DoReviveCharacters(string playerId, string loginToken, UnityAction<CurrencyResult> onFinish)
    {
        var result = new CurrencyResult();
        var gameDb = GameInstance.GameDatabase;
        var player = colPlayer.FindOne(a => a.Id == playerId && a.LoginToken == loginToken);
        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else
        {
            var hardCurrency = GetCurrency(playerId, gameDb.hardCurrency.id);
            var revivePrice = gameDb.revivePrice;
            if (revivePrice > hardCurrency.Amount)
                result.error = GameServiceErrorCode.NOT_ENOUGH_HARD_CURRENCY;
            else
            {
                hardCurrency.Amount -= revivePrice;
                colPlayerCurrency.Update(hardCurrency);
                var resultHardCurrency = new PlayerCurrency();
                PlayerCurrency.CloneTo(hardCurrency, resultHardCurrency);
                result.updateCurrencies.Add(resultHardCurrency);
            }
        }
        onFinish(result);
    }

    protected override void DoSelectFormation(string playerId, string loginToken, string formationName, UnityAction<PlayerResult> onFinish)
    {
        var result = new PlayerResult();
        var gameDb = GameInstance.GameDatabase;
        var player = colPlayer.FindOne(a => a.Id == playerId && a.LoginToken == loginToken);
        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else if (!gameDb.Formations.ContainsKey(formationName))
            result.error = GameServiceErrorCode.INVALID_FORMATION_DATA;
        else
        {
            player.SelectedFormation = formationName;
            colPlayer.Update(player);
            var resultPlayer = new Player();
            Player.CloneTo(player, resultPlayer);
            result.player = resultPlayer;
        }
        onFinish(result);
    }

    protected override void DoSetFormation(string playerId, string loginToken, string characterId, string formationName, int position, UnityAction<FormationListResult> onFinish)
    {
        var result = new FormationListResult();
        var player = colPlayer.FindOne(a => a.Id == playerId && a.LoginToken == loginToken);
        PlayerItem character = null;
        if (!string.IsNullOrEmpty(characterId))
        {
            var dbCharacter = colPlayerItem.FindOne(a => a.Id == characterId && a.PlayerId == playerId);
            character = new PlayerItem();
            PlayerItem.CloneTo(dbCharacter, character);
        }

        if (player == null)
            result.error = GameServiceErrorCode.INVALID_LOGIN_TOKEN;
        else if (character != null && character.CharacterData == null)
            result.error = GameServiceErrorCode.INVALID_ITEM_DATA;
        else
        {
            HelperSetFormation(playerId, characterId, formationName, position);
            result.list.AddRange(DbPlayerFormation.CloneList(colPlayerFormation.Find(a => a.PlayerId == playerId)));
        }
        onFinish(result);
    }
}
