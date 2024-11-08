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

    [SerializeField] private float redirectionInterval = 0.5f; // Time interval in seconds
    private float redirectionTimer = 0f;      // Timer to track redirection intervals
    private Vector3 currentRandomOffset = Vector3.zero;
    private Coroutine roamCoroutine;
    [SerializeField] private float wiggleOffset = 1f;
    [SerializeField] private float waitInPlaceTime = 1f;
    private const float stoppingDistance = 0.5f;
    private const float leftBoundary = -6f;
    private const float fieldDepth = 4f;
    private const float rotationSpeed = 2f;
    [SerializeField] private float asaMinimumCharge = 0.3f;
    [SerializeField] private float pursueDelay = 1f;
    [SerializeField] private float minPursueDistance = 2f;
    private float pursuitSmoothingFactor = 1f;
    //private float pursueDelayFrequency;

    // private bool shouldPerformAbility1 = false;
    private bool isCharging = false;
    private bool canPickUpBall = true;
    // private bool targetBallController = true; // Used to determine if attack will target ball controller or nearest warrior

   private enum SphericalAttackMode
   {
       BallOwner,
       NearestWarrior
   }

    private enum WallMode
    {
        Offensive
    }

    private enum DashMode
    {
        BallOwner,
        Nearest
    }

    // Used to track current Ability Modes
    SphericalAttackMode asaMode = SphericalAttackMode.BallOwner;
    WallMode wallMode = WallMode.Offensive;
    DashMode dashMode = DashMode.BallOwner;



    protected override void PerformAbility1Chance(float chargeAmount)
    {
        if (mc.abilities[0] == null) return;
            
        if (!mc.abilities[0].AbilityOffCooldown()) return;

        // Debug.Log("PerformAbility1Chance. chargeAmount: " + chargeAmount);
        if (Random.value < ability1Chance)
        {
            Debug.Log("PerformAbility1. chargeAmount: " + chargeAmount);
            isPerformingAbility = true;

            Wall(wallMode);
        }
    }

    protected override void PerformAbility2Chance(float chargeAmount)
    {
        if (mc.abilities[1] == null) return;

        if (!mc.abilities[1].AbilityOffCooldown()) return;

        // Debug.Log("PerformAbility2Chance. chargeAmount: " + chargeAmount);
        if (Random.value < ability2Chance)
        {
            Debug.Log("PerformAbility2. chargeAmount: " + chargeAmount);
            isPerformingAbility = true;

            SphericalAttack(asaMode);
        }
    }

    protected override void PerformAbility3Chance(float chargeAmount)
    {
        if (mc.abilities[2] == null) return;

        if (!mc.abilities[2].AbilityOffCooldown()) return;

        // Debug.Log("PerformAbility3Chance. chargeAmount: " + chargeAmount);
        if (Random.value < ability3Chance)
        {
            Debug.Log("PerformAbility3. chargeAmount: " + chargeAmount);
            isPerformingAbility = true;

            Dash(dashMode);
            //DashHelper();
        }
    }

    protected override void PerformShootChance(float chargeAmount)
    {
        // Debug.Log("shootChance: " + shootChance);
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

    private void WarriorHasBall()
    {
        // Reset shootChance to 0.0
        if (shootChance != 0.0f) shootChance = 0.0f;

        // If mino in mino half, warrior with ball in warrior half...
        if (!IsInWarriorHalf(gameObject) && IsInWarriorHalf(mc.BP.ballOwner))
        {
            // Default behavior
            if (!isPerformingAbility) StartRoaming();

            // Set Wall chance and behavior
            ability1Chance = 0.1f; // Wall
            wallMode = WallMode.Offensive;

            // Set Spherical Attack chance and behavior
            ability2Chance = 0.1f; // Spherical Attack
            asaMode = SphericalAttackMode.NearestWarrior; // Target nearest warrior because you don't want to overextend to get ball owner

            // Set Dash chance and behavior
            ability3Chance = 0.1f; // Dash
            dashMode = DashMode.Nearest; // Don't want to overextend so target nearest

        }

        // If mino and warrior with ball in mino half...
        else if (!IsInWarriorHalf(gameObject) && !IsInWarriorHalf(mc.BP.ballOwner))
        {
            StopRoaming();
            if (!isPerformingAbility) // Allow ability to finish if one is happening
            {
                // Default behavior
                mc.movementDirection = (GetDefendGoalPosition() - transform.position).normalized; // Stand in between goal and ball owner

                // Set Wall chance and behavior
                ability1Chance = 0.1f;
                wallMode = WallMode.Offensive;

                // Set Spherical Attack chance and behavior
                ability2Chance = 0.1f;
                asaMode = SphericalAttackMode.BallOwner; 

                // Set Dash chance and behavior
                ability3Chance = 0.1f;
                dashMode = DashMode.BallOwner;
            }
        }

        // If mino and warrior in warrior half...
        else if (IsInWarriorHalf(gameObject) && IsInWarriorHalf(mc.BP.ballOwner))
        {
            StopRoaming();
            if (!isPerformingAbility) // Allow ability to finish if one is happening
            {
                // Default behavior
                PursuePlayer();

                // Set Wall chance and behavior
                ability1Chance = 0.1f;
                wallMode = WallMode.Offensive;

                // Set Spherical Attack chance and behavior
                ability2Chance = 0.1f;
                asaMode = SphericalAttackMode.BallOwner; // Be aggressive, try to get ball

                // Set Dash chance and behavior
                ability3Chance = 0.1f;
                dashMode = DashMode.BallOwner; // Be aggressive, try to get ball
            }
        }

        // If mino in warrior half, warrior in mino half...
        else if (IsInWarriorHalf(gameObject) && !IsInWarriorHalf(mc.BP.ballOwner))
        {
            StopRoaming();
            if (!isPerformingAbility) // Allow ability to finish if one is happening
            {
                // Default behavior
                mc.movementDirection = (monsterGoal.transform.position - transform.position).normalized; // Retreat to own goal

                // Set Wall chance and behavior
                ability1Chance = 0.1f;
                wallMode = WallMode.Offensive;

                // Set Spherical Attack chance and behavior
                ability2Chance = 0.1f;
                asaMode = SphericalAttackMode.BallOwner; // Hurry to kill ball owner to stop goal

                // Set Dash chance and behavior
                ability3Chance = 0.1f;
                dashMode = DashMode.BallOwner; // Rush to get back
            }
        }
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

        // Debug.Log("shootChance: " + shootChance);

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
            canPickUpBall = false;
            StartCoroutine(SetPickUpBallTrue());
            // Debug.Log(transform.forward);
            float distFromGoalMultiplier = Vector3.Distance(warriorGoal.transform.position, transform.position) / (maxShootingRange / 2f);
            mc.BP.GetComponent<Rigidbody>().AddForce(transform.forward * aiShootSpeed);// * distFromGoalMultiplier);
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

    private WarriorController GetNearestWarrior()
    {
        WarriorController[] warriors = FindObjectsOfType<WarriorController>();
        WarriorController nearestWarrior = null;
        float distToNearestWarrior = maxProximityRange;

        foreach (WarriorController warrior in warriors)
        {
            float distanceToWarrior = Vector3.Distance(transform.position, warrior.transform.position);
            if (distanceToWarrior < distToNearestWarrior)
            {
                nearestWarrior = warrior;
                distToNearestWarrior = distanceToWarrior;
            }
        }

        return nearestWarrior;
    }

    // ROAM METHODS
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

    // SPEHRICAL ATTACK METHODS
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
        if (ShouldSphericalAttack(asa) && asa.GetChargeAmount() > asaMinimumCharge)
        {
            // Debug.Log("Activate");
            asa.Activate();
            asa.ANIM.SetBool("isWindingUp", false);
            isPerformingAbility = false;
        }
        else if (asa.GetIsCharging() && asa.GetTimer() >= asa.GetCooldown()) // If it still is true, keep charging
        {
            // Debug.Log("ChargeUp");
            asa.ChargeUp();
        }
        else
        {
            // Debug.Log("ChargeDown");
            asa.ChargeDown();
        }
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

    IEnumerator SphericalAttackNearestWarrior()
    {
        Debug.Log("SphericalAttackNearestWarrior");
        WarriorController nearestWarrior = GetNearestWarrior();
        if (nearestWarrior == null)
        {
            Debug.Log("No warrior close enough to attack");
            isPerformingAbility = false;  // not performing ability so reset bool
            yield break;
        }

        StopRoaming();
        while (isPerformingAbility)
        {
            // If targeted warrior died during ability charge, go to next warrior
            if (nearestWarrior == null)
            {
                Debug.Log("nearestWarrior: " + nearestWarrior);
                nearestWarrior = GetNearestWarrior();
            }
            if (nearestWarrior == null)
            {
                Debug.Log("Break");
                break; // If going to next warrior didn't work because there are none, break
            }

            SphericalAttackHelper();
            mc.movementDirection = (nearestWarrior.gameObject.transform.position - transform.position).normalized;
            yield return null;
        }

        // In case where break happened, just flush ability stuff by using it
        if (isPerformingAbility)
        {
            Debug.Log("Flush spherical attack");
            //SphericalAttackHelper();
            AbilitySphericalAttack asa = (AbilitySphericalAttack) mc.abilities[1];
            asa.ChargeDown();
            isPerformingAbility = false;
            mc.movementDirection = Vector3.zero;
        }
            
    }

    IEnumerator SphericalAttackBallController()
    {

        // Ensure ball owner is not null
        if (mc.BP == null || mc.BP.ballOwner == null)
        {
            Debug.Log("BP or BP.ballOwner is null - don't attack");
            isPerformingAbility = false;  // not performing ability so reset bool
            yield break;
        }
        // Ensure ballOwner is in range
        if (Vector3.Distance(mc.BP.ballOwner.transform.position, transform.position) > maxProximityRange)
        {
            Debug.Log("Ball owner not close enough to attack");
            isPerformingAbility = false;  // not performing ability so reset bool
            yield break;
        }
        GameObject ballController = mc.BP.ballOwner;

        StopRoaming();

        while (isPerformingAbility)
        {
            // If ballOwner died, just retarget to nearest warrior
            if (ballController == null) ballController = GetNearestWarrior().gameObject;

            SphericalAttackHelper();
            mc.movementDirection = (ballController.transform.position - transform.position).normalized;
            yield return null;
        }
    }

    private void SphericalAttack(SphericalAttackMode mode)
    {
        if (mode == SphericalAttackMode.BallOwner)
        {
            StartCoroutine(SphericalAttackBallController());
        }
        else if (mode == SphericalAttackMode.NearestWarrior)
        {
            StartCoroutine(SphericalAttackNearestWarrior());
        }
    }

    IEnumerator SetPickUpBallTrue()
    {
        yield return new WaitForSeconds(0.2f);
        canPickUpBall = true;
    }

    public bool GetCanPickUpBall()
    {
        return canPickUpBall;
    }

    IEnumerator PursuePlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(pursueDelay);

            // Ensure the ball owner is valid before pursuing
            if (mc.BP.ballOwner != null)
            {
                Vector3 targetPosition = mc.BP.ballOwner.transform.position;
                float distanceToPlayer = Vector3.Distance(targetPosition, transform.position);

                // Check if the monster is too close; stop if within minimum distance
                if (distanceToPlayer > minPursueDistance)
                {
                    // Calculate target direction
                    Vector3 targetDirection = (targetPosition - transform.position).normalized;

                    // Smoothly update the movement direction using linear interpolation
                    mc.movementDirection = Vector3.Lerp(mc.movementDirection, targetDirection, Time.deltaTime * pursuitSmoothingFactor);
                }
                else
                {
                    // Stop moving if too close to the player
                    mc.movementDirection = Vector3.zero;
                }
            }
            else
            {
                // If ball owner is null, stop movement
                mc.movementDirection = Vector3.zero;
            }

            yield return null; // Continue to next frame
        }
    }
    private bool ShouldSphericalAttack(AbilitySphericalAttack asa)
    {
        // Should attack either if at full charge, or anytime a warrior is in attack radius
        return asa.GetChargeAmount() >= asa.maxChargeSeconds || WarriorIsInAttackSphereRadius(asa);
    }

    // Defend goal position is in the middle of the ballOwner and the goal
    private Vector3 GetDefendGoalPosition()
    {
        Vector3 vec = mc.BP.ballOwner.transform.position - monsterGoal.transform.position;
        Vector3 dir = vec.normalized;
        float distance = vec.magnitude;
        Vector3 defendPos = monsterGoal.transform.position + (dir * distance / 2);
        Vector3 defendPosIgnoreY = new Vector3(defendPos.x, transform.position.y, defendPos.z);
        // Debug.Log(defendPos);
        return defendPosIgnoreY;
    }

    // WALL METHODS
    private void WallOffensive()
    {
        // Look toward ball
        mc.movementDirection = (mc.BP.gameObject.transform.position - transform.position).normalized;

        // Summon wall
        if (mc.abilities[0] is AbilityMinotaurWall)
        {
            mc.abilities[0].Activate();
        }
    }

    private void Wall(WallMode wallMode)
    {
        Debug.Log("Wall");
        if (wallMode == WallMode.Offensive)
        {
            WallOffensive();
        }
        isPerformingAbility = false;
    }

    // DASH METHODS
    private void DashHelper()
    {
        // Debug.Log("Dash");

        // Make sure first ability is an AbilityChargable
        if (!(mc.abilities[2] is AbilityBullrush)) return;

        AbilityBullrush abr = (AbilityBullrush)mc.abilities[2];

        // Check if off cooldown
        if (abr.GetTimer() < abr.GetCooldown()) return;

        abr.SetIsAutoCharging(true);

        if (abr.GetIsAutoCharging())
        {
            abr.ChargeUp();
            abr.SetIsAutoCharging(true);
        }
        else if (!abr.GetIsAutoCharging())
        {
            abr.ChargeDown();
        }
    }

    IEnumerator DashBallOwner()
    {
        if (mc.BP.ballOwner == null) 
        {
            yield break;
        }

        while (isPerformingAbility)
        {
            // Look at ball owner
            mc.movementDirection = (mc.BP.ballOwner.transform.position - transform.position).normalized;

            // Dash
            DashHelper();

            yield return null;
        }
        Debug.Log("DashOffensive done");
    }

    IEnumerator DashNearest()
    {
        if (mc.BP.ballOwner == null)
        {
            yield break;
        }

        while (isPerformingAbility)
        {
            // Look at ball owner
            mc.movementDirection = (GetNearestWarrior().transform.position - transform.position).normalized;

            // Dash
            DashHelper();

            yield return null;
        }
        Debug.Log("DashBallOwner done");
    }
    private void Dash(DashMode dashMode)
    {
        Debug.Log("Dash");
        if (dashMode == DashMode.BallOwner)
        {
            StartCoroutine(DashBallOwner());
        } else if (dashMode == DashMode.Nearest)
        {
            StartCoroutine(DashNearest());
        }
        else
        {
            Debug.Log("Error in Dash");
        }

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

    private void EnsureBallOwnerValid()
    {
        if (mc.BP == null)
        {
            mc.BP = FindObjectOfType<BallProperties>();
        }

        if (mc.Ball == null) mc.Ball = mc.BP.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //AbilityBullrush abr = (AbilityBullrush)mc.abilities[2];
        //Debug.Log("isAutoCharging: " + abr.GetIsAutoCharging());
        //Debug.Log("autoCharge: " + abr.autoCharge);
    }
    
    /*
     * TODO
     * 
     * Introduce delay in moving to defend position
     * 
     * Optimize GetNearestWarrior so that it uses a global List/Array of warriors rather than retrieving them each frame
     * 
     */
}
