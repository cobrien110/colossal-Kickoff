using System.Collections;
using System.Collections.Generic;
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
    private float slideRange = 3f;
    
    private bool checkToPass = false;
    private bool roamForward = true;
    private Coroutine roamCoroutine;

    [SerializeField]
    private float checkForPassFrequency = 0.5f; // How many seconds between checks

    WarriorController[] teammates = new WarriorController[2];

    private Rigidbody rb;
    [SerializeField] private GameplayManager GM = null;
    private AudioPlayer audioPlayer;

    // Get all WarriorController components (including subclasses)
    [SerializeField]    
    WarriorController[] warriors;

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

        Debug.Log("Teammate 1: " + teammates[0].gameObject.name);
        Debug.Log("Teammate 2: " + teammates[1].gameObject.name);
    }

    public void test()
    {
        Debug.Log("test");
    }

    // Update is called once per frame
    void Update()
    {
        AiBehavior();
    }

    public void AiBehavior()
    {
        if (wc.isStunned) return;

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
            BaseMovement(toBall);;
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
        else if (wc.BP.ballOwner.GetComponent<WarriorController>() != null)
        {
            //Debug.Log("Roaming");
            StartRoaming();
        }
        // If a monster has the ball (do something else, e.g., tackle)
        else if (wc.BP.ballOwner.GetComponent<MonsterController>() != null)
        {
            //Debug.Log("Monster has ball");
            // Stop roaming if it's happening
            StopRoaming();

            // Run to monster and slide tackle
            if (Vector3.Distance(transform.position, monster.transform.position) > slideRange)
            {
                // Chase down monster
                Vector2 toBall = new Vector2(
                    wc.BP.gameObject.transform.position.x - transform.position.x,
                    wc.BP.gameObject.transform.position.z - transform.position.z).normalized;
                BaseMovement(toBall);
            } else
            {
                // Close enough to slide
                wc.Sliding();
            }
        }

        // Stop movement if velocity is very low
        if (rb.velocity.magnitude < 1) wc.movementDirection = Vector3.zero;
    }

    void BaseMovement(Vector2 targetPos)
    {
        if (wc.isSliding) return;

        if (targetPos != Vector2.zero)
        {
            //usingKeyboard = true;
            wc.movementDirection = new Vector3(targetPos.x, 0, targetPos.y).normalized;
            wc.aimingDirection = wc.movementDirection;
        }
        

        rb.velocity = GM.isPlaying ? wc.movementDirection * wc.warriorSpeed : Vector3.zero;
        //rb.velocity = isCharging ? rb.velocity * chargeMoveSpeedMult : rb.velocity;
        if (rb.velocity != Vector3.zero)
        {
            Quaternion newRotation = Quaternion.LookRotation(wc.movementDirection.normalized, Vector3.up);
            transform.rotation = newRotation;
        }

        if (wc.movementDirection != Vector3.zero && GM.isPlaying)
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
        if (wc.BP.ballOwner == gameObject)
        {
            Debug.Log("Kick!");
            wc.BP.ballOwner = null;
            Debug.Log(transform.forward);
            wc.BP.GetComponent<Rigidbody>().AddForce(transform.forward * aiKickSpeed);
            audioPlayer.PlaySoundRandomPitch(audioPlayer.Find("pass"));
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
            float distanceToWarrior1 = (teammates[0].gameObject.transform.position - transform.position).magnitude;
            float distanceToWarrior2 = (teammates[1].gameObject.transform.position - transform.position).magnitude;

            if (distanceToWarrior1 < distanceToWarrior2)
            {
                Pass(teammates[0]);
            } else
            {
                Pass(teammates[1]);
            }
        }

        // Move toward goal until close enough
        float distanceToMonsterGoal = new Vector2(monsterGoal.transform.position.x - transform.position.x,
            monsterGoal.transform.position.z - transform.position.z).magnitude;
        if (distanceToMonsterGoal > aiShootRange)
        {
            //Debug.Log("Moving to goal");
            Vector2 toMonsterGoal = new Vector2(monsterGoal.transform.position.x - transform.position.x,
            monsterGoal.transform.position.z - transform.position.z).normalized;
            BaseMovement(toMonsterGoal);
        } // When close enough, shoot 
        else
        {
            Kick();
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

    bool ShouldPass()
    {
        if (!checkToPass) return false; // Shouldn't even consider passing
        
        // Should consider passing, reset bool
        checkToPass = false;

        // Only continue if passChance check succeeds
        if (Random.value > passChance) return false;

        // Check if nearest teammate is close enough for a pass

        // Debug.Log(warrior.name + ": " + warrior.transform.position);
        float distanceToWarrior1 = (teammates[0].gameObject.transform.position - transform.position).magnitude;
        float distanceToWarrior2 = (teammates[1].gameObject.transform.position - transform.position).magnitude;

        if (Mathf.Min(distanceToWarrior1, distanceToWarrior2) <= aiPassRange)
        {
            return true;
        }
        
        return false;
    }

    void Pass(WarriorController target)
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
            Vector3 targetGoalPosition = roamForward ? monsterGoal.transform.position : warriorGoal.transform.position;

            // Add random z-axis offset to make the movement less linear
            float randomZOffset = Random.Range(-randomZRange, randomZRange); // Random Z offset in the given range
            Vector3 targetWithOffset = new Vector3(targetGoalPosition.x, targetGoalPosition.y, targetGoalPosition.z + randomZOffset);

            float distanceToTravelMultiplier = Random.Range(distanceToTravelMultiplierFloor, 1f);
            // Move towards the current goal
            while (Vector3.Distance(transform.position, targetWithOffset) * distanceToTravelMultiplier > stoppingDistanceFromGoal)
            {
                if (wc.isStunned) break;
                Vector3 directionToGoal = (targetWithOffset - transform.position).normalized;
                //transform.position += directionToGoal * warriorSpeed * Time.deltaTime;
                BaseMovement(new Vector2(directionToGoal.x, directionToGoal.z));

                yield return null;
            }

            // Wait after reaching the goal
            Debug.Log($"Reached {(roamForward ? "Monster goal" : "Warrior goal")}, waiting...");
            yield return new WaitForSeconds(waitInPlaceTime);

            // Reverse the direction (toggle the goal)
            roamForward = !roamForward;

        }
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
        }
    }
}


