using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class AbilitySphericalAttack : AbilityChargeable
{
    
    public float attackHitForce = 150f;
    public float attackVisualOffsetY;
    public LayerMask affectedLayers;
    public bool canSpawnShrapnel = true;
    public GameObject shrapnelPrefab;
    public int shrapnelDamage = 1;
    public float shrapnelSpeed = 450f;

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

                    Debug.Log("AXE HIT BALL!");
                    float kickForce = attackHitForce;
                    Vector3 posA = new Vector3(BP.gameObject.transform.position.x, 0f, BP.gameObject.transform.position.z);
                    Vector3 posB = new Vector3(transform.position.x, 0f, transform.position.z);
                    Vector3 dir = (posA - posB).normalized;
                    Vector3 forceToAdd = dir * kickForce;
                    BP.GetComponent<Rigidbody>().AddForce(forceToAdd);

                }
            }

            if (chargeAmount < maxChargeSeconds)
            {
                audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find("minotaurAxeAttack"), 0.7f);
            }
            else
            {
                audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find("minotaurAxeAttackCharged"), 0.7f);
                if (canSpawnShrapnel) SpawnShrapnel();
            }
            timer = 0;
            chargeAmount = 0;
            isCharging = false;
            ANIM.Play("MinotaurAttack");

            StartCoroutine(MC.MoveDelay());
        }
    }

    void SpawnShrapnel()
    {
        if (shrapnelPrefab == null) return;
        Vector3 pos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        GameObject shrap = Instantiate(shrapnelPrefab, pos, Quaternion.LookRotation(transform.forward, Vector3.up));
        WallShrapnel WS = shrap.GetComponent<WallShrapnel>();
        WS.damage = shrapnelDamage;
        WS.speed = shrapnelSpeed;
    }

    public override void ResizeAttackVisual()
    {
        if (attackVisual == null) return;
        attackVisual.transform.localScale = new Vector3(attackBaseRadius * 2f + chargeAmount * chargeRate * 2f,
            0.05f, attackBaseRadius * 2f + chargeAmount * chargeRate * 2f);
        Vector3 dir = transform.forward * attackRange;
        attackVisual.transform.position = new Vector3(transform.position.x, attackVisual.transform.position.y, transform.position.z) + dir;
        if (BP.ballOwner != null && BP.ballOwner.Equals(gameObject))
        {
            attackVisual.transform.localScale = Vector3.zero;
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
