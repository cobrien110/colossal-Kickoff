using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityFirebreath : AbilityScript
{
    public GameObject fireballPrefab;
    public Transform spawnPoint;
    public float speedOfFireball = 10f;
    public int numToSpawn = 5;
    public float delayBetweenShots = 0.25f;
    public string soundName;

    private float burstTimer;
    private bool isFiring;
    private int shotsFired = 0;

    public void Start()
    {
        Setup();
    }

    public void Update()
    {
        UpdateSetup();

        if (isFiring)
        {
            burstTimer += Time.deltaTime;
            if (burstTimer >= delayBetweenShots)
            {
                FireShot();
            }
        }
    }

    public override void Activate()
    {
        if (!isFiring && cooldown >= timer)
        {
            isFiring = true;
            audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find(soundName), 0.75f);
            timer = 0;
        }
        if (spawnPoint == null)
        {
            spawnPoint = GetComponent<AbilityCreateHands>().head.transform;
            if (spawnPoint == null) // FAILSAFE IF NO HAND CREATION CODE
            {
                spawnPoint = GameObject.Find("MonsterGoal").transform;
            }
        }
    }

    public void FireShot()
    {
        Fireball f = Instantiate(fireballPrefab, spawnPoint.position, Quaternion.identity).GetComponent<Fireball>();
        f.speed = speedOfFireball;
        f.MC = MC;
        f.Activate();
        shotsFired++;
        burstTimer = 0;

        if (shotsFired >= numToSpawn)
        {
            isFiring = false;
            shotsFired = 0;
        }
    }
}
