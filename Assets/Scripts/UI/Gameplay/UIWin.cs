using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWin : UIDataItem<FinishStageResult>
{
    public const string ANIM_KEY_BATTLE_RATING = "Rating";
    public Animator ratingAnimator;
    public UIPlayer uiPlayer;
    public Text textRewardPlayerExp;
    public Text textRewardCharacterExp;
    public UIItemList uiRewardItems;
    public UICurrency uiRewardCurrency;
    public Button buttonRestart;
    public Button buttonGoToManageScene;
    public Button buttonGoToNextStage;
    public BaseStage NextStage
    {
        get
        {
            var unlockStages = BaseGamePlayManager.PlayingStage.unlockStages;
            if (unlockStages != null && unlockStages.Length > 0)
                return unlockStages[0];
            return null;
        }
    }

    private BaseGamePlayManager manager;
    public BaseGamePlayManager Manager
    {
        get
        {
            if (manager == null)
                manager = FindObjectOfType<BaseGamePlayManager>();
            return manager;
        }
    }

    public override void Show()
    {
        base.Show();
        buttonRestart.onClick.RemoveListener(OnClickRestart);
        buttonRestart.onClick.AddListener(OnClickRestart);
        buttonGoToManageScene.onClick.RemoveListener(OnClickGoToManageScene);
        buttonGoToManageScene.onClick.AddListener(OnClickGoToManageScene);
        buttonGoToNextStage.onClick.RemoveListener(OnClickGoToNextStage);
        buttonGoToNextStage.onClick.AddListener(OnClickGoToNextStage);
        buttonGoToNextStage.interactable = NextStage != null;

        if (ratingAnimator != null)
            ratingAnimator.SetInteger(ANIM_KEY_BATTLE_RATING, data.rating);
    }

    public override void Clear()
    {
        if (uiPlayer != null)
            uiPlayer.Clear();

        if (textRewardPlayerExp != null)
            textRewardPlayerExp.text = "0";

        if (textRewardCharacterExp != null)
            textRewardCharacterExp.text = "0";

        if (uiRewardItems != null)
            uiRewardItems.ClearListItems();

        if (uiRewardCurrency != null)
        {
            var currencyData = PlayerCurrency.SoftCurrency.Clone().SetAmount(0, 0);
            uiRewardCurrency.SetData(currencyData);
        }
    }

    public override bool IsEmpty()
    {
        return data == null;
    }

    public override void UpdateData()
    {
        if (uiPlayer != null)
            uiPlayer.SetData(data.player);

        if (textRewardPlayerExp != null)
            textRewardPlayerExp.text = data.rewardPlayerExp.ToString("N0");

        if (textRewardCharacterExp != null)
            textRewardCharacterExp.text = data.rewardCharacterExp.ToString("N0");

        if (uiRewardItems != null)
        {
            uiRewardItems.selectable = false;
            uiRewardItems.multipleSelection = false;
            uiRewardItems.SetListItems(data.rewardItems);
        }

        if (uiRewardCurrency != null)
        {
            var currencyData = PlayerCurrency.SoftCurrency.Clone().SetAmount(data.rewardSoftCurrency, 0);
            uiRewardCurrency.SetData(currencyData);
        }
    }

    public void OnClickRestart()
    {
        Manager.Restart();
    }

    public void OnClickGoToManageScene()
    {
        GameInstance.Singleton.LoadManageScene();
    }

    public void OnClickGoToNextStage()
    {
        var nextStage = NextStage;
        if (nextStage != null)
            BaseGamePlayManager.StartStage(nextStage, null);
    }
}
