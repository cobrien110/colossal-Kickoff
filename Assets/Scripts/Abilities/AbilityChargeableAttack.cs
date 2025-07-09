using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityChargeableAttack : AbilityChargeable
{

    [SerializeField] protected float minAttackDist = 0f;

    public float GetMinAttackDist()
    {
        return minAttackDist;
    }

    public abstract bool IsEnemyInRange(Transform attacker);
    //public abstract float GetAttackRange();
    //public abstract float GetAttackRadius();

}
