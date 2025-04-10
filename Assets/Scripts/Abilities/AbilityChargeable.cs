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
        if (!MC.isStunned && isCharging)
        {
            ChargeAbility();
            if (attackVisualizer != null && !attackVisualizer.activeSelf) attackVisualizer.SetActive(true);
            if (hasChargeUpAnimation)
            {
                // Debug.Log("mc.isStunned: " + MC.isStunned + ", isCharging: " + isCharging);
                ANIM.SetBool("isWindingUp", true);
            }
        }
        else
        {
            chargeAmount = 0f;
            if (attackVisualizer != null && attackVisualizer.activeSelf) attackVisualizer.SetActive(false);
        }

        // activate ability if max charge is reached on auto-charge
        if (autoCharge)
        {
            if (BP.ballOwner == gameObject) // If ball is picked up while charging, deactivate
            {
                ChargeDown();
                chargeAmount = 0;
                isAutoCharging = false;
            }

            if (inputBufferTimer < inputBuffer && isAutoCharging) inputBufferTimer += Time.deltaTime;
            if (chargeAmount >= maxChargeSeconds)
            {
                Debug.Log("AbilityChargeable update: activate");
                Activate();
                ANIM.SetBool("isWindingUp", false);
                isAutoCharging = false;

                // Auto charging abilities for ai monsters need to have their isPerformingAbility set to to false here
                AiMonsterController aiMonsterController = gameObject.GetComponent<AiMonsterController>();
                if (aiMonsterController != null)
                {
                    Debug.Log("SET ISPERFORMINGABILITY FALSE");
                    StartCoroutine(aiMonsterController.SetIsPerformingAbilityDelay(false));
                    //aiMonsterController.SetIsPerformingAbility(false);
                }
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
        if (!canActivate) ChargeDown();
        if (chargeAmount < maxChargeSeconds)
        {
            // Debug.Log("Charging Ability: " + abilityName);
            if (audioPlayer.source.clip == null || audioPlayer.source.clip != audioPlayer.Find(chargeSoundName))
            {
                audioPlayer.PlaySoundVolume(audioPlayer.Find(chargeSoundName), 0.5f);
            }
            chargeAmount += Time.deltaTime * chargeRate;
        }
    }

    public virtual void ChargeUp()
    {
        if (!canActivate) return;
        // Debug.Log("Is Charging Attack");
        isCharging = true;
        if (slowsDownCharacterWhileCharging) MC.isChargingAbility = true;
    }

    public virtual void ChargeDown()
    {
        // Debug.Log("Not attack and not charging");
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
                // Debug.Log("CheckInput: non-autoCharge ability");
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

    public float GetChargeAmount()
    {
        return chargeAmount;
    }

    public bool GetIsCharging()
    {
        return isCharging;
    }

    public void SetIsCharging(bool isCharging)
    {
        this.isCharging = isCharging;
    }

    public void SetIsAutoCharging(bool isAutoCharging)
    {
        this.isAutoCharging = isAutoCharging;
    }

    public bool GetIsAutoCharging()
    {
        return isAutoCharging;
    }

    public float GetInputBufferTimer()
    {
        return inputBufferTimer;
    }

    public float GetInputBuffer()
    {
        return inputBuffer;
    }

    public void SetInputBufferTimer(float inputBufferTimer)
    {
        this.inputBufferTimer = inputBufferTimer;
    }
    public override void Deactivate()
    {
        ChargeDown();
        timer = cooldown;
    }
}
