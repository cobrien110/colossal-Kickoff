using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHandSwipe : AbilityScript
{
    AbilityCreateHands abilityCreateHands; // Reference to the AbilityCreateHands script

    public float swipeDistance = 3.0f;     // Distance in front of the hand where the swipe occurs
    public float swipeWidth = 1.0f;        // Width of the swipe area
    public float swipeHeight = 2.0f;       // Vertical height of the swipe
    public float swipeRange = 5f;
    public float swipeBallForce = 5f;
    private float directionMultiplier = 1f; // Should be 1 or -1
    private float ballSwipeRandX = 0.25f;

    MonsterController monsterController;

    public override void Activate()
    {
        if (timer < cooldown) return;
        timer = 0;

        // Debug.Log("Hand Swipe");

        if (abilityCreateHands == null)
        {
            Debug.LogError("AbilityCreateHands reference is missing.");
            return;
        }

        // 1. Choose an active hand (prioritize hand1 if both are active)
        GameObject chosenHand = abilityCreateHands.hand1IsActive ? abilityCreateHands.hand1 : (abilityCreateHands.hand2IsActive ? abilityCreateHands.hand2 : null);
        int chosenHandIndex = abilityCreateHands.hand1IsActive ? 1 : 2;

        if (chosenHand == null)
        {
            Debug.Log("No active hands available for swiping.");
            return;
        }

        if (chosenHand.transform.forward.z < 0)
        {
            directionMultiplier = Mathf.Abs(directionMultiplier) * -1f;
        } else
        {
            directionMultiplier = Mathf.Abs(directionMultiplier);
        }
        // Debug.Log("Forward: " + chosenHand.transform.forward);


        // 2. Define the swipe area in front of the monster using a box cast
        Vector3 swipeStart = transform.position + new Vector3(swipeRange * directionMultiplier, 0f, -swipeDistance / 2); // Starting position of the hand
        // Debug.Log("Start: " +  swipeStart);
        
        Vector3 swipeDirection = new Vector3(0f, 0f, 1f); // Always upward as far as OverlapBox is concerned

        // Center of the swipe box in front of the monster
        Vector3 boxCenter = swipeStart + swipeDirection * (swipeDistance / 2f);
        // Debug.Log("Center: " + boxCenter);

        // Size of the box (width, height, and length of swipe)
        Vector3 boxSize = new Vector3(swipeWidth, swipeHeight, swipeDistance);

        // Perform an OverlapBox to detect all colliders within the swipe area
        Collider[] objectsInSwipeRange = Physics.OverlapBox(boxCenter, boxSize / 2f, chosenHand.transform.rotation);

        // Determine Direction to hit ball
        Vector3 hitDirection = Vector3.zero;
        bool hitBall = false;

        foreach (Collider obj in objectsInSwipeRange)
        {
            WarriorController warrior = obj.GetComponent<WarriorController>();
            if (warrior != null)
            {
                // Call Die() on any warrior that is hit by the swipe
                warrior.Die();
                Debug.Log("Warrior killed by hand swipe: " + warrior.name);
            }

            BallProperties BP = obj.GetComponent<BallProperties>();
            if (BP != null)
            {
                // Hit ball with swipe
                hitBall = true;
                // Debug.Log("Swipe Ball");

                if (chosenHand == abilityCreateHands.hand1) // Downward swipe
                {
                    Debug.Log("Hit downward");
                    // Hit ball downward (negative z) with slight x-axis randomness
                    float randomX = Random.Range(-ballSwipeRandX, ballSwipeRandX);  // Small random value on x-axis
                    hitDirection = new Vector3(randomX, 0, -1f);  // Move ball in negative z direction
                }
                else // Upward swipe
                {
                    Debug.Log("Hit upward");
                    // Hit ball upward (positive z) with slight x-axis randomness
                    float randomX = Random.Range(-ballSwipeRandX, ballSwipeRandX);  // Small random value on x-axis
                    hitDirection = new Vector3(randomX, 0, 1f);  // Move ball in positive z direction
                }
            }
        }

        // Apply force in the determined direction, multiplied by a force amount
        if (hitBall) monsterController.BP.GetComponent<Rigidbody>().AddForce(hitDirection * swipeBallForce, ForceMode.Impulse);
        Debug.Log("Hit ball");

        // 3. Deactivate the hand after swipe
        gameObject.GetComponent<AbilityCreateHands>().SetHandActive(chosenHandIndex, false);
        // Debug.Log("Hand deactivated after swipe.");
    }

    // Start is called before the first frame update
    void Start()
    {
        Setup();
        abilityCreateHands = GetComponent<AbilityCreateHands>();
        monsterController = GetComponent<MonsterController>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSetup();
    }

    // Optional: Draw Gizmos to visualize the swipe area
    private void OnDrawGizmos()
    {
        if (abilityCreateHands != null)
        {
            GameObject activeHand = abilityCreateHands.hand1IsActive ? abilityCreateHands.hand1 : (abilityCreateHands.hand2IsActive ? abilityCreateHands.hand2 : null);
            if (activeHand != null)
            {
                Gizmos.color = Color.green;

                // Calculate swipe area
                Vector3 swipeStart = transform.position + new Vector3(swipeRange * directionMultiplier, 0f, -swipeDistance / 2); // activeHand.transform.position;
                Vector3 swipeDirection = new Vector3(0f, 0f, 1f); ;
                Vector3 boxCenter = swipeStart + swipeDirection * (swipeDistance / 2f);
                Vector3 boxSize = new Vector3(swipeWidth, swipeHeight, swipeDistance);

                // Draw swipe box
                Gizmos.matrix = activeHand.transform.localToWorldMatrix;
                Gizmos.DrawWireCube(activeHand.transform.InverseTransformPoint(boxCenter), boxSize);
            }
        }
    }
}
