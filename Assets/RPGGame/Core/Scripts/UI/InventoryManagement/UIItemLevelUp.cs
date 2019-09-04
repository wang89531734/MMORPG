using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIItemLevelUp : UIItemWithMaterials
{
    // UI
    public Button levelUpButton;
    // Events
    public UnityEvent eventLevelUpSuccess;
    public UnityEvent eventLevelUpFail;
    // Private
    private int totalLevelUpPrice;

    public override void Show()
    {
        base.Show();
        SetupLevelUp();
    }

    public void SetupLevelUp()
    {
        if (levelUpButton != null)
            levelUpButton.interactable = Item.CanLevelUp;

        var selectedItem = GetSelectedItems();
        var levelUpPrice = Item.LevelUpPrice;
        var increasingExp = 0;
        totalLevelUpPrice = 0;
        foreach (var entry in selectedItem)
        {
            increasingExp += entry.Amount * entry.RewardExp;
            totalLevelUpPrice += entry.Amount * levelUpPrice;
        }

        if (uiAfterInfo != null)
            uiAfterInfo.SetData(Item.CreateLevelUpItem(increasingExp));

        if (uiCurrency != null)
        {
            var currencyData = PlayerCurrency.SoftCurrency.Clone().SetAmount(totalLevelUpPrice, 0);
            uiCurrency.SetData(currencyData);
        }
    }

    protected override List<PlayerItem> GetAvailableItemList()
    {
        if (!Item.IsReachMaxLevel)
        {
            if (Item.CharacterData != null)
            {
                var list = PlayerItem.DataMap.Values.Where(a => a.CharacterData != null && !a.Id.Equals(Item.Id) && a.CanBeMaterial).ToList();
                list.SortRewardExp();
                return list;
            }
            if (Item.EquipmentData != null)
            {
                var list = PlayerItem.DataMap.Values.Where(a => a.EquipmentData != null && !a.Id.Equals(Item.Id) && a.CanBeMaterial).ToList();
                list.SortRewardExp();
                return list;
            }
        }
        return new List<PlayerItem>();
    }

    protected override void OnSetListItem(UIItem ui)
    {
        base.OnSetListItem(ui);
        ui.displayStats = UIItem.DisplayStats.RewardExp;
    }

    protected override void SelectItem(UIDataItem ui)
    {
        base.SelectItem(ui);
        SetupLevelUp();
    }

    protected override void DeselectItem(UIDataItem ui)
    {
        base.DeselectItem(ui);
        SetupLevelUp();
    }

    public void OnClickLevelUp()
    {
        var gameInstance = GameInstance.Singleton;
        var gameService = GameInstance.GameService;
        if (!PlayerCurrency.HaveEnoughSoftCurrency(totalLevelUpPrice))
        {
            gameInstance.WarnNotEnoughSoftCurrency();
            return;
        }
        var idAmountPair = GetSelectedItemIdAmountPair();
        gameService.LevelUpItem(Item.Id, idAmountPair, OnLevelUpSuccess, OnLevelUpFail);
    }

    private void OnLevelUpSuccess(ItemResult result)
    {
        GameInstance.Singleton.OnGameServiceItemResult(result);
        eventLevelUpSuccess.Invoke();
        if (uiSelectedItemList != null)
            uiSelectedItemList.ClearListItems();
        var items = GetAvailableItems();
        var updateItems = result.updateItems;
        foreach (var updateItem in updateItems)
        {
            var id = updateItem.Id;
            if (updateItem.Id == Item.Id)
                Item = updateItem;
            if (items.ContainsKey(id))
                items[id].SetData(updateItem);
        }
        var deleteItemIds = result.deleteItemIds;
        foreach (var deleteItemId in deleteItemIds)
        {
            if (uiAvailableItemList != null)
                uiAvailableItemList.RemoveListItem(deleteItemId);
        }
        var updateCurrencies = result.updateCurrencies;
        foreach (var updateCurrency in updateCurrencies)
        {
            PlayerCurrency.SetData(updateCurrency);
        }
        SetupLevelUp();
    }

    private void OnLevelUpFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
        eventLevelUpFail.Invoke();
    }
}
