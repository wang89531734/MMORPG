using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILootBox : UIDataItem<LootBox>
{
    public Text textTitle;
    public Text textDescription;
    public Image imageIcon;
    public UICurrency[] uiCurrencies;

    public override void Clear()
    {
        SetupInfo(null);
    }

    public override void UpdateData()
    {
        SetupInfo(data);
    }

    private void SetupInfo(LootBox data)
    {
        if (textTitle != null)
            textTitle.text = data == null ? "" : data.title;

        if (textDescription != null)
            textDescription.text = data == null ? "" : data.description;

        if (imageIcon != null)
            imageIcon.sprite = data == null ? null : data.icon;

        if (uiCurrencies != null && uiCurrencies.Length > 0)
        {
            for (var i = 0; i < uiCurrencies.Length; ++i)
            {
                var uiCurrency = uiCurrencies[i];
                uiCurrency.Clear();
                if (data != null)
                {
                    var packIndex = i;
                    if (packIndex > data.lootboxPacks.Length - 1)
                        packIndex = 0;
                    var price = data.lootboxPacks[packIndex].price;
                    PlayerCurrency currencyData = null;
                    switch (data.requirementType)
                    {
                        case LootBoxRequirementType.RequireSoftCurrency:
                            currencyData = PlayerCurrency.SoftCurrency.Clone().SetAmount(price, 0);
                            break;
                        case LootBoxRequirementType.RequireHardCurrency:
                            currencyData = PlayerCurrency.HardCurrency.Clone().SetAmount(price, 0);
                            break;
                    }
                    uiCurrency.SetData(currencyData);
                }
            }
        }
    }

    public override bool IsEmpty()
    {
        return data == null || string.IsNullOrEmpty(data.Id);
    }

    public void OnClickOpen(int packIndex)
    {
        var gameInstance = GameInstance.Singleton;
        var gameService = GameInstance.GameService;
        if (packIndex > data.lootboxPacks.Length - 1)
            packIndex = 0;
        var price = data.lootboxPacks[packIndex].price;
        switch (data.requirementType)
        {
            case LootBoxRequirementType.RequireSoftCurrency:
                if (!PlayerCurrency.HaveEnoughSoftCurrency(price))
                {
                    gameInstance.WarnNotEnoughSoftCurrency();
                    return;
                }
                break;
            case LootBoxRequirementType.RequireHardCurrency:
                if (!PlayerCurrency.HaveEnoughHardCurrency(price))
                {
                    gameInstance.WarnNotEnoughHardCurrency();
                    return;
                }
                break;
        }
        gameService.OpenLootBox(data.Id, packIndex, OnOpenLootBoxSuccess, OnOpenLootBoxFail);
    }

    private void OnOpenLootBoxSuccess(ItemResult result)
    {
        GameInstance.Singleton.OnGameServiceItemResult(result);
        var updateCurrencies = result.updateCurrencies;
        foreach (var updateCurrency in updateCurrencies)
        {
            PlayerCurrency.SetData(updateCurrency);
        }
        var items = new List<PlayerItem>();
        items.AddRange(result.createItems);
        items.AddRange(result.updateItems);
        if (items.Count > 0)
            GameInstance.Singleton.ShowRewardItemsDialog(items);
    }

    private void OnOpenLootBoxFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
    }
}
