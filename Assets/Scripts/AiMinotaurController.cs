using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class AiMinotaurController : AiMonsterController
{
    // Ability Order (by index)
        // Wall - 0
        // Basic Attack - 1
        // Dash - 2
        
    private float redirectionInterval = 0.5f; // Time interval in seconds
    private float redirectionTimer = 0f;      // Timer to track redirection intervals
    private Vector3 currentRandomOffset = Vector3.zero;
    private Coroutine roamCoroutine;
    [SerializeField] private float wiggleOffset = 1f;
    [SerializeField] private float waitInPlaceTime = 1f;
    private const float stoppingDistance = 0.5f;
    private const float leftBoundary = -6f;
    private const float fieldDepth = 4f;
    private const float rotationSpeed = 2f;

    // private bool shouldPerformAbility1 = false;
    private bool isCharging = false;


    protected override void PerformAbility1Chance(float chargeAmount)
    {
        // Debug.Log("PerformAbility1Chance. chargeAmount: " + chargeAmount);
        if (Random.value < ability1Chance)
        {
            Debug.Log("PerformAbility1. chargeAmount: " + chargeAmount);
            isPerformingAbility = true;
            mc.abilities[0].Activate();
        }
    }

    protected override void PerformAbility2Chance(float chargeAmount)
    {
        // Debug.Log("PerformAbility2Chance. chargeAmount: " + chargeAmount);
        if (Random.value < ability2Chance)
        {
            Debug.Log("PerformAbility2. chargeAmount: " + chargeAmount);
            isPerformingAbility = true;
            //SphericalAttack();
            StartCoroutine(SphericalAttack());
            //mc.abilities[1].Activate();
        }
    }

    protected override void PerformAbility3Chance(float chargeAmount)
    {
        // Debug.Log("PerformAbility3Chance. chargeAmount: " + chargeAmount);
        if (Random.value < ability3Chance)
        {
            Debug.Log("PerformAbility3. chargeAmount: " + chargeAmount);
            isPerformingAbility = true;
            mc.abilities[2].Activate();
        }
    }

    protected override void PerformShootChance(float chargeAmount)
    {
        // Debug.Log("PerformShootChance. chargeAmount: " + chargeAmount);
        if (Random.value < shootChance)
        {
            Debug.Log("PerformShoot. chargeAmount: " + chargeAmount);
            Shoot();
        }
    }

    /*
     * Decides the overall logic for the AiMonster.
     * Accounts for movement of monster, and chances
     * that monster will perform any given ability or if
     * it will Shoot at any given moment, based on state of
     * the ball (who possesses it, if anyone) and other factors
     * (such as proximity between warriors and monster, where each
     * are on the field, etc)
     */
    protected override void MonsterBehaviour()
    {
        // Debug.Log("MonsterBehaviour");

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

    private void WarriorHasBall()
    {
        // Debug.Log("WarriorHasBall");

        // Reset shootChance to 0.0
        if (shootChance != 0.0f) shootChance = 0.0f;

        // If mino in mino half, warrior with ball in warrior half...
        if (!IsInWarriorHalf(gameObject) && IsInWarriorHalf(mc.BP.ballOwner))
        {
            // Debug.Log("Monster Left, warrior right");
            // Roam around
            StartRoaming();

            // Occasionally use ability
            ability1Chance = 0.0f;
            ability2Chance = 0.1f;
            ability3Chance = 0.0f;

        }

        // If mino and warrior with ball in mino half...

        // If mino and warrior in warrior half...

        // If mino in warrior half, warrior in mino half...
    }

    private void MonsterHasBall()
    {
        // Monster should not use abilities
        ability1Chance = 0.0f;
        ability2Chance = 0.0f;
        ability3Chance = 0.0f;

        // Stop roaming if its happening
        StopRoaming();

        // Debug.Log("MonsterHasBall");

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
        shootChance = Mathf.Pow((distToGoalFactor + proximityToWarriorFactor) / 2f, 2);

        // If shooting, chargeAmount depends on distance to goal
    }

    private void BallNotPossessed()
    {
        // Reset shootChance to 0.0
        if (shootChance != 0.0f) shootChance = 0.0f;

        // Stop roaming if its happening
        StopRoaming();

        // Debug.Log("BallNotPossessed");

        // Move to ball
        Vector2 toBall = new Vector2(
                mc.BP.gameObject.transform.position.x - transform.position.x,
                mc.BP.gameObject.transform.position.z - transform.position.z).normalized;
        MoveTo(toBall); ;
    }

    protected override void Shoot()
    {
        if (mc.BP.ballOwner == gameObject)
        {
            Debug.Log("Shoot!");

            // Make minotaur look at goal
            Quaternion newRotation = 
                Quaternion.LookRotation((warriorGoal.transform.position - transform.position).normalized, Vector3.up);
            transform.rotation = newRotation;

            mc.BP.ballOwner = null;
            // Debug.Log(transform.forward);
            float distFromGoalMultiplier = Vector3.Distance(warriorGoal.transform.position, transform.position) / (maxShootingRange / 2f);
            mc.BP.GetComponent<Rigidbody>().AddForce(transform.forward * aiShootSpeed * distFromGoalMultiplier);
            //audioPlayer.PlaySoundRandomPitch(audioPlayer.Find("pass"));
        }
    }

    void MoveTo(Vector2 targetPos)
    {
        // Debug.Log("MoveTo: " + targetPos);
        if (targetPos != Vector2.zero)
        {
            //usingKeyboard = true;
            mc.movementDirection = new Vector3(targetPos.x, 0, targetPos.y).normalized;
            mc.aimingDirection = mc.movementDirection;
        }


        rb.velocity = GM.isPlaying ? mc.movementDirection * mc.monsterSpeed : Vector3.zero;
        //rb.velocity = isCharging ? rb.velocity * chargeMoveSpeedMult : rb.velocity;
        if (rb.velocity != Vector3.zero)
        {
            Quaternion newRotation = Quaternion.LookRotation(mc.movementDirection.normalized, Vector3.up);
            transform.rotation = newRotation;
        }

        if (mc.movementDirection != Vector3.zero && GM.isPlaying)
        {
            mc.ANIM.SetBool("isWalking", true);
        }
        else
        {
            mc.ANIM.SetBool("isWalking", false);
        }

    }

    private void WiggleTowardGoal()
    {
        Vector3 goalPosition = warriorGoal.transform.position;

        // Update the timer
        redirectionTimer += Time.deltaTime;

        // Check if it's time to update the random offset
        if (redirectionTimer >= redirectionInterval)
        {
            // Reset the timer
            redirectionTimer = 0f;

            // Generate a new random offset for "wiggle" effect
            currentRandomOffset = new Vector3(
                Random.Range(-wiggleOffset, wiggleOffset),  // Random x offset
                0,                          // Keep y as zero for ground-based movement
                Random.Range(-wiggleOffset, wiggleOffset)   // Random z offset
            );
        }

        // Calculate the base direction toward the goal
        Vector3 toGoal = (goalPosition - transform.position).normalized;

        // Apply the current random offset to the direction vector
        mc.movementDirection = (toGoal + currentRandomOffset).normalized;

        // Set the minotaur’s movement based on the calculated `movementDirection`
        // rb.velocity = mc.movementDirection * mc.monsterSpeed;

        // Update walking animation if applicable
        mc.ANIM.SetBool("isWalking", rb.velocity != Vector3.zero);
    }

    private float GetDistanceToNearestWarrior()
    {
        List<WarriorController> warriors = FindObjectsOfType<WarriorController>().ToList<WarriorController>();
        float distToNearestWarrior = maxProximityRange;
        foreach (WarriorController warrior in warriors)
        {
            float distanceToWarrior = Vector3.Distance(transform.position, warrior.transform.position);
            if (distanceToWarrior < distToNearestWarrior)
            {
                distToNearestWarrior = distanceToWarrior;
            }
        }
        return distToNearestWarrior;
    }

    IEnumerator Roam()
    {
        while (true)
        {
            Debug.Log("Monster roaming");

            // Generate a random x and z position within the left half of the field
            float randomX = Random.Range(leftBoundary, midFieldPoint);
            float randomZ = Random.Range(-fieldDepth, fieldDepth);
            Vector3 randomTargetPosition = new Vector3(randomX, transform.position.y, randomZ);

            // Move toward the random target position
            while (Vector3.Distance(transform.position, randomTargetPosition) > stoppingDistance)
            {
                if (mc.isStunned) yield break;

                // Calculate direction and move toward the target position
                Vector3 directionToTarget = (randomTargetPosition - transform.position).normalized;
                mc.movementDirection = directionToTarget;
                rb.velocity = mc.movementDirection * mc.monsterSpeed;

                // Rotate the minotaur to face the direction it's moving
                Quaternion newRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * rotationSpeed);

                yield return null;
            }

            // Prevent mino from moving once it reached its spot
            mc.movementDirection = Vector3.zero;
            rb.velocity = Vector3.zero;

            // Pause briefly before picking a new random position
            yield return new WaitForSeconds(waitInPlaceTime);
        }
    }


    private void StartRoaming()
    {
        if (isPerformingAbility) return;
        if (roamCoroutine == null)
        {
            roamCoroutine = StartCoroutine(Roam());
        }
    }

    private void StopRoaming()
    {
        if (roamCoroutine != null)
        {
            StopCoroutine(roamCoroutine);
            roamCoroutine = null;
        }
    }

    private void SphericalAttackHelper()
    {
        Debug.Log("SphericalAttack");
        // Make sure first ability is an AbilityChargable
        if (!(mc.abilities[1] is AbilitySphericalAttack)) return;

        AbilitySphericalAttack asa = (AbilitySphericalAttack)mc.abilities[1];

        // Check if off cooldown
        if (asa.GetTimer() < asa.GetCooldown()) return;

        asa.SetIsCharging(true);

        // If input is no longer true, attack
        if (ShouldSphericalAttack(asa) && asa.GetChargeAmount() != 0)
        {
            //shouldPerformAbility1 = false;
            asa.Activate();
            asa.ANIM.SetBool("isWindingUp", false);
            isPerformingAbility = false;
        }
        else if (asa.GetIsCharging() && asa.GetTimer() >= asa.GetCooldown()) // If it still is true, keep charging
        {
            asa.ChargeUp();
        }
        else
        {
            asa.ChargeDown();
        }
    }

    IEnumerator SphericalAttack()
    {
        while (isPerformingAbility)
        {
            SphericalAttack();
            yield return null;
        }
    }

    private bool ShouldSphericalAttack(AbilitySphericalAttack asa)
    {
        // Should attack either if at full charge, or anytime a warrior is in attack radius
        return asa.GetChargeAmount() >= asa.maxChargeSeconds || WarriorIsInAttackSphereRadius(asa);
    }

    private bool WarriorIsInAttackSphereRadius(AbilitySphericalAttack asa)
    {
        Vector3 origin = new Vector3(transform.position.x, transform.position.y + asa.attackVisualOffsetY, transform.position.z);
        Collider[] colliders = Physics.OverlapSphere(origin + transform.forward * asa.attackRange, asa.attackBaseRadius
            + asa.GetChargeAmount() * asa.chargeRate, asa.affectedLayers);

        foreach (Collider col in colliders)
        {
            // Handle collision with each collider
            Debug.Log("SphereCast hit " + col.gameObject.name);
            if (col.gameObject.CompareTag("Warrior"))
            {
                Debug.Log("Warrior in attack sphere radius");
                return true;
            }
        }
        return false;
    }

    private void FixedUpdate()
    {
        // Decides movement behaviour,
        // Also decides chances that abilities and Shooting will occur at any given moment
        MonsterBehaviour();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Sets up variable assignments for this gameobject
        Setup();
    }

    // Update is called once per frame
    void Update()
    {
        // Make sure mc.BP is assigned a value
        if (mc.BP == null)
        {
            mc.BP = FindObjectOfType<BallProperties>();
        }

        if (mc.Ball == null) mc.Ball = mc.BP.gameObject;
    }
    

}
