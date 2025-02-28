using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeSegment : MonoBehaviour
{
    //public int numBeforeSplit = 4;
    public AbilitySnakeSegments SS;
    public int index = -1;
    public GameplayManager GM;
    public Animator ANIM;
    //private SpriteRenderer SR;

    // Start is called before the first frame update
    void Start()
    {
        SS = GameObject.FindGameObjectWithTag("Monster").GetComponent<AbilitySnakeSegments>();
        GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
        //SR = GetComponentInChildren<SpriteRenderer>();
        ANIM = GetComponentInChildren<Animator>();

        if (ANIM == null) return;
        if ((index + 3) % 4 == 0)
        {
            ANIM.SetBool("isWing", true);
        } else
        {
            ANIM.SetBool("isWing", false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GM == null || !GM.isPlaying) return;
        if (SS.minNumOfSegmentsToSplit >= SS.segments.Count) return;
        if (other.CompareTag("Monster"))
        {
            SS.CutSegments(index);
        }
    }
}
