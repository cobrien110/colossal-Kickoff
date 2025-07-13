using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class AiGashadokuroController : AiMonsterController
{
    private Coroutine releaseSlamCoroutine = null;
    AbilityHandSlam abilityHandSlam;
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

        // Set Hand Slam chance
        ability2Chance = 0.4f;

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

        // Monster should not use abilities, except Fire Breaths
        ability1Chance = 0.0f;
        ability2Chance = 0.0f;
        ability3Chance = 0.1f; // Shrine
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

        // If Gasha in Gasha half, warrior with ball in warrior half...
        if (!IsInWarriorHalf(gameObject) && IsInWarriorHalf(mc.BP.ballOwner))
        {
            // Default behavior
            if (!isPerformingAbility) StartRoaming();

            // Set Fire Breath chance and behavior
            ability1Chance = 0.1f; // Fire Breath

            // Set Hand Slam chance and behavior
            ability2Chance = 0.2f; // Hand Slam

            // Set Shrine chance and behavior
            ability3Chance = 0.2f; // Shrine
        }

        // If Gasha and warrior with ball in Gasha half...
        else if (!IsInWarriorHalf(gameObject) && !IsInWarriorHalf(mc.BP.ballOwner))
        {
            if (!isPerformingAbility) // Allow ability to finish if one is happening
            {
                // Default behavior
                StartDefendGoal();

                // Set Fire Breath chance and behavior
                ability1Chance = 0.4f;

                // Set Hand Slam chance and behavior
                ability2Chance = 0.3f;

                // Set Shrine chance and behavior
                ability3Chance = 0.1f;
            }
        }

        // If Gasha and warrior in warrior half...
        else if (IsInWarriorHalf(gameObject) && IsInWarriorHalf(mc.BP.ballOwner))
        {
            // Debug.Log("Gasha and warrior in warrior half");

            if (!isPerformingAbility) // Allow ability to finish if one is happening
            {
                // Debug.Log("isPerformingAbility false");
                // Default behavior
                StartPursuing();

                // Set Fire Breath chance and behavior
                ability1Chance = 0.1f;

                // Set Hand Slam chance and behavior
                ability2Chance = 0.2f;

                // Set Shrine chance and behavior
                ability3Chance = 0.1f;
            }
        }

        // If Gasha in warrior half, warrior in Gasha half...
        else if (IsInWarriorHalf(gameObject) && !IsInWarriorHalf(mc.BP.ballOwner))
        {
            if (!isPerformingAbility) // Allow ability to finish if one is happening
            {
                StopCoroutines();

                // CHANGE TO MAKE GASHA STAY IN RELATIVELY SIMILAR Z POSITION RANGE AS BALLOWNER

                // Default behavior
                mc.movementDirection = (monsterGoal.transform.position - transform.position).normalized; // Retreat to own goal
                mc.movementDirection.y = 0;
                //Debug.Log("GROUND CLIP TEST: DIR = " + mc.movementDirection);

                // Set Fire Breath chance and behavior
                ability1Chance = 0.7f;

                // Set Hand Slam chance and behavior
                ability2Chance = 0.2f;

                // Set Shrine chance and behavior
                ability3Chance = 0.1f;
            }
        }
    }

    // Fire breath
    protected override void PerformAbility1Chance()
    {
        if (mc.abilities[0] == null) return;

        if (!mc.abilities[0].AbilityOffCooldown()) return;

        if (UnityEngine.Random.value < ability1Chance && ShouldFireBreath())
        {
            Debug.Log("PerformAbility1");
            isPerformingAbility = true;

            StopCoroutines();
            mc.abilities[0].Activate();
            isPerformingAbility = false;
        }
    }

    // Hand Slam
    protected override void PerformAbility2Chance()
    {
        if (mc.abilities[1] == null) return;

        if (!mc.abilities[1].AbilityOffCooldown()) return;

        if (UnityEngine.Random.value < ability2Chance && ShouldSlam())
        {
            Debug.Log("PerformAbility2");
            isPerformingAbility = true;

            if (abilityHandSlam != null)
            {
                abilityHandSlam.TryStartSlam();
                StartReleaseSlam();
            }
        }
    }

    // Shrine
    protected override void PerformAbility3Chance()
    {
        if (mc.abilities[2] == null) return;

        if (!mc.abilities[2].AbilityOffCooldown()) return;

        if (UnityEngine.Random.value < ability3Chance)
        {
            Debug.Log("PerformAbility3");
            isPerformingAbility = true;

            StopCoroutines();
            mc.abilities[2].Activate();
            isPerformingAbility= false;
        }
    }

    private IEnumerator ReleaseSlam()
    {
        while (abilityHandSlam.GetSlamWasPressed())
        {
            abilityHandSlam.TryReleaseSlam();
            yield return null;
        }
        isPerformingAbility = false;
    }

    private void StartReleaseSlam()
    {
        if (releaseSlamCoroutine == null)
        {
            Debug.Log("Start ReleaseSlam");
            StopCoroutines();
            releaseSlamCoroutine = StartCoroutine(ReleaseSlam());
        }
    }

    private void StopReleaseSlam()
    {
        if (releaseSlamCoroutine != null)
        {
            Debug.Log("Stop ReleaseSlam");
            StopCoroutine(releaseSlamCoroutine);
            releaseSlamCoroutine = null;

            if (abilityHandSlam != null)
            {
                abilityHandSlam.AbilityReset();
            }
        }
    }

    protected override void StopCoroutines()
    {
        base.StopCoroutines();
        StopReleaseSlam();
    }
    

    // Start is called before the first frame update
    void Start()
    {
        Setup();
        //abilityHandSlam = mc.abilities.OfType<AbilityHandSlam>().FirstOrDefault();
        //Debug.Log("mc.abilities.Count: " + mc.abilities.Count);
        //mc.abilities.ForEach(item => Debug.Log(item));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MonsterBehaviour();
    }

    private bool IsWarriorInSlamRange()
    {
        GameObject nearestWarrior = GetNearestWarrior(transform.position);
        if (nearestWarrior == null) return false;

        float distToNearestWarrior = Vector3.Distance(transform.position, nearestWarrior.transform.position);
        return distToNearestWarrior <= maxProximityRange;
    }

    private bool ShouldSlam()
    {
        if (abilityHandSlam == null) abilityHandSlam = mc.abilities[1] as AbilityHandSlam; // Instaniate handSlam on first call of this method
        if (abilityHandSlam != null)
        {
            return IsWarriorInSlamRange() && abilityHandSlam.ObjectsInStunRange().Length > 0;
        }
        return false;
    }

    private bool ShouldFireBreath()
    {
        // If warrior is in relatively same z range as Gasha
        if (mc.abilities[0] is AbilityFirebreath abilityFirebreath)
        {
            float proximityRangeZ = abilityFirebreath.GetBaseFirebreathWidth();

            return warriors.Any(w => Mathf.Abs(w.transform.position.z - transform.position.z) < proximityRangeZ);
        }
        return false;
    }
}
