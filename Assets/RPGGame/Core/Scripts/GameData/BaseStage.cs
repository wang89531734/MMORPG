using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseStage : BaseGameData
{
    public string stageNumber;
    public Sprite icon;
    [Header("Stamina")]
    public int requireStamina;
    [Header("Rewards")]
    public int randomSoftCurrencyMinAmount;
    public int randomSoftCurrencyMaxAmount;
    public int rewardPlayerExp;
    public int rewardCharacterExp;
    public ItemDrop[] rewardItems;
    [Header("Unlock")]
    public BaseStage[] unlockStages;

    public virtual List<PlayerItem> GetCharacters()
    {
        return new List<PlayerItem>();
    }

    public virtual List<PlayerItem> GetRewardItems()
    {
        var dict = new Dictionary<string, PlayerItem>();
        foreach (var rewardItem in rewardItems)
        {
            var item = rewardItem.item;
            var newEntry = new PlayerItem();
            newEntry.Id = item.Id;
            newEntry.DataId = item.Id;
            newEntry.Amount = 1;
            dict[item.Id] = newEntry;
        }
        return new List<PlayerItem>(dict.Values);
    }

    public virtual string ToJson()
    {
        // Reward Items
        var jsonRewardItems = "";
        foreach (var entry in rewardItems)
        {
            if (!string.IsNullOrEmpty(jsonRewardItems))
                jsonRewardItems += ",";
            jsonRewardItems += entry.ToJson();
        }
        jsonRewardItems = "[" + jsonRewardItems + "]";
        // Unlock Stages
        var jsonUnlockStages = "";
        foreach (var entry in unlockStages)
        {
            if (!string.IsNullOrEmpty(jsonUnlockStages))
                jsonUnlockStages += ",";
            jsonUnlockStages += "\"" + entry.Id + "\"";
        }
        jsonUnlockStages = "[" + jsonUnlockStages + "]";
        return "{\"id\":\"" + Id + "\"," +
            "\"requireStamina\":" + requireStamina + "," +
            "\"randomSoftCurrencyMinAmount\":" + randomSoftCurrencyMinAmount + "," +
            "\"randomSoftCurrencyMaxAmount\":" + randomSoftCurrencyMaxAmount + "," +
            "\"rewardPlayerExp\":" + rewardPlayerExp + "," +
            "\"rewardCharacterExp\":" + rewardCharacterExp + "," +
            "\"rewardItems\":" + jsonRewardItems + "," +
            "\"unlockStages\":" + jsonUnlockStages + "}";
    }
}
