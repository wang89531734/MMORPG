using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class LootBoxReward
{
    public ItemAmount rewardItem;
    public int randomWeight;

    public string Id
    {
        get { return rewardItem == null ? "" : rewardItem.Id; }
    }

    public int Amount
    {
        get { return rewardItem == null ? 0 : rewardItem.amount; }
    }

    public virtual string ToJson()
    {
        if (string.IsNullOrEmpty(Id) || Amount <= 0)
            return "";
        return "{\"id\":\"" + Id + "\"," +
            "\"amount\":" + Amount + "," +
            "\"randomWeight\":" + randomWeight + "}";
    }
}

[System.Serializable]
public class LootBoxPack
{
    [Range(1, 20)]
    public int openAmount = 1;
    public int price = 0;

    public virtual string ToJson()
    {
        return "{\"openAmount\":" + openAmount + "," +
            "\"price\":" + price + "}";
    }
}

public enum LootBoxRequirementType : short
{
    RequireSoftCurrency = 0,
    RequireHardCurrency = 1,
}

public class LootBox : BaseGameData
{
    public Sprite icon;
    public LootBoxRequirementType requirementType;
    public LootBoxPack[] lootboxPacks;
    public LootBoxReward[] lootboxRewards;

    public LootBoxReward RandomReward()
    {
        var weight = new Dictionary<LootBoxReward, int>();
        foreach (var lootboxReward in lootboxRewards)
        {
            weight.Add(lootboxReward, lootboxReward.randomWeight);
        }
        return WeightedRandomizer.From(weight).TakeOne();
    }

    public virtual string ToJson()
    {
        // Lootbox packs
        var jsonLootBoxPacks = "";
        foreach (var entry in lootboxPacks)
        {
            if (!string.IsNullOrEmpty(jsonLootBoxPacks))
                jsonLootBoxPacks += ",";
            jsonLootBoxPacks += entry.ToJson();
        }
        jsonLootBoxPacks = "[" + jsonLootBoxPacks + "]";
        // Lootbox rewards
        var jsonLootBoxRewards = "";
        foreach (var entry in lootboxRewards)
        {
            if (!string.IsNullOrEmpty(jsonLootBoxRewards))
                jsonLootBoxRewards += ",";
            jsonLootBoxRewards += entry.ToJson();
        }
        jsonLootBoxRewards = "[" + jsonLootBoxRewards + "]";
        // Combine
        return "{\"id\":\"" + Id + "\"," +
            "\"requirementType\":"+(short)requirementType + "," +
            "\"lootboxPacks\":"+ jsonLootBoxPacks + "," +
            "\"lootboxRewards\":" + jsonLootBoxRewards + "}";
    }
}
