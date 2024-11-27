using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityQuetzPassive : PassiveAbility
{
    [Header("Bonuses")]
    private float baseSpeed = 3f;
    public float speedBonusPerPoint = 0.2f;
    private AbilityFly AF;
    public float rangeBonusPerPoint = 0.2f;
    private float baseRange;
    [Header("PassiveClouds")]
    public GameObject rainCloudPrefab;
    public int startingCloudAmount = 3;
    public int minCloudAmount = 1;
    public float xSpawnRange = 3;
    public float zSpawnRange = 2;
    public float timeBetweenSpawns = 10f;
    private int cloudCount = 0;
    public float yOffset = -1;
    private AbilitySnakeSegments ASS;

    // Start is called before the first frame update
    void Start()
    {
        Setup();
        baseSpeed = MC.monsterSpeed;
        AF = GetComponent<AbilityFly>();
        ASS = GetComponent<AbilitySnakeSegments>();
        if (AF != null) baseRange = AF.slamRadius;

        //spawn initial clouds
        for (int i = 0; i < startingCloudAmount; i++)
        {
            SpawnCloud();
        }

        Invoke("SpawnCloudRepeating", timeBetweenSpawns);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("speed increase: " + (speedBonusPerPoint * counterAmount));
        AF.baseSpeed = baseSpeed + (speedBonusPerPoint * counterAmount);
        //Debug.Log("new speed: " + (baseSpeed + (speedBonusPerPoint * counterAmount)));
        //Debug.Log("MC speed: " + MC.monsterSpeed);
        AF.slamRadius = baseRange + (rangeBonusPerPoint * counterAmount);

        if (cloudCount <= 0)
        {
            SpawnCloud();
        }
    }

    void SpawnCloud()
    {
        Vector3 pos = new Vector3(Random.Range(-xSpawnRange, xSpawnRange), AF.GetMaxHeight() + yOffset, Random.Range(-zSpawnRange, zSpawnRange));
        Instantiate(rainCloudPrefab, pos, Quaternion.identity);
        cloudCount++;
    }

    void SpawnCloudRepeating()
    {
        SpawnCloud();
        Invoke("SpawnCloudRepeating", timeBetweenSpawns);
    }

    public void EatCloud()
    {
        counterAmount++;
        ASS.AddSegment();
        cloudCount--;
    }

    public override void Deactivate()
    {
        counterAmount = 0;
    }
}
