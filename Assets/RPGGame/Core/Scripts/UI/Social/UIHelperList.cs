using UnityEngine;
using System.Collections;

public class UIHelperList : UIPlayerList
{
    private void OnEnable()
    {
        RefreshList();
    }

    public void RefreshList()
    {
        GameInstance.GameService.GetHelperList(OnRefreshListSuccess, OnRefreshListFail);
    }

    private void OnRefreshListSuccess(FriendListResult result)
    {
        SetListItems(result.list);
    }

    private void OnRefreshListFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
    }
}
