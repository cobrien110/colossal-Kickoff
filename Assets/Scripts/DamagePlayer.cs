using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    public int damage = 1;
    private float cooldown = 0.05f;
    private float timer = 0;
    public float spawnProtectionRadius = 0.75f;

    private void Start()
    {
        GameObject[] obs = GameObject.FindGameObjectsWithTag("WarriorSpawner");
        for (int i = 0; i < obs.Length; i++)
        {
            Vector3 pos = obs[i].transform.position;
            pos.y = transform.position.y;
            float dis = Vector3.Distance(transform.position, obs[i].transform.position);
            //Debug.Log("Distance: " + dis);
            if (dis < spawnProtectionRadius) Destroy(gameObject);
        }
    }

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
