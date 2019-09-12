using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerUnlockItem : BasePlayerData, IPlayerUnlockItem
{
    public static readonly Dictionary<string, PlayerUnlockItem> DataMap = new Dictionary<string, PlayerUnlockItem>();
    public string Id { get { return GetId(PlayerId, DataId); } set { } }
    public string playerId;
    public string PlayerId { get { return playerId; } set { playerId = value; } }
    public string dataId;
    public string DataId { get { return dataId; } set { dataId = value; } }
    public int amount;
    public int Amount { get { return amount; } set { amount = value; } }

    public PlayerUnlockItem Clone()
    {
        var result = new PlayerUnlockItem();
        CloneTo(this, result);
        return result;
    }

    public static void CloneTo(IPlayerUnlockItem from, IPlayerUnlockItem to)
    {
        to.Id = from.Id;
        to.PlayerId = from.PlayerId;
        to.DataId = from.DataId;
        to.Amount = from.Amount;
    }

    public static string GetId(string playerId, string dataId)
    {
        return playerId + "_" + dataId;
    }

    public static void SetData(PlayerUnlockItem data)
    {
        if (data == null || string.IsNullOrEmpty(data.Id))
            return;
        DataMap[data.Id] = data;
    }

    public static bool TryGetData(string playerId, string dataId, out PlayerUnlockItem data)
    {
        return DataMap.TryGetValue(GetId(playerId, dataId), out data);
    }

    public static bool TryGetData(string dataId, out PlayerUnlockItem data)
    {
        return TryGetData(Player.CurrentPlayerId, dataId, out data);
    }

    public static bool RemoveData(string id)
    {
        return DataMap.Remove(id);
    }

    public static void ClearData()
    {
        DataMap.Clear();
    }

    public static void SetDataRange(IEnumerable<PlayerUnlockItem> list)
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

    public static bool IsUnlock(string playerId, BaseItem itemData)
    {
        var Id = GetId(playerId, itemData.Id);
        if (DataMap.ContainsKey(Id))
            return true;
        return false;
    }

    public static bool IsUnlock(BaseItem itemData)
    {
        return IsUnlock(Player.CurrentPlayerId, itemData);
    }
}
