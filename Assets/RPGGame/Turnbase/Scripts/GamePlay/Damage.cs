using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TargetingRigidbody))]
public class Damage : BaseDamage
{
    public enum SpawnMode
    {
        SpawnAtAttacker,
        SpawnAtTarget,
    }

    public enum HitSpawnMode
    {
        HitAtBody,
        HitAtFloor,
    }
    [Header("Effects")]
    public CharacterEffectData hitEffects;
    [Header("Range Attack")]
    public float missileSpeed;
    [Header("Spawn")]
    public SpawnMode spawnMode = SpawnMode.SpawnAtAttacker;
    public HitSpawnMode hitSpawnMode = HitSpawnMode.HitAtBody;
    public float spawnOffsetY = 0;

    private CharacterEntity attacker;
    private CharacterEntity target;
    private float pAtkRate;
    private float mAtkRate;
    private int hitCount;
    private int fixDamage;

    private TargetingRigidbody tempTargetingRigidbody;
    public TargetingRigidbody TempTargetingRigidbody
    {
        get
        {
            if (tempTargetingRigidbody == null)
                tempTargetingRigidbody = GetComponent<TargetingRigidbody>();
            return tempTargetingRigidbody;
        }
    }

    private void FixedUpdate()
    {
        if (target == null)
            return;

        if (!TempTargetingRigidbody.IsMoving)
        {
            if (hitEffects != null)
                hitEffects.InstantiatesTo(target);
            attacker.Attack(target, pAtkRate, mAtkRate, hitCount, fixDamage);
            attacker.Damages.Remove(this);
            Destroy(gameObject);
            target = null;
            return;
        }
    }

    public void Setup(CharacterEntity attacker, CharacterEntity target, float pAtkRate = 1f, float mAtkRate = 1f, int hitCount = 1, int fixDamage = 0)
    {
        this.attacker = attacker;
        this.target = target;
        this.pAtkRate = pAtkRate;
        this.mAtkRate = mAtkRate;
        this.hitCount = hitCount;
        this.fixDamage = fixDamage;

        Vector3 targetPosition = Vector3.zero;
        switch (hitSpawnMode)
        {
            case HitSpawnMode.HitAtBody:
                targetPosition = target.bodyEffectContainer.position;
                break;
            case HitSpawnMode.HitAtFloor:
                targetPosition = target.floorEffectContainer.position;
                break;
        }

        if (missileSpeed == 0)
            TempTransform.position = targetPosition;
        else
        {
            switch (spawnMode)
            {
                case SpawnMode.SpawnAtAttacker:
                    TempTargetingRigidbody.StartMove(targetPosition + (Vector3.up * spawnOffsetY), missileSpeed);
                    break;
                case SpawnMode.SpawnAtTarget:
                    TempTargetingRigidbody.StartMove(targetPosition + (Vector3.up * spawnOffsetY), missileSpeed);
                    break;
            }
        }

        this.attacker.Damages.Add(this);
    }
}
