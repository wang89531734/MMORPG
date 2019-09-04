using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStamina : UIDataItem<PlayerStamina>
{
    public Image imageIcon;
    public Text textAmount;
    public Text recoveryingTime;
    public bool isCurrentPlayerStamina;

    private GameDatabase gameDatabase { get { return GameInstance.GameDatabase; } }
    private int tempMaxStamina;
    public override void UpdateData()
    {
        SetupInfo(data);
    }

    public override void Clear()
    {
        SetupInfo(null);
    }

    private void SetupInfo(PlayerStamina data)
    {
        if (data == null)
            data = new PlayerStamina();

        var staminaData = data.StaminaData;

        if (imageIcon != null)
            imageIcon.sprite = staminaData == null ? null : staminaData.icon;

        if (textAmount != null)
            textAmount.text = data.Amount.ToString("N0");
    }

    protected override void Update()
    {
        base.Update();

        if (isCurrentPlayerStamina)
        {
            tempMaxStamina = 0;
            Stamina staminaTable = null;
            if (gameDatabase != null && gameDatabase.Staminas.TryGetValue(data.dataId, out staminaTable))
            {
                tempMaxStamina = staminaTable.maxAmountTable.Calculate(Player.CurrentPlayer.Level, gameDatabase.playerMaxLevel);
                if (data.Amount < tempMaxStamina)
                {
                    var currentTimeInMillisecond = (System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond) + GameInstance.GameService.ServiceTimeOffset;
                    var diffTimeInMillisecond = currentTimeInMillisecond - data.RecoveredTime;

                    var devideAmount = 1;
                    switch (staminaTable.recoverUnit)
                    {
                        case StaminaUnit.Days:
                            devideAmount = 1000 * 60 * 60 * 24;
                            break;
                        case StaminaUnit.Hours:
                            devideAmount = 1000 * 60 * 60;
                            break;
                        case StaminaUnit.Minutes:
                            devideAmount = 1000 * 60;
                            break;
                        case StaminaUnit.Seconds:
                            devideAmount = 1000;
                            break;
                    }
                    var countDownInMillisecond = (staminaTable.recoverDuration * devideAmount) - diffTimeInMillisecond;
                    var recoveryAmount = (int)(diffTimeInMillisecond / devideAmount) / staminaTable.recoverDuration;
                    if (recoveryAmount > 0)
                    {
                        data.Amount += recoveryAmount;
                        if (data.Amount > tempMaxStamina)
                            data.Amount = tempMaxStamina;
                        data.RecoveredTime = currentTimeInMillisecond;

                        if (textAmount != null)
                            textAmount.text = data.Amount.ToString("N0");
                    }

                    if (recoveryingTime != null)
                    {
                        System.TimeSpan time = System.TimeSpan.FromSeconds(countDownInMillisecond * System.TimeSpan.TicksPerMillisecond / System.TimeSpan.TicksPerSecond);
                        recoveryingTime.text = string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);
                    }
                    else
                    {
                        if (recoveryingTime != null)
                            recoveryingTime.text = "";
                    }
                }
                else
                {
                    if (recoveryingTime != null)
                        recoveryingTime.text = "";
                }
            }
            else
            {
                if (recoveryingTime != null)
                    recoveryingTime.text = "";
            }
        }
    }

    public override bool IsEmpty()
    {
        return data == null || string.IsNullOrEmpty(data.DataId);
    }
}
