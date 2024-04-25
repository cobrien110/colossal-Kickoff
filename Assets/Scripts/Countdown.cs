using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    Animator animator;
    
    // Use this for initialization
    public void Reset() {
        if (animator == null) {
            animator = GetComponent<Animator>();
        }
        animator.SetBool("toReset", true);
    }
    public void Play() {
        animator.SetBool("toReset", false);
    }
}
