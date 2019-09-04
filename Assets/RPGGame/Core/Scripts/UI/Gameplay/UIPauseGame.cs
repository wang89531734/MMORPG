using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPauseGame : UIBase
{
    public Button buttonContinue;
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
        buttonContinue.onClick.RemoveListener(OnClickContinue);
        buttonContinue.onClick.AddListener(OnClickContinue);
        buttonRestart.onClick.RemoveListener(OnClickRestart);
        buttonRestart.onClick.AddListener(OnClickRestart);
        buttonGiveUp.onClick.RemoveListener(OnClickGiveUp);
        buttonGiveUp.onClick.AddListener(OnClickGiveUp);
    }

    public void OnClickContinue()
    {
        Hide();
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
