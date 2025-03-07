using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameSurfaceSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn; 
    public Transform planeTransform;
    public int numberOfSpawns = 5; 

    void Start()
    {
        SpawnObjects();
    }

    void SpawnObjects()
    {
        if (prefabToSpawn == null || planeTransform == null) return;

        Vector3 planeSize = planeTransform.localScale * 10f;

        for (int i = 0; i < numberOfSpawns; i++)
        {
            float randomX = Random.Range(-planeSize.x / 2, planeSize.x / 2);
            float randomZ = Random.Range(-planeSize.z / 2, planeSize.z / 2);

            Vector3 spawnPosition = planeTransform.position + planeTransform.right * randomX + planeTransform.forward * randomZ;

            GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
            spawnedObject.transform.SetParent(planeTransform);
        }
    }
}