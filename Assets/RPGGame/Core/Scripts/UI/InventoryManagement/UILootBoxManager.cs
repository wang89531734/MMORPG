using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UILootBoxManager : UIBase
{
    public UILootBoxList uiLootBoxList;

    public override void Show()
    {
        base.Show();

        if (uiLootBoxList != null)
        {
            var availableLootBoxes = GameInstance.AvailableLootBoxes;
            var allLootBoxes = GameInstance.GameDatabase.LootBoxes;
            var list = allLootBoxes.Values.Where(a => availableLootBoxes.Contains(a.Id)).ToList();
            uiLootBoxList.SetListItems(list);
        }
    }
}
