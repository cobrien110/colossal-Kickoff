using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AiSphinxController : AiMonsterController
{
    private State state = State.BallNotPossessed;
    private enum State
    {
        BallNotPossessed,
        MonsterHasBall,
        WarriorHasBall,
        MummyHasBall
    }
    protected override void BallNotPossessed()
    {
        if (state != State.BallNotPossessed)
        {
            Debug.Log("BallNotPossessed");
            state = State.BallNotPossessed;
        }

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


        if (mc == null)
        {
            Debug.Log("mc: " + mc);
        } else if (mc.BP == null)
        {
            Debug.Log("mc.BP: " + mc.BP);
        } else if (mc.BP.gameObject == null)
        {
            Debug.Log("mc.BP.gameObject: " + mc.BP.gameObject);
        } else if (mc.BP.gameObject.transform.position == null)
        {
            Debug.Log("mc.BP.gameObject.transform.position: " + mc.BP.gameObject.transform.position);
        }
        // If ball and warrior nearest ball are in monster half
        if (mc != null && mc.BP != null &&
            !IsInWarriorHalf(mc.BP.gameObject) && !IsInWarriorHalf(GetNearestWarrior(mc.BP.gameObject.transform.position)?.gameObject))
        {
            ability1Chance = 0.3f;
        }
        else
        {
            ability1Chance = 0.1f;
        }

        // Set Spherical attack chance
        ability2Chance = 0.1f;
        attackMode = AttackMode.NearestWarrior;

        // Set Curse chance and behavior
        ability3Chance = 0.1f;
    }

    protected override void MonsterHasBall()
    {
        if (state != State.MonsterHasBall)
        {
            Debug.Log("WarriorHasBall");
            state = State.MonsterHasBall;
        }

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

        // Monster should not use abilities, unless they can be activated while dribbling
        ability1Chance = 0.1f; // Mummy Explode
        ability2Chance = 0.0f;
        ability3Chance = 0.0f;

        // Stop roaming if its happening
        StopCoroutines();
    }

    protected override void WarriorHasBall()
    {
        if (state != State.WarriorHasBall)
        {
            Debug.Log("WarriorHasBall");
            state = State.WarriorHasBall;
        }

        // Reset shootChance to 0.0
        if (shootChance != 0.0f) shootChance = 0.0f;

        // If monster in monster half, warrior with ball in warrior half...
        if (!IsInWarriorHalf(gameObject) && IsInWarriorHalf(mc.BP.ballOwner))
        {
            StopPursuing();
            // Default behavior
            if (!isPerformingAbility) StartRoaming();

            // Set Mummy Explode chance and behavior
            ability1Chance = 0.1f; // Mummy Explode

            // Set Spherical Attack chance and behavior
            ability2Chance = 0.1f; // Spherical Attack
            attackMode = AttackMode.NearestWarrior; // Target nearest warrior because you don't want to overextend to get ball owner

            // Set Curse chance and behavior
            ability3Chance = 0.1f; // Curse
        }

        // If monster and warrior with ball in monster half...
        else if (!IsInWarriorHalf(gameObject) && !IsInWarriorHalf(mc.BP.ballOwner))
        {
            StopCoroutines();
            if (!isPerformingAbility) // Allow ability to finish if one is happening
            {
                // Default behavior
                StartDefendGoal();

                // Set Mummy Explode chance and behavior
                ability1Chance = 0.3f;

                // Set Spherical Attack chance and behavior
                ability2Chance = 0.2f;
                attackMode = AttackMode.BallOwner;

                // Set Dash chance and behavior
                ability3Chance = 0.1f;
                //dashMode = DashMode.BallOwner;
            }
        }

        // If monster and warrior in warrior half...
        else if (IsInWarriorHalf(gameObject) && IsInWarriorHalf(mc.BP.ballOwner))
        {
            // Debug.Log("monster and warrior in warrior half");
            StopRoaming();

            if (!isPerformingAbility) // Allow ability to finish if one is happening
            {
                // Debug.Log("isPerformingAbility false");
                // Default behavior
                StartPursuing();

                // Set Mummy Explode chance and behavior
                ability1Chance = 0.1f; // Mummy Explode

                // Set Spherical Attack chance and behavior
                ability2Chance = 0.1f;
                attackMode = AttackMode.BallOwner; // Be aggressive, try to get ball

                // Set Dash chance and behavior
                ability3Chance = 0.1f;
                //dashMode = DashMode.BallOwner; // Be aggressive, try to get ball
            }
        }

        // If monster in warrior half, warrior in monster half...
        else if (IsInWarriorHalf(gameObject) && !IsInWarriorHalf(mc.BP.ballOwner))
        {
            StopCoroutines();

            if (!isPerformingAbility) // Allow ability to finish if one is happening
            {
                // Default behavior
                mc.movementDirection = (monsterGoal.transform.position - transform.position).normalized; // Retreat to own goal
                mc.movementDirection.y = 0;
                //Debug.Log("GROUND CLIP TEST: DIR = " + mc.movementDirection);

                // Set Mummy Explode chance and behavior
                ability1Chance = 0.4f; // Mummy Explode

                // Set Spherical Attack chance and behavior
                ability2Chance = 0.1f;
                attackMode = AttackMode.BallOwner; // Hurry to kill ball owner to stop goal

                // Set Dash chance and behavior
                ability3Chance = 0.1f;
                //dashMode = DashMode.BallOwner; // Rush to get back
            }
        }
    }

    private void MummyHasBall()
    {
        if (state != State.MummyHasBall)
        {
            Debug.Log("MummyHasBall");
            state = State.MummyHasBall;
        }
        //StopPursuing();
        //StopCoroutines();
        //mc.movementDirection = Vector3.zero;

        ability1Chance = 0.1f; // Mummy Explode
        ability2Chance = 0.1f; // Spherical Attack
        ability3Chance = 0.1f; // Curse

        if (!IsInWarriorHalf(mc.BP.ballOwner))
        {
            // Mummy in monster half, position safely
            StartDefendGoal();
        } else
        {
            // Mummy in warrior half, support mummy attack
            StartRoaming();
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
        // If mummy has ball
        else if (mc.BP.ballOwner.GetComponent<AIMummy>() != null)
        {
            // Logic
            MummyHasBall();
        }
        else
        {
            Debug.Log("Error in MonsterBehaviour logic");
        }
    }

    // Mummy Explode
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

    // Curse
    protected override void PerformAbility3Chance()
    {
        if (mc.abilities[2] == null) return;

        if (!mc.abilities[2].AbilityOffCooldown()) return;

        if (UnityEngine.Random.value < ability3Chance || true)
        {
            Debug.Log("PerformAbility3");
            isPerformingAbility = true;

            StopCoroutines();
            Curse();
            isPerformingAbility = false;
        }
    }

    private void Curse()
    {
        // Look toward nearest warrior
        WarriorController nearestWarrior = GetNearestWarrior(transform.position);
        if (nearestWarrior == null) return;
        Vector3 nearestWarriorPos = nearestWarrior.transform.position;
        Vector3 dir = (nearestWarriorPos - transform.position).normalized;

        LookInDirection(dir);

        // Activate curse
        mc.abilities[2].Activate();
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
