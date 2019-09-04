using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIPlayer : UIDataItem<Player>
{
    public Text textProfileName;
    public UILevel uiLevel;
    public UIItem uiMainCharacter;
    public UIPlayerList uiPlayerList;
    public Button buttonFriendRequest;
    public Button buttonFriendAccept;
    public Button buttonFriendDecline;
    public Button buttonFriendDelete;
    // Events
    public UnityEvent eventFriendRequestSuccess;
    public UnityEvent eventFriendRequestFail;
    public UnityEvent eventFriendAcceptSuccess;
    public UnityEvent eventFriendAcceptFail;
    public UnityEvent eventFriendDeclineSuccess;
    public UnityEvent eventFriendDeclineFail;
    public UnityEvent eventFriendDeleteSuccess;
    public UnityEvent eventFriendDeleteFail;

    public override void UpdateData()
    {
        SetupInfo(data);
        if (buttonFriendRequest != null)
        {
            buttonFriendRequest.onClick.RemoveListener(OnClickFriendRequest);
            buttonFriendRequest.onClick.AddListener(OnClickFriendRequest);
            buttonFriendRequest.gameObject.SetActive(!IsEmpty());
        }
        if (buttonFriendAccept != null)
        {
            buttonFriendAccept.onClick.RemoveListener(OnClickFriendAccept);
            buttonFriendAccept.onClick.AddListener(OnClickFriendAccept);
            buttonFriendAccept.gameObject.SetActive(!IsEmpty());
        }
        if (buttonFriendDecline != null)
        {
            buttonFriendDecline.onClick.RemoveListener(OnClickFriendDecline);
            buttonFriendDecline.onClick.AddListener(OnClickFriendDecline);
            buttonFriendDecline.gameObject.SetActive(!IsEmpty());
        }
        if (buttonFriendDelete != null)
        {
            buttonFriendDelete.onClick.RemoveListener(OnClickFriendDelete);
            buttonFriendDelete.onClick.AddListener(OnClickFriendDelete);
            buttonFriendDelete.gameObject.SetActive(!IsEmpty());
        }
    }

    public override void Clear()
    {
        SetupInfo(null);
    }

    private void SetupInfo(Player data)
    {
        if (data == null)
            data = new Player();

        if (textProfileName != null)
            textProfileName.text = data.ProfileName;

        // Stats
        if (uiLevel != null)
        {
            uiLevel.level = data.Level;
            uiLevel.maxLevel = data.MaxLevel;
            uiLevel.collectExp = data.CollectExp;
            uiLevel.nextExp = data.NextExp;
        }

        if (uiMainCharacter != null)
        {
            if (string.IsNullOrEmpty(data.MainCharacter) || !GameInstance.GameDatabase.Items.ContainsKey(data.MainCharacter))
            {
                uiMainCharacter.data = null;
            }
            else
            {
                uiMainCharacter.data = new PlayerItem()
                {
                    DataId = data.MainCharacter,
                    Exp = data.MainCharacterExp,
                };
            }
        }
    }

    public override bool IsEmpty()
    {
        return data == null || string.IsNullOrEmpty(data.Id);
    }

    public void OnClickFriendRequest()
    {
        GameInstance.GameService.FriendRequest(data.Id, OnFriendRequestSuccess, OnFriendRequestFail);
    }

    private void OnFriendRequestSuccess(GameServiceResult result)
    {
        if (eventFriendRequestSuccess != null)
            eventFriendRequestSuccess.Invoke();
    }

    private void OnFriendRequestFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
        if (eventFriendRequestFail != null)
            eventFriendRequestFail.Invoke();
    }

    public void OnClickFriendAccept()
    {
        GameInstance.GameService.FriendAccept(data.Id, OnFriendAcceptSuccess, OnFriendAcceptFail);
    }

    private void OnFriendAcceptSuccess(GameServiceResult result)
    {
        if (uiPlayerList != null)
            uiPlayerList.RemoveListItem(data.Id);
        if (eventFriendAcceptSuccess != null)
            eventFriendAcceptSuccess.Invoke();
    }

    private void OnFriendAcceptFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
        if (eventFriendAcceptFail != null)
            eventFriendAcceptFail.Invoke();
    }

    public void OnClickFriendDecline()
    {
        GameInstance.GameService.FriendDecline(data.Id, OnFriendDeclineSuccess, OnFriendDeclineFail);
    }

    private void OnFriendDeclineSuccess(GameServiceResult result)
    {
        if (uiPlayerList != null)
            uiPlayerList.RemoveListItem(data.Id);
        if (eventFriendDeclineSuccess != null)
            eventFriendDeclineSuccess.Invoke();
    }

    private void OnFriendDeclineFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
        if (eventFriendDeclineFail != null)
            eventFriendDeclineFail.Invoke();
    }

    public void OnClickFriendDelete()
    {
        GameInstance.GameService.FriendDelete(data.Id, OnFriendDeleteSuccess, OnFriendDeleteFail);
    }

    private void OnFriendDeleteSuccess(GameServiceResult result)
    {
        if (uiPlayerList != null)
            uiPlayerList.RemoveListItem(data.Id);
        if (eventFriendDeleteSuccess != null)
            eventFriendDeleteSuccess.Invoke();
    }

    private void OnFriendDeleteFail(string error)
    {
        GameInstance.Singleton.OnGameServiceError(error);
        if (eventFriendDeleteFail != null)
            eventFriendDeleteFail.Invoke();
    }
}
