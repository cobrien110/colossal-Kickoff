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

    // Start is called before the first frame update
    void Start()
    {
        Setup();
        baseSpeed = MC.monsterSpeed;
        SR = MC.spriteObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (timer < cooldown)
        {
            timer += Time.deltaTime;
        }
    }

    public void Activate()
    {
        if (timer >= cooldown)
        {
            MC.canBeStunned = false;
            MC.monsterSpeed = baseSpeed + speedBoost;
            Invoke("Deactivate", duration);
            SR.color = onColor;
            timer = 0;
        }
    }

    public override void Deactivate()
    {
        MC.canBeStunned = true;
        MC.monsterSpeed = baseSpeed;
        SR.color = Color.white;
    }
}
