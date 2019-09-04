using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILose : UIBase
{
    public Button buttonRevive;
    public Button buttonRestart;
    public Button buttonGiveUp;

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
        buttonRevive.onClick.RemoveListener(OnClickRevive);
        buttonRevive.onClick.AddListener(OnClickRevive);
        buttonRestart.onClick.RemoveListener(OnClickRestart);
        buttonRestart.onClick.AddListener(OnClickRestart);
        buttonGiveUp.onClick.RemoveListener(OnClickGiveUp);
        buttonGiveUp.onClick.AddListener(OnClickGiveUp);
    }

    public void OnClickRevive()
    {
        Hide();
        Manager.Revive(Show);
    }

    public void OnClickRestart()
    {
        Hide();
        Manager.Restart();
    }

    public void OnClickGiveUp()
    {
        Hide();
        Manager.Giveup(Show);
    }
}
