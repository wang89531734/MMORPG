using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFormation : UIBase
{
    public string formationName;
    public Transform[] uiContainers;
    public GameObject guideObject;
    private UIFormationManager manager;
    private UIItem slotPrefab;
    private readonly List<UIItem> UIFormationSlots = new List<UIItem>();

    public void SetFormationData(UIFormationManager manager)
    {
        this.manager = manager;
        SetFormationData(manager.uiFormationSlotPrefab);
    }


    public void SetFormationData(UIItem slotPrefab)
    {
        this.slotPrefab = slotPrefab;
        if (UIFormationSlots.Count == 0)
        {
            foreach (var uiContainer in uiContainers)
            {
                var newItemObject = Instantiate(slotPrefab.gameObject);
                newItemObject.transform.SetParent(uiContainer);
                newItemObject.transform.localScale = Vector3.one;
                newItemObject.SetActive(true);

                var rectTransform = newItemObject.GetComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.sizeDelta = Vector2.zero;
                rectTransform.anchoredPosition = Vector2.zero;

                var newItem = newItemObject.GetComponent<UIItem>();
                newItem.SetData(null);
                newItem.notShowInTeamStatus = true;
                newItem.clickMode = UIDataItemClickMode.Default;
                newItem.eventClick.RemoveListener(OnClickUITeamMember);
                newItem.eventClick.AddListener(OnClickUITeamMember);
                UIFormationSlots.Add(newItem);
            }
        }
        var i = 0;
        foreach (var uiItem in UIFormationSlots)
        {
            PlayerFormation playerFormation = null;
            if (PlayerFormation.TryGetData(formationName, i, out playerFormation))
            {
                var itemId = playerFormation.ItemId;
                PlayerItem item = null;
                if (!string.IsNullOrEmpty(itemId) && PlayerItem.DataMap.TryGetValue(itemId, out item))
                {
                    uiItem.SetData(item);
                    uiItem.SetGraphicsAlpha(1);
                }
                else
                {
                    uiItem.SetData(null);
                    uiItem.SetGraphicsAlpha(0);
                }
            }
            else
            {
                uiItem.SetData(null);
                uiItem.SetGraphicsAlpha(0);
            }
            ++i;
        }
    }

    private void OnClickUITeamMember(UIDataItem ui)
    {
        var uiItem = ui as UIItem;
        var position = GetFormationPosition(uiItem);
        if (manager != null)
        {
            if (manager.SelectedItem != null)
            {
                GameInstance.GameService.SetFormation(manager.SelectedItem.data.Id, formationName, position, OnSetFormationSuccess, OnSetFormationFail);
                manager.ClearSelectedItem();
            }
            else if (!uiItem.IsEmpty())
                GameInstance.GameService.SetFormation(string.Empty, formationName, position, OnSetFormationSuccess, OnSetFormationFail);
        }
    }

    private void OnSetFormationSuccess(FormationListResult result)
    {
        GameInstance.Singleton.OnGameServiceFormationListResult(result);
        SetFormationData(slotPrefab);
    }

    private void OnSetFormationFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
    }

    public void ShowGuideObject()
    {
        if (guideObject != null)
            guideObject.SetActive(true);
    }

    public void HideGuideObject()
    {
        if (guideObject != null)
            guideObject.SetActive(true);
    }

    public int GetFormationPosition(UIItem ui)
    {
        return UIFormationSlots.IndexOf(ui);
    }
}
