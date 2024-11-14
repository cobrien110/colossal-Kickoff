using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GashadokuroHand : MonoBehaviour
{
    AbilityCreateHands createHandsScript;
    [HideInInspector] public BallProperties BP;

    private GameObject monster; // Reference to the MonsterController GameObject
    private Vector3 previousMonsterPosition; // To track previous position of the monster

    void Start()
    {
        createHandsScript = FindObjectOfType<AbilityCreateHands>();
        BP = FindObjectOfType<BallProperties>();
        monster = createHandsScript.gameObject; // Assuming this is the GameObject containing the MonsterController

        // Set the initial position for tracking
        if (monster != null)
        {
            previousMonsterPosition = monster.transform.position;
        }
    }

    void Update()
    {
        if (monster == null) return;

        // Track MonsterController's current position and movement direction
        Vector3 currentMonsterPosition = monster.transform.position;

        // Determine if the MonsterController moved left or right on the x-axis
        float horizontalMovement = currentMonsterPosition.x - previousMonsterPosition.x;

        /*
        // If moving to the right (positive x), set hand rotation to 0 degrees
        if (horizontalMovement > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0); // Face right
        }
        // If moving to the left (negative x), set hand rotation to 180 degrees
        else if (horizontalMovement < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0); // Face left
        }*/

        // Update the previous position to the current position
        previousMonsterPosition = currentMonsterPosition;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (BP == null) return;
        if (BP.ballOwner != null) return;

        // Check if it is a non-trigger collider and has BallProperties
        if (!collider.isTrigger && collider.gameObject.GetComponent<BallProperties>() != null)
        {
            Debug.Log("Hand hit ball: " + collider.name);

            Vector3 hitballDirection =
                (new Vector3(collider.gameObject.transform.position.x, 0, collider.gameObject.transform.position.z)
                - new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z)).normalized;
            collider.gameObject.GetComponent<Rigidbody>().AddForce(hitballDirection * createHandsScript.hitballSpeed);
        } else if (collider.gameObject.GetComponent<SoulOrb>() != null) {
            Debug.Log("Hand hit orb: " + collider.name);

            Vector3 hitballDirection =
                (new Vector3(collider.gameObject.transform.position.x, 0, collider.gameObject.transform.position.z)
                - new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z)).normalized;
            collider.gameObject.GetComponent<Rigidbody>().AddForce(hitballDirection * createHandsScript.hitballSpeed);
        }
    }
}
