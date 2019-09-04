using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attribute<T>
{
    public T minValue;
    public T maxValue;
    public float growth;
    public abstract T Calculate(int currentLevel, int maxLevel);
    public virtual string ToJson()
    {
        return "{\"minValue\":" + minValue+ "," +
            "\"maxValue\":" + maxValue + "," +
            "\"growth\":" + growth + "}";
    }
}

[Serializable]
public class Int32Attribute : Attribute<int>
{
    public Int32Attribute Clone()
    {
        var result = new Int32Attribute();
        result.minValue = minValue;
        result.maxValue = maxValue;
        result.growth = growth;
        return result;
    }

    public override int Calculate(int currentLevel, int maxLevel)
    {
        if (currentLevel <= 0)
            currentLevel = 1;
        if (maxLevel <= 0)
            maxLevel = 1;
        if (currentLevel == 1)
            return minValue;
        if (currentLevel == maxLevel)
            return maxValue;
        return minValue + Mathf.RoundToInt((maxValue - minValue) * Mathf.Pow((float)(currentLevel - 1) / (float)(maxLevel - 1), growth));
    }
    
    public static Int32Attribute operator *(Int32Attribute a, float multiplier)
    {
        var result = a.Clone();
        result.minValue = Mathf.RoundToInt(a.minValue * multiplier);
        result.maxValue = Mathf.RoundToInt(a.maxValue * multiplier);
        return result;
    }
}

[Serializable]
public class SingleAttribute : Attribute<float>
{
    public SingleAttribute Clone()
    {
        var result = new SingleAttribute();
        result.minValue = minValue;
        result.maxValue = maxValue;
        result.growth = growth;
        return result;
    }

    public override float Calculate(int currentLevel, int maxLevel)
    {
        if (currentLevel <= 0)
            currentLevel = 1;
        if (maxLevel <= 0)
            maxLevel = 1;
        if (currentLevel == 1)
            return minValue;
        if (currentLevel == maxLevel)
            return maxValue;
        return minValue + ((maxValue - minValue) * Mathf.Pow((float)(currentLevel - 1) / (float)(maxLevel - 1), growth));
    }

    public static SingleAttribute operator *(SingleAttribute a, float multiplier)
    {
        var result = a.Clone();
        result.minValue = a.minValue * multiplier;
        result.maxValue = a.maxValue * multiplier;
        return result;
    }
}

[Serializable]
public class Attributes
{
    [Tooltip("Max Hp, When battle if character's Hp = 0, The character will die")]
    public Int32Attribute hp = new Int32Attribute();
    [Tooltip("P.Attack (P stands for physical), This will minus to pDef to calculate damage")]
    public Int32Attribute pAtk = new Int32Attribute();
    [Tooltip("P.Defend (P stands for physical), pAtk will minus to this to calculate damage")]
    public Int32Attribute pDef = new Int32Attribute();
    [Tooltip("M.Attack (M stands for magical), This will minus to mDef to calculate damage")]
    public Int32Attribute mAtk = new Int32Attribute();
    [Tooltip("M.Defend (M stands for magical), mAtk will minus to this to calculate damage")]
    public Int32Attribute mDef = new Int32Attribute();
    [Tooltip("Speed, Character with higher speed will have more chance to attack")]
    public Int32Attribute spd = new Int32Attribute();
    [Tooltip("Evasion, Character with higher evasion will have more chance to avoid damage from character with lower accuracy")]
    public Int32Attribute eva = new Int32Attribute();
    [Tooltip("Accuracy, Character with higher accuracy will have more chance to take damage to character with lower evasion")]
    public Int32Attribute acc = new Int32Attribute();

    public Attributes Clone()
    {
        Attributes result = new Attributes();
        result.hp = hp.Clone();
        result.pAtk = pAtk.Clone();
        result.pDef = pDef.Clone();
        result.mAtk = mAtk.Clone();
        result.mDef = mDef.Clone();
        result.spd = spd.Clone();
        result.eva = eva.Clone();
        result.acc = acc.Clone();
        return result;
    }

    public CalculationAttributes CreateCalculationAttributes(int currentLevel, int maxLevel)
    {
        CalculationAttributes result = new CalculationAttributes();
        result.hp = hp.Calculate(currentLevel, maxLevel);
        result.pAtk = pAtk.Calculate(currentLevel, maxLevel);
        result.pDef = pDef.Calculate(currentLevel, maxLevel);
        result.mAtk = mAtk.Calculate(currentLevel, maxLevel);
        result.mDef = mDef.Calculate(currentLevel, maxLevel);
        result.spd = spd.Calculate(currentLevel, maxLevel);
        result.eva = eva.Calculate(currentLevel, maxLevel);
        result.acc = acc.Calculate(currentLevel, maxLevel);
        return result;
    }

    public Attributes CreateOverrideMaxLevelAttributes(int defaultMaxLevel, int newMaxLevel)
    {
        Attributes attributes = new Attributes();
        var hp = this.hp.Clone();
        hp.maxValue = this.hp.Calculate(newMaxLevel, defaultMaxLevel);
        attributes.hp = hp;

        var pAtk = this.pAtk.Clone();
        pAtk.maxValue = this.pAtk.Calculate(newMaxLevel, defaultMaxLevel);
        attributes.pAtk = pAtk;

        var pDef = this.pDef.Clone();
        pDef.maxValue = this.pDef.Calculate(newMaxLevel, defaultMaxLevel);
        attributes.pDef = pDef;

        var mAtk = this.mAtk.Clone();
        mAtk.maxValue = this.mAtk.Calculate(newMaxLevel, defaultMaxLevel);
        attributes.mAtk = mAtk;

        var mDef = this.mDef.Clone();
        mDef.maxValue = this.mDef.Calculate(newMaxLevel, defaultMaxLevel);
        attributes.mDef = mDef;

        var spd = this.spd.Clone();
        spd.maxValue = this.spd.Calculate(newMaxLevel, defaultMaxLevel);
        attributes.spd = spd;

        var eva = this.eva.Clone();
        eva.maxValue = this.eva.Calculate(newMaxLevel, defaultMaxLevel);
        attributes.eva = eva;

        var acc = this.acc.Clone();
        acc.maxValue = this.acc.Calculate(newMaxLevel, defaultMaxLevel);
        attributes.acc = acc;

        return attributes;
    }

    public static Attributes operator *(Attributes a, float b)
    {
        Attributes result = new Attributes();
        result.hp = a.hp * b;
        result.pAtk = a.pAtk * b;
        result.pDef = a.pDef * b;
        result.mAtk = a.mAtk * b;
        result.mDef = a.mDef * b;
        result.spd = a.spd * b;
        result.eva = a.eva * b;
        result.acc = a.acc * b;
        return result;
    }
}

[Serializable]
public class CalculationAttributes
{
    [Header("Fix attributes")]
    [Tooltip("C.hp (C stands for Character) = C.hp + this")]
    public float hp;
    [Tooltip("C.pAtk (C stands for Character) = C.pAtk + this")]
    public float pAtk;
    [Tooltip("C.pDef (C stands for Character) = C.pDef + this")]
    public float pDef;
    [Tooltip("C.mAtk (C stands for Character) = C.mAtk + this")]
    public float mAtk;
    [Tooltip("C.mDef (C stands for Character) = C.mDef + this")]
    public float mDef;
    [Tooltip("C.Spd (C stands for Character) = C.Spd + this")]
    public float spd;
    [Tooltip("C.Eva (C stands for Character) = C.Eva + this")]
    public float eva;
    [Tooltip("C.Acc (C stands for Character) = C.Acc + this")]
    public float acc;
    [Header("Rate attributes")]
    [Tooltip("C.hp (C stands for Character) = C.hp + (this * C.hp)")]
    public float hpRate;
    [Tooltip("C.pAtk (C stands for Character) = C.pAtk + (this * C.pAtk)")]
    public float pAtkRate;
    [Tooltip("C.pDef (C stands for Character) = C.pDef + (this * C.pDef)")]
    public float pDefRate;
    [Tooltip("C.mAtk (C stands for Character) = C.mAtk + (this * C.mAtk)")]
    public float mAtkRate;
    [Tooltip("C.mDef (C stands for Character) = C.mDef + (this * C.mDef)")]
    public float mDefRate;
    [Tooltip("C.Spd (C stands for Character) = C.Spd + (this * C.Spd)")]
    public float spdRate;
    [Tooltip("C.Eva (C stands for Character) = C.Eva + (this * C.Eva)")]
    public float evaRate;
    [Tooltip("C.Acc (C stands for Character) = C.Acc + (this * C.Acc)")]
    public float accRate;
    [Header("Critical attributes")]
    [Range(0f, 1f)]
    [Tooltip("Chance to critical attack")]
    public float critChance;
    [Range(1f, 100f)]
    [Tooltip("Damage when critical attack = this * Damage")]
    public float critDamageRate;
    [Header("Block attributes")]
    [Range(0f, 1f)]
    [Tooltip("Chance to block")]
    public float blockChance;
    [Range(1f, 100f)]
    [Tooltip("Damage when block = this / Damage")]
    public float blockDamageRate;

    public CalculationAttributes Clone()
    {
        CalculationAttributes result = new CalculationAttributes();
        result.hp = hp;
        result.pAtk = pAtk;
        result.pDef = pDef;
        result.mAtk = mAtk;
        result.mDef = mDef;
        result.spd = spd;
        result.eva = eva;
        result.acc = acc;

        result.hpRate = hpRate;
        result.pAtkRate = pAtkRate;
        result.pDefRate = pDefRate;
        result.mAtkRate = mAtkRate;
        result.mDefRate = mDefRate;
        result.spdRate = spdRate;
        result.evaRate = evaRate;
        result.accRate = accRate;

        result.critChance = critChance;
        result.critDamageRate = critDamageRate;

        result.blockChance = blockChance;
        result.blockDamageRate = blockDamageRate;
        return result;
    }

    #region Calculating between CalculationAttributes and CalculationAttributes
    public static CalculationAttributes operator +(CalculationAttributes a, CalculationAttributes b)
    {
        CalculationAttributes result = a.Clone();
        result.hp += b.hp;
        result.pAtk += b.pAtk;
        result.pDef += b.pDef;
        result.mAtk += b.mAtk;
        result.mDef += b.mDef;
        result.spd += b.spd;
        result.eva += b.eva;
        result.acc += b.acc;

        result.hpRate += b.hpRate;
        result.pAtkRate += b.pAtkRate;
        result.pDefRate += b.pDefRate;
        result.mAtkRate += b.mAtkRate;
        result.mDefRate += b.mDefRate;
        result.spdRate += b.spdRate;
        result.evaRate += b.evaRate;
        result.accRate += b.accRate;

        result.critChance += b.critChance;
        result.critDamageRate += b.critDamageRate;

        result.blockChance += b.blockChance;
        result.blockDamageRate += b.blockDamageRate;
        return result;
    }

    public static CalculationAttributes operator -(CalculationAttributes a, CalculationAttributes b)
    {
        CalculationAttributes result = a.Clone();
        result.hp -= b.hp;
        result.pAtk -= b.pAtk;
        result.pDef -= b.pDef;
        result.mAtk -= b.mAtk;
        result.mDef -= b.mDef;
        result.spd -= b.spd;
        result.eva -= b.eva;
        result.acc -= b.acc;

        result.hpRate -= b.hpRate;
        result.pAtkRate -= b.pAtkRate;
        result.pDefRate -= b.pDefRate;
        result.mAtkRate -= b.mAtkRate;
        result.mDefRate -= b.mDefRate;
        result.spdRate -= b.spdRate;
        result.evaRate -= b.evaRate;
        result.accRate -= b.accRate;

        result.critChance -= b.critChance;
        result.critDamageRate -= b.critDamageRate;

        result.blockChance -= b.blockChance;
        result.blockDamageRate -= b.blockDamageRate;
        return result;
    }

    public static CalculationAttributes operator *(CalculationAttributes a, float b)
    {
        CalculationAttributes result = new CalculationAttributes();
        result.hp = Mathf.CeilToInt(a.hp * b);
        result.pAtk = Mathf.CeilToInt(a.pAtk * b);
        result.pDef = Mathf.CeilToInt(a.pDef * b);
        result.mAtk = Mathf.CeilToInt(a.mAtk * b);
        result.mDef = Mathf.CeilToInt(a.mDef * b);
        result.spd = Mathf.CeilToInt(a.spd * b);
        result.eva = Mathf.CeilToInt(a.eva * b);
        result.acc = Mathf.CeilToInt(a.acc * b);

        result.hpRate = a.hpRate * b;
        result.pAtkRate = a.pAtkRate * b;
        result.pDefRate = a.pDefRate * b;
        result.mAtkRate = a.mAtkRate * b;
        result.mDefRate = a.mDefRate * b;
        result.spdRate = a.spdRate * b;
        result.evaRate = a.evaRate * b;
        result.accRate = a.accRate * b;

        result.critChance = a.critChance * b;
        result.critDamageRate = a.critDamageRate * b;

        result.blockChance = a.blockChance * b;
        result.blockDamageRate = a.blockDamageRate * b;
        return result;
    }
    #endregion

    public string GetDescription(CalculationAttributes bonusAttributes)
    {
        var result = "";

        if (hp != 0 || bonusAttributes.hp != 0)
        {
            result += LanguageManager.FormatAttribute(GameText.TITLE_ATTRIBUTE_HP, hp, bonusAttributes.hp);
            result += "\n";
        }
        if (pAtk != 0 || bonusAttributes.pAtk != 0)
        {
            result += LanguageManager.FormatAttribute(GameText.TITLE_ATTRIBUTE_PATK, pAtk, bonusAttributes.pAtk);
            result += "\n";
        }
        if (pDef != 0 || bonusAttributes.pDef != 0)
        {
            result += LanguageManager.FormatAttribute(GameText.TITLE_ATTRIBUTE_PDEF, pDef, bonusAttributes.pDef);
            result += "\n";
        }
        if (mAtk != 0 || bonusAttributes.mAtk != 0)
        {
            result += LanguageManager.FormatAttribute(GameText.TITLE_ATTRIBUTE_MATK, mAtk, bonusAttributes.mAtk);
            result += "\n";
        }
        if (mDef != 0 || bonusAttributes.mDef != 0)
        {
            result += LanguageManager.FormatAttribute(GameText.TITLE_ATTRIBUTE_MDEF, mDef, bonusAttributes.mDef);
            result += "\n";
        }
        if (spd != 0 || bonusAttributes.spd != 0)
        {
            result += LanguageManager.FormatAttribute(GameText.TITLE_ATTRIBUTE_SPD, spd, bonusAttributes.spd);
            result += "\n";
        }
        if (eva != 0 || bonusAttributes.eva != 0)
        {
            result += LanguageManager.FormatAttribute(GameText.TITLE_ATTRIBUTE_EVA, eva, bonusAttributes.eva);
            result += "\n";
        }
        if (acc != 0 || bonusAttributes.acc != 0)
        {
            result += LanguageManager.FormatAttribute(GameText.TITLE_ATTRIBUTE_ACC, acc, bonusAttributes.acc);
            result += "\n";
        }
        if (hpRate != 0 || bonusAttributes.hpRate != 0)
        {
            result += LanguageManager.FormatAttribute(GameText.TITLE_ATTRIBUTE_HP_RATE, hpRate, bonusAttributes.hpRate, true);
            result += "\n";
        }
        if (pAtkRate != 0 || bonusAttributes.pAtkRate != 0)
        {
            result += LanguageManager.FormatAttribute(GameText.TITLE_ATTRIBUTE_PATK_RATE, pAtkRate, bonusAttributes.pAtkRate, true);
            result += "\n";
        }
        if (pDefRate != 0 || bonusAttributes.pDefRate != 0)
        {
            result += LanguageManager.FormatAttribute(GameText.TITLE_ATTRIBUTE_PDEF_RATE, pDefRate, bonusAttributes.pDefRate, true);
            result += "\n";
        }
        if (mAtkRate != 0 || bonusAttributes.mAtkRate != 0)
        {
            result += LanguageManager.FormatAttribute(GameText.TITLE_ATTRIBUTE_MATK_RATE, mAtkRate, bonusAttributes.mAtkRate, true);
            result += "\n";
        }
        if (mDefRate != 0 || bonusAttributes.mDefRate != 0)
        {
            result += LanguageManager.FormatAttribute(GameText.TITLE_ATTRIBUTE_MDEF_RATE, mDefRate, bonusAttributes.mDefRate, true);
            result += "\n";
        }
        if (spdRate != 0 || bonusAttributes.spdRate != 0)
        {
            result += LanguageManager.FormatAttribute(GameText.TITLE_ATTRIBUTE_SPD_RATE, spdRate, bonusAttributes.spdRate, true);
            result += "\n";
        }
        if (evaRate != 0 || bonusAttributes.evaRate != 0)
        {
            result += LanguageManager.FormatAttribute(GameText.TITLE_ATTRIBUTE_EVA_RATE, evaRate, bonusAttributes.evaRate, true);
            result += "\n";
        }
        if (accRate != 0 || bonusAttributes.accRate != 0)
        {
            result += LanguageManager.FormatAttribute(GameText.TITLE_ATTRIBUTE_ACC_RATE, accRate, bonusAttributes.accRate, true);
            result += "\n";
        }
        if (critChance != 0 || bonusAttributes.critChance != 0)
        {
            result += LanguageManager.FormatAttribute(GameText.TITLE_ATTRIBUTE_CRIT_CHANCE, critChance, bonusAttributes.critChance, true);
            result += "\n";
        }
        if (critDamageRate != 0 || bonusAttributes.critDamageRate != 0)
        {
            result += LanguageManager.FormatAttribute(GameText.TITLE_ATTRIBUTE_CRIT_DAMAGE_RATE, critDamageRate, bonusAttributes.critDamageRate, true);
            result += "\n";
        }
        if (blockChance != 0 || bonusAttributes.blockChance != 0)
        {
            result += LanguageManager.FormatAttribute(GameText.TITLE_ATTRIBUTE_BLOCK_CHANCE, blockChance, bonusAttributes.blockChance, true);
            result += "\n";
        }
        if (blockDamageRate != 0 || bonusAttributes.blockDamageRate != 0)
        {
            result += LanguageManager.FormatAttribute(GameText.TITLE_ATTRIBUTE_BLOCK_DAMAGE_RATE, blockDamageRate, bonusAttributes.blockDamageRate, true);
        }
        return result;
    }
}
