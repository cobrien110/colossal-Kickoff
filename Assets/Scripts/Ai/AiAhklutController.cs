using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AiAhklutController : AiMonsterController
{

    protected override void BallNotPossessed()
    {
        // ResetAbilities();

        // Stop roaming and pursuing if its happening
        StopCoroutines();

        // Reset shootChance to 0.0
        if (shootChance != 0.0f) shootChance = 0.0f;

        if (!isPerformingAbility)
        {
            // Default behaviour
            Vector2 toBall = new Vector2(
                    mc.BP.gameObject.transform.position.x - transform.position.x,
                    mc.BP.gameObject.transform.position.z - transform.position.z).normalized;
            MoveTo(toBall);
        }

        // Set Spherical attack chance
        ability2Chance = 0.1f;
        attackMode = AttackMode.NearestWarrior;

        // Set Dash chance and behavior
        ability3Chance = 0.1f;

        // Debug.Log("BallNotPossessed");
    }

    protected override void MonsterBehaviour()
    {
        if (mc.isStunned)
        {
            mc.movementDirection = Vector3.zero;
            mc.GetComponent<Rigidbody>().velocity = Vector3.zero;
            return;
        }

        // If goal was scored, stop movement and behavior
        if (mc != null && mc.BP != null && !mc.BP.isInteractable)
        {
            mc.movementDirection = Vector3.zero;
            return;
        }

        // Debug.Log("MonsterBehaviour");

        // Make sure mc.BP is assigned a value
        EnsureBallOwnerValid();

        // If no one has ball...
        if (mc.BP.ballOwner == null)
        {
            // Debug.Log("BallNotPossessed");
            // Logic
            BallNotPossessed();
        }
        // If I have ball...
        else if (mc.BP.ballOwner == gameObject)
        {
            // Debug.Log("MonsterHasBall");
            // Logic
            MonsterHasBall();
        }
        // If warrior has ball...
        else if (mc.BP.ballOwner.GetComponent<WarriorController>() != null)
        {
            // Debug.Log("WarriorHasBall");
            // Logic
            WarriorHasBall();
        }
        else
        {
            Debug.Log("Error in MonsterBehaviour logic");
        }
    }

    protected override void MonsterHasBall()
    {
        // Default behaviour
        if (!isPerformingAbility)
        {
            ResetAbilities();

            // "Wiggle" your way towards the goal
            WiggleTowardGoal();

            // Constant chance to shoot depending on:
            // Distance to warrior goal
            float distToGoalFactor = Mathf.Clamp01(1 - (Vector3.Distance(warriorGoal.transform.position, transform.position) / maxShootingRange));
            // proximity to closest warrior
            float proximityToWarriorFactor = Mathf.Clamp01(1 - (GetDistanceToNearestWarrior() / maxProximityRange));
            // How clear path is to goal
            // float pathToGoalFactor = 0.0f;

            // Calculate shootChance based on these factors
            shootChance = Mathf.Pow((distToGoalFactor/* + proximityToWarriorFactor*/) / 2f, 2);

            // If shooting, chargeAmount depends on distance to goal
        }
        // If dash is being charged, charge is down
        else
        {
            ResetAbilities();
        }

        // Monster should not use abilities, except wall
        ability1Chance = 0.1f; // Wall
        ability2Chance = 0.0f;
        ability3Chance = 0.0f;

        // Stop roaming if its happening
        StopCoroutines();
    }

    // Howl
    protected override void PerformAbility1Chance()
    {
        if (mc.abilities[0] == null) return;

        if (!mc.abilities[0].AbilityOffCooldown()) return;

        if (UnityEngine.Random.value < ability1Chance)
        {
            Debug.Log("PerformAbility1");
            isPerformingAbility = true;

            StopCoroutines();
            mc.abilities[0].Activate();
            isPerformingAbility = false;
        }
    }

    // Spherical Attack
    protected override void PerformAbility2Chance()
    {
        if (mc.abilities[1] == null) return;

        if (!mc.abilities[1].AbilityOffCooldown()) return;

        if (UnityEngine.Random.value < ability2Chance)
        {
            Debug.Log("PerformAbility2");
            isPerformingAbility = true;

            StopCoroutines();
            StartChargeableAttack(attackMode);
        }
    }

    // Dive
    protected override void PerformAbility3Chance()
    {
        return;
        if (mc.abilities[2] == null) return;

        if (!mc.abilities[2].AbilityOffCooldown()) return;

        if (UnityEngine.Random.value < ability3Chance)
        {
            Debug.Log("PerformAbility3");
            isPerformingAbility = true;

            StopCoroutines();
            //Dash(dashMode);
            //DashHelper();
        }
    }

    protected override void WarriorHasBall()
    {
        // Reset shootChance to 0.0
        if (shootChance != 0.0f) shootChance = 0.0f;

        // If ahklut in ahklut half, warrior with ball in warrior half...
        if (!IsInWarriorHalf(gameObject) && IsInWarriorHalf(mc.BP.ballOwner))
        {
            StopPursuing();
            // Default behavior
            if (!isPerformingAbility) StartRoaming();

            // Set Wall chance and behavior
            ability1Chance = 0.2f; // Wall

            // Set Spherical Attack chance and behavior
            ability2Chance = 0.1f; // Spherical Attack
            attackMode = AttackMode.NearestWarrior; // Target nearest warrior because you don't want to overextend to get ball owner

            // Set Dash chance and behavior
            ability3Chance = 0.1f; // Dash

        }

        // If ahklut and warrior with ball in ahklut half...
        else if (!IsInWarriorHalf(gameObject) && !IsInWarriorHalf(mc.BP.ballOwner))
        {
            StopCoroutines();
            if (!isPerformingAbility) // Allow ability to finish if one is happening
            {
                // Default behavior
                StartDefendGoal();

                // Set Wall chance and behavior
                ability1Chance = 0.1f;

                // Set Spherical Attack chance and behavior
                ability2Chance = 0.1f;
                attackMode = AttackMode.BallOwner;

                // Set Dash chance and behavior
                ability3Chance = 0.1f;
            }
        }

        // If ahklut and warrior in warrior half...
        else if (IsInWarriorHalf(gameObject) && IsInWarriorHalf(mc.BP.ballOwner))
        {
            // Debug.Log("ahklut and warrior in warrior half");
            StopRoaming();

            if (!isPerformingAbility) // Allow ability to finish if one is happening
            {
                // Debug.Log("isPerformingAbility false");
                // Default behavior
                StartPursuing();

                // Set Wall chance and behavior
                ability1Chance = 0.1f;

                // Set Spherical Attack chance and behavior
                ability2Chance = 0.1f;
                attackMode = AttackMode.BallOwner; // Be aggressive, try to get ball

                // Set Dash chance and behavior
                ability3Chance = 0.1f;
            }
        }

        // If ahklut in warrior half, warrior in ahklut half...
        else if (IsInWarriorHalf(gameObject) && !IsInWarriorHalf(mc.BP.ballOwner))
        {
            StopCoroutines();

            if (!isPerformingAbility) // Allow ability to finish if one is happening
            {
                // Default behavior
                mc.movementDirection = (monsterGoal.transform.position - transform.position).normalized; // Retreat to own goal
                mc.movementDirection.y = 0;
                //Debug.Log("GROUND CLIP TEST: DIR = " + mc.movementDirection);

                // Set Wall chance and behavior
                ability1Chance = 0.1f;

                // Set Spherical Attack chance and behavior
                ability2Chance = 0.1f;
                attackMode = AttackMode.BallOwner; // Hurry to kill ball owner to stop goal

                // Set Dash chance and behavior
                ability3Chance = 0.1f;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MonsterBehaviour();
    }
}
