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

    // Start is called before the first frame update
    void Start()
    {
        DAD = GetComponent<DeleteAfterDelay>();
        DAD.NewTimer(duration);
        timer = timeBetweenSpawns / 2f;
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
    }

    void CreateNewOrb()
    {
        SoulOrb o = Instantiate(orbPrefab, launchSpot.position, Quaternion.identity).GetComponent<SoulOrb>();
        Vector3 dir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        Vector3 force = orbLaunchSpeed * dir;
        Debug.Log(force);
        o.Launch(force);
    }
}
