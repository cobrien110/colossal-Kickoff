using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBullrush : AbilityChargeable
{
    [Header("Ability Specific Variables")]
    public float dashSpeed = 2f;
    public float duration = 1f;
    //public float chargeTime = 1f;
    public string dashSoundEffect = "minotaurDash";
    //private float inputBuffer = 0f;
    //public float inputBufferTime = 1f;

    public GameObject magmaPrefab;
    public float magmaSpawnDistance = 0.1f;
    private Vector3 lastMagmaSpawn;
    public float magmaSpawnTime = 1f;
    private bool isSpawningMagma = false;
    public float killCooldownReduction = 3f;

    private void Start()
    {
        Setup();
    }

    public override void Activate()
    {
        if (!canActivate) return;
        //if (BP.ballOwner == gameObject || MC.isStunned) return; // ensure no dashing or dash charging when you have ball
        if (!GM.isPlaying)
        {
            isCharging = false;
            chargeAmount = 0;
            return;
        }

        if (BP.ballOwner != gameObject && !MC.isStunned)
        {
            // Check if no direction is being input, set default direction
            if (MC.movementDirection == Vector3.zero)
            {
                MC.movementDirection = MC.transform.forward;
            }

            
            //Debug.Log("Dashing");
            ST.UpdateMAbUsed();
            UM.UpdateMonsterAbilitiesSB();
            MC.isDashing = true;
            ANIM.SetBool("isWindingUp", false);
            ANIM.Play(activatedAnimationName);
            audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find(dashSoundEffect), 0.75f);

            // Add force in direction of the player input for this warrior (movementDirection)
            float charge = chargeAmount + 1f;
            Vector3 dashVelocity = MC.movementDirection.normalized * charge * dashSpeed;
            Debug.Log("Dash Charge: " + chargeAmount);
            MC.rb.AddForce(dashVelocity);

            Invoke("StopDashing", duration);
            isSpawningMagma = true;
            SpawnMagma();
            Invoke("StopSpawnMagma", magmaSpawnTime);
        }
        else
        {
            //Debug.Log("Dash failed");
        }

        // Update the last dash time
        timer = 0;
        chargeAmount = 0;
        isCharging = false;
        MC.isChargingAbility = false;
    }

    private void StopDashing()
    {
        Debug.Log("No longer dashing");
        // ANIM.SetBool("isSliding", false);
        MC.isDashing = false;
    }

    private void SpawnMagma()
    {
        if (!GM.isPlaying || !isSpawningMagma) return;
        if (lastMagmaSpawn == null || Vector3.Distance(lastMagmaSpawn, transform.position) >= magmaSpawnDistance)
        {
            Vector3 spawnPos = transform.position;
            spawnPos.y = -0.312f; //Why is this an arbitruary number :(
            Instantiate(magmaPrefab, spawnPos, transform.rotation);
            lastMagmaSpawn = transform.position;
        }
        Invoke("SpawnMagma", 0.02f);
    }

    private void StopSpawnMagma()
    {
        isSpawningMagma = false;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (!this.enabled || MC == null) return;
        // Debug.Log("Monster Collision with: " + collider.gameObject.name);
        if (MC.isDashing)
        {
            if (collider.tag.Equals("Warrior"))
            {
                Debug.Log("Dash killed warrior");
                collider.gameObject.GetComponent<WarriorController>().Die();
                timer += killCooldownReduction;
                Mathf.Clamp(timer, 0f, cooldown); // Reduce cooldown on kill
            }
            /*
            if (collider.CompareTag("MinoWall"))
            {
                collider.gameObject.GetComponent<MinoWall>().ShatterWall();
            }
            */
        }
    }
}
