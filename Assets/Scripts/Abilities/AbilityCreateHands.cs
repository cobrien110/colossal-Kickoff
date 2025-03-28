using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AbilityCreateHands : PassiveAbility
{
    [Header("Ability Variables")]
    public GameObject handPrefab;
    public GameObject headPrefab;
    public float spawnDistance = 1.5f;
    public float handSpawnTime = 1f;

    public GameObject hand1;
    public GameObject hand2;
    private GashadokuroHand gashaHand1;
    private GashadokuroHand gashaHand2;
    public GameObject head;
    public Animator headAnimator;
    public Vector3 headSpawnPosition;
    public AbilityFirebreath AF;

    [HideInInspector] public bool hand1IsActive = true;
    [HideInInspector] public bool hand2IsActive = true;

    private float hand1Timer = 0f;
    private float hand2Timer = 0f;

    public float hitballSpeed = 50f;
    public Sprite secondHandSprite;

    private AbilityHandSlam abilityHandSlam;

    // Start is called before the first frame update
    void Start()
    {
        Setup();
        
        // Instantiate the hands without parenting them to the monster
        hand1 = Instantiate(handPrefab, new Vector3(100f, 0f, 100f), Quaternion.identity);
        hand2 = Instantiate(handPrefab, new Vector3(100f, 0f, 100f), Quaternion.identity);

        hand1IsActive = false;
        hand2IsActive = false;
        hand1Timer = handSpawnTime / 2f;
        hand2Timer = handSpawnTime / 2f;

        head = Instantiate(headPrefab, headSpawnPosition, Quaternion.identity);
        headAnimator = head.GetComponent<Animator>();
        AF.head = head;

        gashaHand1 = hand1.GetComponent<GashadokuroHand>();
        gashaHand2 = hand2.GetComponent<GashadokuroHand>();
        //Apply correct animations
        gashaHand1.ANIM.runtimeAnimatorController = gashaHand1.animType1;
        gashaHand2.ANIM.runtimeAnimatorController = gashaHand2.animType2;

        abilityHandSlam = GetComponent<AbilityHandSlam>();
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
        if (hand1 != null && gashaHand1 != null && !gashaHand1.GetIsDetached()) // Ignore if hand is detached
        {
            hand1.transform.position = new Vector3(MC.transform.position.x, hand1.transform.position.y, MC.transform.position.z + spawnDistance);
        }
        // If this hand is detached, set its position to be above visualizer
        else if (hand1 != null && gashaHand1 && gashaHand1.GetIsDetached())
        {
            hand1.transform.position = new Vector3(abilityHandSlam.attackVisualizer.transform.position.x,
                hand1.transform.position.y, abilityHandSlam.attackVisualizer.transform.position.z);
        }

        if (hand2 != null && gashaHand2 != null && !gashaHand2.GetIsDetached()) // Ignore if hand is detached
        {
            hand2.transform.position = new Vector3(MC.transform.position.x, hand2.transform.position.y, MC.transform.position.z - spawnDistance);
        }
        // If this hand is detached, set its position to be above visualizer
        else if (hand2 != null && gashaHand2 && gashaHand2.GetIsDetached())
        {
            hand2.transform.position = new Vector3(abilityHandSlam.attackVisualizer.transform.position.x,
                hand2.transform.position.y, abilityHandSlam.attackVisualizer.transform.position.z);
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

    public bool AHandIsDetached()
    {
        if (gashaHand1 == null || gashaHand2 == null)
        {
            Debug.Log("Gasha hand(s) are null");
            return false;
        }
        return gashaHand1.GetIsDetached() || gashaHand2.GetIsDetached();
    }

}
