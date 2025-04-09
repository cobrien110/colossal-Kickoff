using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySummonShrine : AbilityScript
{
    AbilityCreateHands abilityCreateHands; // Reference to the AbilityCreateHands script

    public float yOffset = 0.5f;

    // Soul Shrine
    public float structure1Duration = 20f;
    public GameObject structure1;
    public float orbLaunchSpeed = 4;
    public float timeBetweenSpawns = 5f;

    // Torii Goal
    public float structure2Duration = 12f;
    public GameObject structure2;
    public float orbLaunchSpeed2 = 10;

    MonsterController monsterController;
    private AbilityGashaPassive AGP;
    private GoalWithBarrier GOAL;
    public string soundName;

    public override void Activate()
    {
        if (!canActivate) return;

        //Debug.Log("Timer: " + timer + ", Cooldown: " + cooldown);
        if (timer < cooldown)
        {
            //Debug.Log("Return");
            return;
        }

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

        timer = 0;
        // 2. Create a structure
        // If passive is full, create a gate
        Vector3 pos = chosenHand.transform.position;
        pos.y += yOffset;

        if (AGP.counterAmount == AGP.counterMax)
        {
            Debug.Log("Summoning Gate");
            GashaGate gate = Instantiate(structure2, pos, Quaternion.identity).GetComponentInChildren<GashaGate>();
            gate.launchSpeed = orbLaunchSpeed2;
            gate.duration = structure2Duration;
            AGP.counterAmount = 0;
        }
        // Else create a shrine
        else
        {
            Debug.Log("Summoning Shrine");
            GashaShrine shrine = Instantiate(structure1, pos, Quaternion.identity).GetComponent<GashaShrine>();
            shrine.orbLaunchSpeed = orbLaunchSpeed;
            shrine.timeBetweenSpawns = timeBetweenSpawns;
            shrine.duration = structure1Duration;
        }

        // 3. Deactivate the hand after swipe
        gameObject.GetComponent<AbilityCreateHands>().SetHandActive(chosenHandIndex, false);

        audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find(soundName), 0.75f);
        ST.UpdateMAbUsed();
        UM.UpdateMonsterAbilitiesSB();
    }

    // Start is called before the first frame update
    void Start()
    {
        Setup();
        abilityCreateHands = GetComponent<AbilityCreateHands>();
        monsterController = GetComponent<MonsterController>();
        AGP = GetComponent<AbilityGashaPassive>();
        GOAL = GameObject.FindGameObjectWithTag("MonsterGoal").GetComponent<GoalWithBarrier>();
        GOAL.SetBonusBars(true);
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
