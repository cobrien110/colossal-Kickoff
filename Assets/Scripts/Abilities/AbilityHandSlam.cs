using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AbilityHandSlam : AbilityScript
{
    [Header("Ability Vars")]
    AbilityCreateHands abilityCreateHands;
    public float slamRadius = 2.0f; // Radius within which the hand slam will kill warriors
    float ejectForce = 10f;
    public GameObject visEffect;
    public string soundName;
    public float slamLength = 2f;

    private Rigidbody monsterRB;

    public override void Activate()
    {
        Debug.Log("Timer: " + timer + ", Cooldown: " + cooldown);
        if (timer < cooldown)
        {
            Debug.Log("Hand slam not off cooldown");
            return; // Ability not off cooldown
        }

        if (!abilityCreateHands.hand1IsActive && !abilityCreateHands.hand2IsActive)
        {
            Debug.Log("No hand is active");
            return; // No hand is active
        }

        // Ensure that we have reference to the hands
        if (abilityCreateHands == null)
        {
            Debug.LogError("AbilityCreateHands reference is missing.");
            return;
        }

        // Attack visual
        attackVisualizer.SetActive(true);
        StartCoroutine(DisableVisual());

        // 1. Determine which hand is closer to the nearest WarriorController
        /*GameObject closestWarrior = FindClosestWarrior();
        if (closestWarrior == null)
        {
            Debug.Log("No warriors found to target.");
            return;
        }*/

        Debug.Log("Activate hand slam");
        timer = 0;

        // Get distances from each hand to the closest warrior
        //float distanceToHand1 = Vector3.Distance(abilityCreateHands.hand1.transform.position, closestWarrior.transform.position);
        //float distanceToHand2 = Vector3.Distance(abilityCreateHands.hand2.transform.position, closestWarrior.transform.position);

        GameObject chosenHand;
        int chosenHandIndex;

        // Choose hand based on if monster movement direction, up or down
        float zDirection = monsterRB.velocity.z;

        if (zDirection > 0)
        {
            chosenHand = abilityCreateHands.hand1;
            chosenHandIndex = 1;
        } else
        {
            chosenHand = abilityCreateHands.hand2;
            chosenHandIndex = 2;
        }

        // Debug.Log("Chosen hand for slam: " + chosenHand.name);

        // 2. Check for all WarriorControllers within the slam radius
        Vector3 point1 = transform.position; // Start of the capsule (left sphere)
        Vector3 point2 = transform.position + Vector3.right * slamLength; // End of the capsule (right sphere)

        bool ejectBall = false;
        Collider[] hitColliders = Physics.OverlapCapsule(point1, point2, slamRadius);
        foreach (Collider obj in hitColliders)
        {
            Debug.Log("Hit: " + obj.name);
            WarriorController warrior = obj.GetComponent<WarriorController>();
            BallProperties ball = obj.GetComponent<BallProperties>();
            if (warrior != null)
            {
                if (BP.ballOwner == obj.gameObject) ejectBall = true;

                // Kill Warrior
                warrior.Die(); // Destroys the warrior game object
                // Debug.Log("Warrior killed by slam: " + warrior.name);
            } 
            // If its just the ball that is hit, make sure it gets ejected
            else if (ball != null) ejectBall = true;
        }

        if (ejectBall)
        {
            EjectBall();
        }

        gameObject.GetComponent<AbilityCreateHands>().SetHandActive(chosenHandIndex, false);
        Instantiate(visEffect, chosenHand.transform.position, Quaternion.identity);
        audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find(soundName), 0.75f);
    }



    // Start is called before the first frame update
    void Start()
    {
        Setup();
        abilityCreateHands = GetComponent<AbilityCreateHands>();
        BP = FindObjectOfType<BallProperties>();
        monsterRB = gameObject.GetComponent<Rigidbody>();

        attackVisualizer.transform.localScale *= slamRadius * 2;
    }

    private void Update()
    {
        UpdateSetup();

        // Prevent visualizer from rotating
        attackVisualizer.transform.rotation = Quaternion.Euler(0, 0, 90);
        attackVisualizer.transform.position = 
            new Vector3(transform.position.x + slamLength / 2, transform.position.y, transform.position.z);
    }

    // Finds the closest WarriorController in the scene
    private GameObject FindClosestWarrior()
    {
        GameObject[] allWarriors = GameObject.FindGameObjectsWithTag("Warrior"); // Make sure warriors are tagged correctly
        GameObject closest = null;
        float minDistance = Mathf.Infinity;

        // Find the closest warrior
        foreach (GameObject warrior in allWarriors)
        {
            float distance = Vector3.Distance(transform.position, warrior.transform.position);
            if (distance < minDistance)
            {
                closest = warrior;
                minDistance = distance;
            }
        }
        return closest;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return; // Only draw when in play mode

        // Define points and radius for the capsule
        Vector3 point1 = transform.position;
        Vector3 point2 = transform.position + Vector3.right * slamLength;

        // Draw spheres to represent the capsule endpoints
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(point1, slamRadius);
        Gizmos.DrawWireSphere(point2, slamRadius);

        // Draw lines connecting the spheres to give the appearance of a capsule
        Vector3 direction = (point2 - point1).normalized;
        Vector3 offset = Vector3.Cross(direction, Vector3.up).normalized * slamRadius;

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
    }

    IEnumerator DisableVisual()
    {
        yield return new WaitForSeconds(1);

        attackVisualizer.SetActive(false);
    }
}
