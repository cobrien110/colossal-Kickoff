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
    public float chargeRate = 1.2f;
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
        // add counter when charging, decrease after activation
        if (isCharging && counterAmount < counterMax && !isActive)
        {
            counterAmount += Time.deltaTime * chargeRate;
        } else if (counterAmount > 0 && isActive && !MC.isIntangible)
        {
            counterAmount -= Time.deltaTime * decayRate;
            if (counterAmount <= 0)
            {
                isActive = false;
            }
        }

        if (counterAmount >= counterMax && !isActive)
        {
            //active
            isActive = true;
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

        UpdateChargeBar();
    }

    public void Deactivate()
    {
        isActive = false;
        counterAmount = 0;
    }
}
