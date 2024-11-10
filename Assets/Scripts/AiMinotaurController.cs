using System;
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

    [Header("AI Mino Stats & Behaviour")]
    [SerializeField] private float redirectionInterval = 0.5f; // Time interval in seconds
    private float redirectionTimer = 0f;      // Timer to track redirection intervals
    private Vector3 currentRandomOffset = Vector3.zero;
    private Coroutine roamCoroutine;
    private Coroutine pursueCoroutine;
    private Coroutine defendGoalCoroutine;
    [SerializeField] private float wiggleOffset = 1f;
    [SerializeField] private float waitInPlaceTime = 1f;
    private const float stoppingDistance = 0.5f;
    private const float rotationSpeed = 2f;
    [SerializeField] private float asaMinimumCharge = 0.3f;
    [SerializeField] private float pursueDelay = 1f;
    [SerializeField] private float minPursueDistance = 2f;
    private float pursuitSmoothingFactor = 1f;
    [SerializeField] private float defendGoalDelay = 0.5f;
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
        BlockWarrior,
        BlockGoal,
        Offensive
    }

    private enum DashMode
    {
        BallOwner,
        Nearest,
        Ball
    }

    // Used to track current Ability Modes
    SphericalAttackMode asaMode = SphericalAttackMode.BallOwner;
    WallMode wallMode = WallMode.BlockGoal;
    DashMode dashMode = DashMode.BallOwner;



    protected override void PerformAbility1Chance()
    {
        if (mc.abilities[0] == null) return;
            
        if (!mc.abilities[0].AbilityOffCooldown()) return;

        if (UnityEngine.Random.value < ability1Chance)
        {
            Debug.Log("PerformAbility1");
            isPerformingAbility = true;

            StopCoroutines();
            Wall(wallMode);
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
            SphericalAttack(asaMode);
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
            Dash(dashMode);
            //DashHelper();
        }
    }

    protected override void PerformShootChance()
    {
        if (UnityEngine.Random.value < shootChance && mc.BP != null && mc.BP.ballOwner != null && mc.BP.ballOwner == gameObject)
        {
            Debug.Log("PerformShoot");
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
            StopPursuing();
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
            StopCoroutines();
            if (!isPerformingAbility) // Allow ability to finish if one is happening
            {
                // Default behavior
                StartDefendGoal();

                // Set Wall chance and behavior
                ability1Chance = 0.1f;
                wallMode = WallMode.BlockGoal;

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
            // Debug.Log("mino and warrior in warrior half");
            StopRoaming();

            if (!isPerformingAbility) // Allow ability to finish if one is happening
            {
                // Debug.Log("isPerformingAbility false");
                // Default behavior
                StartPursuing();

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
            StopCoroutines();

            if (!isPerformingAbility) // Allow ability to finish if one is happening
            {
                // Default behavior
                mc.movementDirection = (monsterGoal.transform.position - transform.position).normalized; // Retreat to own goal

                // Set Wall chance and behavior
                ability1Chance = 0.1f;
                wallMode = WallMode.BlockGoal;

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
            shootChance = Mathf.Pow((distToGoalFactor + proximityToWarriorFactor) / 2f, 2);

            // If shooting, chargeAmount depends on distance to goal
        }

        // Monster should not use abilities
        ability1Chance = 0.0f;
        ability2Chance = 0.0f;
        ability3Chance = 0.0f;

        // Stop roaming if its happening
        StopCoroutines();

        // Debug.Log("MonsterHasBall");
        // Debug.Log("shootChance: " + shootChance);

    }

    private void BallNotPossessed()
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

        // Set Wall chance and behavior
        // If ball is going toward own goal at a high enough speed
        if (BallGoingTowardOwnGoal())
        {
            ability1Chance = 0.4f;
            wallMode = WallMode.BlockGoal;
        } else {
            ability1Chance = 0.3f;
            wallMode = WallMode.BlockWarrior; // Try to block warrior from getting to ball
        }

        // Set Spherical attack chance
        ability2Chance = 0.0f;

        // Set Dash chance and behavior
        ability3Chance = 0.1f;
        dashMode = DashMode.Ball; // Dash at ball

        // Debug.Log("BallNotPossessed");
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
                UnityEngine.Random.Range(-wiggleOffset, wiggleOffset),  // Random x offset
                0,                          // Keep y as zero for ground-based movement
                UnityEngine.Random.Range(-wiggleOffset, wiggleOffset)   // Random z offset
            );
        }

        // Calculate the base direction toward the goal
        Vector3 toGoal = (goalPosition - transform.position).normalized;
        Vector3 toGoalIgnoreY = new Vector3 ( toGoal.x, transform.position.y, toGoal.z);

        // Apply the current random offset to the direction vector
        mc.movementDirection = (toGoalIgnoreY + currentRandomOffset).normalized;

        // Set the minotaur’s movement based on the calculated `movementDirection`
        // rb.velocity = mc.movementDirection * mc.monsterSpeed;

        // Update walking animation if applicable
        mc.ANIM.SetBool("isWalking", rb.velocity != Vector3.zero);
    }

    private float GetDistanceToNearestWarrior()
    {
        float distToNearestWarrior = maxProximityRange;
        foreach (GameObject warrior in warriors)
        {
            float distanceToWarrior = Vector3.Distance(transform.position, warrior.transform.position);
            if (distanceToWarrior < distToNearestWarrior)
            {
                distToNearestWarrior = distanceToWarrior;
            }
        }
        return distToNearestWarrior;
    }

    private WarriorController GetNearestWarrior(Vector3 pos)
    {
        GameObject nearestWarrior = null;
        float distToNearestWarrior = maxProximityRange;

        foreach (GameObject warrior in warriors)
        {
            float distanceToWarrior = Vector3.Distance(pos, warrior.transform.position);
            if (distanceToWarrior < distToNearestWarrior)
            {
                nearestWarrior = warrior;
                distToNearestWarrior = distanceToWarrior;
            }
        }

        return nearestWarrior.GetComponent<WarriorController>();
    }

    // ROAM METHODS
    IEnumerator Roam()
    {
        while (true)
        {
            Debug.Log("Monster roaming");

            // Determine a random position within the left half of the field
            float randomX = UnityEngine.Random.Range(leftBoundary, midFieldPoint);
            float randomZ = UnityEngine.Random.Range(-fieldDepth, fieldDepth);
            Vector3 randomTargetPosition = new Vector3(randomX, transform.position.y, randomZ);

            // If there is a ball owner, bias the random target position toward it
            if (mc.BP.ballOwner != null)
            {
                Vector3 ballOwnerPosition = mc.BP.ballOwner.transform.position;

                // Calculate the bias factor (range: 0 = no bias, 1 = full bias)
                float biasFactor = 0.35f; // Adjust this value to control how strongly it biases toward the ball owner
                randomTargetPosition = Vector3.Lerp(randomTargetPosition, new Vector3(ballOwnerPosition.x, transform.position.y, ballOwnerPosition.z), biasFactor);
                randomTargetPosition = new Vector3(randomTargetPosition.x, transform.position.y, randomTargetPosition.z); // Ignore y
            }

            // Move toward the random target position
            while (Vector3.Distance(transform.position, randomTargetPosition) > stoppingDistance)
            {
                if (mc.isStunned) yield break;

                // Calculate direction and move toward the target position
                Vector3 directionToTarget = (randomTargetPosition - transform.position).normalized;
                Vector3 directionToTargetIgnoreY = new Vector3(directionToTarget.x, transform.position.y, directionToTarget.z);
                mc.movementDirection = directionToTargetIgnoreY;
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
            Debug.Log("Start roaming");
            roamCoroutine = StartCoroutine(Roam());
        }
    }

    private void StopRoaming()
    {
        if (roamCoroutine != null)
        {
            Debug.Log("Stop roaming");
            StopCoroutine(roamCoroutine);
            roamCoroutine = null;
        }
    }

    private void StartPursuing()
    {
        if (isPerformingAbility) return;
        if (pursueCoroutine == null)
        {
            Debug.Log("Start pursuing");
            pursueCoroutine = StartCoroutine(PursuePlayer());
        }
    }

    private void StopPursuing()
    {
        if (pursueCoroutine != null)
        {
            Debug.Log("Stop pursuing");
            StopCoroutine(pursueCoroutine);
            pursueCoroutine = null;
        }
    }

    private void StartDefendGoal()
    {
        // if (isPerformingAbility) return;
        if (defendGoalCoroutine == null)
        {
            Debug.Log("Start Defend Goal");
            defendGoalCoroutine = StartCoroutine(DefendGoal());
        }
    }

    private void StopCoroutines()
    {
        StopPursuing();
        StopRoaming();
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
        WarriorController nearestWarrior = GetNearestWarrior(transform.position);
        if (nearestWarrior == null)
        {
            Debug.Log("No warrior close enough to attack");
            isPerformingAbility = false;  // not performing ability so reset bool
            yield break;
        }

        StopCoroutines();
        while (isPerformingAbility)
        {
            // If targeted warrior died during ability charge, go to next warrior
            if (nearestWarrior == null)
            {
                Debug.Log("nearestWarrior: " + nearestWarrior);
                nearestWarrior = GetNearestWarrior(transform.position);
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

        StopCoroutines();

        while (isPerformingAbility)
        {
            // If ballOwner died, just retarget to nearest warrior
            if (ballController == null) ballController = GetNearestWarrior(transform.position).gameObject;

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
            Debug.Log("Pursuing player");
            yield return new WaitForSeconds(pursueDelay);

            // Ensure the ball owner is valid before pursuing
            if (mc.BP.ballOwner != null)
            {
                Debug.Log("Pursuing player, ball owner is valid");
                Vector3 targetPosition = mc.BP.ballOwner.transform.position;
                targetPosition = new Vector3(targetPosition.x, transform.position.y, targetPosition.z); // Ignore y
                float distanceToPlayer = Vector3.Distance(targetPosition, transform.position);

                // Check if the monster is too close; stop if within minimum distance
                if (distanceToPlayer > minPursueDistance)
                {
                    // Calculate target direction
                    Vector3 targetDirection = (targetPosition - transform.position).normalized;
                    Vector3 targetDirectionIgnoreY = new Vector3(targetDirection.x, transform.position.y, targetDirection.z);

                    // Smoothly update the movement direction using linear interpolation
                    mc.movementDirection = targetDirectionIgnoreY;
                        //Vector3.Lerp(mc.movementDirection, targetDirection, Time.deltaTime * pursuitSmoothingFactor);
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
        if (mc.BP == null) return new Vector3(transform.position.x, transform.position.y, transform.position.z);
        if (mc.BP.ballOwner == null) return new Vector3(transform.position.x, transform.position.y, transform.position.z);
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
        // Ensure wall is in correct slot
        if (!(mc.abilities[0] is AbilityMinotaurWall))
        {
            isPerformingAbility = false;
            return;
        }

        AbilityMinotaurWall amw =  (AbilityMinotaurWall)mc.abilities[0];
        GameObject ball = mc.BP.gameObject;

        // Only wall if in range
        if (Vector3.Distance(ball.transform.position, transform.position) > amw.wallSpawnDistance + 1f) // Allow walling a bit outside of ball range
        {
            isPerformingAbility = false;
            return;
        }

        // Look toward ball
        mc.movementDirection = (ball.transform.position - transform.position).normalized;

        // Summon wall
        mc.abilities[0].Activate();
    }

    private void WallBlockWarrior()
    {
        // Ensure wall is in correct slot
        if (!(mc.abilities[0] is AbilityMinotaurWall))
        {
            isPerformingAbility = false;
            return;
        }

        if (mc.BP == null)
        {
            isPerformingAbility |= false;
            return;
        }

        GameObject ball = mc.BP.gameObject;
        if (ball == null)
        {
            isPerformingAbility = false;
            return;
        }

        AbilityMinotaurWall amw = (AbilityMinotaurWall)mc.abilities[0];

        List<GameObject> warriors = GetWarriorsToBlock();
        if (warriors.Count < 1)
        {
            Debug.Log("No warriors to block");
            isPerformingAbility = false;
            return;
        }

        foreach (GameObject warrior in warriors)
        {
            Vector3 toWarrior = (warrior.transform.position - transform.position).normalized;
            Vector3 wallPos = transform.position + (toWarrior * amw.wallSpawnDistance);

            Vector3 warriorToWall = (wallPos - warrior.transform.position).normalized;
            Vector3 ballToWall = (wallPos - ball.transform.position).normalized;

            // Check if wall would be between warrior and ball
            if (Vector3.Dot(warriorToWall, ballToWall) < -0.5f)
            {
                Debug.Log("Blocking warrior");
                // Wall would be between warrior and ball, thus blocking warrior

                // Look toward warrior
                mc.movementDirection = toWarrior;

                // Summon wall
                mc.abilities[0].Activate();

                // No need to continue, ability was activated
                return;
            }
            
        }
        
        Debug.Log("Wall would not have blocked - No activation");
        isPerformingAbility = false;

    }

    private void WallBlockGoal()
    {
        // Ensure wall is in correct slot
        if (!(mc.abilities[0] is AbilityMinotaurWall))
        {
            isPerformingAbility = false;
            return;
        }

        AbilityMinotaurWall amw = (AbilityMinotaurWall)mc.abilities[0];

        // Look toward own goal
        mc.movementDirection = (monsterGoal.transform.position - transform.position).normalized;

        // Summon wall
        mc.abilities[0].Activate();
    }

    private void Wall(WallMode wallMode)
    {
        Debug.Log("Wall");
        if (wallMode == WallMode.Offensive)
        {
            WallOffensive();
        } else if (wallMode == WallMode.BlockWarrior)
        {
            WallBlockWarrior();
        } else if (wallMode == WallMode.BlockGoal)
        {
            WallBlockGoal();
        }
        isPerformingAbility = false;
    }

    private bool BallGoingTowardOwnGoal()
    {
        GameObject ball = mc.BP.gameObject;
        if (ball == null) return false;

        Rigidbody ballRB = ball.GetComponent<Rigidbody>();
        Vector3 ballToGoal = (monsterGoal.transform.position - ball.transform.position).normalized;

        // If ball velocity is higher enough, is within certain distance to own goal, and velocity is toward own goal, return true
        if (ballRB.velocity.magnitude > 2f && Vector3.Distance(ball.transform.position, monsterGoal.transform.position) < 8f
            && Vector3.Dot(ballRB.velocity.normalized, ballToGoal) > 0.7f)
        {
            // Debug.Log("Ball is going toward monster goal");
            return true;
        }

        return false;
    }

    private List<GameObject> GetWarriorsToBlock()
    {
        if (mc.BP.gameObject == null) return null;

        List<GameObject> warriorsToBlock = new List<GameObject>();

        foreach (GameObject warrior in warriors) {

            Vector3 minoToBall = (mc.BP.gameObject.transform.position - transform.position).normalized;
            Vector3 warriorToBall = (mc.BP.gameObject.transform.position - warrior.transform.position).normalized;

            // If warrior is on same side of ball, add to list
            if (Vector3.Dot(minoToBall, warriorToBall) > 0.5f)
            {
                Debug.Log(warrior.name + " is on the same side of the ball");
                warriorsToBlock.Add(warrior.gameObject);
            }

            // If warrior is on opposite side of ball, add to list
            if (Vector3.Dot(minoToBall, warriorToBall) < -0.5f)
            {
                warriorsToBlock.Add(warrior.gameObject);
                Debug.Log(warrior.name + " is on the opposite side of the ball");
            }
        }

        return warriorsToBlock;

    }

    // DASH METHODS
    private void DashHelper()
    {
        // Debug.Log("Dash Helper");

        // Make sure first ability is an AbilityChargable
        if (!(mc.abilities[2] is AbilityBullrush)) return;

        AbilityBullrush abr = (AbilityBullrush)mc.abilities[2];

        if (!abr.GetIsAutoCharging()) return;

        if (abr.GetIsAutoCharging())
        {
            abr.ChargeUp();
            // abr.SetIsAutoCharging(true);
        }
        else if (!abr.GetIsAutoCharging())
        {
            abr.ChargeDown();
        }
    }

    IEnumerator DashBallOwner()
    {
        if (mc.BP == null)
        {
            Debug.Log("BP is null");
            isPerformingAbility = false;
            yield break;
        }

        if (mc.BP.ballOwner == null) 
        {
            Debug.Log("ballOwner is null");
            isPerformingAbility = false;
            yield break;
        }

        while (isPerformingAbility)
        {
            // Look at ball owner
            if (mc.BP != null && mc.BP.ballOwner != null)
            {
                Vector3 toBallOwner = (mc.BP.ballOwner.transform.position - transform.position).normalized;
                toBallOwner = new Vector3(toBallOwner.x, transform.position.y, toBallOwner.z);
                mc.movementDirection = toBallOwner;
            }

            // Dash
            DashHelper();

            yield return null;
        }

        Debug.Log("DashBallOwner done");
    }

    IEnumerator DashNearest()
    {
        if (mc.BP == null)
        {
            Debug.Log("BP is null");
            isPerformingAbility = false;
            yield break;
        }

        if (mc.BP.ballOwner == null)
        {
            Debug.Log("ballOwner is null");
            isPerformingAbility = false;
            yield break;
        }

        WarriorController nearestWarrior = GetNearestWarrior(transform.position);
        if (nearestWarrior == null) yield break;

        while (isPerformingAbility)
        {
            // Look at nearest warrior
            if (nearestWarrior != null)
            {
                Vector3 toNearestWarrior = (nearestWarrior.transform.position - transform.position).normalized;
                toNearestWarrior = new Vector3(toNearestWarrior.x, transform.position.y, toNearestWarrior.z);
                mc.movementDirection = toNearestWarrior;
            }

            // Dash
            DashHelper();

            yield return null;
        }

        Debug.Log("DashNearest done");
    }
    IEnumerator DashBall()
    {
        if (mc.BP == null)
        {
            Debug.Log("BP is null");
            isPerformingAbility = false;
            yield break;
        }

        if (mc.BP.gameObject == null)
        {
            Debug.Log("ball is null");
            isPerformingAbility = false;
            yield break;
        }

        while (isPerformingAbility)
        {
            // Look at ball owner
            if (mc.BP != null && mc.BP.gameObject != null)
            {
                Vector3 toBall = (mc.BP.gameObject.transform.position - transform.position).normalized;
                toBall = new Vector3(toBall.x, transform.position.y, toBall.z);
                mc.movementDirection = toBall;
            }

            // Dash
            DashHelper();

            yield return null;
        }

        Debug.Log("DashBall done");
    }
    private void Dash(DashMode dashMode)
    {
        Debug.Log("Dash");

        // Make sure first ability is an AbilityChargable
        if (!(mc.abilities[2] is AbilityBullrush))
        {
            isPerformingAbility = false;
            return;
        }

        AbilityBullrush abr = (AbilityBullrush)mc.abilities[2];
        
        if (abr.GetTimer() < abr.GetCooldown())
        {
            Debug.Log("Dash not off cooldown");
            isPerformingAbility = false;
            return;
        }

        abr.SetIsAutoCharging(true);
        if (dashMode == DashMode.BallOwner)
        {
            StartCoroutine(DashBallOwner());
        } else if (dashMode == DashMode.Nearest)
        {
            StartCoroutine(DashNearest());
        }
        else if (dashMode == DashMode.Ball)
        {
            StartCoroutine(DashBall());
        }
        else
        {
            Debug.Log("Error in Dash");
        }

    }

    private void ResetAbilities()
    {
        // Debug.Log("Reset Abilities");
        if (isPerformingAbility) isPerformingAbility = false;
        
        if (mc.abilities[2] is AbilityBullrush)
        {
            AbilityBullrush abr = (AbilityBullrush)mc.abilities[2];
            if (abr.GetIsCharging() || abr.GetIsAutoCharging())
            {
                abr.SetIsAutoCharging(false);
                abr.ChargeDown();
                abr.SetInputBufferTimer(0);
                abr.SetIsCharging(false);
                //abr.SetTimer(0);
            }
        }

        if (mc.abilities[1] is AbilitySphericalAttack)
        {
            AbilitySphericalAttack asa = (AbilitySphericalAttack)mc.abilities[1];

            if (asa.GetIsAutoCharging() || asa.GetIsCharging())
            {
                asa.SetIsAutoCharging(false);
                asa.SetIsCharging(false);
                asa.SetInputBufferTimer(0);
                asa.ChargeDown();
                //asa.SetTimer(0);
            }
        }
    }

    IEnumerator DefendGoal()
    {
        yield return new WaitForSeconds(defendGoalDelay);
        mc.movementDirection = (GetDefendGoalPosition() - transform.position).normalized; // Stand in between goal and ball owner
        defendGoalCoroutine = null;
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

        if (mc.abilities[1] != null && (mc.abilities[1].attackVisualizer.transform.position.y > 0
            || mc.abilities[1].attackVisualizer.transform.position.x > 0))
        {
            // Debug.Log("Attack visual change position!!!!");
        }

        if (isPerformingAbility)
        {
            // Debug.Log("IsPerformingAbility: " + isPerformingAbility);
        }

        //Debug.Log("ability1Chance: " + ability1Chance);
        //Debug.Log("ability2Chance: " + ability2Chance);
        //Debug.Log("ability3Chance: " + ability3Chance);
    }
    
    /*
     * TODO
     * 
     * Make ability chances based on math rather than set values
     * 
     * Fix bug where monster can't pickup ball if he is on top of ball when he kills warrior with ball (Doesn't cue OnTriggerEnter)
     * 
     */

}
