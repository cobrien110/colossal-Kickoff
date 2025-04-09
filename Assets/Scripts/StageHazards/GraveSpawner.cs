using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] group1;

    [SerializeField] private GameObject[] gravePrefabs;

    [SerializeField] private GameplayManager GM = null;

    private List<GraveHazard> spawnedGraves = new List<GraveHazard>();
    private bool wasPlayingLastFrame = false;

    void Start()
    {
        OnGameStart();
    }

    void Update()
    {
        if (GM == null) return;

        bool isPlaying = GM.IsPlayingGet();

        //Detect round end (isPlaying becomes false)
        if (wasPlayingLastFrame && !isPlaying)
        {
            //Called at end of round, before next begins
            OnRoundStart(); 
        }

        wasPlayingLastFrame = isPlaying;
    }

    public void OnGameStart()
    {
        //Place one random grave at each group1 point, keep them in memory

        foreach (Transform point in group1)
        {
            GameObject prefab = gravePrefabs[Random.Range(0, gravePrefabs.Length)];
            GameObject grave = Instantiate(prefab, point.position, point.rotation, point);
            spawnedGraves.Add(grave.GetComponent<GraveHazard>());
        }
    }

    public void OnRoundStart()
    {
        //Reset any state on graves here (if needed), but don’t destroy them

        foreach (GraveHazard grave in spawnedGraves)
        {
            if (grave != null)
            {
                grave.ResetGrave();
            }
        }
    }
}