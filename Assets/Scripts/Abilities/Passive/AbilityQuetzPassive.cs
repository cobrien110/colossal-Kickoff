using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityQuetzPassive : PassiveAbility
{
    private float baseSpeed = 3f;
    public float speedBonusPerPoint = 0.2f;
    private AbilityFly AF;
    public float rangeBonusPerPoint = 0.2f;
    private float baseRange;
    // Start is called before the first frame update
    void Start()
    {
        Setup();
        baseSpeed = MC.monsterSpeed;
        AF = GetComponent<AbilityFly>();
        if (AF != null) baseRange = AF.slamRadius;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("speed increase: " + (speedBonusPerPoint * counterAmount));
        AF.baseSpeed = baseSpeed + (speedBonusPerPoint * counterAmount);
        //Debug.Log("new speed: " + (baseSpeed + (speedBonusPerPoint * counterAmount)));
        //Debug.Log("MC speed: " + MC.monsterSpeed);
        AF.slamRadius = baseRange + (rangeBonusPerPoint * counterAmount);
    }
}
