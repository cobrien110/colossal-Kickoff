using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySummonShrine : AbilityScript
{
    AbilityCreateHands abilityCreateHands; // Reference to the AbilityCreateHands script

    // Soul Shrine
    public float structure1Duration = 20f;
    public GameObject structure1;
    public float orbLaunchSpeed = 4;
    public float timeBetweenSpawns = 5f;

    // Torii Goal
    public float structure2Duration = 12f;
    public GameObject structure2;

    MonsterController monsterController;

    public override void Activate()
    {
        //Debug.Log("Timer: " + timer + ", Cooldown: " + cooldown);
        if (timer < cooldown)
        {
            //Debug.Log("Return");
            return;
        }
        timer = 0;

        Debug.Log("Summoning structure");

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
            Debug.Log("No active hands available for summoning.");
            return;
        }

        // 2. Create a structure
        GashaShrine shrine = Instantiate(structure1, chosenHand.transform.position, Quaternion.identity).GetComponent<GashaShrine>();
        shrine.orbLaunchSpeed = orbLaunchSpeed;
        shrine.timeBetweenSpawns = timeBetweenSpawns;

        // 3. Deactivate the hand after swipe
        gameObject.GetComponent<AbilityCreateHands>().SetHandActive(chosenHandIndex, false);
        
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
        
    }
}
