using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkSoundPlayer : MonoBehaviour
{
    public AudioPlayer AP;
    private Animator AN;
    // Start is called before the first frame update
    void Start()
    {
        if (AP == null) AP = GetComponentInChildren<AudioPlayer>();
        AN = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayWalkSound()
    {
        if (AP == null
            || (!AN.GetBool("isWalking") && !AN.GetBool("isSliding") && !AN.GetBool("isCharging") && !AN.GetBool("isChargingUp"))) return;
        AP.PlaySoundRandom();
    }
}
