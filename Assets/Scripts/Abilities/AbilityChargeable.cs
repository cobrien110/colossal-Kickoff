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
    public bool slowsDownCharacterWhileCharging = false;
    public string chargeSoundName = "minotaurAxeCharge";
    public bool hasChargeUpAnimation;

    public bool willStopWhenDribbling = true;
    public bool canHitBall = true;
    public float attackHitForce = 150f;

    [Header("Auto Charging")]
    public bool autoCharge = false;
    private bool isAutoCharging = false;
    public float inputBuffer = 0.4f;
    private float inputBufferTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        Setup();
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
            if (attackVisualizer != null && !attackVisualizer.activeSelf) attackVisualizer.SetActive(true);
            if (hasChargeUpAnimation) ANIM.SetBool("isWindingUp", true);
        }
        else
        {
            chargeAmount = 0f;
            if (attackVisualizer != null && attackVisualizer.activeSelf) attackVisualizer.SetActive(false);
        }

        // activate ability if max charge is reached on auto-charge
        if (autoCharge)
        {
            if (inputBufferTimer < inputBuffer && isAutoCharging) inputBufferTimer += Time.deltaTime;
            if (chargeAmount >= maxChargeSeconds)
            {
                Activate();
                ANIM.SetBool("isWindingUp", false);
                isAutoCharging = false;
            }
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
            Debug.Log("Charging Ability: " + abilityName);
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
        if (slowsDownCharacterWhileCharging) MC.isChargingAbility = true;
    }

    public virtual void ChargeDown()
    {
        Debug.Log("Not attack and not charging");
        isCharging = false;
        if (slowsDownCharacterWhileCharging) MC.isChargingAbility = false;
        ANIM.SetBool("isWindingUp", false);
    }

    public virtual void CheckInputs(InputAction.CallbackContext context)
    {
        if (!GM.isPlaying || MC.isStunned)
        {
            isCharging = false;
            chargeAmount = 0;
            return;
        }

        if (timer >= cooldown)
        {
            if (autoCharge) // Do this if this ability charges itself once pressed
            {
                if (isAutoCharging && inputBufferTimer >= inputBuffer && context.action.WasPerformedThisFrame())
                {
                    inputBufferTimer = 0;
                    Activate();
                    isAutoCharging = false;
                }
                else if (context.action.WasPerformedThisFrame() || isAutoCharging)
                {
                    ChargeUp();
                    isAutoCharging = true;
                } else if (!isAutoCharging)
                {
                    ChargeDown();
                }
            } else // Do this if the ability requires to be held down
            {
                // If input is no longer true, attack
                if (context.action.WasReleasedThisFrame() && chargeAmount != 0)
                {
                    Activate();
                    ANIM.SetBool("isWindingUp", false);
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
    }

    public virtual void ResizeAttackVisual()
    {
        if (attackVisualizer == null) return;
    }
}
