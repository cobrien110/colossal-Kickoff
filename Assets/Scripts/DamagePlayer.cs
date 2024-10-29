using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    public int damage = 1;
    private float cooldown = 0.05f;
    private float timer = 0;

    private void OnTriggerStay(Collider other)
    {
        WarriorController WC = other.GetComponent<WarriorController>();
        if (WC != null)
        {
            timer += Time.deltaTime;
            if (timer >= cooldown)
            {
                WC.DamageWithInstantInvincibility(damage);
                timer = 0;
            }
        }
    }
}
