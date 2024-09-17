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

    public string diveSound = "";
    public string emergeSound = "";

    private void Start()
    {
        Setup();
        baseMonsterSpeed = MC.monsterSpeed;
        inputBuffer = inputBufferTime;
    }

    private void Update()
    {
        UpdateSetup();
        if (inputBuffer < inputBufferTime) inputBuffer += Time.deltaTime;

        float t = activeDuration / durationOfSpeedBoost;
        if (isActive)
        {
            activeDuration += Time.deltaTime;
            MC.monsterSpeed = Mathf.Lerp(initialSpeed, speed, t);
            MC.isIntangible = true;
            timerPaused = true;
        } else
        {
            activeDuration = 0;
            MC.monsterSpeed = baseMonsterSpeed;
            MC.isIntangible = false;
            timerPaused = false;
        }
    }

    public override void Activate()
    {
        if (isActive && inputBuffer >= inputBufferTime)
        {
            isActive = false;
            inputBuffer = 0f;
            audioPlayer.PlaySoundRandomPitch(audioPlayer.Find(emergeSound));
        }
        if (timer >= cooldown && inputBuffer >= inputBufferTime)
        {
            if (!isActive)
            {
                isActive = true;
                timer = 0;
                inputBuffer = 0f;
                audioPlayer.PlaySoundRandomPitch(audioPlayer.Find(diveSound));
            }
        }
    }
}
