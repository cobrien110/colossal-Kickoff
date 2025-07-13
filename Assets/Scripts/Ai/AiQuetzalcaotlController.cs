using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AiQuetzalcaotlController : AiMonsterController
{
    private Coroutine flyCoroutine = null;
    [SerializeField] private float minFlyDistance; // min distance Quetz must be from target to use fly
    private FlyMode flyMode = FlyMode.Ball;
    [SerializeField] private float flyToTargetThreshold; // How close is considered "close enough" when flying to target position

    private enum FlyMode
    {
        Ball,
        BallOwner,
        Offensive
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

            StartChargeableAttack(attackMode);
        }
    }

    // Fly
    protected override void PerformAbility3Chance()
    {
        if (mc.abilities[2] == null) return;

        if (!mc.abilities[2].AbilityOffCooldown()) return;

        if (UnityEngine.Random.value < ability3Chance)
        {
            Debug.Log("PerformAbility3");

            StartFly();
        }
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

        // If ball in warrior half, and warrior nearest ball in warrior half
        if (IsInWarriorHalf(mc.BP.gameObject) && IsInWarriorHalf(GetNearestWarrior(mc.BP.transform.position)))
        {
            // Set Fly chance and behavior
            ability3Chance = 0.2f;
            flyMode = FlyMode.Offensive; // Go after warrior nearest ball
        }
        // If ball in warrior half, and warrior nearest ball in monster half
        else if (IsInWarriorHalf(mc.BP.gameObject) && !IsInWarriorHalf(GetNearestWarrior(mc.BP.transform.position)))
        {
            // Set Fly chance and behavior
            ability3Chance = 0.2f;
            flyMode = FlyMode.Ball; // Fly at ball
        }
        // If ball in monster half, and warrior nearest ball in monster half
        else if (IsInWarriorHalf(mc.BP.gameObject) && !IsInWarriorHalf(GetNearestWarrior(mc.BP.transform.position)))
        {
            // Set Fly chance and behavior
            ability3Chance = 0.3f;
            flyMode = FlyMode.Offensive; // Go after warrior nearest ball
        }
        // If ball in monster half, and warrior nearest ball in warrior half
        else if (IsInWarriorHalf(mc.BP.gameObject) && !IsInWarriorHalf(GetNearestWarrior(mc.BP.transform.position)))
        {
            // Set Fly chance and behavior
            ability3Chance = 0.1f;
            flyMode = FlyMode.Ball; // Go after warrior nearest ball
        }


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
            shootChance = Mathf.Pow((distToGoalFactor/* + proximityToWarriorFactor*/) / 1f, 2);

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

        flyMode = FlyMode.BallOwner; // ballOwner is warrior, so target ballOwner

        // If quetz in quetz half, warrior with ball in warrior half...
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

        // If quetz and warrior with ball in quetz half...
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

        // If quetz and warrior in warrior half...
        else if (IsInWarriorHalf(gameObject) && IsInWarriorHalf(mc.BP.ballOwner))
        {
            // Debug.Log("quetz and warrior in warrior half");

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

        // If quetz in warrior half, warrior in quetz half...
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

                GameObject ball = mc.BP.gameObject;
                Vector3 bombToMonsterGoal = (monsterGoal.transform.position - bombObj.transform.position).normalized;
                Vector3 bombToBall = (ball.transform.position - bombObj.transform.position).normalized;
                Vector3 bombToWarriorGoal = (warriorGoal.transform.position - bombObj.transform.position).normalized;
                
                float minDefensiveBombDist = 5f;

                // Check if ball is moving toward own goal, and is positioned by bomb opposite said goal (so bomb will hit ball away from monster goal)
                bool shouldBombDefensive = BallGoingTowardOwnGoal() && Vector3.Dot(bombToBall, bombToMonsterGoal) < 0
                    && Vector3.Distance(ball.transform.position, monsterGoal.transform.position) > minDefensiveBombDist;

                // Check if ball is by bomb, positioned on side near warrior goal (so bomb will hit ball toward warrior goal)
                float directionalAlignmentThreshold = 0.3f;
                float maxOffensiveBombDist = 5f;
                bool shouldBombOffensive = Vector3.Dot(bombToBall, bombToWarriorGoal) > directionalAlignmentThreshold
                    && Vector3.Distance(ball.transform.position, warriorGoal.transform.position) < maxOffensiveBombDist;


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

    private void StartFly()
    {
        if (mc == null || mc.BP == null) return;
        if (flyCoroutine != null) return; // fly already active
        if (!ShouldFly()) return;
        StopCoroutines();
        isPerformingAbility = true;
        //Vector3 targetLocation = GetBallTargetPosition(flyMode);
        //flyCoroutine = StartCoroutine(flyToTarget(targetLocation));
        flyCoroutine = StartCoroutine(FlyToTarget());
    }

    private bool ShouldFly()
    {
        GameObject target = null;
        if (flyMode == FlyMode.BallOwner && mc.BP.ballOwner != null)
            target = mc.BP.ballOwner;
        else
            target = mc.BP.gameObject;
        //else
        //{
        //    Debug.LogError("target incorrectly assigned due to flyMode error");
        //    target = gameObject;
        //}

        Debug.Log("target: " + target);

        // If distance between here and target is too little
        if (Vector3.Distance(transform.position, target.transform.position) < minFlyDistance)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private IEnumerator FlyToTarget()
    {
        if (mc == null || mc.abilities[2] == null) yield break; // Ensure fly ability is valid

        // Activate fly to go underground
        mc.abilities[2].Activate();

        AbilityFly abilityfly = mc.abilities[2] as AbilityFly;

        while (abilityfly.GetIsActive())
        {
            Vector3 target = GetFlyTargetPosition();

            if (Vector3.Distance(transform.position, target) < flyToTargetThreshold)
            {
                yield return null;
                mc.movementDirection = Vector3.zero;
                continue;
            }
            Debug.Log("target: " + target);
            Debug.Log("Vector3.Distance(transform.position, target.transform.position): " + Vector3.Distance(transform.position, target));
            Debug.Log("flyToTargetThreshold: " + flyToTargetThreshold);
            Vector3 dirToTarget = (target - transform.position).normalized;
            dirToTarget = new Vector3(dirToTarget.x, 0, dirToTarget.z); // Ignore Y


            // Set movement direction while diving to be towards target
            mc.movementDirection = dirToTarget;

            yield return null;
        }

        flyCoroutine = null;
        isPerformingAbility = false;
    }

    private Vector3 GetFlyTargetPosition()
    {
        if (mc == null || mc.BP == null) return transform.position;
        if (flyMode == FlyMode.BallOwner && mc.BP.ballOwner != null)
        {
            return mc.BP.ballOwner.transform.position;
        } else if (flyMode == FlyMode.Offensive)
        {
            // Target warrior nearest ball
            WarriorController target = null;
            GetNearestWarrior(mc.BP.gameObject.transform.position).TryGetComponent<WarriorController>(out target);
            return target != null ? target.transform.position : Vector3.zero;
        } else // flyMode == Ball
        {
            return mc.BP.transform.position;
        }
    }

    private void StopFly()
    {
        if (flyCoroutine != null)
        {
            Debug.Log("StopFly");
            StopCoroutine(flyCoroutine);
            flyCoroutine = null;

            // Flush & Reset ability
            AbilityFly abilityFly = mc.abilities[2] as AbilityFly;
            if (abilityFly != null)
            {
                abilityFly.Activate();
                isPerformingAbility = false;
                abilityFly.Deactivate();
            }
        }
    }

    protected override void StopCoroutines()
    {
        base.StopCoroutines();
        StopFly();
    }
}
