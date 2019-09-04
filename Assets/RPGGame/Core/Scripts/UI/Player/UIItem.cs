using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class UIItemEvent : UnityEvent<UIItem> { }

public class UIItem : UIDataItem<PlayerItem>
{
    public enum DisplayStats
    {
        Level,
        SellPrice,
        RewardExp,
        SelectedAmount,
        AttributeHp,
        AttributePAtk,
        AttributePDef,
        AttributeMAtk,
        AttributeMDef,
        AttributeSpd,
        AttributeEva,
        AttributeAcc,
        AttributeHpRate,
        AttributePAtkRate,
        AttributePDefRate,
        AttributeMAtkRate,
        AttributeMDefRate,
        AttributeSpdRate,
        AttributeEvaRate,
        AttributeAccRate,
        AttributeCritChance,
        AttributeCritDamageRate,
        AttributeBlockChance,
        AttributeBlockDamageRate,
        Hidden,
    }
    [Header("General")]
    public Text textTitle;
    public Text textDescription;
    public Image imageIcon;
    [Header("Relates Level Up UIs")]
    public Button buttonLevelUp;
    public UIItemLevelUp uiLevelUp;
    public UnityEvent eventSelectLevelUpItem;
    [Header("Relates Evolve UIs")]
    public Button buttonEvolve;
    public UIItemEvolve uiEvolve;
    public UnityEvent eventSelectEvolveItem;
    [Header("Relates Sell UIs")]
    public Button buttonSell;
    public UIItemSell uiSell;
    public UnityEvent eventSelectSellItem;
    [Header("Relates Equipment Manager UIs")]
    public Button buttonEquipmentManage;
    public UIEquipmentManager uiEquipmentManager;
    public UnityEvent eventSelectEquipmentManageCharacter;
    [Header("Relates Formation Manager UIs")]
    public UIFormationManager uiFormationManager;
    [Header("Usage status")]
    public GameObject inTeamObject;
    public GameObject inSelectedTeamObject;
    public GameObject equippedObject;
    public bool notShowInTeamStatus;
    public bool notShowEquippedStatus;
    [Header("Stats")]
    public DisplayStats displayStats;
    public Text textDisplayStats;
    public Text textAttributes;
    public UILevel uiLevel;
    public UIAttributes uiAttributes;
    public UICurrency uiEvolvePrice;
    public UICurrency uiSellPrice;
    public Text textRewardExp;
    public Text textSelectedAmount;
    [Header("Character")]
    public Transform characterModelContainer;
    public int characterModelLayer;
    [Header("Options")]
    public bool useFormatForInfo;
    public bool excludeEquipmentAttributes;

    // Selection
    private int selectedAmount;
    public int SelectedAmount
    {
        get { return selectedAmount; }
        set
        {
            var maxAmount = data == null ? 0 : data.Amount;
            selectedAmount = value > maxAmount ? maxAmount : value;
            SetupSelectedAmount();
        }
    }

    private int requiredAmount;
    public int RequiredAmount
    {
        get { return requiredAmount; }
        set
        {
            requiredAmount = value;
            SetupSelectedAmount();
        }
    }

    public override bool Selected
    {
        get { return SelectedAmount > 0; }
        set
        {
            if (value)
                SelectedAmount = data.Amount;
            else
                SelectedAmount = 0;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        if (inTeamObject != null)
            inTeamObject.SetActive(false);
        if (inSelectedTeamObject != null)
            inSelectedTeamObject.SetActive(false);
        if (equippedObject != null)
            equippedObject.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();
        UpdateDisplayStats();
        if (!IsEmpty())
        {
            if (!notShowInTeamStatus)
            {
                var isInAnyTeam = data.InTeamFormations.Count > 0;
                var isInSelectedTeam = uiFormationManager != null && data.IsInTeamFormation(uiFormationManager.SelectedFormationName);
                if (inTeamObject != null)
                    inTeamObject.SetActive(isInAnyTeam);
                if (inSelectedTeamObject != null)
                {
                    inSelectedTeamObject.SetActive(isInSelectedTeam);
                    if (inTeamObject != null && isInSelectedTeam)
                        inTeamObject.SetActive(false);
                }
            }
            else
            {
                if (inTeamObject != null)
                    inTeamObject.SetActive(false);
                if (inSelectedTeamObject != null)
                    inSelectedTeamObject.SetActive(false);
            }

            if (!notShowEquippedStatus)
            {
                var isEquipped = data.EquippedByItem != null;
                if (equippedObject != null)
                    equippedObject.SetActive(isEquipped);
            }
            else
            {
                if (equippedObject != null)
                    equippedObject.SetActive(false);
            }
        }
        else
        {
            if (inTeamObject != null)
                inTeamObject.SetActive(false);
            if (inSelectedTeamObject != null)
                inSelectedTeamObject.SetActive(false);
            if (equippedObject != null)
                equippedObject.SetActive(false);
        }
    }

    private void UpdateDisplayStats()
    {
        if (textDisplayStats == null)
            return;

        if (IsEmpty())
        {
            textDisplayStats.text = "";
            return;
        }

        var attributes = data.Attributes;
        var itemData = data.ItemData;

        switch (displayStats)
        {
            case DisplayStats.Level:
                textDisplayStats.text = LanguageManager.FormatInfo(GameText.TITLE_LEVEL, data.Level);
                return;
            case DisplayStats.SellPrice:
                textDisplayStats.text = data.SellPrice.ToString("N0");
                return;
            case DisplayStats.RewardExp:
                textDisplayStats.text = data.RewardExp.ToString("N0");
                return;
            case DisplayStats.SelectedAmount:
                textDisplayStats.text = SelectedAmount.ToString("N0");
                return;
            case DisplayStats.AttributeHp:
                textDisplayStats.text = attributes == null ? "" : LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_HP, attributes.hp);
                return;
            case DisplayStats.AttributePAtk:
                textDisplayStats.text = attributes == null ? "" : LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_PATK, attributes.pAtk);
                return;
            case DisplayStats.AttributePDef:
                textDisplayStats.text = attributes == null ? "" : LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_PDEF, attributes.pDef);
                return;
            case DisplayStats.AttributeMAtk:
                textDisplayStats.text = attributes == null ? "" : LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_MATK, attributes.mAtk);
                return;
            case DisplayStats.AttributeMDef:
                textDisplayStats.text = attributes == null ? "" : LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_MDEF, attributes.mDef);
                return;
            case DisplayStats.AttributeSpd:
                textDisplayStats.text = attributes == null ? "" : LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_SPD, attributes.spd);
                return;
            case DisplayStats.AttributeEva:
                textDisplayStats.text = attributes == null ? "" : LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_EVA, attributes.eva);
                return;
            case DisplayStats.AttributeAcc:
                textDisplayStats.text = attributes == null ? "" : LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_ACC, attributes.acc);
                return;
            case DisplayStats.AttributeHpRate:
                textDisplayStats.text = attributes == null ? "" : LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_HP_RATE, attributes.hpRate);
                return;
            case DisplayStats.AttributePAtkRate:
                textDisplayStats.text = attributes == null ? "" : LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_PATK_RATE, attributes.pAtkRate);
                return;
            case DisplayStats.AttributePDefRate:
                textDisplayStats.text = attributes == null ? "" : LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_PDEF_RATE, attributes.pDefRate);
                return;
            case DisplayStats.AttributeMAtkRate:
                textDisplayStats.text = attributes == null ? "" : LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_MATK_RATE, attributes.mAtkRate);
                return;
            case DisplayStats.AttributeMDefRate:
                textDisplayStats.text = attributes == null ? "" : LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_MDEF_RATE, attributes.mDefRate);
                return;
            case DisplayStats.AttributeSpdRate:
                textDisplayStats.text = attributes == null ? "" : LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_SPD_RATE, attributes.spdRate);
                return;
            case DisplayStats.AttributeEvaRate:
                textDisplayStats.text = attributes == null ? "" : LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_EVA_RATE, attributes.evaRate);
                return;
            case DisplayStats.AttributeAccRate:
                textDisplayStats.text = attributes == null ? "" : LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_ACC_RATE, attributes.accRate);
                return;
            case DisplayStats.AttributeCritChance:
                textDisplayStats.text = attributes == null ? "" : LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_CRIT_CHANCE, attributes.critChance);
                return;
            case DisplayStats.AttributeCritDamageRate:
                textDisplayStats.text = attributes == null ? "" : LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_CRIT_DAMAGE_RATE, attributes.critDamageRate);
                return;
            case DisplayStats.AttributeBlockChance:
                textDisplayStats.text = attributes == null ? "" : LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_BLOCK_CHANCE, attributes.blockChance);
                return;
            case DisplayStats.AttributeBlockDamageRate:
                textDisplayStats.text = attributes == null ? "" : LanguageManager.FormatInfo(GameText.TITLE_ATTRIBUTE_BLOCK_DAMAGE_RATE, attributes.blockDamageRate);
                return;
            default:
                textDisplayStats.text = "";
                return;
        }
    }

    public override void UpdateData()
    {
        SetupInfo(data);
        SetupSelectedAmount();
        if (buttonLevelUp != null)
        {
            buttonLevelUp.onClick.RemoveListener(OnClickLevelUp);
            buttonLevelUp.onClick.AddListener(OnClickLevelUp);
            buttonLevelUp.interactable = !IsEmpty() && data.CanLevelUp;
        }
        if (buttonEvolve != null)
        {
            buttonEvolve.onClick.RemoveListener(OnClickEvolve);
            buttonEvolve.onClick.AddListener(OnClickEvolve);
            buttonEvolve.interactable = !IsEmpty() && data.CanEvolve;
        }
        if (buttonSell != null)
        {
            buttonSell.onClick.RemoveListener(OnClickSell);
            buttonSell.onClick.AddListener(OnClickSell);
            buttonSell.interactable = !IsEmpty() && data.CanSell;
        }
        if (buttonEquipmentManage != null)
        {
            buttonEquipmentManage.onClick.RemoveListener(OnClickManageEquipment);
            buttonEquipmentManage.onClick.AddListener(OnClickManageEquipment);
            buttonEquipmentManage.interactable = !IsEmpty() && data.CharacterData != null;
        }
    }

    public override void Clear()
    {
        SetupInfo(null);
        SelectedAmount = 0;
        RequiredAmount = 0;
        SetupSelectedAmount();
    }
    
    private void SetupInfo(PlayerItem data)
    {
        if (data == null)
            data = new PlayerItem();

        var attributes = data.Attributes;
        var itemData = data.ItemData;

        if (textTitle != null)
            textTitle.text = itemData == null ? "" : itemData.title;

        if (textDescription != null)
            textDescription.text = itemData == null ? "" : itemData.description;

        if (imageIcon != null)
            imageIcon.sprite = data.Icon;

        if (textAttributes != null)
            textAttributes.text = attributes == null ? "" : attributes.GetDescription(data.EquipmentBonus);

        // Attributes
        if (uiAttributes != null)
        {
            if (data.ActorItemData != null)
                uiAttributes.Show();
            else
                uiAttributes.Hide();

            if (excludeEquipmentAttributes)
                uiAttributes.SetData(attributes);
            else
                uiAttributes.SetData(attributes == null ? data.EquipmentBonus : attributes + data.EquipmentBonus);
        }

        // Stats
        if (uiLevel != null)
        {
            uiLevel.gameObject.SetActive(data.ActorItemData != null);
            uiLevel.level = data.Level;
            uiLevel.maxLevel = data.MaxLevel;
            uiLevel.collectExp = data.CollectExp;
            uiLevel.nextExp = data.NextExp;
        }

        if (uiEvolvePrice != null)
        {
            var amount = data.Tier == null ? 0 : data.EvolvePrice;
            var currencyData = PlayerCurrency.SoftCurrency.Clone().SetAmount(amount, 0);
            uiEvolvePrice.SetData(currencyData);
        }

        if (uiSellPrice != null)
        {
            var amount = data.Tier == null ? 0 : data.SellPrice;
            var currencyData = PlayerCurrency.SoftCurrency.Clone().SetAmount(amount, 0);
            uiSellPrice.SetData(currencyData);
        }

        if (textRewardExp != null)
            textRewardExp.text = useFormatForInfo ? LanguageManager.FormatInfo(GameText.TITLE_REWARD_EXP, data.RewardExp) : data.RewardExp.ToString("N0");

        if (characterModelContainer != null)
        {
            characterModelContainer.RemoveAllChildren();
            if (data.CharacterData != null && data.CharacterData.model != null)
            {
                var character = Instantiate(data.CharacterData.model);
                character.Container = characterModelContainer;
                var characterRenderers = character.GetComponentsInChildren<Renderer>();
                foreach (var characterRenderer in characterRenderers)
                {
                    characterRenderer.gameObject.layer = characterModelLayer;
                }
            }
        }
    }

    public void SetupSelectedAmount()
    {
        SetupSelectedAmount(SelectedAmount, RequiredAmount);
    }

    public void SetupSelectedAmount(int selectedAmount, int requiredAmount)
    {
        if (textSelectedAmount != null)
        {
            if (selectedAmount == 0 && requiredAmount == 0)
                textSelectedAmount.text = "";
            else if (requiredAmount == 0)
                textSelectedAmount.text = selectedAmount.ToString("N0");
            else if (selectedAmount == 0)
                textSelectedAmount.text = "0/" + requiredAmount.ToString("N0");
            else
                textSelectedAmount.text = selectedAmount.ToString("N0") + "/" + requiredAmount.ToString("N0");
        }
    }

    public void Select(int amount, bool invokeEvent = true)
    {
        SelectedAmount = amount;
        if (invokeEvent)
            eventSelect.Invoke(this);
    }

    public void OnClickLevelUp()
    {
        if (uiLevelUp != null)
        {
            uiLevelUp.Item = data;
            eventSelectLevelUpItem.Invoke();
        }
    }

    public void OnClickEvolve()
    {
        if (uiEvolve != null)
        {
            uiEvolve.Item = data;
            eventSelectEvolveItem.Invoke();
        }
    }

    public void OnClickSell()
    {
        if (uiSell != null)
        {
            uiSell.SetSelectingItemIds(new List<string>() { data.Id });
            eventSelectSellItem.Invoke();
        }
    }

    public void OnClickManageEquipment()
    {
        if (uiEquipmentManager != null)
        {
            uiEquipmentManager.Character = data;
            eventSelectEquipmentManageCharacter.Invoke();
        }
    }

    public override bool IsEmpty()
    {
        return data == null || string.IsNullOrEmpty(data.DataId);
    }
}
