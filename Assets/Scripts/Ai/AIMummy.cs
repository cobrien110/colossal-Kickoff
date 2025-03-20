using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMummy : MonoBehaviour
{
    MonsterController mc;
    GameObject monsterGoal;
    GameObject warriorGoal;
    GameObject monster;
    AiMummyManager aiMummyManager;

    [Header("AI Stats/Behavior")]
    [SerializeField] private float mummySpeed = 5f;
    [SerializeField] private float aiKickSpeed = 300f;
    [SerializeField] private float aiShootRange = 4f;
    [SerializeField] private float aiPassRange = 6f;
    [SerializeField] private float passChance = 0.1f; // 10% chance to pass
    [SerializeField] private float stoppingDistanceFromGoal = 5f;
    [SerializeField] private float waitInPlaceTime = 1.5f;
    [SerializeField] private float randomZRange = 2f;
    [SerializeField] private float distanceToTravelMultiplierFloor = 0.4f;
    [SerializeField] private float slideRange = 3f;
    [SerializeField] private float slideSpeed = 5.0f;
    [SerializeField] private float slideCooldown = 1f;
    [SerializeField] private float slideDuration = 0.35f;
    [SerializeField] private float mummyLifeSpan = 12f;
    private float timer = 0f;
    private bool isSliding = false;
    private float lastSlideTime = -1f;

    private static bool kickHappened;


    [SerializeField]
    private GameObject ballPosition;

    private Vector3 movementDirection;

    private bool checkToPass = false;
    private bool roamForward = true;
    private Coroutine roaoroutine;

    [SerializeField] private float checkForPassFrequency = 0.5f; // How many seconds between checks

    //AIMummy[] teammates = new AIMummy[1];
    List<AIMummy> teammates = new List<AIMummy>();

    private Rigidbody rb;
    [SerializeField] private GameplayManager GM = null;
    public AudioPlayer audioPlayer;
    private AbilitySphinxPassive ASP;

    private bool isPursuing = false;

    // Get all WarriorController components (including subclasses)
    ////[SerializeField]
    ////WarriorController[] warriors;
    private Animator ANIM;

    // If there is no ball owner yet a warrior or monster is on top of ball, OnTriggerStay will wait this long until making that character pick up ball
    private float pickupBallCooldown = 0.25f;
    [SerializeField] private float pickupBallTimer = 0f;

    private void Awake()
    {
        mc = FindObjectOfType<MonsterController>();
        rb = GetComponent<Rigidbody>();
        GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
        monsterGoal = GameObject.FindWithTag("MonsterGoal");
        warriorGoal = GameObject.FindWithTag("WarriorGoal");
        monster = FindObjectOfType<MonsterController>().gameObject;
        audioPlayer = GetComponent<AudioPlayer>();
        audioPlayer.PlaySoundVolume(audioPlayer.Find("sphinxMummyGroan"), 0.75f);
        aiMummyManager = mc.gameObject.GetComponent<AiMummyManager>();
        ANIM = GetComponentInChildren<Animator>();
        ASP = mc.GetComponent<AbilitySphinxPassive>();

        //Debug.Log(": " + );
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CheckForPass());
        StartCoroutine(DelayedTeammateAssignment());
        
        //warriors = FindObjectsOfType<WarriorController>();
        //int index = 0;
        // Debug.Log("teammates: " + FindObjectsOfType<WarriorController>());

        /* teammates.Clear();
        Debug.Log("Mummy: " + gameObject.name);
        Debug.Log("Mummy start, FindObjectsOfType<AIMummy>(): " + FindObjectsOfType<AIMummy>().Length);
        foreach (AIMummy mummy in FindObjectsOfType<AIMummy>())
        {
            if (mummy.gameObject != gameObject) // Ensure it's not the same object
            {
                teammates.Add(mummy);
                //teammates[index] = mummy;
                //index++;
            }
        }
        */

        //foreach (AIMummy mummy in teammates) Debug.Log(mummy.gameObject.name);
        //Debug.Log("Teammate 1: " + teammates[0].gameObject.name);
        //Debug.Log("Teammate 2: " + teammates[1].gameObject.name);
    }

    public void test()
    {
        Debug.Log("test");
    }

    // Update is called once per frame
    void Update()
    {
        if (!GM.isPlaying) return;
        if (mc.BP == null) mc.BP = FindObjectOfType<BallProperties>();
        if (mc.Ball == null) mc.Ball = mc.BP.gameObject;
        if (!isPursuing) AiBehavior();
        Dribbling();

        // Despawn mummies after lifespan is reached
        if (timer < mummyLifeSpan)
        {
            timer += Time.deltaTime;
        } else if (mc.BP.ballOwner != gameObject)
        {
            Die(true);
        }
    }

    public void AiBehavior()
    {
        //if (isStunned) return;

        // If no one has the ball
        if (mc.BP.ballOwner == null)
        {
            // Stop roaming if it's happening
            StopRoaming();

            //Debug.Log("Unpossessed");
            // Move toward the ball
            Vector2 toBall = new Vector2(
                mc.BP.gameObject.transform.position.x - transform.position.x,
                mc.BP.gameObject.transform.position.z - transform.position.z).normalized;
            BaseMovement(toBall); ;
        }
        // If this mummy has the ball
        else if (mc.BP.ballOwner == gameObject)
        {
            //Debug.Log("Has Ball");
            // Stop roaming if it's happening
            StopRoaming();

            //Debug.Log("HasBall");
            HasBall();
        }
        // If a teammate has the ball (start roaming)
        else if (mc.BP.ballOwner.GetComponent<AIMummy>() != null
            || mc.BP.ballOwner.GetComponent<MonsterController>() != null)
        {
            //Debug.Log("Roaming");
            StartRoaming();
        }
        // If a warrior has the ball (do something else, e.g., tackle)
        else if (mc.BP.ballOwner.GetComponent<WarriorController>() != null)
        {
            //Debug.Log("Monster has ball");
            // Stop roaming if it's happening
            StopRoaming();

            // Run to warrior ball owner and slide tackle
            if (Vector3.Distance(transform.position, mc.BP.ballOwner.transform.position) > slideRange)
            {
                // Chase down warrior
                Vector2 toBall = new Vector2(
                    mc.BP.gameObject.transform.position.x - transform.position.x,
                    mc.BP.gameObject.transform.position.z - transform.position.z).normalized;
                BaseMovement(toBall);
            }
            else
            {
                //Debug.Log("Distance: " + Vector3.Distance(transform.position, mc.BP.ballOwner.transform.position));
                //Debug.Log("Slide Range: " + slideRange);
                // Close enough to slide
                Sliding();
            }
        }

        // Stop movement if velocity is very low
        if (rb.velocity.magnitude < 1) movementDirection = Vector3.zero;
    }

    void BaseMovement(Vector2 targetPos)
    {
        //if (.isSliding) return;

        if (targetPos != Vector2.zero)
        {
            //usingKeyboard = true;
            movementDirection = new Vector3(targetPos.x, 0, targetPos.y).normalized;
            ////aimingDirection = movementDirection;
        }


        rb.velocity = GM.isPlaying ? movementDirection * mummySpeed : Vector3.zero;
        //rb.velocity = isCharging ? rb.velocity * chargeMoveSpeedMult : rb.velocity;
        if (rb.velocity != Vector3.zero)
        {
            Quaternion newRotation = Quaternion.LookRotation(movementDirection.normalized, Vector3.up);
            transform.rotation = newRotation;
        }

        if (movementDirection != Vector3.zero && GM.isPlaying)
        {
            ANIM.SetBool("isWalking", true);
        }
        else
        {
            ANIM.SetBool("isWalking", false);
        }

    }

    private bool IsWallBetweenBallAndPlayer()
    {
        Vector3 direction = (mc.BP.transform.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, mc.BP.transform.position);

        // Define the layers to check using a LayerMask
        int layerMask = LayerMask.GetMask("InvisibleWall", "Ground");

        // Perform the raycast
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, distance, layerMask))
        {
            return true; // Something is blocking the path
        }

        return false; // No obstacles in the way
    }

    // The pass and shoot method for Ai Warriors
    void Kick()
    {
        if (mc.BP.ballOwner == gameObject)
        {
            Debug.Log("Kick!");

            // For CallForPass gravity field
            kickHappened = true;
            StartCoroutine(ResetKickHappened());

            // Prevent ball from getting kicked "through" walls
            if (mc != null && mc.BP != null && IsWallBetweenBallAndPlayer())
            {
                Debug.Log("Correcting ball position before kick");
                mc.BP.gameObject.transform.position =
                    new Vector3(transform.position.x, mc.BP.gameObject.transform.position.y, transform.position.z); // Ignore Y axis
            }

            // Debug.Log("ballOwner set to null");
            mc.BP.ballOwner = null;
            mc.BP.previousKicker = gameObject;
            //Debug.Log(transform.forward);
            mc.BP.GetComponent<Rigidbody>().AddForce(transform.forward * aiKickSpeed);
            audioPlayer.PlaySoundRandomPitch(audioPlayer.Find("pass"));
            ANIM.Play("WarriorKick");
        }
    }

    void HasBall()
    {
        // Mummy should now be dribbling using Dribbling method in this class
        //Debug.Log("Dribbling");
        if (ShouldPass())
        {
            Debug.Log("Passing");
            // Pass

            AIMummy clostestMummy = null;
            float closestDistance = 100f;
            foreach (AIMummy mummy in teammates)
            {
                if (mummy == null) continue;
                float distance = Vector3.Distance(mummy.transform.position, transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    clostestMummy = mummy;
                }
            }

            if (clostestMummy == null) return;

            Pass(clostestMummy.gameObject);
            // Determine target
            ////float distanceToWarrior1 = (teammates[0].gameObject.transform.position - transform.position).magnitude;
            ////float distanceToWarrior2 = (teammates[1].gameObject.transform.position - transform.position).magnitude;

            /*if (distanceToWarrior1 < distanceToWarrior2)
            {
                Pass(teammates[0]);
            }
            else
            {
                Pass(teammates[1]);
            }*/
        }

        // Move toward goal until close enough
        float distanceToWarriorGoal = new Vector2(warriorGoal.transform.position.x - transform.position.x,
            warriorGoal.transform.position.z - transform.position.z).magnitude;
        if (distanceToWarriorGoal > aiShootRange)
        {
            //Debug.Log("Moving to goal");
            Vector2 toWarriorGoal = new Vector2(warriorGoal.transform.position.x - transform.position.x,
            warriorGoal.transform.position.z - transform.position.z).normalized;
            BaseMovement(toWarriorGoal);
        } // When close enough, shoot 
        else
        {
            Shoot();
        }

    }

    private void Shoot()
    {
        Vector3 toGoal = (warriorGoal.transform.position - transform.position).normalized;
        movementDirection = toGoal;

        // Set rotation
        Quaternion newRotation = Quaternion.LookRotation(movementDirection.normalized, Vector3.up);
        transform.rotation = newRotation;

        Kick();
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

    bool ShouldPass()
    {
        if (!checkToPass) return false; // Shouldn't even consider passing

        // Should consider passing, reset bool
        checkToPass = false;

        // Only continue if passChance check succeeds
        if (Random.value > passChance) return false;

        // Check if nearest teammate is close enough for a pass

        float closestDistance = 100f;
        foreach (AIMummy mummy in teammates)
        {
            if (mummy == null) continue;
            float distance = Vector3.Distance(mummy.transform.position, transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
            }
        }

        // Debug.Log(warrior.name + ": " + warrior.transform.position);
        //float distanceToMummy1 = (teammates[0].gameObject.transform.position - transform.position).magnitude;
        ////float distanceToWarrior2 = (teammates[1].gameObject.transform.position - transform.position).magnitude;

        /*if (Mathf.Min(distanceToWarrior1, distanceToWarrior2) <= aiPassRange)
        {
            return true;
        }*/

        if (closestDistance <= aiPassRange) return true;

        return false;
    }

    public void Pass(GameObject target)
    {
        // Turn to teammate

        // Calculate the direction from this GameObject to the target
        Vector3 directionToTarget = target.transform.position - transform.position;

        // Ensure the direction vector is not zero (to avoid errors)
        if (directionToTarget != Vector3.zero)
        {
            // Calculate the rotation needed to face the target
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            // Apply the rotation to this GameObject
            transform.rotation = targetRotation;
        }

        // Kick in their direction
        Kick();
    }

    IEnumerator Roam()
    {
        while (true)
        {

            // Determine the goal based on isMovingTowardsGoal1
            Vector3 targetGoalPosition = roamForward ? warriorGoal.transform.position : monsterGoal.transform.position;

            // Add random z-axis offset to make the movement less linear
            float randomZOffset = Random.Range(-randomZRange, randomZRange); // Random Z offset in the given range
            //Debug.Log("Random Z offset: " + randomZOffset);
            Vector3 targetWithOffset = new Vector3(targetGoalPosition.x, targetGoalPosition.y, targetGoalPosition.z + randomZOffset);

            float distanceToTravelMultiplier = Random.Range(distanceToTravelMultiplierFloor, 1f);
            // Move towards the current goal
            while (Vector3.Distance(transform.position, targetWithOffset) * distanceToTravelMultiplier > stoppingDistanceFromGoal)
            {
                ////if (isStunned) break;
                Vector3 directionToGoal = (targetWithOffset - transform.position).normalized;
                //transform.position += directionToGoal * warriorSpeed * Time.deltaTime;
                BaseMovement(new Vector2(directionToGoal.x, directionToGoal.z));

                yield return null;
            }

            // Wait after reaching the goal
            //Debug.Log($"Reached {(roamForward ? "Monster goal" : "Warrior goal")}, waiting " + Vector3.Distance(transform.position, targetWithOffset)
            //    + " units from goal");
            yield return new WaitForSeconds(waitInPlaceTime);

            // Reverse the direction (toggle the goal)
            roamForward = !roamForward;

        }
    }

    void Dribbling()
    {
        if (mc.BP.ballOwner == gameObject)
        {
            //UM.ShowChargeBar(true);
            //UM.UpdateChargeBarText("Monster");
            mc.Ball.transform.position = ballPosition.transform.position; // new Vector3(transform.position.x, 2, transform.position.z);
        }
    }

    private void StartRoaming()
    {
        if (roaoroutine == null)
        {
            // Debug.Log(gameObject.name + " start roaming coroutine");
            roaoroutine = StartCoroutine(Roam());
        }
    }

    private void StopRoaming()
    {
        if (roaoroutine != null)
        {
            StopCoroutine(roaoroutine);
            roaoroutine = null;
        }
    }

    public void Sliding()
    {
        // Debug.Log("Mummy slide");
        //if (isStunned) return;
        // Check if enough time has passed since the last slide
        if (Time.time - lastSlideTime >= slideCooldown)
        {
            //Debug.Log("Slide off cooldown - ready to use");
            //Debug.Log("movementDirection: " + movementDirection);
            if (movementDirection != Vector3.zero && mc.BP.ballOwner != gameObject)
            {
                Debug.Log("Sliding");
                isSliding = true;
                //isInvincible = true;

                // Add force in direction of the player input for this warrior (movementDirection)
                Vector3 slideVelocity = movementDirection.normalized * slideSpeed;
                rb.AddForce(slideVelocity);
                audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find("slide"), 0.5f);

                // Set isSliding to false after a delay
                Invoke("StopSliding", slideDuration);

                // Update the last slide time
                lastSlideTime = Time.time;
                ANIM.SetBool("isSliding", true);
            }
        }
    }

    void StopSliding()
    {
        Debug.Log("No longer sliding");
        ANIM.SetBool("isSliding", false);
        isSliding = false;
        //isInvincible = false;
    }

    public bool IsSliding()
    {
        return isSliding;
    }

    public void Die(bool shouldRespawn)
    {
        // Debug.Log("Mummy despawned");

        if (shouldRespawn)
        {
            // Start the respawn coroutine from AiMummyManager
            aiMummyManager.StartCoroutine(aiMummyManager.TriggerDelayedRespawn());
        }
        if (ASP != null)
        {
            ASP.AddCounter();
        }

        // Destroy this mummy
        Destroy(gameObject);
    }

    IEnumerator DelayedTeammateAssignment()
    {
        yield return new WaitForEndOfFrame(); // Wait for a frame to ensure all old mummies are destroyed
        teammates.Clear();
        foreach (AIMummy mummy in FindObjectsOfType<AIMummy>())
        {
            if (mummy.gameObject != gameObject) // Ensure it's not the same object
            {
                teammates.Add(mummy);
            }
        }
        Debug.Log(gameObject.name + " : Teammates assigned");

        foreach (AIMummy mummy in teammates) Debug.Log(mummy.gameObject.name);
    }

    public bool IsPursuing()
    {
        return isPursuing;
    }

    public void SetIsPursuing(bool isPursuing)
    {
        this.isPursuing = isPursuing;
    }

    private void OnTriggerStay(Collider other)
    {
        BallProperties BP = other.GetComponent<BallProperties>();
        // Debug.Log("Other: " + other);

        if (BP == null)
        {
            // Debug.Log("No Ball found");
            //pickupBallTimer = pickupBallCooldown;
        }
        else if (BP.ballOwner != null)
        {
            // Debug.Log("Already have ball OR someone else has ball");
            pickupBallTimer = pickupBallCooldown;
        }
        // If ball hasn't been in warrior's colliders long enough
        else if (BP != null && pickupBallTimer > 0)
        {
            // Count down timer
            // Debug.Log("Waiting to pick up ball");
            pickupBallTimer -= Time.deltaTime;
        }
        // If has been in warrior's collider long enough
        else if (BP != null && pickupBallTimer <= 0 && BP.isInteractable)
        {
            // if you were last kicker and ball is in singleMode, return
            if (BP.isInSingleOutMode && BP.previousKicker == gameObject) return;

            // Pick up ball
            Debug.Log("Pick up ball");
            pickupBallTimer = pickupBallCooldown;
            BP.GetComponent<Rigidbody>().velocity = Vector3.zero;
            BP.ballOwner = gameObject;
            BP.SetOwner(BP.ballOwner);
        }
    }

    public static void SetKickHappened(bool kickHappened)
    {
        AIMummy.kickHappened = kickHappened;
    }

    public static bool GetKickHappened()
    {
        return AIMummy.kickHappened;
    }

    private IEnumerator ResetKickHappened()
    {
        yield return new WaitForSeconds(0.4f); // however long to allow kickHappened to be true, before reseting back to false
        kickHappened = false;
    }
}
