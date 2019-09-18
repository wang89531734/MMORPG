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

    public override void UpdateData()
    {
        SetupInfo(data);
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
}
