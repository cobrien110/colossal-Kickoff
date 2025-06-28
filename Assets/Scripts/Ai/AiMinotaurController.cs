using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AiMinotaurController : AiMonsterController
{
    // Ability Order (by index)
    // Wall - 0
    // Basic Attack - 1
    // Dash - 2

    // [Header("AI Mino Stats & Behaviour")]
    
    
    
 
    
    //private float pursueDelayFrequency;

    // private bool shouldPerformAbility1 = false;
    // private bool targetBallController = true; // Used to determine if attack will target ball controller or nearest warrior

    

    private enum WallMode
    {
        BlockWarrior,
        BlockGoal,
        Offensive
    }

    private enum DashMode
    {
        BallOwner,
        Nearest,
        Ball
    }

    // Used to track current Ability Modes
    
    WallMode wallMode = WallMode.BlockGoal;
    DashMode dashMode = DashMode.BallOwner;



    protected override void PerformAbility1Chance()
    {
        if (mc.abilities[0] == null) return;
            
        if (!mc.abilities[0].AbilityOffCooldown()) return;

        if (UnityEngine.Random.value < ability1Chance)
        {
            Debug.Log("PerformAbility1");
            isPerformingAbility = true;

            StopCoroutines();
            Wall(wallMode);
        }
    }

    protected override void PerformAbility2Chance()
    {
        if (mc.abilities[1] == null) return;

        if (!mc.abilities[1].AbilityOffCooldown()) return;

        if (UnityEngine.Random.value < ability2Chance)
        {
            Debug.Log("PerformAbility2");
            isPerformingAbility = true;

            StopCoroutines();
            StartChargeableAttack(attackMode);
        }
    }

    protected override void PerformAbility3Chance()
    {
        if (mc.abilities[2] == null) return;

        if (!mc.abilities[2].AbilityOffCooldown()) return;

        if (UnityEngine.Random.value < ability3Chance)
        {
            Debug.Log("PerformAbility3");
            isPerformingAbility = true;

            StopCoroutines();
            Dash(dashMode);
            //DashHelper();
        }
    }

    /*
     * Decides the overall logic for the AiMonster.
     * Accounts for movement of monster, and chances
     * that monster will perform any given ability or if
     * it will Shoot at any given moment, based on state of
     * the ball (who possesses it, if anyone) and other factors
     * (such as proximity between warriors and monster, where each
     * are on the field, etc)
     */
    protected override void MonsterBehaviour()
    {
        // If goal was scored, stop movement and behavior
        if (mc != null && mc.BP != null && !mc.BP.isInteractable)
        {
            mc.movementDirection = Vector3.zero;
            return;
        }

        // Debug.Log("MonsterBehaviour");

        // Make sure mc.BP is assigned a value
        EnsureBallOwnerValid();

        // If no one has ball...
        if (mc.BP.ballOwner == null)
        {
            // Debug.Log("BallNotPossessed");
            // Logic
            BallNotPossessed();
        }
        // If I have ball...
        else if (mc.BP.ballOwner == gameObject)
        {
            // Debug.Log("MonsterHasBall");
            // Logic
            MonsterHasBall();
        }
        // If warrior has ball...
        else if (mc.BP.ballOwner.GetComponent<WarriorController>() != null)
        {
            // Debug.Log("WarriorHasBall");
            // Logic
            WarriorHasBall();
        }
        else
        {
            Debug.Log("Error in MonsterBehaviour logic");
        }
    }

    protected override void WarriorHasBall()
    {
        // Reset shootChance to 0.0
        if (shootChance != 0.0f) shootChance = 0.0f;

        // If mino in mino half, warrior with ball in warrior half...
        if (!IsInWarriorHalf(gameObject) && IsInWarriorHalf(mc.BP.ballOwner))
        {
            StopPursuing();
            // Default behavior
            if (!isPerformingAbility) StartRoaming();

            // Set Wall chance and behavior
            ability1Chance = 0.2f; // Wall
            wallMode = WallMode.Offensive;

            // Set Spherical Attack chance and behavior
            ability2Chance = 0.1f; // Spherical Attack
            attackMode = AttackMode.NearestWarrior; // Target nearest warrior because you don't want to overextend to get ball owner

            // Set Dash chance and behavior
            ability3Chance = 0.1f; // Dash
            dashMode = DashMode.Nearest; // Don't want to overextend so target nearest

        }

        // If mino and warrior with ball in mino half...
        else if (!IsInWarriorHalf(gameObject) && !IsInWarriorHalf(mc.BP.ballOwner))
        {
            StopCoroutines();
            if (!isPerformingAbility) // Allow ability to finish if one is happening
            {
                // Default behavior
                StartDefendGoal();

                // Set Wall chance and behavior
                ability1Chance = 0.1f;
                wallMode = WallMode.BlockGoal;

                // Set Spherical Attack chance and behavior
                ability2Chance = 0.1f;
                attackMode = AttackMode.BallOwner; 

                // Set Dash chance and behavior
                ability3Chance = 0.1f;
                dashMode = DashMode.BallOwner;
            }
        }

        // If mino and warrior in warrior half...
        else if (IsInWarriorHalf(gameObject) && IsInWarriorHalf(mc.BP.ballOwner))
        {
            // Debug.Log("mino and warrior in warrior half");
            StopRoaming();

            if (!isPerformingAbility) // Allow ability to finish if one is happening
            {
                // Debug.Log("isPerformingAbility false");
                // Default behavior
                StartPursuing();

                // Set Wall chance and behavior
                ability1Chance = 0.1f;
                wallMode = WallMode.Offensive;

                // Set Spherical Attack chance and behavior
                ability2Chance = 0.1f;
                attackMode = AttackMode.BallOwner; // Be aggressive, try to get ball

                // Set Dash chance and behavior
                ability3Chance = 0.1f;
                dashMode = DashMode.BallOwner; // Be aggressive, try to get ball
            }
        }

        // If mino in warrior half, warrior in mino half...
        else if (IsInWarriorHalf(gameObject) && !IsInWarriorHalf(mc.BP.ballOwner))
        {
            StopCoroutines();

            if (!isPerformingAbility) // Allow ability to finish if one is happening
            {
                // Default behavior
                mc.movementDirection = (monsterGoal.transform.position - transform.position).normalized; // Retreat to own goal
                mc.movementDirection.y = 0;
                //Debug.Log("GROUND CLIP TEST: DIR = " + mc.movementDirection);

                // Set Wall chance and behavior
                ability1Chance = 0.1f;
                wallMode = WallMode.BlockGoal;

                // Set Spherical Attack chance and behavior
                ability2Chance = 0.1f;
                attackMode = AttackMode.BallOwner; // Hurry to kill ball owner to stop goal

                // Set Dash chance and behavior
                ability3Chance = 0.1f;
                dashMode = DashMode.BallOwner; // Rush to get back
            }
        }
    }

    protected override void MonsterHasBall()
    {
        // Default behaviour
        if (!isPerformingAbility)
        {
            ResetAbilities();

            // "Wiggle" your way towards the goal
            WiggleTowardGoal();

            // Constant chance to shoot depending on:
            // Distance to warrior goal
            float distToGoalFactor = Mathf.Clamp01(1 - (Vector3.Distance(warriorGoal.transform.position, transform.position) / maxShootingRange));
            // proximity to closest warrior
            float proximityToWarriorFactor = Mathf.Clamp01(1 - (GetDistanceToNearestWarrior() / maxProximityRange));
            // How clear path is to goal
            // float pathToGoalFactor = 0.0f;

            // Calculate shootChance based on these factors
            shootChance = Mathf.Pow((distToGoalFactor/* + proximityToWarriorFactor*/) / 2f, 2);

            // If shooting, chargeAmount depends on distance to goal
        }
        // If dash is being charged, charge is down
        else
        {
            ResetAbilities();
        }

        // Monster should not use abilities
        ability1Chance = 0.0f;
        ability2Chance = 0.0f;
        ability3Chance = 0.0f;

        // Stop roaming if its happening
        StopCoroutines();
    }

    protected override void BallNotPossessed()
    {
        // ResetAbilities();

        // Stop roaming and pursuing if its happening
        StopCoroutines();

        // Reset shootChance to 0.0
        if (shootChance != 0.0f) shootChance = 0.0f;

        if (!isPerformingAbility)
        {
            // Default behaviour
            Vector2 toBall = new Vector2(
                    mc.BP.gameObject.transform.position.x - transform.position.x,
                    mc.BP.gameObject.transform.position.z - transform.position.z).normalized;
            MoveTo(toBall);
        }

        // Set Wall chance and behavior
        // If ball is going toward own goal at a high enough speed
        if (BallGoingTowardOwnGoal())
        {
            ability1Chance = 0.4f;
            wallMode = WallMode.BlockGoal;
        } else {
            ability1Chance = 0.3f;
            wallMode = WallMode.BlockWarrior; // Try to block warrior from getting to ball
        }

        // Set Spherical attack chance
        ability2Chance = 0.1f;
        attackMode = AttackMode.NearestWarrior;

        // Set Dash chance and behavior
        ability3Chance = 0.1f;
        dashMode = DashMode.Ball; // Dash at ball

        // Debug.Log("BallNotPossessed");
    }

    // WALL METHODS
    private void WallOffensive()
    {
        // Ensure wall is in correct slot
        if (!(mc.abilities[0] is AbilityMinotaurWall))
        {
            isPerformingAbility = false;
            return;
        }

        AbilityMinotaurWall amw =  (AbilityMinotaurWall)mc.abilities[0];
        GameObject ball = mc.BP.gameObject;

        // Only wall if in range
        if (Vector3.Distance(ball.transform.position, transform.position) > amw.wallSpawnDistance + 1f) // Allow walling a bit outside of ball range
        {
            isPerformingAbility = false;
            return;
        }

        // Look toward ball
        Vector3 dir = (ball.transform.position - transform.position).normalized;
        mc.movementDirection = new Vector3(dir.x, 0, dir.z);
        //Debug.Log("GROUND CLIP TEST: DIR = " + mc.movementDirection);

        // Summon wall
        mc.abilities[0].Activate();
    }

    private void WallBlockWarrior()
    {
        // Ensure wall is in correct slot
        if (!(mc.abilities[0] is AbilityMinotaurWall))
        {
            isPerformingAbility = false;
            return;
        }

        if (mc.BP == null)
        {
            isPerformingAbility |= false;
            return;
        }

        GameObject ball = mc.BP.gameObject;
        if (ball == null)
        {
            isPerformingAbility = false;
            return;
        }

        AbilityMinotaurWall amw = (AbilityMinotaurWall)mc.abilities[0];

        List<GameObject> warriors = GetWarriorsToBlock();
        if (warriors.Count < 1)
        {
            Debug.Log("No warriors to block");
            isPerformingAbility = false;
            return;
        }

        foreach (GameObject warrior in warriors)
        {
            Vector3 toWarrior = (warrior.transform.position - transform.position).normalized;
            Vector3 toWarriorIgnoreY = new Vector3(toWarrior.x, 0, toWarrior.z);
            Vector3 wallPos = transform.position + (toWarriorIgnoreY * amw.wallSpawnDistance);

            Vector3 warriorToWall = (wallPos - warrior.transform.position).normalized;
            Vector3 ballToWall = (wallPos - ball.transform.position).normalized;

            // Check if wall would be between warrior and ball
            if (Vector3.Dot(warriorToWall, ballToWall) < -0.5f)
            {
                Debug.Log("Blocking warrior");
                // Wall would be between warrior and ball, thus blocking warrior

                // Look toward warrior
                mc.movementDirection = toWarriorIgnoreY;
                //Debug.Log("GROUND CLIP TEST: DIR = " + mc.movementDirection);

                // Summon wall
                mc.abilities[0].Activate();

                // No need to continue, ability was activated
                return;
            }
            
        }
        
        Debug.Log("Wall would not have blocked - No activation");
        isPerformingAbility = false;

    }

    private void WallBlockGoal()
    {
        // Ensure wall is in correct slot
        if (!(mc.abilities[0] is AbilityMinotaurWall))
        {
            isPerformingAbility = false;
            return;
        }

        AbilityMinotaurWall amw = (AbilityMinotaurWall)mc.abilities[0];

        // Look toward own goal
        Vector3 dir = (monsterGoal.transform.position - transform.position).normalized;
        mc.movementDirection = new Vector3(dir.x, 0, dir.z);
        //Debug.Log("GROUND CLIP TEST: DIR = " + mc.movementDirection);

        // Summon wall
        mc.abilities[0].Activate();
    }

    private void Wall(WallMode wallMode)
    {
        Debug.Log("Wall");
        if (wallMode == WallMode.Offensive)
        {
            WallOffensive();
        } else if (wallMode == WallMode.BlockWarrior)
        {
            WallBlockWarrior();
        } else if (wallMode == WallMode.BlockGoal)
        {
            WallBlockGoal();
        }
        isPerformingAbility = false;
    }

    private List<GameObject> GetWarriorsToBlock()
    {
        if (mc.BP.gameObject == null) return null;

        List<GameObject> warriorsToBlock = new List<GameObject>();

        foreach (GameObject warrior in warriors) {

            Vector3 minoToBall = (mc.BP.gameObject.transform.position - transform.position).normalized;
            Vector3 warriorToBall = (mc.BP.gameObject.transform.position - warrior.transform.position).normalized;

            // If warrior is on same side of ball, add to list
            if (Vector3.Dot(minoToBall, warriorToBall) > 0.5f)
            {
                Debug.Log(warrior.name + " is on the same side of the ball");
                warriorsToBlock.Add(warrior.gameObject);
            }

            // If warrior is on opposite side of ball, add to list
            if (Vector3.Dot(minoToBall, warriorToBall) < -0.5f)
            {
                warriorsToBlock.Add(warrior.gameObject);
                // Debug.Log(warrior.name + " is on the opposite side of the ball");
            }
        }

        return warriorsToBlock;

    }

    // DASH METHODS
    private void DashHelper()
    {
        // Debug.Log("Dash Helper");

        // Make sure first ability is an AbilityChargable
        if (!(mc.abilities[2] is AbilityBullrush)) return;

        AbilityBullrush abr = (AbilityBullrush)mc.abilities[2];

        if (!abr.GetIsAutoCharging()) return;

        if (abr.GetIsAutoCharging())
        {
            abr.ChargeUp();
            // abr.SetIsAutoCharging(true);
        }
        else if (!abr.GetIsAutoCharging())
        {
            abr.ChargeDown();
        }
    }

    IEnumerator DashBallOwner()
    {
        if (mc.BP == null)
        {
            Debug.Log("BP is null");
            isPerformingAbility = false;
            yield break;
        }

        if (mc.BP.ballOwner == null) 
        {
            Debug.Log("ballOwner is null");
            isPerformingAbility = false;
            yield break;
        }

        while (isPerformingAbility)
        {
            // Look at ball owner
            if (mc.BP != null && mc.BP.ballOwner != null)
            {
                Vector3 toBallOwner = (mc.BP.ballOwner.transform.position - transform.position).normalized;
                toBallOwner = new Vector3(toBallOwner.x, 0, toBallOwner.z);
                mc.movementDirection = toBallOwner;
                //Debug.Log("GROUND CLIP TEST: DIR = " + mc.movementDirection);
            }

            // Dash
            DashHelper();

            yield return null;
        }

        Debug.Log("DashBallOwner done");
    }

    IEnumerator DashNearest()
    {
        if (mc.BP == null)
        {
            Debug.Log("BP is null");
            isPerformingAbility = false;
            yield break;
        }

        if (mc.BP.ballOwner == null)
        {
            Debug.Log("ballOwner is null");
            isPerformingAbility = false;
            yield break;
        }

        WarriorController nearestWarrior = GetNearestWarrior(transform.position);
        if (nearestWarrior == null) yield break;

        while (isPerformingAbility)
        {
            // Look at nearest warrior
            if (nearestWarrior != null)
            {
                Vector3 toNearestWarrior = (nearestWarrior.transform.position - transform.position).normalized;
                toNearestWarrior = new Vector3(toNearestWarrior.x, 0, toNearestWarrior.z);
                mc.movementDirection = toNearestWarrior;
                //Debug.Log("GROUND CLIP TEST: DIR = " + mc.movementDirection);
            }

            // Dash
            DashHelper();

            yield return null;
        }

        Debug.Log("DashNearest done");
    }
    IEnumerator DashBall()
    {
        if (mc.BP == null)
        {
            Debug.Log("BP is null");
            isPerformingAbility = false;
            yield break;
        }

        if (mc.BP.gameObject == null)
        {
            Debug.Log("ball is null");
            isPerformingAbility = false;
            yield break;
        }

        while (isPerformingAbility)
        {
            // Look at ball owner
            if (mc.BP != null && mc.BP.gameObject != null)
            {
                Vector3 toBall = (mc.BP.gameObject.transform.position - transform.position).normalized;
                toBall = new Vector3(toBall.x, 0, toBall.z);
                mc.movementDirection = toBall;
                //Debug.Log("GROUND CLIP TEST: DIR = " + mc.movementDirection);
            }

            // Dash
            DashHelper();

            yield return null;
        }

        Debug.Log("DashBall done");
    }
    private void Dash(DashMode dashMode)
    {
        Debug.Log("Dash");

        // Make sure first ability is an AbilityChargable
        if (!(mc.abilities[2] is AbilityBullrush))
        {
            isPerformingAbility = false;
            return;
        }

        AbilityBullrush abr = (AbilityBullrush)mc.abilities[2];
        
        if (abr.GetTimer() < abr.GetCooldown())
        {
            Debug.Log("Dash not off cooldown");
            isPerformingAbility = false;
            return;
        }

        abr.SetIsAutoCharging(true);
        if (dashMode == DashMode.BallOwner)
        {
            StartCoroutine(DashBallOwner());
        } else if (dashMode == DashMode.Nearest)
        {
            StartCoroutine(DashNearest());
        }
        else if (dashMode == DashMode.Ball)
        {
            StartCoroutine(DashBall());
        }
        else
        {
            Debug.Log("Error in Dash");
        }

    }

    

    

    private void FixedUpdate()
    {
        // Decides movement behaviour,
        // Also decides chances that abilities and Shooting will occur at any given moment
        MonsterBehaviour();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Sets up variable assignments for this gameobject
        Setup();
    }

    // Update is called once per frame
    void Update()
    {
        //AbilityBullrush abr = (AbilityBullrush)mc.abilities[2];
        //Debug.Log("isAutoCharging: " + abr.GetIsAutoCharging());
        //Debug.Log("autoCharge: " + abr.autoCharge);

        if (mc.abilities[1] != null && (mc.abilities[1].attackVisualizer.transform.position.y > 0
            || mc.abilities[1].attackVisualizer.transform.position.x > 0))
        {
            // Debug.Log("Attack visual change position!!!!");
        }

        if (isPerformingAbility)
        {
            // Debug.Log("IsPerformingAbility: " + isPerformingAbility);
        }

        //Debug.Log("ability1Chance: " + ability1Chance);
        //Debug.Log("ability2Chance: " + ability2Chance);
        //Debug.Log("ability3Chance: " + ability3Chance);

        // To fix issue where mino can't pickup ball if he killed a warrior with the ball while he was already in the ball colider
        if (mc.BP != null && mc.BP.gameObject != null && mc.BP.ballOwner == null
            && Vector3.Distance(transform.position, mc.BP.gameObject.transform.position) < 0.1f)
        {
            Debug.Log("Mino on top of ballOwner that is just killed, manually set it to be ballOwner");
            mc.BP.ballOwner = gameObject;
        }

        // Debug.Log("AiMinoController update");
    }
    
    /*
     * TODO
     * 
     * Make ability chances based on math rather than set values
     * 
     * Fixed: Fix bug where monster can't pickup ball if he is on top of ball when he kills warrior with ball (Doesn't cue OnTriggerEnter)
     * 
     * Fixed: If mino starts charging and picks up ball, he keeps going slow. Charge needs to be charged down
     * 
     */

}
