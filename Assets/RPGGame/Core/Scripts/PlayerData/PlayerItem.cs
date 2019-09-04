using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PlayerItem : BasePlayerData, ILevel, IPlayerItem
{
    public static readonly Dictionary<string, PlayerItem> DataMap = new Dictionary<string, PlayerItem>();
    public string id;
    public string Id { get { return id; } set { id = value; } }
    public string playerId;
    public string PlayerId { get { return playerId; } set { playerId = value; } }
    public string dataId;
    public string DataId
    {
        get { return dataId; }
        set
        {
            if (dataId != value)
            {
                dirtyExp = -1;
                evolveMaterials = null;
                dataId = value;
            }
        }
    }
    public int amount;
    public int Amount { get { return amount; } set { amount = value; } }
    public int exp;
    public int Exp { get { return exp; } set { exp = value; } }
    public string equipItemId;
    public string EquipItemId { get { return equipItemId; } set { equipItemId = value; } }
    public string equipPosition;
    public string EquipPosition { get { return equipPosition; } set { equipPosition = value; } }

    private int level = -1;
    private int collectExp = -1;
    private int dirtyExp = -1;  // Exp for dirty check to calculate `Level` and `CollectExp` fields
    private Dictionary<string, int> evolveMaterials = null;

    public PlayerItem()
    {
        Id = "";
        PlayerId = "";
        DataId = "";
        Amount = 1;
        Exp = 0;
        EquipItemId = "";
        EquipPosition = "";
    }

    public PlayerItem Clone()
    {
        var result = new PlayerItem();
        CloneTo(this, result);
        return result;
    }

    public static void CloneTo(IPlayerItem from, IPlayerItem to)
    {
        to.Id = from.Id;
        to.PlayerId = from.PlayerId;
        to.DataId = from.DataId;
        to.Amount = from.Amount;
        to.Exp = from.Exp;
        to.EquipItemId = from.EquipItemId;
        to.EquipPosition = from.EquipPosition;
    }

    public PlayerItem CreateLevelUpItem(int increaseExp)
    {
        PlayerItem result = Clone();
        result.Exp += increaseExp;
        return result;
    }

    public PlayerItem CreateEvolveItem()
    {
        PlayerItem result = Clone();
        if (EvolveItem != null)
            result.DataId = EvolveItem.Id;
        if (GameInstance.GameDatabase.resetItemLevelAfterEvolve)
            result.Exp = 0;
        return result;
    }

    #region Non Serialize Fields
    public BaseItem ItemData
    {
        get
        {
            if (GameDatabase != null && GameDatabase.Items.ContainsKey(DataId))
                return GameDatabase.Items[DataId];
            return null;
        }
    }
    
    public BaseActorItem ActorItemData
    {
        get { return ItemData == null ? null : ItemData as BaseActorItem; }
    }
    
    public CharacterItem CharacterData
    {
        get { return ActorItemData == null ? null : ActorItemData as CharacterItem; }
    }
    
    public EquipmentItem EquipmentData
    {
        get { return ActorItemData == null ? null : ActorItemData as EquipmentItem; }
    }
    
    public ItemTier Tier
    {
        get { return ActorItemData == null ? null : ActorItemData.itemTier; }
    }
    
    public Sprite Icon
    {
        get { return ItemData == null ? null : ItemData.icon; }
    }
    
    public int Level
    {
        get
        {
            CalculateLevelAndRemainExp();
            return level;
        }
    }
    
    public int CollectExp
    {
        get
        {
            CalculateLevelAndRemainExp();
            return collectExp;
        }
    }
    
    public int MaxLevel
    {
        get { return ActorItemData == null ? 1 : Tier.maxLevel; }
    }
    
    public SpecificItemEvolve SpecificEvolveInfo
    {
        get { return ActorItemData == null ? null : ActorItemData.GetSpecificItemEvolve(); }
    }
    
    public BaseItem EvolveItem
    {
        get { return SpecificEvolveInfo == null ? null : SpecificEvolveInfo.GetEvolveItem(); }
    }
    
    public int EvolvePrice
    {
        get { return Tier == null ? 0 : Tier.evolvePrice; }
    }
    
    public int NextExp
    {
        get { return Tier == null ? 0 : Tier.expTable.Calculate(Level, Tier.maxLevel); }
    }
    
    public int SellPrice
    {
        get
        {
            if (ActorItemData == null)
                return 0;
            if (ActorItemData.useFixSellPrice)
                return ActorItemData.fixSellPrice;
            return Tier == null ? 0 : Tier.sellPriceTable.Calculate(Level, Tier.maxLevel);
        }
    }
    
    public int LevelUpPrice
    {
        get
        {
            if (ActorItemData == null)
                return 0;
            if (ActorItemData.useFixLevelUpPrice)
                return ActorItemData.fixLevelUpPrice;
            return Tier == null ? 0 : Tier.levelUpPriceTable.Calculate(Level, Tier.maxLevel);
        }
    }
    
    public int RewardExp
    {
        get
        {
            if (ActorItemData == null)
                return 0;
            if (ActorItemData.useFixRewardExp)
                return ActorItemData.fixRewardExp;
            return Tier == null ? 0 :Tier.rewardExpTable.Calculate(Level, Tier.maxLevel);
        }
    }
    
    public bool IsReachMaxLevel
    {
        get { return Level == MaxLevel; }
    }
    
    public Dictionary<string, int> EvolveMaterials
    {
        get
        {
            if (evolveMaterials == null)
                MakeEvolveMaterialsMap();
            return evolveMaterials;
        }
    }
    
    public CalculationAttributes EquipmentBonus
    {
        get
        {
            var equippedItems = EquippedItems.Values;
            var result = new CalculationAttributes();
            foreach (var equippedItem in equippedItems)
                result += equippedItem.Attributes;
            return result;
        }
    }
    
    public CalculationAttributes Attributes
    {
        get
        {
            // If item is character or equipment
            if (CharacterData != null)
            {
                var result = new CalculationAttributes();
                result += CharacterData.attributes.CreateCalculationAttributes(Level, MaxLevel);
                if (GameDatabase != null)
                    result += GameDatabase.characterBaseAttributes;
                return result;
            }
            if (EquipmentData != null)
            {
                var result = new CalculationAttributes();
                result += EquipmentData.attributes.CreateCalculationAttributes(Level, MaxLevel);
                result += EquipmentData.extraAttributes;
                return result;
            }
            return null;
        }
    }
    
    public List<PlayerFormation> InTeamFormations
    {
        get
        {
            if (CharacterData == null)
                return new List<PlayerFormation>();
            var valueList = PlayerFormation.DataMap.Values;
            var list = valueList.Where(entry =>
                entry.PlayerId == PlayerId &&
                entry.ItemId == Id).ToList();
            return list;
        }
    }
    
    public PlayerItem EquippedByItem
    {
        get
        {
            PlayerItem equippedByItem;
            if (EquipmentData != null && !string.IsNullOrEmpty(EquipItemId) && DataMap.TryGetValue(EquipItemId, out equippedByItem))
                return equippedByItem;
            return null;
        }
    }
    
    public Dictionary<string, PlayerItem> EquippedItems
    {
        get
        {
            var result = new Dictionary<string, PlayerItem>();

            if (CharacterData == null)
                return result;

            var valueList = DataMap.Values;
            var list = valueList.Where(entry => 
                entry.PlayerId == PlayerId &&
                entry.EquipmentData != null &&
                entry.EquipItemId == Id &&
                !string.IsNullOrEmpty(entry.EquipPosition) &&
                entry.Amount > 0).ToList();

            foreach (var entry in list)
            {
                result.Add(entry.EquipPosition, entry);
            }

            return result;
        }
    }
    
    public bool CanLevelUp
    {
        get { return !IsReachMaxLevel && (CharacterData != null || EquipmentData != null); }
    }
    
    public bool CanEvolve
    {
        get { return IsReachMaxLevel && EvolveItem != null && Level >= MaxLevel; }
    }
    
    public bool CanSell
    {
        get { return !PlayerFormation.ContainsDataWithItemId(Id) && EquippedByItem == null; }
    }
    
    public bool CanBeMaterial
    {
        get { return !PlayerFormation.ContainsDataWithItemId(Id) && EquippedByItem == null; }
    }
    
    public bool CanBeEquipped
    {
        get {  return EquipmentData != null; }
    }

    public bool IsInTeamFormation(string formationName)
    {
        var formations = InTeamFormations;
        foreach (var formation in formations)
        {
            if (formation.DataId == formationName)
                return true;
        }
        return false;
    }
    #endregion

    private void CalculateLevelAndRemainExp()
    {
        if (Tier == null)
        {
            level = 1;
            collectExp = 0;
            return;
        }
        if (dirtyExp == -1 || dirtyExp != Exp)
        {
            dirtyExp = Exp;
            var remainExp = Exp;
            for (level = 1; level < MaxLevel; ++level)
            {
                var nextExp = Tier.expTable.Calculate(level, Tier.maxLevel);
                if (remainExp - nextExp < 0)
                    break;
                remainExp -= nextExp;
            }
            collectExp = remainExp;
        }
    }

    private void MakeEvolveMaterialsMap()
    {
        evolveMaterials = new Dictionary<string, int>();

        if (EvolveItem == null)
            return;

        var requiredMaterials = SpecificEvolveInfo.requiredMaterials;
        foreach (var requiredMaterial in requiredMaterials)
        {
            if (requiredMaterial.item == null || string.IsNullOrEmpty(requiredMaterial.item.name))
                continue;
            var dataId = requiredMaterial.item.name;
            if (!evolveMaterials.ContainsKey(dataId))
                evolveMaterials.Add(dataId, requiredMaterial.amount);
            else
                evolveMaterials[dataId] += requiredMaterial.amount;
        }
    }

    public static void SetData(PlayerItem data)
    {
        if (data == null || string.IsNullOrEmpty(data.Id))
            return;
        DataMap[data.Id] = data;
    }

    public static bool RemoveData(string id)
    {
        return DataMap.Remove(id);
    }

    public static void ClearData()
    {
        DataMap.Clear();
    }

    public static void SetDataRange(IEnumerable<PlayerItem> list)
    {
        foreach (var data in list)
        {
            SetData(data);
        }
    }

    public static void RemoveDataRange(IEnumerable<string> ids)
    {
        foreach (var id in ids)
        {
            RemoveData(id);
        }
    }

    public static void RemoveDataRange(string playerId)
    {
        var values = DataMap.Values;
        foreach (var value in values)
        {
            if (value.PlayerId == playerId)
                RemoveData(value.Id);
        }
    }

    public static void RemoveDataRange()
    {
        RemoveDataRange(Player.CurrentPlayerId);
    }

    public static PlayerItem CreateActorItemWithLevel(BaseActorItem itemData, int level)
    {
        if (level <= 0)
            level = 1;
        var itemTier = itemData.itemTier;
        var sumExp = 0;
        var result = new PlayerItem();
        for (var i = 1; i < level; ++i)
        {
            sumExp += itemTier.expTable.Calculate(i + 1, itemTier.maxLevel);
        }
        result.DataId = itemData.Id;
        result.Exp = sumExp;
        return result;
    }
}