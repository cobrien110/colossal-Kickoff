using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBullrush : AbilityChargeable
{
    [Header("Ability Specific Variables")]
    public float dashSpeed = 2f;
    public float duration = 1f;
    public string dashSoundEffect = "minotaurDash";

    public override void Activate()
    {
        if (BP.ballOwner == gameObject || MC.isStunned) return; // ensure no dashing or dash charging when you have ball
        if (!GM.isPlaying)
        {
            isCharging = false;
            chargeAmount = 0;
            return;
        }

        if (MC.movementDirection != Vector3.zero && BP.ballOwner != gameObject && !MC.isStunned)
        {
            Debug.Log("Dashing");
            MC.isDashing = true;
            ANIM.SetBool("isWindingUp", false);
            ANIM.Play(activatedAnimationName);
            audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find(dashSoundEffect), 0.75f);

            // Add force in direction of the player input for this warrior (movementDirection)
            Vector3 dashVelocity = MC.movementDirection.normalized * chargeAmount * dashSpeed;
            Debug.Log("Dash Charge: " + chargeAmount);
            MC.rb.AddForce(dashVelocity);

            Invoke("StopDashing", duration);
        }
        else
        {
            Debug.Log("Dash failed");
        }
        // Update the last dash time
        timer = 0; ;
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
        // Debug.Log("Monster Collision with: " + collider.gameObject.name);
        if (MC.isDashing && collider.tag.Equals("Warrior"))
        {
            Debug.Log("Dash killed warrior");
            collider.gameObject.GetComponent<WarriorController>().Die();
        }
    }
}
