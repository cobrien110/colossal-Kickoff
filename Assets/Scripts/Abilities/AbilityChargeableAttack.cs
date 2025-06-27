using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityChargeableAttack : AbilityChargeable
{

    public abstract bool IsEnemyInRange(Transform attacker);
    //public abstract float GetAttackRange();
    //public abstract float GetAttackRadius();

}
