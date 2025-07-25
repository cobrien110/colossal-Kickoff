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
    private GameObject diveParticles;

    private void Start()
    {
        Setup();
        diveParticles = GM.SceneIM.GetDiveParticle();
        baseMonsterSpeed = MC.monsterSpeed;
        inputBuffer = inputBufferTime;
        AAP = GetComponent<AbilityAkhlutPassive>();
        AH = GetComponent<AbilityHowl>();
    }

    private void Update()
    {
        UpdateSetup();
        // Input buffer prevents ability from being activated and deactivated with a single press
        //Debug.Log("inputBuffer: " + inputBuffer);
        if (inputBuffer < inputBufferTime) inputBuffer += Time.deltaTime;

        float t = activeDuration / durationOfSpeedBoost;

        if (GM.isGameOver)
        {
            isActive = false;
        }

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
            
            MC.isIntangible = false;
            timerPaused = false;
            AAP.isCharging = false;
            if (diveLooper != null)
            {
                diveLooper.source.Stop();
            }
            
        }
        ANIM.SetBool("isDiving", isActive);
    }

    public override void Activate()
    {
        if (!canActivate) return;

        Debug.Log("Activate - Dive");

        if (isActive && inputBuffer >= inputBufferTime)
        {
            Debug.Log("Activate - Dive: isActive");
            MC.isIntangible = false;
            isActive = false;
            inputBuffer = 0f;
            audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find(emergeSound), 0.7f);

            // create crystal if passive is charged
            if (GM.isPlaying && AAP.GetActive() && crystalPrefab != null && AH != null && AH.currentCrystal == null)
            {
                AH.currentCrystal = Instantiate(crystalPrefab, transform.position, Quaternion.identity).GetComponent<IceCrystal>();
                AH.currentCrystal.stunTime = AH.stunTime;
                AH.currentCrystal.MC = MC;
                AH.currentCrystal.speed = AH.crystalSpeed;
                AH.currentCrystal.radius = AH.crystalRadius;
                //return;
            } else if (GM.isPlaying && AAP.GetActive() && crystalPrefab != null && AH != null && AH.currentCrystal != null)
            {
                AH.currentCrystal.SetNewPoint(transform.position);
            }
        } else
        {
            Debug.Log("isActive: " + isActive);
            Debug.Log("inputBuffer >= inputBufferTime: " + (inputBuffer >= inputBufferTime));
        }
        if (timer >= cooldown && inputBuffer >= inputBufferTime)
        {
            if (!isActive)
            {
                Debug.Log("Activate - Dive: not Active");
                MC.isIntangible = true;
                isActive = true;
                timer = 0;
                inputBuffer = 0f;
                audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find(diveSound), 0.7f);
                Instantiate(diveParticles, transform.position, Quaternion.identity);
                ST.UpdateMAbUsed();
                UM.UpdateMonsterAbilitiesSB();
            }
        }
    }

    public override void Deactivate()
    {
        isActive = false;
        inputBuffer = 0f;
        MC.monsterSpeed = baseMonsterSpeed;

        // create crystal if passive is charged
        if (GM.isPlaying && AAP.GetActive() && crystalPrefab != null && AH != null && AH.currentCrystal == null)
        {
            /*
            AH.currentCrystal = Instantiate(crystalPrefab, transform.position, Quaternion.identity).GetComponent<IceCrystal>();
            AH.currentCrystal.stunTime = AH.stunTime;
            AH.currentCrystal.MC = MC;
            AH.currentCrystal.speed = AH.crystalSpeed;
            AH.currentCrystal.radius = AH.crystalRadius;
            return;
            */
        }
        /*
        if (GM.isPlaying && AH != null && AH.currentCrystal != null)
        {
            // if crystal exists, move it
            AH.currentCrystal.SetNewPoint(transform.position);
        }
        */
    }

    public bool GetIsActive()
    {
        return isActive;
    }

    public void ResetBuffer()
    {
        inputBuffer = inputBufferTime;
    }
}
