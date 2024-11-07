using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDive : AbilityScript
{
    [Header("Ability Specific Variables")]
    public bool isActive = false;
    public float initialSpeed = 6f;
    public float speed = 4f;
    public float durationOfSpeedBoost = .5f;

    private float baseMonsterSpeed = 3f;
    private float inputBuffer = 0f;
    public float inputBufferTime = 1f;

    private float activeDuration = 0f;

    public bool isAttack;
    public float delayBeforeAttack = 0.3f;

    public string diveSound = "";
    public string emergeSound = "";
    public string diveLoopSound = "";
    public AudioPlayer diveLooper;
    private AbilityAkhlutPassive AAP;
    private AbilityHowl AH;
    public GameObject crystalPrefab;

    private void Start()
    {
        Setup();
        baseMonsterSpeed = MC.monsterSpeed;
        inputBuffer = inputBufferTime;
        AAP = GetComponent<AbilityAkhlutPassive>();
        AH = GetComponent<AbilityHowl>();
    }

    private void Update()
    {
        UpdateSetup();
        // Input buffer prevents ability from being activated and deactivated with a single press
        if (inputBuffer < inputBufferTime) inputBuffer += Time.deltaTime;

        float t = activeDuration / durationOfSpeedBoost;
        if (isActive) // If ability has been toggled on
        {
            // Boost speed and gradually roll back speed boost
            activeDuration += Time.deltaTime;
            MC.monsterSpeed = Mathf.Lerp(initialSpeed, speed, t);
            MC.isIntangible = true;
            timerPaused = true; // Pause cooldown while active
            AAP.isCharging = true;
            if (diveLooper != null && !diveLooper.isPlaying())
            {
                diveLooper.PlaySoundVolume(diveLooper.Find(diveLoopSound), 0.25f);
            }
        } else
        {
            activeDuration = 0;
            MC.monsterSpeed = baseMonsterSpeed;
            MC.isIntangible = false;
            timerPaused = false;
            AAP.isCharging = false;
            if (diveLooper != null)
            {
                diveLooper.source.Stop();
            }

            // create crystal if passive is charged
            if (GM.isPlaying && AAP.GetActive() && crystalPrefab != null && AH != null && AH.currentCrystal == null)
            {
                AH.currentCrystal = Instantiate(crystalPrefab, transform.position, Quaternion.identity).GetComponent<IceCrystal>();
                AH.currentCrystal.stunTime = AH.stunTime;
                AH.currentCrystal.MC = MC;
                AH.currentCrystal.radius = AH.crystalRadius;
                return;
            }
        }
        ANIM.SetBool("isDiving", isActive);
    }

    public override void Activate()
    {
        if (isActive && inputBuffer >= inputBufferTime)
        {
            isActive = false;
            inputBuffer = 0f;
            audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find(emergeSound), 0.7f);
        }
        if (timer >= cooldown && inputBuffer >= inputBufferTime)
        {
            if (!isActive)
            {
                isActive = true;
                timer = 0;
                inputBuffer = 0f;
                audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find(diveSound), 0.7f);
            }
        }
    }

    public override void Deactivate()
    {
        isActive = false;
        inputBuffer = 0f;
    }
}
