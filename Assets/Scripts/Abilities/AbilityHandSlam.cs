using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityHandSlam : AbilityDelayed
{
    [Header("Ability Vars")]
    AbilityCreateHands abilityCreateHands;
    public float slamRadius = 2.0f; // Radius within which the hand slam will kill warriors
    public float ejectForce = 10f;
    public GameObject visEffect;
    public string soundName;
    public float slamLength = 2f;
    public float stunTime = 0.5f;

    private Rigidbody monsterRB;
    private GameObject chosenHand;
    private int chosenHandIndex;
    private Vector3 attackPosStart;
    private Vector3 attackPosEnd;
    private Vector3 visualizerPos;
    private AbilityGashaPassive AGP;
    [SerializeField] private GameObject orb;
    [SerializeField] private int spawnOrbQty = 3;
    [SerializeField] private float hitOrbPower = 5f;

    [SerializeField] private float slamDelay = 0.25f;
    [SerializeField] private float stunRadiusMult = 4.5f;
    private bool slamWasPressed = false; // Prevents a release of the trigger from activating a slam that was not pressed while off cooldown
    private bool canSlam = true; // Prevents a slam from happeing while a current one is still in progress

    private const float visualOffsetY = -0.3f;

    // private StatTracker ST;

    public override void Activate()
    {

        if (timer < cooldown) return;

        Debug.Log("Activate hand slam");
        //ST.UpdateMAbUsed();
        timer = 0;

        // If ability is charged, then shoot out some orbs toward the warrior side
        if (AGP.counterAmount == AGP.counterMax)
        {
            Debug.Log("Charged ability activated! Spawning orbs.");

            for (int i = 0; i < spawnOrbQty; i++)
            {
                // Randomize spawn position slightly
                Vector3 randomOffset = new Vector3(0, Random.Range(-0.2f, 0.2f), Random.Range(-0.5f, 0.5f));
                Vector3 spawnPosition = transform.position + new Vector3(1f, 0, 0) + randomOffset;

                // Instantiate the orb
                GameObject orbInstance = Instantiate(orb, spawnPosition, Quaternion.identity);

                // Apply force to the orb in the positive x-direction with some randomness
                Vector3 force = new Vector3(1f * hitOrbPower, 0, Random.Range(-0.2f, 0.2f));
                //orbInstance.GetComponent<Rigidbody>().AddForce(forceDirection * hitOrbPower, ForceMode.Impulse);
                orbInstance.GetComponent<SoulOrb>().Launch(force);
            }

            AGP.counterAmount = 0;
        }

        // Perform the hand slam effect
        Vector3 point1 = attackPosStart; // Start of the capsule (left sphere)
        Vector3 point2 = attackPosEnd;   // End of the capsule (right sphere)

        bool ejectBall = false;
        Collider[] hitColliders = Physics.OverlapCapsule(point1, point2, slamRadius);
        foreach (Collider obj in hitColliders)
        {
            // Debug.Log("Hit: " + obj.name);
            WarriorController warrior = obj.GetComponent<WarriorController>();
            BallProperties ball = obj.GetComponent<BallProperties>();
            if (warrior != null)
            {
                if (BP.ballOwner == obj.gameObject) ejectBall = true;

                // Kill Warrior
                warrior.Die(); // Destroys the warrior game object
            }
            else if (ball != null)
            {
                // If the ball is hit, ensure it gets ejected
                ejectBall = true;
            }
        }

        if (ejectBall)
        {
            EjectBall();
        }

        // Apply the stun effect in a larger capsule
        float stunRadius = slamRadius * stunRadiusMult; // Adjust stun radius as needed
        Vector3 stunPoint1 = point1 - new Vector3(1f, 0, 0); // Slightly offset points for the larger capsule
        Vector3 stunPoint2 = point2 + new Vector3(1f, 0, 0);

        Collider[] stunColliders = Physics.OverlapCapsule(stunPoint1, stunPoint2, stunRadius);
        foreach (Collider obj in stunColliders)
        {
            WarriorController warrior = obj.GetComponent<WarriorController>();
            if (warrior != null)
            {
                Debug.Log($"Stunned warrior: {warrior.name}");
                warrior.Stun(stunTime); // Call the stun method on the warrior
            }
        }

        // Finalize the hand slam
        gameObject.GetComponent<AbilityCreateHands>().SetHandActive(chosenHandIndex, false);
        Instantiate(visEffect, chosenHand.transform.position, Quaternion.identity);
        audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find(soundName), 0.75f);

        // Enable new slams to occur
        canSlam = true;

        // Reattach hand and visualizer, disable visualizer
        chosenHand.GetComponent<GashadokuroHand>().SetIsDetached(false);
        attackVisualizer.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        Setup();
        ST = GameObject.Find("Stat Tracker").GetComponent<StatTracker>();
        abilityCreateHands = GetComponent<AbilityCreateHands>();
        BP = FindObjectOfType<BallProperties>();
        monsterRB = gameObject.GetComponent<Rigidbody>();

        //attackVisualizer.transform.localScale *= slamRadius * 2.5f;
        //attackVisualizer.transform.localScale = new Vector3(
        //    slamRadius * 2,   // Width (X)
        //    slamLength * 2,   // Height (Y) (Unity capsules use half-height scaling)
        //    slamRadius * 2    // Depth (Z)
        //);


        // Unparent visualizer from monster
        attackVisualizer.transform.parent = null;
        AGP = GetComponent<AbilityGashaPassive>();
    }

    private void Update()
    {
        UpdateSetup();

        // Prevent visualizer from rotating
        attackVisualizer.transform.rotation = Quaternion.Euler(0, 0, 90);

        // If hand isn't detached, set visualizer to follow monster
        if (!abilityCreateHands.AHandIsDetached())
        {
            attackVisualizer.transform.position =
            new Vector3(transform.position.x + slamLength / 2, visualOffsetY, transform.position.z);
        }

        // Activate ability if monster picks up ball while hand is detached
        if (MC != null && MC.BP != null && MC.BP.ballOwner == gameObject
            && abilityCreateHands.AHandIsDetached())
        {
            StartCoroutine(TriggerSlam());
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return; // Only draw when in play mode

        // Define points and radius for the capsule
        Vector3 point1 = transform.position; // - new Vector3(1f, 0, 0);
        Vector3 point2 = transform.position + Vector3.right * slamLength; // + new Vector3(1f, 0, 0);

        // Draw spheres to represent the capsule endpoints
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(point1, slamRadius);
        Gizmos.DrawWireSphere(point2, slamRadius);

        // Draw lines connecting the spheres to give the appearance of a capsule
        Vector3 direction = (point2 - point1).normalized;
        Vector3 offset = Vector3.Cross(direction, Vector3.up).normalized * slamRadius * stunRadiusMult;

        Gizmos.DrawLine(point1 + offset, point2 + offset);
        Gizmos.DrawLine(point1 - offset, point2 - offset);
        Gizmos.DrawLine(point1 + Vector3.Cross(direction, Vector3.forward).normalized * slamRadius,
                        point2 + Vector3.Cross(direction, Vector3.forward).normalized * slamRadius);
        Gizmos.DrawLine(point1 - Vector3.Cross(direction, Vector3.forward).normalized * slamRadius,
                        point2 - Vector3.Cross(direction, Vector3.forward).normalized * slamRadius);
    }


    private void EjectBall()
    {
        Debug.Log("Eject ball");

        // Define a range for the angle in degrees (e.g., 0 to 60 degrees relative to the positive x-axis)
        float minAngle = 0f;
        float maxAngle = 60f;

        // Generate a random angle within the specified range
        float randomAngle = Random.Range(minAngle, maxAngle);

        // Convert the angle to radians and calculate the direction vector
        float angleInRadians = randomAngle * Mathf.Deg2Rad;
        Vector3 randomDirection = new Vector3(Mathf.Cos(angleInRadians), 0, Mathf.Sin(angleInRadians)).normalized;

        // Apply the force to the ball's Rigidbody in the calculated direction
        BP.gameObject.GetComponent<Rigidbody>().AddForce(randomDirection * ejectForce, ForceMode.Impulse);
        BP.previousKicker = gameObject;
    }

    IEnumerator TriggerSlam()
    {
        yield return new WaitForSeconds(slamDelay);

        // Activate
        //attackPosStart = transform.position;
        //attackPosEnd = transform.transform.position + Vector3.right * slamLength;

        Debug.Log("attackPosStart: " + attackPosStart);
        Debug.Log("attackPosEnd: " + attackPosEnd);

        Activate();

        
    }

    public void TryStartSlam()
    {
        if (!ShouldAttemptSlam()) return;

        if (canSlam)
        {
            Debug.Log("Hand slam - Ability pressed");
            slamWasPressed = true;
            canSlam = false;

            attackPosStart = transform.position;
            attackPosEnd = transform.transform.position + Vector3.right * slamLength;

            // Detach hand and visualizer, show visualizer

            // Show attack visual
            attackVisualizer.SetActive(true);

            // Choose hand based on if monster movement direction, up or down
            float zDirection = monsterRB.velocity.z;

            // If hand1 is not available
            if (!abilityCreateHands.hand1.activeSelf)
            {
                // Use hand2
                chosenHand = abilityCreateHands.hand2;
                chosenHandIndex = 2;
            }
            else if (!abilityCreateHands.hand2.activeSelf) // If hand2 is not available
            {
                // Use hand1
                chosenHand = abilityCreateHands.hand1;
                chosenHandIndex = 1;
            }
            else // Both hands are available, so choose based on zDirection
            {
                if (zDirection > 0)
                {
                    chosenHand = abilityCreateHands.hand2;
                    chosenHandIndex = 2;
                }
                else
                {
                    chosenHand = abilityCreateHands.hand1;
                    chosenHandIndex = 1;
                }
            }

            // Detach hand
            chosenHand.GetComponent<GashadokuroHand>().SetIsDetached(true);
        }
    }

    public void TryReleaseSlam()
    {
        if (!ShouldAttemptSlam()) return;

        if (slamWasPressed)
        {
            slamWasPressed = false;
            Debug.Log("Hand slam - Ability released");

            StartCoroutine(TriggerSlam());
        }
        
    }

    private bool ShouldAttemptSlam()
    {
        if((!GM.isPlaying || MC.isStunned)
            || (timer < cooldown)
            || (!abilityCreateHands.hand1IsActive && !abilityCreateHands.hand2IsActive)
            || (abilityCreateHands == null))
        {
            return false;
        }
        return true;
    }

    public override void CheckInputsDelayed(InputAction.CallbackContext context)
    {
        // Debug.Log("Ability delayed input action: " + context);

        if (context.action.WasPressedThisFrame())
        {
            TryStartSlam();
        } else if (context.action.WasReleasedThisFrame())
        {
            TryReleaseSlam();
        }
    }

    public override void AbilityReset()
    {
        Debug.Log("Reseting Hand Slam");

        // Flush ability if its being charged
        if (slamWasPressed) StartCoroutine(TriggerSlam());

        // Reset control variables from ability
        slamWasPressed= false;
        canSlam = true;

        // Ensure both hands are activate
        abilityCreateHands.hand1.SetActive(true);
        abilityCreateHands.hand2.SetActive(true);

        // Reset cooldown
        timer = cooldown;
    }

}
