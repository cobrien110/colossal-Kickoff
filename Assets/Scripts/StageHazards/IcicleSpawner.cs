using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcicleSpawner : MonoBehaviour
{
    public float spawnTimeMin = 1;
    public float spawnTimeMax = 2;
    private float spawnTimer = 0;
    private float thisSpawnTime;

    public float maxDistanceX = 3;
    public float maxDistanceZ = 2;

    public GameObject iciclePrefab;
    // Start is called before the first frame update
    void Start()
    {
        ResetTimer();
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= thisSpawnTime)
        {
            Vector3 spawnLoc = new Vector3(Random.Range(-maxDistanceX, maxDistanceX), 0f, Random.Range(-maxDistanceZ, maxDistanceZ));
            Instantiate(iciclePrefab, transform.position + spawnLoc, Quaternion.identity);
            ResetTimer();
        }
    }

    private void ResetTimer()
    {
        spawnTimer = 0f;
        thisSpawnTime = Random.Range(spawnTimeMin, spawnTimeMax);
    }
}
