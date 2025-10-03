using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimOnEnable : MonoBehaviour
{
    public string animationName; 

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        if (animator != null && !string.IsNullOrEmpty(animationName))
        {
            animator.Play(animationName, -1, 0f);
        }
    }
}