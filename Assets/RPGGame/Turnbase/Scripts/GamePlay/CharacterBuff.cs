using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBuff : BaseCharacterBuff
{
    public GamePlayManager Manager { get { return GamePlayManager.Singleton; } }
    private int healAmount;
    private int turnsCount;
    public SkillBuff CastedBuff { get { return Buff as SkillBuff; } }
    public int ApplyTurns { get { return CastedBuff.GetApplyTurns(Level); } }
    public int TurnsCount { get { return turnsCount; } }
    public int RemainsTurns { get { return ApplyTurns - TurnsCount; } }

    public CharacterBuff(int level, BaseSkill skill, int buffIndex, BaseCharacterEntity giver, BaseCharacterEntity receiver) : base(level, skill, buffIndex, giver, receiver)
    {
        healAmount = 0;
        var pAtkHealRate = PAtkHealRate;
        var mAtkHealRate = MAtkHealRate;
        if (pAtkHealRate != 0)
            healAmount += Mathf.CeilToInt(giver.Item.Attributes.pAtk * pAtkHealRate);
        if (mAtkHealRate != 0)
            healAmount += Mathf.CeilToInt(giver.Item.Attributes.mAtk * mAtkHealRate);
        ApplyHeal();
    }

    public void IncreaseTurnsCount()
    {
        if (IsEnd())
            return;

        ApplyHeal();
        ++turnsCount;
    }

    public void ApplyHeal()
    {
        if (Mathf.Abs(healAmount) <= 0)
            return;

        Receiver.Hp += healAmount;
        if (healAmount > 0)
            Manager.SpawnHealText(Mathf.Abs(healAmount), Receiver);
        else
            Manager.SpawnPoisonText(Mathf.Abs(healAmount), Receiver);
    }

    public bool IsEnd()
    {
        return TurnsCount >= ApplyTurns;
    }

    public override float GetRemainsDurationRate()
    {
        return (float)TurnsCount / (float)ApplyTurns;
    }

    public override float GetRemainsDuration()
    {
        return RemainsTurns;
    }
}
