using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIItemWithMaterials : UIItemSelection
{
    public UIItem uiBeforeInfo;
    public UIItem uiAfterInfo;
    public UICurrency uiCurrency;

    protected PlayerItem item;
    public PlayerItem Item
    {
        get { return item; }
        set
        {
            item = value;

            if (uiBeforeInfo != null)
                uiBeforeInfo.SetData(item);

            if (uiAfterInfo != null)
                uiAfterInfo.SetData(item);
        }
    }
    
    public override void Show()
    {
        base.Show();

        if (uiBeforeInfo != null)
            uiBeforeInfo.SetData(Item);

        if (uiAfterInfo != null)
            uiAfterInfo.SetData(Item);

        if (uiCurrency != null)
        {
            var currencyData = PlayerCurrency.SoftCurrency.Clone().SetAmount(0, 0);
            uiCurrency.SetData(currencyData);
        }
    }

    public override void Hide()
    {
        base.Hide();

        if (uiBeforeInfo != null)
            uiBeforeInfo.Clear();

        if (uiAfterInfo != null)
            uiAfterInfo.Clear();

        if (uiCurrency != null)
        {
            var currencyData = PlayerCurrency.SoftCurrency.Clone().SetAmount(0, 0);
            uiCurrency.SetData(currencyData);
        }
    }
}
