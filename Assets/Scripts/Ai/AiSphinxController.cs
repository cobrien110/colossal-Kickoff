using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AiSphinxController : AiMonsterController
{
    protected override void BallNotPossessed()
    {
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

        // Set Wall chance and behavior
        // If ball is going toward own goal at a high enough speed
        if (BallGoingTowardOwnGoal())
        {
            ability1Chance = 0.4f;
            //wallMode = WallMode.BlockGoal;
        }
        else
        {
            ability1Chance = 0.3f;
            //wallMode = WallMode.BlockWarrior; // Try to block warrior from getting to ball
        }

        // Set Spherical attack chance
        ability2Chance = 0.1f;
        attackMode = AttackMode.NearestWarrior;

        // Set Dash chance and behavior
        ability3Chance = 0.1f;
        //dashMode = DashMode.Ball; // Dash at ball
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

        // Monster should not use abilities
        ability1Chance = 0.0f;
        ability2Chance = 0.0f;
        ability3Chance = 0.0f;

        // Stop roaming if its happening
        StopCoroutines();
    }

    protected override void WarriorHasBall()
    {
        // Reset shootChance to 0.0
        if (shootChance != 0.0f) shootChance = 0.0f;

        // If mino in mino half, warrior with ball in warrior half...
        if (!IsInWarriorHalf(gameObject) && IsInWarriorHalf(mc.BP.ballOwner))
        {
            StopPursuing();
            // Default behavior
            if (!isPerformingAbility) StartRoaming();

            // Set Wall chance and behavior
            ability1Chance = 0.2f; // Wall
            //wallMode = WallMode.Offensive;

            // Set Spherical Attack chance and behavior
            ability2Chance = 0.1f; // Spherical Attack
            attackMode = AttackMode.NearestWarrior; // Target nearest warrior because you don't want to overextend to get ball owner

            // Set Dash chance and behavior
            ability3Chance = 0.1f; // Dash
            //dashMode = DashMode.Nearest; // Don't want to overextend so target nearest

        }

        // If mino and warrior with ball in mino half...
        else if (!IsInWarriorHalf(gameObject) && !IsInWarriorHalf(mc.BP.ballOwner))
        {
            StopCoroutines();
            if (!isPerformingAbility) // Allow ability to finish if one is happening
            {
                // Default behavior
                StartDefendGoal();

                // Set Wall chance and behavior
                ability1Chance = 0.1f;
                //wallMode = WallMode.BlockGoal;

                // Set Spherical Attack chance and behavior
                ability2Chance = 0.1f;
                attackMode = AttackMode.BallOwner;

                // Set Dash chance and behavior
                ability3Chance = 0.1f;
                //dashMode = DashMode.BallOwner;
            }
        }

        // If mino and warrior in warrior half...
        else if (IsInWarriorHalf(gameObject) && IsInWarriorHalf(mc.BP.ballOwner))
        {
            // Debug.Log("mino and warrior in warrior half");
            StopRoaming();

            if (!isPerformingAbility) // Allow ability to finish if one is happening
            {
                // Debug.Log("isPerformingAbility false");
                // Default behavior
                StartPursuing();

                // Set Wall chance and behavior
                ability1Chance = 0.1f;
                //wallMode = WallMode.Offensive;

                // Set Spherical Attack chance and behavior
                ability2Chance = 0.1f;
                attackMode = AttackMode.BallOwner; // Be aggressive, try to get ball

                // Set Dash chance and behavior
                ability3Chance = 0.1f;
                //dashMode = DashMode.BallOwner; // Be aggressive, try to get ball
            }
        }

        // If mino in warrior half, warrior in mino half...
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
                //wallMode = WallMode.BlockGoal;

                // Set Spherical Attack chance and behavior
                ability2Chance = 0.1f;
                attackMode = AttackMode.BallOwner; // Hurry to kill ball owner to stop goal

                // Set Dash chance and behavior
                ability3Chance = 0.1f;
                //dashMode = DashMode.BallOwner; // Rush to get back
            }
        }
    }

    protected override void MonsterBehaviour()
    {
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

    protected override void PerformAbility1Chance()
    {
        if (mc.abilities[0] == null) return;

        if (!mc.abilities[0].AbilityOffCooldown()) return;

        if (UnityEngine.Random.value < ability1Chance)
        {
            Debug.Log("PerformAbility1");
            isPerformingAbility = true;

            StopCoroutines();
            //Wall(wallMode);
        }
    }

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

    protected override void PerformAbility3Chance()
    {
        if (mc.abilities[2] == null) return;

        if (!mc.abilities[2].AbilityOffCooldown()) return;

        if (UnityEngine.Random.value < ability3Chance)
        {
            Debug.Log("PerformAbility3");
            isPerformingAbility = true;

            StopCoroutines();
            //Dash(dashMode);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    void FixedUpdate()
    {
        MonsterBehaviour();
    }
}
