using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAnimationData : BaseAttackAnimationData
{
    public bool isRangeAttack;
}

public static class AttackAnimationDataExtension
{
    public static bool GetIsRangeAttack(this AttackAnimationData attackAnimation)
    {
        return attackAnimation == null ? false : attackAnimation.isRangeAttack;
    }
}
