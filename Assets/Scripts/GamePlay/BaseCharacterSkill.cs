using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCharacterSkill
{
    public int Level { get; protected set; }
    public BaseSkill Skill { get; protected set; }

    public BaseCharacterSkill(int level, BaseSkill skill)
    {
        Level = level;
        Skill = skill;
    }

    public abstract float GetCoolDownDurationRate();
    public abstract float GetCoolDownDuration();
}
