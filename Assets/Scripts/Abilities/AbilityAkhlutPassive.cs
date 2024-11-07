using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityAkhlutPassive : PassiveAbility
{
    public Color col;
    private SpriteRenderer SR;
    private AbilitySphericalAttack ASA;
    private float baseRadius;
    public float rangeBoostMult = 2f;
    public float decayRate = .25f;
    public bool isCharging = false;

    // Start is called before the first frame update
    void Start()
    {
        Setup();
        SR = MC.spriteObject.GetComponent<SpriteRenderer>();
        ASA = GetComponent<AbilitySphericalAttack>();
        baseRadius = ASA.attackBaseRadius;
    }

    // Update is called once per frame
    void Update()
    {
        if (isCharging && counterAmount < counterMax)
        {
            counterAmount += Time.deltaTime;
        } else if (counterAmount > 0)
        {
            counterAmount -= Time.deltaTime * decayRate;
        }

        if (counterAmount >= counterMax/2)
        {
            //active
            isActive = true;
            
        } else
        {
            isActive = false;
        }

        if (isActive)
        {
            ASA.attackBaseRadius = baseRadius + (counterAmount * rangeBoostMult);
            SR.color = col;
        } else
        {
            ASA.attackBaseRadius = baseRadius;
            SR.color = Color.white;
        }
    }
}
