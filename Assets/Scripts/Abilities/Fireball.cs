using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public MonsterController MC;
    public float speed = 1;
    public Rigidbody RB;

    // Start is called before the first frame update
    public void Activate()
    {
        Vector3 dir = (MC.transform.position - transform.position).normalized;
        //transform.LookAt(MC.transform, Vector3.up);
        RB.AddForce(dir * speed, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            RB.velocity = Vector3.zero;
        }

        if (other.CompareTag("Warrior"))
        {
            WarriorController WC = other.GetComponent<WarriorController>();
            WC.Damage(1);
            if (!WC.isInvincible)
            {
                Destroy(gameObject);
            }
        }
    }
}
