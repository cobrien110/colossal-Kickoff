using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AiQuetzalcaotlController : AiMonsterController
{
    private enum FlyMode
    {
        Ball,
        BallOwner
    }

    private FlyMode flyMode = FlyMode.Ball;

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

    // Mines
    protected override void PerformAbility1Chance()
    {
        if (mc.abilities[0] == null) return;

        if (!mc.abilities[0].AbilityOffCooldown()) return;

        if (UnityEngine.Random.value < ability1Chance && ShouldBomb())
        {
            Debug.Log("PerformAbility1");
            isPerformingAbility = true;

            // StopCoroutines();
            mc.abilities[0].Activate();
            isPerformingAbility = false;
        }
    }

    // Square Attack
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

    // Fly
    protected override void PerformAbility3Chance()
    {
        return;
        throw new System.NotImplementedException();
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
        else if (mc.BP.ballOwner.CompareTag("Warrior"))
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

    protected override void BallNotPossessed()
    {
        if (state != State.BallNotPossessed)
        {
            Debug.Log("BallNotPossessed");
            state = State.BallNotPossessed;
            stateChanged = true;
        }

        // ResetAbilities();

        // Reset shootChance to 0.0
        if (shootChance != 0.0f) shootChance = 0.0f;

        if (!isPerformingAbility)
        {
            // Stop roaming and pursuing if its happening
            StopCoroutines();

            // Default behaviour
            Vector2 toBall = new Vector2(
                    mc.BP.gameObject.transform.position.x - transform.position.x,
                    mc.BP.gameObject.transform.position.z - transform.position.z).normalized;
            MoveTo(toBall);
        }

        // Set Square attack chance
        ability2Chance = 0.4f;
        attackMode = AttackMode.NearestWarrior;

        // Set Fly chance and behavior
        ability3Chance = 0.1f;
        flyMode = FlyMode.Ball; // Fly at ball

        // Debug.Log("BallNotPossessed");
    }

    protected override void MonsterHasBall()
    {
        if (state != State.MonsterHasBall)
        {
            Debug.Log("MonsterHasBall");
            state = State.MonsterHasBall;
            stateChanged = true;
            StopCoroutines();
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

            // Stop roaming if its happening
            StopCoroutines();
        }

        // Monster should not use abilities, except mines
        ability1Chance = 0.5f; // Mines
        ability2Chance = 0.0f;
        ability3Chance = 0.0f;
    }

    protected override void WarriorHasBall()
    {
        if (state != State.WarriorHasBall)
        {
            Debug.Log("WarriorHasBall");
            state = State.WarriorHasBall;
            stateChanged = true;
        }

        // Reset shootChance to 0.0
        if (shootChance != 0.0f) shootChance = 0.0f;

        flyMode = FlyMode.BallOwner; // Warrior has the ball, so target ballOwner

        // If mino in mino half, warrior with ball in warrior half...
        if (!IsInWarriorHalf(gameObject) && IsInWarriorHalf(mc.BP.ballOwner))
        {
            // Default behavior
            if (!isPerformingAbility) StartRoaming();

            // Set Mine chance and behavior
            ability1Chance = 0.7f; // Mine

            // Set Square Attack chance and behavior
            ability2Chance = 0.2f; // Square Attack
            attackMode = AttackMode.NearestWarrior; // Target nearest warrior because you don't want to overextend to get ball owner

            // Set Fly chance and behavior
            ability3Chance = 0.1f; // Fly
        }

        // If mino and warrior with ball in mino half...
        else if (!IsInWarriorHalf(gameObject) && !IsInWarriorHalf(mc.BP.ballOwner))
        {
            if (!isPerformingAbility) // Allow ability to finish if one is happening
            {
                // Default behavior
                StartDefendGoal();

                // Set Mine chance and behavior
                ability1Chance = 0.5f;

                // Set Square Attack chance and behavior
                ability2Chance = 0.2f;
                attackMode = AttackMode.BallOwner;

                // Set Fly chance and behavior
                ability3Chance = 0.1f;
            }
        }

        // If mino and warrior in warrior half...
        else if (IsInWarriorHalf(gameObject) && IsInWarriorHalf(mc.BP.ballOwner))
        {
            // Debug.Log("mino and warrior in warrior half");

            if (!isPerformingAbility) // Allow ability to finish if one is happening
            {
                // Debug.Log("isPerformingAbility false");
                // Default behavior
                StartPursuing();

                // Set Mine chance and behavior
                ability1Chance = 0.5f;

                // Set Square Attack chance and behavior
                ability2Chance = 0.2f;
                attackMode = AttackMode.BallOwner; // Be aggressive, try to get ball

                // Set Fly chance and behavior
                ability3Chance = 0.1f;
            }
        }

        // If mino in warrior half, warrior in mino half...
        else if (IsInWarriorHalf(gameObject) && !IsInWarriorHalf(mc.BP.ballOwner))
        {
            if (!isPerformingAbility) // Allow ability to finish if one is happening
            {
                StopCoroutines();

                // Default behavior
                mc.movementDirection = (monsterGoal.transform.position - transform.position).normalized; // Retreat to own goal
                mc.movementDirection.y = 0;
                //Debug.Log("GROUND CLIP TEST: DIR = " + mc.movementDirection);

                // Set Mine chance and behavior
                ability1Chance = 0.5f;

                // Set Square Attack chance and behavior
                ability2Chance = 0.2f;
                attackMode = AttackMode.BallOwner; // Hurry to kill ball owner to stop goal

                // Set Fly chance and behavior
                ability3Chance = 0.1f;
            }
        }
    }

    private bool IsWarriorInBomb()
    {
        AbilitySnakeSegments ASS = GetComponent<AbilitySnakeSegments>();
        if (ASS != null)
        {
            foreach (GameObject bombObj in ASS.cutSegments)
            {
                SnakeBomb snakeBomb = bombObj.GetComponent<SnakeBomb>();
                if (snakeBomb != null && snakeBomb.WarriorInRadius()) return true;
            }
        }
        return false;
    }

    private bool ShouldBombBall()
    {
        AbilitySnakeSegments ASS = GetComponent<AbilitySnakeSegments>();
        if (ASS != null)
        {
            foreach (GameObject bombObj in ASS.cutSegments)
            {
                SnakeBomb snakeBomb = bombObj.GetComponent<SnakeBomb>();
                if (!snakeBomb.GetIsBallInRadius() || mc.BP.ballOwner != null) continue; // Ignore if ball isn't in radius, or if it is possessed

                Vector3 bombToMonsterGoal = (monsterGoal.transform.position - bombObj.transform.position).normalized;
                Vector3 bombToBall = (mc.BP.gameObject.transform.position - bombObj.transform.position).normalized;
                Vector3 bombToWarriorGoal = (warriorGoal.transform.position - bombObj.transform.position).normalized;

                // Check if ball is moving toward own goal, but is positioned by bomb opposite said goal (so bomb will hit ball away from monster goal)
                bool shouldBombDefensive = BallGoingTowardOwnGoal() && Vector3.Dot(bombToBall, bombToMonsterGoal) < 0;

                // Check if ball is by bomb, positioned on side near warrior goal (so bomb will hit ball toward warrior goal)
                float directionalAlignmentThreshold = 0.3f;
                bool shouldBombOffensive = Vector3.Dot(bombToBall, bombToWarriorGoal) > directionalAlignmentThreshold;


                if (shouldBombDefensive || shouldBombOffensive)
                {
                    Debug.Log("shouldBombDefensive: " + shouldBombDefensive);
                    Debug.Log("shouldBombOffensive: " + shouldBombOffensive);
                    return true;
                }
            }
        }
        return false;
    }

    private bool ShouldBomb()
    {
        Debug.Log("IsWarriorInBomb(): " + IsWarriorInBomb());
        Debug.Log("ShouldBombBall(): " + ShouldBombBall());
        return IsWarriorInBomb() || ShouldBombBall();
    }
}
