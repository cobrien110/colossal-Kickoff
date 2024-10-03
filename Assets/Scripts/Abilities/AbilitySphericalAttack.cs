using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AbilitySphericalAttack : AbilityChargeable
{
    [Header("Attack Stats")]
    public float attackRange = 1f;
    public float attackBaseRadius = 1f;

    [Header("Ability Specific Variables")]
    public float attackVisualOffsetY;
    public LayerMask affectedLayers;
    public bool canSpawnProjectile = true;
    public GameObject projectilePrefab;
    public int projectileDamage = 1;
    public float projectileSpeed = 450f;

    public override void Activate()
    {
        Debug.Log("Attack!");
        if (BP != null && BP.ballOwner != gameObject && GM.isPlaying)
        {
            Debug.Log("Performing Attack");
            Debug.Log(chargeAmount);
            Vector3 origin = new Vector3(transform.position.x, transform.position.y + attackVisualOffsetY, transform.position.z);
            Collider[] colliders = Physics.OverlapSphere(origin + transform.forward * attackRange, attackBaseRadius + chargeAmount * chargeRate, affectedLayers);

            foreach (Collider col in colliders)
            {
                // Handle collision with each collider
                Debug.Log("SphereCast hit " + col.gameObject.name);
                if (col.gameObject.CompareTag("Warrior"))
                {
                    WarriorController WC = col.GetComponent<WarriorController>();
                    if (!WC.isInvincible)
                        WC.Die();
                    else
                        Debug.Log("Warrior is invincible");
                }
                if (col.gameObject.CompareTag("Ball") && BP.ballOwner == null)
                {
                    // SWIPE AWAY BALL - UNUSED FOR NOW
                    if (canHitBall)
                    {
                        Debug.Log("AXE HIT BALL!");
                        float kickForce = attackHitForce;
                        Vector3 posA = new Vector3(BP.gameObject.transform.position.x, 0f, BP.gameObject.transform.position.z);
                        Vector3 posB = new Vector3(transform.position.x, 0f, transform.position.z);
                        Vector3 dir = (posA - posB).normalized;
                        Vector3 forceToAdd = dir * kickForce;
                        BP.GetComponent<Rigidbody>().AddForce(forceToAdd);
                    }
                }
            }

            if (chargeAmount < maxChargeSeconds)
            {
                audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find("minotaurAxeAttack"), 0.7f);
            }
            else
            {
                audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find("minotaurAxeAttackCharged"), 0.7f);
                if (canSpawnProjectile) SpawnShrapnel();
            }
            timer = 0;
            chargeAmount = 0;
            isCharging = false;
            ANIM.Play(activatedAnimationName);

            StartCoroutine(MC.MoveDelay());
        }
    }

    void SpawnShrapnel()
    {
        if (projectilePrefab == null) return;
        Vector3 pos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        GameObject shrap = Instantiate(projectilePrefab, pos, Quaternion.LookRotation(transform.forward, Vector3.up));
        WallShrapnel WS = shrap.GetComponent<WallShrapnel>();
        WS.damage = projectileDamage;
        WS.speed = projectileSpeed;
    }

    public override void ResizeAttackVisual()
    {
        if (attackVisualizer == null) return;
        attackVisualizer.transform.localScale = new Vector3(attackBaseRadius * 2f + chargeAmount * chargeRate * 2f,
            0.05f, attackBaseRadius * 2f + chargeAmount * chargeRate * 2f);
        Vector3 dir = transform.forward * attackRange;
        attackVisualizer.transform.position = new Vector3(transform.position.x, attackVisualizer.transform.position.y, transform.position.z) + dir;
        if (BP.ballOwner != null && BP.ballOwner.Equals(gameObject))
        {
            attackVisualizer.transform.localScale = Vector3.zero;
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 direction = transform.forward;
        Gizmos.color = Color.red;
        Vector3 origin = new Vector3(transform.position.x, transform.position.y + attackVisualOffsetY, transform.position.z);
        Gizmos.DrawWireSphere(origin + direction * attackRange, attackBaseRadius + chargeAmount * chargeRate);
    }
}
