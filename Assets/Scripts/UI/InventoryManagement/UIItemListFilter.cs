[System.Serializable]
public struct UIItemListFilterSetting
{
    public bool showCharacter;
    public bool showEquipment;
    public string category;
}

public static class UIItemListFilter
{
    public static bool Filter(PlayerItem item, UIItemListFilterSetting setting)
    {
        var isCharacter = IsCharacter(item);
        var isEquipment = IsEquipment(item);
        if (setting.showCharacter && !isCharacter)
            return false;
        if (!setting.showCharacter && isCharacter)
            return false;
        if (setting.showEquipment && !isEquipment)
            return false;
        if (!setting.showEquipment && isEquipment)
            return false;
        if (!string.IsNullOrEmpty(setting.category) && !MatchCategory(item, setting.category))
            return false;
        return true;
    }

    public static bool IsCharacter(PlayerItem item)
    {
        return item != null && item.ItemData != null && item.CharacterData != null;
    }

    public static bool IsEquipment(PlayerItem item)
    {
        return item != null && item.ItemData != null && item.EquipmentData != null;
    }

    public static bool MatchCategory(PlayerItem item, string category)
    {
        return item != null && item.ItemData != null && item.ItemData.category == category;
    }
}
