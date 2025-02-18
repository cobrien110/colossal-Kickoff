using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    Animator animator;
    AudioPlayer AP;
    
    // Use this for initialization
    public void Reset() {
        if (animator == null) {
            animator = GetComponent<Animator>();
        }
        animator.SetBool("toReset", true);

        if (AP == null)
        {
            AP = GetComponent<AudioPlayer>();
        }
    }


    public void Play() {
        animator.SetBool("toReset", false);
        AP.PlaySound(AP.Find("countdown"));
    }
}
