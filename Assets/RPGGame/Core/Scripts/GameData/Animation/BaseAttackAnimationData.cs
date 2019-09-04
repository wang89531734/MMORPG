using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class BaseAttackAnimationData : AnimationData
{
    [System.Obsolete("We're going to use `hitDurationRate`, you can use context menu `Set Hit Duration Rate` to set it, This will be removed on next version")]
    public float hitDuration = 0;
    [Range(0f, 1f)]
    public float hitDurationRate;
    public BaseDamage damage;

    [ContextMenu("Set Hit Duration Rate")]
    public void SetHitDurationRate()
    {
        if (hitDuration > 0 && AnimationDuration > 0)
            hitDurationRate = hitDuration / AnimationDuration;
        if (hitDurationRate > 1)
            hitDurationRate = 1;
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }
}

public static class BaseAttackAnimationDataExtension
{
    public static float GetHitDuration(this BaseAttackAnimationData attackAnimation)
    {
        return attackAnimation == null ? 0f : attackAnimation.hitDurationRate * attackAnimation.AnimationDuration;
    }

    public static BaseDamage GetDamage(this BaseAttackAnimationData attackAnimation)
    {
        return attackAnimation == null ? null : attackAnimation.damage;
    }
}