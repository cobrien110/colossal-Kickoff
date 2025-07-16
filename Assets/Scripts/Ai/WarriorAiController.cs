using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/*
 * Controls humans that have no player assigned to them. Uses the functionality found in
 * WariorController class, but simply assigns appropriate input into methods in place of a real person's manual input.
 */
public class WarriorAiController : MonoBehaviour
{
    WarriorController wc;
    GameObject monsterGoal;
    GameObject warriorGoal;
    GameObject monster;

    [Header("AI Stats/Behavior")]
    [SerializeField]
    private float aiKickSpeed = 300f;
    [SerializeField]
    private float aiShootRange = 4f;
    [SerializeField]
    private float aiPassRange = 6f;
    [SerializeField]
    private float aiPassRangeMin = 1f;
    [SerializeField]
    private float passChance = 0.1f; // 10% chance to pass
    [SerializeField]
    private float stoppingDistanceFromGoal = 5f;
    [SerializeField]
    private float waitInPlaceTime = 1.5f;
    [SerializeField]
    private float randomZRange = 2f;
    [SerializeField]
    private float distanceToTravelMultiplierFloor = 0.4f;
    [SerializeField]
    private float distanceToTravelMultiplierCeiling = 0.7f;
    [SerializeField]
    private float slideRange = 3f;
    [SerializeField]
    private float slideRangeMin = 0.35f;
    [SerializeField]
    private float dodgeChance = 0.3f;
    [SerializeField] private float kickCooldown = 0.75f;
    private static float kickTimer = 0f;
    [SerializeField] private float actionDelay = 0.25f;
    [SerializeField] private float anticipationSeconds = 0.5f;
    [SerializeField] private float flockWeight = 0.4f;
    bool reachedLocation = false;


    private Coroutine aiCoroutine;
    private bool canUpdateAI = true; // Controls AI decision-making

    private bool checkToPass = false;
    private bool roamForward = true;
    private Coroutine roamCoroutine;

    [SerializeField]
    private float checkForPassFrequency = 0.5f; // How many seconds between checks

    WarriorController[] teammates = new WarriorController[2];

    private Rigidbody rb;
    [SerializeField] private GameplayManager GM = null;
    private AudioPlayer audioPlayer;

    // Get all WarriorController components
    [SerializeField]    
    WarriorController[] warriors;

    private bool disableBehavior = false;

    // Backing up
    private float isBackingUpTimer = 0;

    private void Awake()
    {
        wc = GetComponent<WarriorController>();
        rb = GetComponent<Rigidbody>();
        GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
        monsterGoal = GameObject.FindWithTag("MonsterGoal");
        warriorGoal = GameObject.FindWithTag("WarriorGoal");
        monster = FindObjectOfType<MonsterController>().gameObject;
        audioPlayer = GetComponent<AudioPlayer>();
        //Debug.Log("wc: " + wc);
    }

    // Start is called before the first frame update
    void Start()
    {
        aiCoroutine = StartCoroutine(AiBehaviorCoroutine());
        StartCoroutine(CheckForPass());
        //warriors = FindObjectsOfType<WarriorController>();
        int index = 0;
        // Debug.Log("teammates: " + FindObjectsOfType<WarriorController>());
        foreach (WarriorController warrior in FindObjectsOfType<WarriorController>())
        {
            if (warrior.gameObject != gameObject) // Ensure it's not the same object
            {
                teammates[index] = warrior;
                index++;
            }
        }

        // Debug.Log("Teammate 1: " + teammates[0].gameObject.name);
        // Debug.Log("Teammate 2: " + teammates[1].gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        if (canUpdateAI)
        {
            AiBehavior();
            canUpdateAI = false; // Prevent AI from making instant consecutive decisions
        }
        PerformMovement();

        if (kickTimer > 0) kickTimer -= Time.deltaTime;

        if (GM.GetPodiumSequenceStarted())
        {
            if (roamCoroutine == null) StopMovement(); // Stop movement initially once podium sequence start
            StartRoaming();
        }
    }

    private IEnumerator AiBehaviorCoroutine()
    {
        while (true)
        {
            canUpdateAI = true; // Allow AI to make a new decision
            yield return new WaitForSeconds(actionDelay); // Delay next decision update
        }
    }

    private void PerformMovement()
    {
        if (wc.isStunned)
        {
            StopMovement();
            return;
        }

        if (!wc.IsSliding()) rb.velocity = GM.isPlaying ? wc.movementDirection * wc.warriorSpeed : Vector3.zero;

        if (rb.velocity != Vector3.zero && !wc.IsSliding())
        {
            Quaternion newRotation = Quaternion.LookRotation(wc.movementDirection.normalized, Vector3.up);
            transform.rotation = newRotation;
        }

        if (rb.velocity.magnitude < 1)
        {
            wc.movementDirection = Vector3.zero;
        }

        if (wc.movementDirection == Vector3.zero) wc.ANIM.SetBool("isWalking", false);
    }

    public void AiBehavior()
    {
        if (wc == null || wc.BP == null) return; // Safety check

        // If goal was scored, stop movement and behavior
        if (wc != null && wc.BP != null && !wc.BP.isInteractable
            || disableBehavior) 
        {
            StopMovement();
            return;
        }

        if (wc.isStunned) return;
        if (wc.IsSliding()) return;


        // If no one has the ball
        if (wc.BP.ballOwner == null)
        {
            // Stop roaming if it's happening
            StopRoaming();

            //Debug.Log("Unpossessed");
            // Move toward the ball
            Vector2 toBall = new Vector2(
                wc.BP.gameObject.transform.position.x - transform.position.x,
                wc.BP.gameObject.transform.position.z - transform.position.z).normalized;

            BaseMovement(toBall, true);
        }
        // If this warrior has the ball
        else if (wc.BP.ballOwner == gameObject)
        {
            //Debug.Log("Has Ball");
            // Stop roaming if it's happening
            StopRoaming();

            //Debug.Log("HasBall");
            HasBall();
        }
        // If a teammate has the ball (start roaming)
        else if (wc.BP.ballOwner.CompareTag("Warrior"))
        {
            //Debug.Log("Roaming");
            StartRoaming();
        }
        // If a monster has the ball (do something else, e.g., tackle)
        else if (wc.BP.ballOwner.CompareTag("Monster")
            || wc.BP.ballOwner.CompareTag("Mummy"))
        {
            //Debug.Log("Monster has ball");
            // Stop roaming if it's happening
            StopRoaming();

            // Run to monster and slide tackle
            if (Vector3.Distance(transform.position, wc.BP.ballOwner.transform.position) > slideRange)
            {
                isBackingUpTimer = 0;

                Vector3 anticipatedBallPos = wc.BP.GetAnticipatedPosition(anticipationSeconds);
                // Chase down monster
                Vector2 toBall = new Vector2(
                    anticipatedBallPos.x - transform.position.x,
                    anticipatedBallPos.z - transform.position.z).normalized;

                BaseMovement(toBall, true);
            } else if (Vector3.Distance(transform.position, wc.BP.ballOwner.transform.position) > slideRangeMin // Ensure not too close too slide
                && Vector3.Dot(wc.movementDirection, (wc.BP.transform.position - transform.position).normalized) > 0.75f) // Ensure warrior is actually moving toward ball
            {
                isBackingUpTimer = 0;

                // Close enough to slide, but not too close
                AnticipateSlide();
            } else // if (Vector3.Distance(transform.position, wc.BP.ballOwner.transform.position) <= slideRangeMin)
            {
                if (isBackingUpTimer >= 1f)
                {
                    Debug.Log("Has been backing up for too long. Slide");

                    AnticipateSlide();
                    isBackingUpTimer = 0;
                    return;
                }

                isBackingUpTimer += actionDelay == 0 ? Time.deltaTime : actionDelay;
                Debug.Log(name + " too close to slide, backing up");

                // Too close, back up toward own goal
                Vector2 toOwnGoal = new Vector2(warriorGoal.transform.position.x - transform.position.x,
                    warriorGoal.transform.position.z - transform.position.z).normalized;
                BaseMovement(toOwnGoal);
            }

        }

    }

    private void AnticipateSlide()
    {
        Vector3 toBall = (wc.BP.GetAnticipatedPosition(0.5f) - transform.position).normalized;
        wc.movementDirection = toBall;
        wc.rb.velocity = toBall * wc.warriorSpeed;
        wc.Sliding();
    }

    void BaseMovement(Vector2 targetPos)
    {
        BaseMovement(targetPos, false);
    }

    void BaseMovement(Vector2 targetPos, bool useBoidMovement)
    {
        if (wc.isSliding) return;

        if (useBoidMovement)
        {
            Vector2 flockingOffset = GetFlockingOffset();
            targetPos = (targetPos + flockingOffset * flockWeight).normalized;
        }

        if (targetPos != Vector2.zero)
        {
            //usingKeyboard = true;
            wc.movementDirection = new Vector3(targetPos.x, 0, targetPos.y).normalized;
            wc.aimingDirection = wc.movementDirection;
            //Debug.Log("MovementDirection: " +  wc.movementDirection);
        }
        

        //rb.velocity = GM.isPlaying ? wc.movementDirection * wc.warriorSpeed : Vector3.zero;
        //rb.velocity = isCharging ? rb.velocity * chargeMoveSpeedMult : rb.velocity;
        //if (rb.velocity != Vector3.zero)
        //{
        //    Quaternion newRotation = Quaternion.LookRotation(wc.movementDirection.normalized, Vector3.up);
        //    transform.rotation = newRotation;
        //}

        if (wc.movementDirection != Vector3.zero && GM.isPlaying && !wc.GetIsDead())
        {
            wc.ANIM.SetBool("isWalking", true);
        }
        else
        {
            wc.ANIM.SetBool("isWalking", false);
        }

    }

    // The pass and shoot method for Ai Warriors
    void Kick()
    {
        if (wc.isSliding) return;
        if (kickTimer > 0) return;

        kickTimer = kickCooldown;

        if (wc.BP.ballOwner == gameObject)
        {
            Debug.Log("Kick!");
            if (wc != null) wc.SetKickHappened(true);

            // Prevent ball from getting kicked "through" walls
            if (wc != null && wc.BP != null && wc.IsWallBetweenBallAndPlayer())
            {
                Debug.Log("Correcting ball position before kick");
                wc.BP.gameObject.transform.position =
                    new Vector3(transform.position.x, wc.BP.gameObject.transform.position.y, transform.position.z); // Ignore Y axis
            }

            // Debug.Log("ballOwner set to null");
            wc.BP.ballOwner = null;
            wc.BP.previousKicker = gameObject;
            wc.BP.lastKicker = gameObject;
            Rigidbody ballRB = wc.BP.GetComponent<Rigidbody>();
            // Debug.Log("Ball speed before kick: " + ballRB.velocity.magnitude);
            ballRB.AddForce(transform.forward * aiKickSpeed);
            // Debug.Log("Ball speed after kick: " + ballRB.velocity.magnitude);
            audioPlayer.PlaySoundRandomPitch(audioPlayer.Find("pass"));
            wc.ANIM.Play("WarriorKick");
            if (GM.passIndicator)
            {
                //wc.BP.SetBallColor(Color.blue);
            }
        }
    }

    void HasBall()
    {
        // Warrior should now be dribbling using Dribbling method in WarriorController
        //Debug.Log("Dribbling");
        if (ShouldPass())
        {
            // Pass

            // Determine target
            WarriorController target = GetWarriorPassTarget();

            if (target != null) Pass(target);

            //if (distanceToWarrior1 < distanceToWarrior2)
            //{
            //    Pass(teammates[0]);
            //} else
            //{
            //    Pass(teammates[1]);
            //}
            return;
        }

        // Move toward goal until close enough
        float distanceToMonsterGoal = new Vector2(monsterGoal.transform.position.x - transform.position.x,
            monsterGoal.transform.position.z - transform.position.z).magnitude;
        if (distanceToMonsterGoal > aiShootRange)
        {
            //Debug.Log("Moving to goal");
            Vector2 toMonsterGoal = new Vector2(monsterGoal.transform.position.x - transform.position.x,
            monsterGoal.transform.position.z - transform.position.z).normalized;

            //Vector2 flockingOffset = GetFlockingOffset();
            //Vector2 blendedDirection = (toMonsterGoal + flockingOffset * flockWeight).normalized;

            //BaseMovement(blendedDirection);

            BaseMovement(toMonsterGoal);
        } // When close enough, shoot 
        else
        {
            Shoot();
            // Kick();
        }

    }

    IEnumerator CheckForPass()
    {
        while (true)
        {
            // Debug.Log("CheckForPass courintine called");
            checkToPass = true;
            yield return new WaitForSeconds(checkForPassFrequency);
        }
    }

    private bool ShouldPass()
    {
        if (!checkToPass) return false; // Shouldn't even consider passing
        
        // Should consider passing, reset bool
        checkToPass = false;

        // Only continue if passChance check succeeds
        if (Random.value > passChance) return false;

        // Check if nearest teammate is close enough for a pass

        // Debug.Log(warrior.name + ": " + warrior.transform.position);
        //float distanceToWarrior1 = 100f;
        //float distanceToWarrior2 = 100f;
        //if (teammates[0] != null) { distanceToWarrior1 = Vector3.Distance(teammates[0].transform.position, transform.position); }
        //if (teammates[1] != null) { distanceToWarrior2 = Vector3.Distance(teammates[1].transform.position, transform.position); }

        if (ValidWarriorPassTargets().Count > 0)
        {
            return true;
        }
        
        return false;
    }

    private HashSet<WarriorController> ValidWarriorPassTargets()
    {
        HashSet<WarriorController> validTargets = new HashSet<WarriorController>();

        // Use a properly constructed LayerMask (assumes you have a "Warrior" layer)
        int layerMask = LayerMask.GetMask("Warrior", "Monster");

        foreach (var target in teammates)
        {
            Vector3 dirToTarget = (target.transform.position - transform.position).normalized;

            if (Physics.Raycast(transform.position, dirToTarget, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                if (IsInPassRange(target) && hit.collider.gameObject == target.gameObject)
                {
                    validTargets.Add(target);
                }
            }
        }

        return validTargets;
    }


    private WarriorController GetWarriorPassTarget()
    {
        WarriorController target = null;
        float maxDistInRage = 0f;
        HashSet<WarriorController> potentialTargets = ValidWarriorPassTargets();
        foreach (WarriorController wc in potentialTargets)
        {
            float distToWarrior = Vector3.Distance(wc.transform.position, transform.position);
            if (distToWarrior > maxDistInRage)
            {
                maxDistInRage = distToWarrior;
                target = wc;
            }
        }
        return target;
    }

    private bool IsInPassRange(WarriorController wc)
    {
        float dist = Vector3.Distance(transform.position, wc.transform.position);
        return dist > aiPassRangeMin && dist < aiPassRange;
    }

    private void Shoot()
    {
        if (wc.isSliding) return;
        if (kickTimer > 0) return;

        Debug.Log("Shoot");

        // Turn to monster goal

        // Calculate the direction from this GameObject to the target
        Vector3 directionToTarget = monsterGoal.transform.position - transform.position;
        Vector3 directionToTargetIgnoreY = new Vector3(directionToTarget.x, transform.position.y, directionToTarget.z);

        // Ensure the direction vector is not zero (to avoid errors)
        if (directionToTargetIgnoreY != Vector3.zero)
        {
            // Calculate the rotation needed to face the target
            Quaternion targetRotation = Quaternion.LookRotation(directionToTargetIgnoreY);

            // Apply the rotation to this GameObject
            transform.rotation = targetRotation;
        }

        // Kick in their direction
        Kick();
    }

    private void Pass(WarriorController target)
    {
        if (wc.isSliding) return;
        if (kickTimer > 0) return;

        // kickTimer = kickCooldown;

        Debug.Log("Pass");

        // Estimate where the target will be
        Vector3 predictedPosition = PredictFuturePosition(target);

        // Turn to predicted position
        Vector3 directionToTarget = predictedPosition - transform.position;
        Vector3 directionToTargetIgnoreY = new Vector3(directionToTarget.x, transform.position.y, directionToTarget.z);

        // Ensure the direction vector is not zero (to avoid errors)
        if (directionToTargetIgnoreY != Vector3.zero)
        {
            // Calculate the rotation needed to face the target
            Quaternion targetRotation = Quaternion.LookRotation(directionToTargetIgnoreY);

            // Apply the rotation to this GameObject
            transform.rotation = targetRotation;
        }

        // Kick in their direction
        Kick();
    }

    // Predicts where the target will be after a short duration
    private Vector3 PredictFuturePosition(WarriorController target)
    {
        float leadTime = 0.5f; // Adjust this value for more or less leading
        Rigidbody targetRb = target.GetComponent<Rigidbody>();

        if (targetRb != null)
        {
            return target.transform.position + targetRb.velocity * leadTime;
        }

        return target.transform.position; // Default to current position if no Rigidbody
    }

    // Used to pass to a player who is calling for a pass
    public void CallForPassing(WarriorController target)
    {
        disableBehavior = true;
        Pass(target);
        StartCoroutine(EnableBehaviorDelayed());
    }

    IEnumerator Roam()
    {
        reachedLocation = false;
        while (true)
        {
            
            // Determine the goal based on isMovingTowardsGoal1
            Vector3 targetGoalPosition = roamForward ? monsterGoal.transform.position : warriorGoal.transform.position;

            float goalPosOffsetX = roamForward ? 2f : -2f;
            targetGoalPosition = new Vector3(targetGoalPosition.x + goalPosOffsetX, 0, targetGoalPosition.z);

            // Determine relative z position of this warrior to warrior with ball
            float zDiff = 0;
            if (wc.BP != null && wc.BP.ballOwner != null) zDiff = transform.position.z - wc.BP.ballOwner.transform.position.z;

            float randomZOffset = 0;

            // Add random z-axis offset (away from ballOwner for spacing) to make the movement less linear
            if (zDiff > 0)
            {
                randomZOffset = Random.Range(0, randomZRange); // Random Z offset in the given range
                
            } else if (zDiff < 0)
            {
                randomZOffset = Random.Range(-randomZRange, 0); // Random Z offset in the given range
            }

            Vector3 targetWithOffset = new Vector3(targetGoalPosition.x, 0, targetGoalPosition.z + randomZOffset);

            float distanceToTravelMultiplier = Random.Range(distanceToTravelMultiplierFloor, distanceToTravelMultiplierCeiling);
            if (!roamForward) distanceToTravelMultiplier /= 2f; // Don't want to travel too far backward

            //float maxDistFromCenterField = 5f;
            float distToTarget = Vector3.Distance(transform.position, targetWithOffset);

            // Debug.Log(name + " roam target: " + targetWithOffset);

            float maxRoamDistFromMidfield = 6f;
            Invoke("ResetReachedLocation", 0.5f);

            // Move towards the current goal
            while (distToTarget * distanceToTravelMultiplier > stoppingDistanceFromGoal)
            {
                if ((Vector3.Distance(transform.position, Vector3.zero) > maxRoamDistFromMidfield && !reachedLocation)  // Prevent warrior from roaming too far from midfield
                || wc.isStunned)
                {
                    break;
                }
                
                Vector3 directionToGoal = (targetWithOffset - transform.position).normalized;
                //transform.position += directionToGoal * warriorSpeed * Time.deltaTime;

                Vector2 dirToGoal = new Vector2(directionToGoal.x, directionToGoal.z);

                BaseMovement(dirToGoal, true);

                yield return null;
            }

            reachedLocation = true;

            // Wait after reaching the goal
            // Debug.Log($"Reached {(roamForward ? "Monster goal" : "Warrior goal")}, waiting...");
            StopMovement();
            yield return new WaitForSeconds(waitInPlaceTime);

            // Reverse the direction (toggle the goal)
            roamForward = !roamForward;

        }
    }

    private void ResetReachedLocation()
    {
        reachedLocation = false;
    }

    private void StopMovement()
    {
        wc.movementDirection = Vector3.zero;
        rb.velocity = Vector3.zero;
        wc.ANIM.SetBool("isWalking", false);
    }

    private void StartRoaming()
    {
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
            roamForward = true;
        }
    }

    public float GetDodgeChance()
    {
        return dodgeChance;
    }

    private IEnumerator EnableBehaviorDelayed()
    {
        yield return new WaitForSeconds(0.25f);
        disableBehavior = false;
    }

    // Boid Methods
    private Vector2 GetFlockingOffset()
    {
        Vector2 offset = Vector2.zero;
        int count = 0;

        foreach (var teammate in teammates)
        {
            if (teammate == null || teammate == this) continue;

            float dist = Vector3.Distance(transform.position, teammate.transform.position);

            if (dist < 4f) // Adjust range based on tuning
            {
                // Separation (push away from close teammates)
                Vector3 away = transform.position - teammate.transform.position;
                offset += new Vector2(away.x, away.z).normalized / Mathf.Max(dist, 0.01f);
                count++;
            }

            if (dist < 10f)
            {
                // Cohesion (move slightly toward team average position)
                Vector3 to = teammate.transform.position - transform.position;
                offset += new Vector2(to.x, to.z).normalized * 0.1f; // Weaker weight
            }
        }

        return count > 0 ? offset.normalized : Vector2.zero;
    }

}


