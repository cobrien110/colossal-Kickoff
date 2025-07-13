using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GashadokuroHand : MonoBehaviour
{
    AbilityCreateHands createHandsScript;
    [HideInInspector] public BallProperties BP;

    private GameObject monster; // Reference to the MonsterController GameObject
    private Vector3 previousMonsterPosition; // To track previous position of the monster
    private bool isDetached = false;
    public RuntimeAnimatorController animType1;
    public RuntimeAnimatorController animType2;
    public Animator ANIM;

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
        /*if (monster == null) return;

        // Track MonsterController's current position and movement direction
        Vector3 currentMonsterPosition = monster.transform.position;

        // Determine if the MonsterController moved left or right on the x-axis
        float horizontalMovement = currentMonsterPosition.x - previousMonsterPosition.x;

        // Update the previous position to the current position
        previousMonsterPosition = currentMonsterPosition;*/
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (monster == null) return;
        // Debug.Log("GashaHand OnTriggerEnter");
        if (BP == null)
        {
            // Ensure BP is assigned a valid value
            MonsterController mc = monster.GetComponent<MonsterController>();
            if (mc == null)
            {
                mc = FindObjectOfType<MonsterController>();
            }
            BP = mc.BP;
        }

        // Debug.Log("BP: " + BP);

        SoulOrb SO = collider.gameObject.GetComponent<SoulOrb>();

        // Check if it is a non-trigger collider, has BallProperties, and that no character is possessing ball
        if (!collider.isTrigger && collider.gameObject.GetComponent<BallProperties>() != null
            && BP.ballOwner == null)
        {
            // Debug.Log("Hand hit ball: " + collider.name);

            Vector3 hitballDirection =
                (new Vector3(collider.gameObject.transform.position.x, 0, collider.gameObject.transform.position.z)
                - new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z)).normalized;

            // Calculate the angle of the hit direction relative to the positive x-axis
            float angle = Vector3.SignedAngle(Vector3.right, hitballDirection, Vector3.up);

            // Define valid angle range (e.g., between -60 degrees and 60 degrees)
            float minValidAngle = -60f;
            float maxValidAngle = 60f;

            // Correct the direction if the angle is outside the valid range
            if (angle < minValidAngle)
            {
                angle = minValidAngle;
            }
            else if (angle > maxValidAngle)
            {
                angle = maxValidAngle;
            }

            // Calculate the corrected direction based on the clamped angle
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            hitballDirection = rotation * Vector3.right;

            // Apply force to the ball
            collider.gameObject.GetComponent<Rigidbody>().AddForce(hitballDirection * createHandsScript.hitballSpeed);

            //update owner
            BP.previousKicker = gameObject;
            BP.playerTest = BP.previousKicker;
        }
        else if (SO != null)
        {
            // Debug.Log("Hand hit orb: " + collider.name);

            Vector3 hitballDirection =
                (new Vector3(collider.gameObject.transform.position.x, 0, collider.gameObject.transform.position.z)
                - new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z)).normalized;
            Vector3 force = hitballDirection * createHandsScript.hitballSpeed;
            SO.RB.AddForce(force);
            SO.SetTeam(true);
        }
    }


    public bool GetIsDetached()
    {
        return isDetached;
    }

    public void SetIsDetached(bool isDetached)
    {
        this.isDetached = isDetached;
        string name = createHandsScript.hand1.Equals(this.gameObject) ? "hand1" : "hand2";
        Debug.Log(name +  " - SetIsDetached: " + isDetached);
    }
}
