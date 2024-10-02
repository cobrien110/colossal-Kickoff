using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHandSlam : AbilityScript
{
    AbilityCreateHands abilityCreateHands;
    public float slamRadius = 2.0f; // Radius within which the hand slam will kill warriors
    float ejectForce = 10f;

    public override void Activate()
    {
        if (timer < cooldown) return;
        timer = 0;
        // Debug.Log("Activate Hand Slam");

        // Ensure that we have reference to the hands
        if (abilityCreateHands == null)
        {
            Debug.LogError("AbilityCreateHands reference is missing.");
            return;
        }

        // 1. Determine which hand is closer to the nearest WarriorController
        GameObject closestWarrior = FindClosestWarrior();
        if (closestWarrior == null)
        {
            // Debug.Log("No warriors found to target.");
            return;
        }

        // Get distances from each hand to the closest warrior
        float distanceToHand1 = Vector3.Distance(abilityCreateHands.hand1.transform.position, closestWarrior.transform.position);
        float distanceToHand2 = Vector3.Distance(abilityCreateHands.hand2.transform.position, closestWarrior.transform.position);

        GameObject chosenHand;
        int chosenHandIndex;
        if (distanceToHand1 <= distanceToHand2)
        {
            chosenHand = abilityCreateHands.hand1;
            chosenHandIndex = 1;
        } else
        {
            chosenHand = abilityCreateHands.hand2;
            chosenHandIndex = 2;
        }

        // Debug.Log("Chosen hand for slam: " + chosenHand.name);

        // 2. Check for all WarriorControllers within the slam radius of the chosen hand
        Collider[] objectsInRange = Physics.OverlapSphere(chosenHand.transform.position, slamRadius);
        bool ejectBall = false;
        foreach (Collider obj in objectsInRange)
        {
            WarriorController warrior = obj.GetComponent<WarriorController>();
            if (warrior != null)
            {
                if (BP.ballOwner == obj.gameObject) ejectBall = true;

                // Kill Warrior
                warrior.Die(); // Destroys the warrior game object
                // Debug.Log("Warrior killed by slam: " + warrior.name);
            }
        }

        if (ejectBall)
        {
            EjectBall();
        }

        gameObject.GetComponent<AbilityCreateHands>().SetHandActive(chosenHandIndex, false);
    }

    // Start is called before the first frame update
    void Start()
    {
        Setup();
        abilityCreateHands = GetComponent<AbilityCreateHands>();
        BP = FindObjectOfType<BallProperties>();
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
        if (abilityCreateHands != null)
        {
            Gizmos.color = Color.red;
            // Draw slam radius for both hands to visualize
            if (abilityCreateHands.hand1 != null)
                Gizmos.DrawWireSphere(abilityCreateHands.hand1.transform.position, slamRadius);
            if (abilityCreateHands.hand2 != null)
                Gizmos.DrawWireSphere(abilityCreateHands.hand2.transform.position, slamRadius);
        }
    }

    private void EjectBall()
    {
        Debug.Log("Eject ball");

        // Calculate a random direction on the XZ plane
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;

        // Apply the force to the ball's Rigidbody in the random direction
        BP.gameObject.GetComponent<Rigidbody>().AddForce(randomDirection * ejectForce, ForceMode.Impulse);
    }
}
