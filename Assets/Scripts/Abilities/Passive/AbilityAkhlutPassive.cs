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

    public bool usesBloodsense;
    public LayerMask affectedLayers;
    public float speedBonus = 5f;
    public float activeSpeedBonus = 0.5f;
    public float senseRadius = 1f;
    private float baseSpeed;
    public Transform senseOriginTransform;
    public ParticleSystem PS;

    // Start is called before the first frame update
    void Start()
    {
        Setup();
        SR = MC.spriteObject.GetComponent<SpriteRenderer>();
        ASA = GetComponent<AbilitySphericalAttack>();
        baseRadius = ASA.attackBaseRadius;
        baseSpeed = MC.monsterSpeed;
        if (senseOriginTransform == null) senseOriginTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (usesBloodsense && !MC.isIntangible && !MC.isStunned)
        {
            Collider[] colliders = Physics.OverlapSphere(senseOriginTransform.position, senseRadius, affectedLayers);
            bool hitThing = false;
            foreach (Collider col in colliders)
            {
                if (hitThing) return;
                WarriorController WC = col.GetComponent<WarriorController>();
                if (WC != null && WC.GetHealth() < WC.healthMax && MC.BP.ballOwner != gameObject)
                {
                    Debug.Log("bloodsense hit" + col.name);
                    MC.monsterSpeed = isActive ? baseSpeed + activeSpeedBonus : baseSpeed + speedBonus;
                    hitThing = true;
                    if (!PS.isPlaying) PS.Play();
                }
            }
            if (!hitThing)
            {
                MC.monsterSpeed = baseSpeed;
                PS.Stop();
            }
        }
        if (usesBloodsense && MC.isIntangible && PS.isPlaying)
        {
            PS.Stop();
        }


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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (senseOriginTransform == null) return;
        Vector3 origin = senseOriginTransform.position;
        float radius = senseRadius;
        Gizmos.DrawWireSphere(origin, radius);
    }
}
