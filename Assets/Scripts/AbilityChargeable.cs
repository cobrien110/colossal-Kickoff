using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class AbilityChargeable : AbilityScript
{
    [Header("Chargable Variables")]
    public float chargeRate = 1;
    public float maxChargeSeconds = 2;
    protected bool isCharging = false;
    protected float chargeAmount = 0f;
    public string chargeSoundName = "minotaurAxeCharge";

    [Header("Attack Stats")]
    public float attackRange = 1f;
    public float attackBaseRadius = 1f;
    public bool willStopWhenDribbling = true;
    public bool canHitBall = true;
    public float attackHitForce = 150f;

    // Start is called before the first frame update
    void Start()
    {
        Setup();
        attackVisual = MC.attackVisual;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSetup();
        if (willStopWhenDribbling)
        {
            StopWhenDribbling();
        }
        ResizeAttackVisual();
        if (isCharging)
        {
            ChargeAbility();
            if (attackVisual != null && !attackVisual.activeSelf) attackVisual.SetActive(true);
        }
        else
        {
            chargeAmount = 0f;
            if (attackVisual != null && attackVisual.activeSelf) attackVisual.SetActive(false);
        }

    }

    protected void StopWhenDribbling()
    {
        if (BP.ballOwner == gameObject)
        {
            if (isCharging)
            {
                isCharging = false;
                chargeAmount = 0;
            }
        }
    }

    public virtual void ChargeAbility()
    {
        if (MC.isStunned) return;
        if (chargeAmount < maxChargeSeconds)
        {
            // Debug.Log("charging attack");
            if (audioPlayer.source.clip == null || audioPlayer.source.clip != audioPlayer.Find(chargeSoundName))
            {
                audioPlayer.PlaySoundVolume(audioPlayer.Find(chargeSoundName), 0.5f);
            }
            chargeAmount += Time.deltaTime * chargeRate;
        }
    }

    public virtual void ChargeUp()
    {
        Debug.Log("Is Charging Attack");
        isCharging = true;
        MC.isCharging = true;

    }

    public virtual void ChargeDown()
    {
        Debug.Log("Not attack and not charging");
        isCharging = false;
        MC.isCharging = false;
    }

    public virtual void CheckInputs(InputAction.CallbackContext context)
    {
        if (!GM.isPlaying)
        {
            isCharging = false;
            chargeAmount = 0;
            return;
        }

        if (timer >= cooldown)
        {
            // If input is no longer true, attack
            if (context.action.WasReleasedThisFrame() && chargeAmount != 0)
            {
                Activate();
            }
            else if (context.action.IsInProgress() && timer >= cooldown) // If it still is true, keep charging
            {
                ChargeUp();
            }
            else
            {
                ChargeDown();
            }
        }
    }

    public virtual void ResizeAttackVisual()
    {
        if (attackVisual == null) return;
    }
}
