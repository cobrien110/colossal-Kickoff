using System.Collections;
using UnityEngine;

public class AiAhklutController : AiMonsterController
{
    [Header("AiAhklutController Specific Variables")]
    private float diveTimer;
    [SerializeField] private float maxDiveDuration; // Max duration that dive ability can be active
    [SerializeField] private float diveToTargetThreshold = 0.5f; // How close is considered "close enough" when diving to target position
    private DiveMode diveMode = DiveMode.Ball;
    private Coroutine diveCoroutine = null;
    [SerializeField] private float minDiveDistance; // min distance Ahklut must be from target to use dive
    [SerializeField] private float diveToTargetPosOffset = 1f;

    private enum DiveMode
    {
        BallOwner,
        Ball
    }

    protected override void BallNotPossessed()
    {
        if (state != State.BallNotPossessed)
        {
            Debug.Log("BallNotPossessed");
            state = State.BallNotPossessed;
            stateChanged = true;
            // StopCoroutines();
        }

        // ResetAbilities();

        // Stop roaming and pursuing if its happening
        // StopCoroutines();

        // Reset shootChance to 0.0
        if (shootChance != 0.0f) shootChance = 0.0f;

        if (!isPerformingAbility)
        {
            StopCoroutines();
            // Default behaviour
            Vector2 toBall = new Vector2(
                    mc.BP.gameObject.transform.position.x - transform.position.x,
                    mc.BP.gameObject.transform.position.z - transform.position.z).normalized;
            MoveTo(toBall);
        }

        // Higher chance to howl to stop ball from going toward goal
        if (BallGoingTowardOwnGoal())
        {
            // Set Howl chance
            ability1Chance = 0.3f;
        }
        else
        {
            // Set Howl chance
            ability1Chance = 0.1f;
        }

        // Set Spherical attack chance
        ability2Chance = 0.1f;
        attackMode = AttackMode.NearestWarrior;

        // Set Dive chance and behavior
        ability3Chance = 0.2f;
        diveMode = DiveMode.Ball;

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

    protected override void MonsterHasBall()
    {
        if (state != State.MonsterHasBall)
        {
            Debug.Log("MonsterHasBall");
            state = State.MonsterHasBall;
            stateChanged = true;
        }

        // Default behaviour
        if (!isPerformingAbility)
        {
            StopCoroutines();
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

        // Monster should not use abilities, except howl
        ability1Chance = 0.1f; // Howl
        ability2Chance = 0.0f;
        ability3Chance = 0.0f;
    }

    // Howl
    protected override void PerformAbility1Chance()
    {
        if (mc.abilities[0] == null) return;

        if (!mc.abilities[0].AbilityOffCooldown()) return;

        if (UnityEngine.Random.value < ability1Chance && ShouldHowl())
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
        if (mc.abilities[2] == null) return;

        if (!mc.abilities[2].AbilityOffCooldown()) return;

        if (UnityEngine.Random.value < ability3Chance)
        {
            Debug.Log("PerformAbility3");

            StartDive();
        }
    }

    protected override void WarriorHasBall()
    {
        if (state != State.WarriorHasBall)
        {
            Debug.Log("WarriorHasBall");
            state = State.WarriorHasBall;
            stateChanged = true;
            //StopCoroutines();
        }

        // Reset shootChance to 0.0
        if (shootChance != 0.0f) shootChance = 0.0f;

        diveMode = DiveMode.BallOwner; // Warrior is current ballOwner, so target it

        // If ahklut in ahklut half, warrior with ball in warrior half...
        if (!IsInWarriorHalf(gameObject) && IsInWarriorHalf(mc.BP.ballOwner))
        {
            // Default behavior
            if (!isPerformingAbility) StartRoaming();

            // Set Howl chance
            ability1Chance = 0.05f; // Howl

            // Set Spherical Attack chance and behavior
            ability2Chance = 0.1f; // Spherical Attack
            attackMode = AttackMode.NearestWarrior; // Target nearest warrior because you don't want to overextend to get ball owner

            // Set Dive chance
            ability3Chance = 0.1f; // Dive

        }

        // If ahklut and warrior with ball in ahklut half...
        else if (!IsInWarriorHalf(gameObject) && !IsInWarriorHalf(mc.BP.ballOwner))
        {
            if (!isPerformingAbility) // Allow ability to finish if one is happening
            {
                // Default behavior
                StartDefendGoal();

                // Set Howl chance
                ability1Chance = 0.2f;

                // Set Spherical Attack chance and behavior
                ability2Chance = 0.1f;
                attackMode = AttackMode.BallOwner;

                // Set Dive chance
                ability3Chance = 0.3f;

            }
        }

        // If ahklut and warrior in warrior half...
        else if (IsInWarriorHalf(gameObject) && IsInWarriorHalf(mc.BP.ballOwner))
        {
            // Debug.Log("ahklut and warrior in warrior half");

            if (!isPerformingAbility) // Allow ability to finish if one is happening
            {
                // Debug.Log("isPerformingAbility false");
                // Default behavior
                StartPursuing();

                // Set Howl chance
                ability1Chance = 0.2f;

                // Set Spherical Attack chance and behavior
                ability2Chance = 0.1f;
                attackMode = AttackMode.BallOwner; // Be aggressive, try to get ball

                // Set Dive chance
                ability3Chance = 0.2f;
            }
        }

        // If ahklut in warrior half, warrior in ahklut half...
        else if (IsInWarriorHalf(gameObject) && !IsInWarriorHalf(mc.BP.ballOwner))
        {

            if (!isPerformingAbility) // Allow ability to finish if one is happening
            {
                StopCoroutines();

                // Default behavior
                mc.movementDirection = (monsterGoal.transform.position - transform.position).normalized; // Retreat to own goal
                mc.movementDirection.y = 0;
                //Debug.Log("GROUND CLIP TEST: DIR = " + mc.movementDirection);

                // Set Howl chance
                ability1Chance = 0.1f;

                // Set Spherical Attack chance and behavior
                ability2Chance = 0.1f;
                attackMode = AttackMode.BallOwner; // Hurry to kill ball owner to stop goal

                // Set Dive chance
                ability3Chance = 0.4f;
            }
        }
    }

    private bool ShouldHowl()
    {
        AbilityHowl abilityHowl = mc.abilities[0] as AbilityHowl;
        if (abilityHowl == null)
        {
            Debug.Log("Cast to AbilityHowl Failed");
            return false;
        }

        return abilityHowl.WarriorInRadius() // Would hit a warrior
            || (abilityHowl.BallInRadius() && BallGoingTowardOwnGoal()); // Or the ball while its going toward own goal
    }

    private IEnumerator DiveToTarget()
    {
        if (mc == null || mc.abilities[2] == null) yield break; // Ensure dive ability is valid
        Vector3 target = GetDiveTargetPosition();

        // Activate dive to go underground
        mc.abilities[2].Activate();

        // While distance between Ahklut and target is greater than diveToTargetThreshold
        // AND the dive hasn't been active for too long (because the speed boost rapidly diminishes)
        while (Vector3.Distance(transform.position, target) > diveToTargetThreshold
            && diveTimer < maxDiveDuration)
        {
            Debug.Log("target: " + target);
            Debug.Log("Vector3.Distance(transform.position, target.transform.position): " + Vector3.Distance(transform.position, target));
            Debug.Log("diveToTargetThreshold: " + diveToTargetThreshold);
            Debug.Log("diveTimer: " + diveTimer);
            Debug.Log("maxDiveDuration: " + maxDiveDuration);
            diveTimer += Time.deltaTime;
            Vector3 dirToTarget = (target - transform.position).normalized;
            dirToTarget = new Vector3(dirToTarget.x, 0, dirToTarget.z); // Ignore Y
            

            // Set movement direction while diving to be towards target
            mc.movementDirection = dirToTarget;

            yield return null;
        }

        // Reset inputBuffer on AbilityDive (to fix an issue that prevents proper second activation)
        AbilityDive abilityDive = mc.abilities[2] as AbilityDive;
        abilityDive.ResetBuffer();

        // Target has been reached, or we've reached maxDiveDuration
        // Activate ability again to go above ground
        mc.abilities[2].Activate();
        
        diveCoroutine = null;
        isPerformingAbility = false;
        diveTimer = 0f;
    }

    private void StartDive()
    {
        if (mc == null || mc.BP == null) return;
        if (diveCoroutine != null) return; // Dive already active
        if (!ShouldDive()) return;
        StopCoroutines();
        isPerformingAbility = true;
        //Vector3 targetLocation = GetBallTargetPosition(diveMode);
        //diveCoroutine = StartCoroutine(DiveToTarget(targetLocation));
        diveCoroutine = StartCoroutine(DiveToTarget());
    }

    private Vector3 GetDiveTargetPosition()
    {
        if (mc == null || mc.BP == null) return transform.position;
        if (diveMode == DiveMode.BallOwner && mc.BP.ballOwner != null)
        {
            Vector3 ballOwnerPos = mc.BP.ballOwner.transform.position;
            Vector3 ballOwnerToGoalDir = (monsterGoal.transform.position - ballOwnerPos).normalized;
            Vector3 targetPos = ballOwnerPos + (ballOwnerToGoalDir * diveToTargetPosOffset);
            Vector3 targetPosFinal = new Vector3(Mathf.Clamp(targetPos.x, -8, 8), targetPos.y, targetPos.z);
            return targetPosFinal;
        } else
        {
            Vector3 ballPos = mc.BP.transform.position;
            Vector3 ballToGoalDir = (monsterGoal.transform.position - ballPos).normalized;
            Vector3 targetPos = ballPos + (ballToGoalDir * diveToTargetPosOffset);
            Vector3 targetPosFinal = new Vector3(Mathf.Clamp(targetPos.x, -8, 8), targetPos.y, targetPos.z);
            return targetPosFinal;
        }
    }

    private void StopDive()
    {
        if (diveCoroutine != null)
        {
            Debug.Log("StopDive");
            StopCoroutine(diveCoroutine);
            diveCoroutine = null;

            // Flush & Reset ability
            AbilityDive abilityDive = mc.abilities[2] as AbilityDive;
            if (abilityDive != null)
            {
                abilityDive.Activate();
                isPerformingAbility = false;
                abilityDive.Deactivate();
            }
        }
    }


    protected override void StopCoroutines()
    {
        base.StopCoroutines();
        StopDive();
    }

    private bool ShouldDive()
    {
        GameObject target = null;
        if (diveMode == DiveMode.BallOwner && mc.BP.ballOwner != null)
            target = mc.BP.ballOwner;
        else
            target = mc.BP.gameObject;
        //else
        //{
        //    Debug.LogError("target incorrectly assigned due to diveMode error");
        //    target = gameObject;
        //}

        Debug.Log("target: " + target);

        // If distance between here and target is too little
        if (Vector3.Distance(transform.position, target.transform.position) < minDiveDistance)
        {
            return false;
        }
        else
        {
            return true;
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
        //SetupFixedUpdate();
        MonsterBehaviour();
    }
}
