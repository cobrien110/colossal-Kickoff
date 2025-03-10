using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GashaShrine : MonoBehaviour
{
    private DeleteAfterDelay DAD;
    public float duration = 10f;
    public GameObject orbPrefab;
    public float orbLaunchSpeed;
    public float timeBetweenSpawns = 3f;
    private float timer;
    public Transform launchSpot;

    public GameObject visual;
    public Transform startPt;
    public Transform endPt;
    private float startTime;
    private float journeyLength;
    private bool movingBack = false;
    public float speed = 3f;

    // Start is called before the first frame update
    void Start()
    {
        DAD = GetComponent<DeleteAfterDelay>();
        DAD.NewTimer(duration);
        timer = timeBetweenSpawns / 2f;

        startTime = Time.time;
        visual.transform.position = startPt.transform.position;
        journeyLength = Vector3.Distance(startPt.position, endPt.position);
        StartCoroutine(Swap());
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timeBetweenSpawns)
        {
            CreateNewOrb();
            timer = 0;
        }

        float distCovered = (Time.time - startTime) * speed;
        float fractionOfJourney = distCovered / journeyLength;
        visual.transform.position = Vector3.Lerp(visual.transform.position, endPt.position, fractionOfJourney);
    }

    void CreateNewOrb()
    {
        SoulOrb o = Instantiate(orbPrefab, launchSpot.position, Quaternion.identity).GetComponent<SoulOrb>();
        Vector3 dir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        Vector3 force = orbLaunchSpeed * dir;
        Debug.Log(force);
        o.Launch(force);
    }

    IEnumerator Swap()
    {
        yield return new WaitForSeconds(duration - 1);
        MoveBackToGround();
    }

    public void MoveBackToGround()
    {
        startTime = Time.time;
        Transform temp = endPt;
        endPt = startPt;
        startPt = temp;
        movingBack = true;
    }
}
