using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinoWall : MonoBehaviour
{
    public Transform startPt;
    public Transform endPt;
    public float speed = 1f;
    private float startTime;
    private float journeyLength;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = startPt.position;
        startTime = Time.time;
        // Calculate the journey length.
        journeyLength = Vector3.Distance(startPt.position, endPt.position);
    }

    // Update is called once per frame
    void Update()
    {
        float distCovered = (Time.time - startTime) * speed;
        float fractionOfJourney = distCovered / journeyLength;
        transform.position = Vector3.Lerp(transform.position, endPt.position, fractionOfJourney);
    }
}
