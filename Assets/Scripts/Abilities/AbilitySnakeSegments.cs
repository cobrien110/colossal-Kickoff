using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySnakeSegments : PassiveAbility
{
    public int numOfSpawnSegments = 3;
    public GameObject segmentPrefab;
    public GameObject tailPrefab;
    public GameObject cutSegmentPrefab;
    public float followDistance = 0.5f;
    public float speed = 1f;
    public float movementThreshold = 0.001f;

    public int minNumOfSegmentsToSplit = 4;

    private Transform head;
    [HideInInspector] public List<GameObject> segments = new List<GameObject>();
    [HideInInspector] public List<GameObject> cutSegments = new List<GameObject>();
    private Vector3 lastHeadPosition;

    // Start is called before the first frame update
    void Start()
    {
        Setup();

        head = MC.spriteObject.transform;
        lastHeadPosition = head.position;

        for (int i = 0; i < numOfSpawnSegments; i++)
        {
            AddSegment();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Vector3.Distance(head.position, lastHeadPosition) > movementThreshold)
        {
            // Move segments
            for (int i = segments.Count - 1; i > 0; i--)
            {
                // Make segment look at previous
                segments[i].transform.LookAt(segments[i - 1].transform, Vector3.up);

                // Make each segment follow the previous one
                segments[i].transform.position = Vector3.Lerp(segments[i].transform.position,
                    segments[i - 1].transform.position - segments[i - 1].transform.forward * followDistance, Time.deltaTime * speed);
            }

            // Make the first segment follow the head
            segments[0].transform.LookAt(head, Vector3.up);
            segments[0].transform.position = Vector3.Lerp(segments[0].transform.position, head.position - head.forward * followDistance, Time.deltaTime * speed);
        }

        // Update the previous head position
        lastHeadPosition = head.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            AddSegment();
        }
    }

    public void AddSegment()
    {
        // Calculate position of the new segment
        Vector3 newPosition = Vector3.zero;
        if (segments.Count > 0)
        {
            GameObject oldTail = segments[segments.Count - 1];

            // Spawn behind the last segment
            newPosition = oldTail.transform.position - oldTail.transform.forward * followDistance;

            // Replace old tail with a segment
            GameObject newSegment = Instantiate(segmentPrefab, oldTail.transform.position, Quaternion.identity);
            segments.Remove(oldTail);
            Destroy(oldTail);
            segments.Add(newSegment);
            newSegment.GetComponent<SnakeSegment>().index = segments.Count - 1;
        }
        else
        {
            // First segment starts behind the head
            newPosition = head.position - head.forward * followDistance;
        }

        // Instantiate new segment and add to list
        GameObject newTail = Instantiate(tailPrefab, newPosition, Quaternion.identity);
        segments.Add(newTail);
        newTail.GetComponent<SnakeSegment>().index = segments.Count - 1;
    }

    public void CutSegments(int index)
    {
        if (index < minNumOfSegmentsToSplit) return;
        for (int i = segments.Count - 1; i >= index; i--)
        {
            RemoveSegment(i);
        }
        /*
        int indexToStartRemoval = -1;
        for (int i = 0; i < segments.Count; i++)
        {
            if (segments[i].Equals(segmentToRemove))
            {
                indexToStartRemoval = i;
            }
        }
        if (indexToStartRemoval != -1)
        {
            for (int i = segments.Count - 1; i > indexToStartRemoval; i--)
            {
                RemoveSegment(i);
            }
        }
        */
    }

    public void RemoveSegment(int index)
    {
        if (index < minNumOfSegmentsToSplit) return;
        
        GameObject oldTail = segments[index];
        GameObject newTail = segments[index - 1];

        // Create a cut segment
        GameObject cutSegment = Instantiate(cutSegmentPrefab, oldTail.transform.position, oldTail.transform.rotation);
        cutSegments.Add(cutSegment);

        // Destroy both the old tail and the segment that will
        // become the new tail.
        segments.Remove(oldTail);
        Destroy(oldTail);

        segments.Remove(newTail);
        Destroy(newTail);

        // Spawn in a new segment to create the new tail if needed
        AddSegment();
    }

    public void ResetSegments()
    {
        // remove boms
        for (int i = 0; i < cutSegments.Count; i++)
        {
            Destroy(cutSegments[i]);
        }
        cutSegments.Clear();

        // reset segments to starting amount
        for (int i = 0; i < segments.Count; i++)
        {
            Destroy(segments[i]);
        }
        segments.Clear();

        // Re add new segments
        lastHeadPosition = head.position;

        for (int i = 0; i < numOfSpawnSegments; i++)
        {
            AddSegment();
        }
    }
}
