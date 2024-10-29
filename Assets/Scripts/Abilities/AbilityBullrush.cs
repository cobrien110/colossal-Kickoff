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

    private void Start()
    {
        Setup();
        //inputBuffer = inputBufferTime;
    }

    public override void Activate()
    {
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

    private void OnTriggerEnter(Collider collider)
    {
        if (!this.enabled) return;
        // Debug.Log("Monster Collision with: " + collider.gameObject.name);
        if (MC.isDashing && collider.tag.Equals("Warrior"))
        {
            Debug.Log("Dash killed warrior");
            collider.gameObject.GetComponent<WarriorController>().Die();
        }
    }
}
