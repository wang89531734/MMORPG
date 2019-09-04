using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSkill : BaseCharacterSkill
{
    private int turnsCount;
    public Skill CastedSkill { get { return Skill as Skill; } }
    public int CoolDownTurns { get { return CastedSkill.GetCoolDownTurns(Level); } }
    public int TurnsCount { get { return turnsCount; } }
    public int RemainsTurns { get { return CoolDownTurns - TurnsCount; } }

    public CharacterSkill(int level, BaseSkill skill) : base(level, skill)
    {
        turnsCount = CoolDownTurns;
    }

    public void IncreaseTurnsCount()
    {
        if (IsReady())
            return;

        ++turnsCount;
    }

    public bool IsReady()
    {
        return TurnsCount >= CoolDownTurns;
    }

    public void OnUseSkill()
    {
        turnsCount = 0;
    }

    public override float GetCoolDownDurationRate()
    {
        return (float)TurnsCount / (float)CoolDownTurns;
    }

    public override float GetCoolDownDuration()
    {
        return RemainsTurns;
    }
}
