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
    private float headSpace = 0.25f;
    public float speed = 1f;
    public float movementThreshold = 0.001f;

    public int minNumOfSegmentsToSplit = 4;

    private Transform head;
    [HideInInspector] public List<GameObject> segments = new List<GameObject>();
    [HideInInspector] public List<GameObject> cutSegments = new List<GameObject>();
    private Vector3 lastHeadPosition;

    public string popSound = "";

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

    private void Awake()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if ((Vector3.Distance(head.position, lastHeadPosition) > movementThreshold)
            || Vector3.Distance(segments[0].transform.position, head.position) > headSpace)
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
            segments[0].transform.position = Vector3.Lerp(segments[0].transform.position, head.position - head.parent.forward * (followDistance + headSpace), Time.deltaTime * speed);
        }

        // Update the previous head position
        lastHeadPosition = head.position;
    }

    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.O))
        {
            AddSegment();
        }
        */
    }

    public void AddSegment()
    {
        // Calculate position of the new segment
        Vector3 newPosition = Vector3.zero;
        bool lookAtHead = false;
        Vector3 oldAngles = Vector3.zero;
        if (segments.Count > 0)
        {
            GameObject oldTail = segments[segments.Count - 1];
            oldAngles = oldTail.transform.eulerAngles;

            // Spawn behind the last segment
            newPosition = oldTail.transform.position - oldTail.transform.forward * followDistance;

            // Replace old tail with a segment
            GameObject newSegment = Instantiate(segmentPrefab, oldTail.transform.position, oldTail.transform.rotation);
            segments.Remove(oldTail);
            Destroy(oldTail);
            segments.Add(newSegment);
            newSegment.GetComponent<SnakeSegment>().index = segments.Count - 1;
        }
        else
        {
            // First segment starts behind the head
            Vector3 tempHead = MC.transform.position;
            tempHead = new Vector3(tempHead.x, head.position.y, tempHead.z);
            Vector3 dir = MC.transform.forward;
            //Debug.Log("head dir = " + dir);
            newPosition = tempHead - dir * followDistance;
            //Debug.Log("head dir pos = " + newPosition);
            lookAtHead = true;
        }

        // Instantiate new tail and add to list
        GameObject newTail = Instantiate(tailPrefab, newPosition, Quaternion.identity);
        if (lookAtHead) newTail.transform.eulerAngles = MC.transform.eulerAngles;
        else newTail.transform.eulerAngles = oldAngles;
        segments.Add(newTail);
        newTail.GetComponent<SnakeSegment>().index = segments.Count - 1;
    }

    public void CutSegments(int index)
    {
        if (index < minNumOfSegmentsToSplit || MC.isIntangible) return;
        for (int i = segments.Count - 1; i >= index; i--)
        {
            RemoveSegment(i);
        }
        audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find(popSound), 0.8f);
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
        GameObject cutSegment = Instantiate(cutSegmentPrefab, oldTail.transform.position, Quaternion.identity);
        cutSegment.transform.eulerAngles = Vector3.zero;
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
        // remove bombs
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

    public void RebuildSegments()
    {
        int currentSegments = segments.Count;

        // reset segments
        for (int i = 0; i < segments.Count; i++)
        {
            Destroy(segments[i]);
        }
        segments.Clear();

        // Re add new segments
        lastHeadPosition = head.position;

        for (int i = 0; i < currentSegments; i++)
        {
            AddSegmentAtPos();
        }
    }

    public Vector3 GetTailPosition()
    {
        Vector3 newPos = segments[segments.Count - 1].transform.position;
        newPos.y = transform.position.y;
        return newPos;
    }

    public void AddSegmentAtPos()
    {
        // Calculate position of the new segment
        Vector3 newPosition = Vector3.zero;
        bool lookAtHead = false;
        Vector3 oldAngles = Vector3.zero;
        if (segments.Count > 0)
        {
            GameObject oldTail = segments[segments.Count - 1];
            oldAngles = oldTail.transform.eulerAngles;

            // Spawn behind the last segment
            newPosition = oldTail.transform.position;

            // Replace old tail with a segment
            GameObject newSegment = Instantiate(segmentPrefab, oldTail.transform.position, oldTail.transform.rotation);
            segments.Remove(oldTail);
            Destroy(oldTail);
            segments.Add(newSegment);
            newSegment.GetComponent<SnakeSegment>().index = segments.Count - 1;
        }
        else
        {
            // First segment starts behind the head
            Vector3 tempHead = MC.transform.position;
            tempHead = new Vector3(tempHead.x, head.position.y, tempHead.z);
            Vector3 dir = MC.transform.forward;
            //Debug.Log("head dir = " + dir);
            newPosition = tempHead;
            //Debug.Log("head dir pos = " + newPosition);
            lookAtHead = true;
        }

        // Instantiate new tail and add to list
        GameObject newTail = Instantiate(tailPrefab, newPosition, Quaternion.identity);
        if (lookAtHead) newTail.transform.eulerAngles = MC.transform.eulerAngles;
        else newTail.transform.eulerAngles = oldAngles;
        segments.Add(newTail);
        newTail.GetComponent<SnakeSegment>().index = segments.Count - 1;
    }
}
