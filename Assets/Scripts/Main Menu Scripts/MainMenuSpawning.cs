using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSpawning : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public Transform spawnPoint1;
    public Transform spawnPoint2;

    public float minSpawnTime = 1f; 
    public float maxSpawnTime = 5f;

    void Start()
    {
        StartCoroutine(SpawnRoutine(spawnPoint1));
        StartCoroutine(SpawnRoutine(spawnPoint2));
    }

    IEnumerator SpawnRoutine(Transform spawnPoint)
    {
        while (true)
        {
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            if (prefabToSpawn && spawnPoint)
            {
                GameObject obj = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
                Destroy(obj, 10f);
            }
        }
    }
}