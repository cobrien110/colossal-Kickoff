using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeSegment : MonoBehaviour
{
    //public int numBeforeSplit = 4;
    public AbilitySnakeSegments SS;
    public int index = -1;

    // Start is called before the first frame update
    void Start()
    {
        SS = GameObject.FindGameObjectWithTag("Monster").GetComponent<AbilitySnakeSegments>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (SS.minNumOfSegmentsToSplit >= SS.segments.Count) return;
        if (other.CompareTag("Monster"))
        {
            SS.CutSegments(index);
        }
    }
}
