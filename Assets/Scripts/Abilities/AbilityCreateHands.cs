using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityCreateHands : PassiveAbility
{
    [Header("Ability Variables")]
    public GameObject handPrefab;
    public float spawnDistance = 1.5f;
    public float handSpawnTime = 1f;

    public GameObject hand1;
    public GameObject hand2;

    [HideInInspector] public bool hand1IsActive = true;
    [HideInInspector] public bool hand2IsActive = true;

    private float hand1Timer = 0f;
    private float hand2Timer = 0f;

    public float hitballSpeed = 50f;

    // Start is called before the first frame update
    void Start()
    {
        Setup();
        //hand1 = Instantiate(handPrefab, MC.gameObject.transform);
        //hand1.transform.position = new Vector3(spawnDistance + hand1.transform.position.x, hand1.transform.position.y, hand1.transform.position.z);
        //hand2 = Instantiate(handPrefab, MC.gameObject.transform);
        //hand2.transform.position = new Vector3(-spawnDistance + hand2.transform.position.x, hand2.transform.position.y, hand2.transform.position.z);

        // Instantiate the hands without parenting them to the monster
        hand1 = Instantiate(handPrefab, new Vector3(100f, 0f, 100f), Quaternion.identity);
        hand2 = Instantiate(handPrefab, new Vector3(100f, 0f, 100f), Quaternion.identity);

        hand1IsActive = false;
        hand2IsActive = false;
        hand1Timer = handSpawnTime / 2f;
        hand2Timer = handSpawnTime / 2f;
    }

    public void SetHandActive(int handNum, bool isActive)
    {
        if (handNum == 1)
        {
            hand1IsActive = isActive;
        } else
        {
            hand2IsActive = isActive;
        }
    }

    private void Update()
    {
        // Maintain hand positions relative to the monster without rotation
        if (hand1 != null)
        {
            hand1.transform.position = new Vector3(MC.transform.position.x, hand1.transform.position.y, MC.transform.position.z + spawnDistance);
        }

        if (hand2 != null)
        {
            hand2.transform.position = new Vector3(MC.transform.position.x, hand2.transform.position.y, MC.transform.position.z - spawnDistance);
        }

        // Count up timers if hand is dead
        if (hand1Timer < handSpawnTime && !hand1IsActive)
        {
            hand1Timer += Time.deltaTime;
        }
        if (hand2Timer < handSpawnTime && !hand2IsActive)
        {
            hand2Timer += Time.deltaTime;
        }

        // Respawn hands
        if (hand1Timer >= handSpawnTime)
        {
            audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find("gashaHandSpawn"), 1f);
            SetHandActive(1, true);
            hand1Timer = 0;
        }
        if (hand2Timer >= handSpawnTime)
        {
            SetHandActive(2, true);
            hand2Timer = 0;
        }

        hand1.SetActive(hand1IsActive);
        hand2.SetActive(hand2IsActive);

        
    }

    public void KillHand(int handNum)
    {
        SetHandActive(handNum, false);
        audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find("gashaHandDeath"), 1f);
    }

}
