using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [SerializeField] private GameObject carPrefab;
    [SerializeField] private GameplayManager GM;
    [SerializeField] private float spawnTimer = 0;
    [SerializeField] private float thisSpawnTime = 10;
    //[SerializeField] private float spawnDistanceX = -15;
    //[SerializeField] private float spawnDistanceZ = 0;

    void Start()
    {
        spawnTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (GM.IsPlayingGet())
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= thisSpawnTime)
            {
                Instantiate(carPrefab, new Vector3(-15.0f, 0.0f, 0.0f), Quaternion.identity);
                spawnTimer = 0;
            }
        } else
        {
            spawnTimer = 0;
        }
    }
}
