using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIItemSelection : UIBase
{
    public UIItemList uiAvailableItemList;
    public UIItemList uiSelectedItemList;
    public int limitSelection = 0;

    protected abstract List<PlayerItem> GetAvailableItemList();

    public override void Show()
    {
        base.Show();

        if (uiAvailableItemList != null)
        {
            uiAvailableItemList.limitSelection = limitSelection;
            uiAvailableItemList.selectable = true;
            uiAvailableItemList.multipleSelection = true;
            uiAvailableItemList.eventSelect.RemoveListener(SelectItem);
            uiAvailableItemList.eventSelect.AddListener(SelectItem);
            uiAvailableItemList.eventDeselect.RemoveListener(DeselectItem);
            uiAvailableItemList.eventDeselect.AddListener(DeselectItem);
            uiAvailableItemList.SetListItems(GetAvailableItemList(), OnSetListItem);
        }

        if (uiSelectedItemList != null)
            uiSelectedItemList.selectable = false;
    }

    protected virtual void OnSetListItem(UIItem ui) { }

    public override void Hide()
    {
        base.Hide();

        if (uiAvailableItemList != null)
            uiAvailableItemList.ClearListItems();

        if (uiSelectedItemList != null)
            uiSelectedItemList.ClearListItems();
    }

    protected virtual void SelectItem(UIDataItem ui)
    {
        if (uiSelectedItemList == null)
            return;
        uiSelectedItemList.SetListItem((ui as UIItem).data);
    }

    protected virtual void DeselectItem(UIDataItem ui)
    {
        if (uiSelectedItemList == null)
            return;
        uiSelectedItemList.RemoveListItem((ui as UIItem).data.Id);
    }

    public Dictionary<string, UIItem> GetAvailableItems()
    {
        if (uiAvailableItemList == null)
            return new Dictionary<string, UIItem>();
        return uiAvailableItemList.UIEntries;
    }

    public int GetSelectedAmount()
    {
        if (uiAvailableItemList == null)
            return 0;
        return uiAvailableItemList.SelectedAmount;
    }

    public List<UIItem> GetSelectedUIs()
    {
        if (uiAvailableItemList == null)
            return new List<UIItem>();
        return uiAvailableItemList.GetSelectedUIList();
    }

    public List<UIItem> GetSelectedUIs(string dataId)
    {
        if (uiAvailableItemList == null)
            return new List<UIItem>();
        return uiAvailableItemList.GetSelectedUIList(dataId);
    }

    public List<PlayerItem> GetSelectedItems()
    {
        if (uiAvailableItemList == null)
            return new List<PlayerItem>();
        return uiAvailableItemList.GetSelectedDataList();
    }

    public List<PlayerItem> GetSelectedItems(string dataId)
    {
        if (uiAvailableItemList == null)
            return new List<PlayerItem>();
        return uiAvailableItemList.GetSelectedDataList(dataId);
    }

    public List<string> GetSelectedItemIds()
    {
        if (uiAvailableItemList == null)
            return new List<string>();
        return uiAvailableItemList.GetSelectedIdList();
    }

    public Dictionary<string, int> GetSelectedItemIdAmountPair()
    {
        if (uiAvailableItemList == null)
            return new Dictionary<string, int>();
        return uiAvailableItemList.GetSelectedItemIdAmountPair();
    }
}
