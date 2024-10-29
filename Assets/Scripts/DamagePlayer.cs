using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerStay(Collider other)
    {
        WarriorController WC = other.GetComponent<WarriorController>();
        if (WC != null)
        {
            WC.Damage(damage);
        }
    }
}
