using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityMinotaurBoost : PassiveAbility
{
    public float duration = 5f;
    public float speedBoost = .5f;
    private float baseSpeed = 4f;
    public Color onColor = Color.red;
    private SpriteRenderer SR;
    public float degradeRate = 1f;
    public int bonusAmount = 1;
    private float deminishingReturns = 2;

    // Start is called before the first frame update
    void Start()
    {
        Setup();
        baseSpeed = MC.monsterSpeed;
        SR = MC.spriteObject.GetComponent<SpriteRenderer>();
        counterMax = duration;
    }

    // Update is called once per frame
    private void Update()
    {
        if (timer < cooldown)
        {
            timer += Time.deltaTime;
        }
        if (isActive)
        {
            counterAmount -= Time.deltaTime * degradeRate;

            if (counterAmount <= 0)
            {
                Deactivate();
            }
        }
        UpdateChargeBar();
    }

    public void Activate()
    {
        if (timer >= cooldown)
        {
            MC.canBeStunned = false;
            MC.monsterSpeed = baseSpeed + speedBoost;
            //Invoke("Deactivate", duration);
            SR.color = onColor;
            timer = 0;
            counterAmount = counterMax;
            isActive = true;
        }
    }

    public override void Deactivate()
    {
        MC.canBeStunned = true;
        MC.monsterSpeed = baseSpeed;
        SR.color = Color.white;
        counterAmount = 0;
        isActive = false;
        deminishingReturns = 2;
    }

    public void AddBonus()
    {
        counterAmount += bonusAmount * (1 / Mathf.Log(deminishingReturns++, 2));
        if (counterAmount >= counterMax)
        {
            counterAmount = counterMax;
        }
    }
}
