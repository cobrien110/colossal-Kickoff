using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class AiMonsterController : MonoBehaviour
{
    #region variables
    protected float ability1Chance = 0.0f;
    protected float ability2Chance = 0.0f;
    protected float ability3Chance = 0.0f;
    protected float shootChance = 0.0f;
    protected MonsterController mc;
    protected Rigidbody rb;
    protected GameplayManager GM;
    protected GameObject warriorGoal;
    protected GameObject monsterGoal;
    protected bool isPerformingAbility = false;
    [HideInInspector] public List<GameObject> warriors;
    // protected List<WarriorController> warriors;

    // Stats
    [Header("AI Monster Stats & Behaviour")]
    [SerializeField] protected float aiShootSpeed;
    [SerializeField] private float performActionChanceFrequency = 0.25f; // How often monster checks to perform ability
    [SerializeField] protected float maxShootingRange = 16f; // 16 is an estimate of the width of the whole field
    [SerializeField] protected float maxProximityRange = 8f; // Max distance monster will target a warrior from
    [SerializeField] protected float midFieldPoint = 0f; // Represent the value on x axis that is midfield
    [SerializeField] protected float leftBoundary = -4f; // Left boundary for monster roaming purposes
    [SerializeField] protected float fieldDepth = 3f; // field depth for monster roaming purposes

    // Variables Transferred Over From AiMinotaurController

    [SerializeField] protected float attackMinimumCharge = 0.3f;
    [SerializeField] protected float smoothFactor = 3f; // Controls how smoothly the movement direction adjusts. Try values between 3 - 7 for best results
    [SerializeField] private float defendGoalDelay = 0.5f;
    [SerializeField] private float pursueDelay = 1f;
    [SerializeField] private float minPursueDistance = 2f;
    private const float stoppingDistance = 0.5f;
    private const float rotationSpeed = 2f;
    [SerializeField] private float waitInPlaceTime = 1f;

    // WiggleTowardsGoal variables
    [SerializeField] private float redirectionInterval = 0.5f; // Time interval in seconds
    private float redirectionTimer = 0f;      // Timer to track redirection intervals
    private Vector3 currentRandomOffset = Vector3.zero;
    [SerializeField] private float redirectionSmoothness = 0.2f; // Adjust how quickly the wiggle offset transitions to the next offset. A value between 0.1 - 0.5 should work well
    [SerializeField] private float wiggleOffset = 1f;

    // Non-Stats
    protected Coroutine roamCoroutine;
    protected Coroutine pursueCoroutine;
    private Coroutine defendGoalCoroutine;
    private bool canPickUpBall = true;
    protected State state = State.BallNotPossessed;
    protected bool stateChanged = false;
    private Coroutine attackCoroutine = null;
    protected enum State
    {
        BallNotPossessed,
        MonsterHasBall,
        WarriorHasBall,
        MummyHasBall
    }

    protected enum AttackMode
    {
        BallOwner,
        NearestWarrior
    }

    // Used to track current Ability Modes
    protected AttackMode attackMode = AttackMode.BallOwner;
    #endregion variables

    // Not necessarily all of these will use the chargeAmount
    protected abstract void PerformAbility1Chance();
    protected abstract void PerformAbility2Chance();
    protected abstract void PerformAbility3Chance();
    protected void PerformShootChance()
    {
        if (UnityEngine.Random.value < shootChance && mc.BP != null && mc.BP.ballOwner != null && mc.BP.ballOwner == gameObject)
        {
            Debug.Log("PerformShoot");
            Shoot();
        }
    }
    protected abstract void MonsterBehaviour();
    protected abstract void BallNotPossessed();
    protected abstract void MonsterHasBall();
    protected abstract void WarriorHasBall();

    protected bool IsInWarriorHalf(GameObject gameObject)
    {
        if (gameObject == null) return false;
        return gameObject.transform.position.x > midFieldPoint;
    }

    IEnumerator PerformActionChances()
    {
        while (true)
        {
            if (GM.IsPlayingGet() && mc != null && mc.BP != null && mc.BP.isInteractable)
            {
                // Debug.Log("isPlaying");
                if (!isPerformingAbility) PerformAbility1Chance();
                if (!isPerformingAbility) PerformAbility2Chance();
                if (!isPerformingAbility) PerformAbility3Chance();
                PerformShootChance();
            } else if (mc != null)
            {
                // Debug.Log("IsPlayingGet: " + GM.IsPlayingGet());
            }
            
            yield return new WaitForSeconds(performActionChanceFrequency);
        }
    }

    protected void Setup()
    {
        mc = GetComponent<MonsterController>();
        rb = GetComponent<Rigidbody>();
        GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
        warriorGoal = GameObject.FindWithTag("WarriorGoal");
        monsterGoal = GameObject.FindWithTag("MonsterGoal");
        warriors = FindObjectsOfType<WarriorController>().Select(w => w.gameObject).ToList();


        StartCoroutine(PerformActionChances());
        // StartCoroutine(PrintWarriors());
    }

    public void SetIsPerformingAbility(bool isPerformingAbility)
    {
        this.isPerformingAbility = isPerformingAbility;
    }

    public IEnumerator SetIsPerformingAbilityDelay(bool isPerformingAbility)
    {
        yield return new WaitForSeconds(0.5f);

        this.isPerformingAbility = isPerformingAbility;
    }

    // Methods Transferred Over From AimonsterController

    #region Basic Attack
    // SPEHRICAL ATTACK METHODS
    //protected IEnumerator SphericalAttackNearestWarrior()
    //{
    //    Debug.Log("SphericalAttackNearestWarrior");
    //    WarriorController nearestWarrior = null;
    //    try
    //    {
    //        nearestWarrior = GetNearestWarrior(transform.position);
    //    }
    //    catch
    //    {
    //        nearestWarrior = null;
    //    }
    //    if (nearestWarrior == null)
    //    {
    //        Debug.Log("No warrior close enough to attack");
    //        isPerformingAbility = false;  // not performing ability so reset bool
    //        yield break;
    //    }

    //    StopCoroutines();
    //    while (isPerformingAbility)
    //    {
    //        // If targeted warrior died during ability charge, go to next warrior
    //        if (nearestWarrior == null)
    //        {
    //            Debug.Log("nearestWarrior: " + nearestWarrior);
    //            nearestWarrior = GetNearestWarrior(transform.position);
    //        }
    //        if (nearestWarrior == null)
    //        {
    //            Debug.Log("Break");
    //            break; // If going to next warrior didn't work because there are none, break
    //        }

    //        SphericalAttackHelper();
    //        Vector3 dir = (nearestWarrior.gameObject.transform.position - transform.position).normalized;
    //        // mc.movementDirection = new Vector3(dir.x, 0, dir.z);
    //        mc.movementDirection = Vector3.Lerp(mc.movementDirection, new Vector3(dir.x, 0, dir.z), Time.deltaTime * smoothFactor);
    //        //Debug.Log("GROUND CLIP TEST: DIR = " + mc.movementDirection);

    //        yield return null;
    //    }

    //    // In case where break happened, just flush ability stuff by using it
    //    if (isPerformingAbility)
    //    {
    //        Debug.Log("Flush spherical attack");
    //        //SphericalAttackHelper();
    //        AbilitySphericalAttack attack = (AbilitySphericalAttack)mc.abilities[1];
    //        attack.ChargeDown();
    //        isPerformingAbility = false;
    //        mc.movementDirection = Vector3.zero;
    //    }

    //}

    //protected void SphericalAttackHelper()
    //{
    //    // Debug.Log("SphericalAttack");
    //    // Make sure first ability is an AbilityChargable
    //    if (!(mc.abilities[1] is AbilitySphericalAttack)) return;

    //    AbilitySphericalAttack attack = (AbilitySphericalAttack)mc.abilities[1];

    //    // Check if off cooldown
    //    if (attack.GetTimer() < attack.GetCooldown()) return;

    //    attack.SetIsCharging(true);

    //    // If input is no longer true, attack
    //    if (ShouldSphericalAttack(attack) && attack.GetChargeAmount() > attackMinimumCharge)
    //    {
    //        // Debug.Log("Activate");
    //        attack.Activate();
    //        attack.ANIM.SetBool("isWindingUp", false);
    //        isPerformingAbility = false;
    //    }
    //    else if (attack.GetIsCharging() && attack.GetTimer() >= attack.GetCooldown()) // If it still is true, keep charging
    //    {
    //        // Debug.Log("ChargeUp");
    //        attack.ChargeUp();
    //    }
    //    else
    //    {
    //        // Debug.Log("ChargeDown");
    //        attack.ChargeDown();
    //    }
    //}

    protected void HandleChargeableAttack(AbilityChargeableAttack ability)
    {
        Debug.Log("HandleChargeableAttack");
        if (ability.GetTimer() < ability.GetCooldown()) return;

        ability.SetIsCharging(true);

        if (ShouldAttack(ability) && ability.GetChargeAmount() > attackMinimumCharge)
        {
            Debug.Log("Activate");
            ability.Activate();
            ability.ANIM.SetBool("isWindingUp", false);
            isPerformingAbility = false;
            attackCoroutine = null;
        }
        else if (ability.GetIsCharging() && ability.GetTimer() >= ability.GetCooldown() && mc.BP.ballOwner != gameObject)
        {
            ability.ChargeUp();
        }
        else
        {
            attackCoroutine = null;
            ability.ChargeDown();
            isPerformingAbility = false;
        }
    }


    //protected IEnumerator SphericalAttackBallController()
    //{

    //    // Ensure ball owner is not null
    //    if (mc.BP == null || mc.BP.ballOwner == null)
    //    {
    //        Debug.Log("BP or BP.ballOwner is null - don't attack");
    //        isPerformingAbility = false;  // not performing ability so reset bool
    //        yield break;
    //    }
    //    // Ensure ballOwner is in range
    //    if (Vector3.Distance(mc.BP.ballOwner.transform.position, transform.position) > maxProximityRange)
    //    {
    //        Debug.Log("Ball owner not close enough to attack");
    //        isPerformingAbility = false;  // not performing ability so reset bool
    //        yield break;
    //    }
    //    GameObject ballController = mc.BP.ballOwner;

    //    StopCoroutines();

    //    while (isPerformingAbility)
    //    {
    //        // If ballOwner died, just retarget to nearest warrior
    //        if (ballController == null) ballController = GetNearestWarrior(transform.position).gameObject;

    //        SphericalAttackHelper();
    //        Vector3 dir = (ballController.transform.position - transform.position).normalized;
    //        //mc.movementDirection = new Vector3(dir.x, 0, dir.z);
    //        mc.movementDirection = Vector3.Lerp(mc.movementDirection, new Vector3(dir.x, 0, dir.z), Time.deltaTime * smoothFactor);
    //        //Debug.Log("GROUND CLIP TEST: DIR = " + mc.movementDirection);
    //        yield return null;
    //    }
    //}

    protected IEnumerator ChargeableAttackController(GameObject target, AbilityChargeableAttack ability)
    {
        Debug.Log("ChargeableAttackController");
        //GameObject target = targetSelector(transform);

        if (target == null)
        {
            Debug.Log("No target to attack");
            isPerformingAbility = false;
            yield break;
        }

        // StopCoroutines();

        while (isPerformingAbility)
        {
            Debug.Log("Target: " + target.name);
            if (target == null)
            {
                //target = targetSelector(transform);
                target = GetNearestWarrior(transform.position);
                if (target == null) break;
            }

            HandleChargeableAttack(ability);

            if (Vector3.Distance(transform.position, target.transform.position) > ability.GetMinAttackDist())
            {
                Vector3 dir = (target.transform.position - transform.position).normalized;
                mc.movementDirection = Vector3.Lerp(mc.movementDirection, new Vector3(dir.x, 0, dir.z), Time.deltaTime * smoothFactor);
            } else
            {
                Vector3 dirToTarget = (target.transform.position - transform.position).normalized;
                dirToTarget = new Vector3(dirToTarget.x, 0, dirToTarget.z);
                Quaternion newRotation = Quaternion.LookRotation(dirToTarget, Vector3.up);
                transform.rotation = newRotation;
                mc.movementDirection = Vector3.zero;
            }

            yield return null;
        }

        if (isPerformingAbility)
        {
            ability.ChargeDown();
            isPerformingAbility = false;
            mc.movementDirection = Vector3.zero;
        }
    }


    protected bool WarriorIsInAttackSphereRadius(AbilitySphericalAttack attack)
    {
        Vector3 origin = new Vector3(transform.position.x, transform.position.y + attack.attackVisualOffsetY, transform.position.z);
        Collider[] colliders = Physics.OverlapSphere(origin + transform.forward * attack.attackRange, attack.attackBaseRadius
            + attack.GetChargeAmount() * attack.chargeRate, attack.affectedLayers);

        foreach (Collider col in colliders)
        {
            // Handle collision with each collider
            // Debug.Log("SphereCast hit " + col.gameObject.name);
            if (col.gameObject.CompareTag("Warrior"))
            {
                Debug.Log("Warrior in attack sphere radius");
                return true;
            }
        }
        return false;
    }

    //private bool ShouldSphericalAttack(AbilitySphericalAttack attack)
    //{
    //    // Should attack either if at full charge, or anytime a warrior is in attack radius
    //    return attack.GetChargeAmount() >= attack.maxChargeSeconds || WarriorIsInAttackSphereRadius(attack);
    //}

    private bool ShouldAttack(AbilityChargeableAttack ability)
    {
        return ability.GetChargeAmount() >= ability.maxChargeSeconds || ability.IsEnemyInRange(transform);
    }


    //protected void SphericalAttack(AttackMode mode)
    //{
    //    if (mode == AttackMode.BallOwner)
    //    {
    //        StartCoroutine(SphericalAttackBallController());
    //    }
    //    else if (mode == AttackMode.NearestWarrior)
    //    {
    //        StartCoroutine(SphericalAttackNearestWarrior());
    //    }
    //}

    protected void StartChargeableAttack(AttackMode mode)
    {
        Debug.Log("StartChargeableAttack");
        StopCoroutines();
        AbilityChargeableAttack ability = mc.abilities[1] as AbilityChargeableAttack;
        if (ability == null) return;
        StopCoroutines();

        if (mode == AttackMode.BallOwner)
        {
            Debug.Log("StartChargeableAttack: AttackMode BallOwner");
            GameObject target = mc.BP?.ballOwner != null &&
                                 Vector3.Distance(mc.BP.ballOwner.transform.position, transform.position) <= maxProximityRange
                                 ? mc.BP.ballOwner
                                 : null;
            attackCoroutine = StartCoroutine(ChargeableAttackController(target, ability));
        }
        else if (mode == AttackMode.NearestWarrior)
        {
            Debug.Log("StartChargeableAttack: AttackMode NearestWarrior");
            WarriorController nearest = GetNearestWarrior(transform.position)?.GetComponent<WarriorController>();
            if (nearest != null)
            {
                GameObject target = nearest != null && Vector3.Distance(nearest.transform.position, transform.position) < maxProximityRange ? nearest.gameObject : null;
                attackCoroutine = StartCoroutine(ChargeableAttackController(target, ability));
            }
        } else
        {
            Debug.Log("StartChargeableAttack: ERROR - No AttackMode");
        }
    }

    private void StopChargeableAttack()
    {
        if (attackCoroutine != null)
        {
            Debug.Log("StopChargeableAttack");
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;

            // Flush & Reset ability
            AbilityChargeableAttack ACA = GetComponent<AbilityChargeableAttack>();
            if (ACA != null)
            {
                ACA.Activate();
                isPerformingAbility = false;
                ACA.ChargeDown();
            }
        }
    }
    #endregion Basic Attack

    // GENERAL METHODS

    #region General Methods
    protected GameObject GetNearestWarrior(Vector3 pos)
    {
        GameObject nearestWarrior = null;
        float distToNearestWarrior = float.MaxValue;

        foreach (GameObject warrior in warriors)
        {
            if (!warrior.activeSelf || !warrior.CompareTag("Warrior")) continue;
            float distanceToWarrior = Vector3.Distance(pos, warrior.transform.position);
            if (distanceToWarrior < distToNearestWarrior)
            {
                nearestWarrior = warrior;
                distToNearestWarrior = distanceToWarrior;
            }
        }

        if (nearestWarrior != null && nearestWarrior.TryGetComponent<WarriorController>(out var wc) && wc.GetIsDead()) Debug.LogWarning("Targeting dead warrior!");
        return nearestWarrior;
    }

    protected virtual void StopCoroutines()
    {
        StopPursuing();
        StopRoaming();
        StopDefendGoal();
        StopChargeableAttack();
    }

    protected void StopPursuing()
    {
        if (pursueCoroutine != null)
        {
            Debug.Log("Stop pursuing");
            StopCoroutine(pursueCoroutine);
            pursueCoroutine = null;
        }
    }

    protected void StopRoaming()
    {
        if (roamCoroutine != null)
        {
            Debug.Log("Stop roaming");
            StopCoroutine(roamCoroutine);
            roamCoroutine = null;
        }
    }

    protected void StopDefendGoal()
    {
        if (defendGoalCoroutine != null)
        {
            Debug.Log("Stop defending goal");
            StopCoroutine(defendGoalCoroutine);
            defendGoalCoroutine = null;
        }
    }

    protected void EnsureBallOwnerValid()
    {
        if (mc == null) GetComponent<MonsterController>();

        if (mc.BP == null)
        {
            mc.BP = FindObjectOfType<BallProperties>();
        }

        if (mc.Ball == null) mc.Ball = mc.BP.gameObject;
    }

    protected void ResetAbilities()
    {
        // Debug.Log("Reset Abilities");
        if (isPerformingAbility) isPerformingAbility = false;

        //if (mc.abilities[2] is AbilityBullrush)
        //{
        //    AbilityBullrush abr = (AbilityBullrush)mc.abilities[2];
        //    if (abr.GetIsCharging() || abr.GetIsAutoCharging())
        //    {
        //        abr.SetIsAutoCharging(false);
        //        abr.ChargeDown();
        //        abr.SetInputBufferTimer(0);
        //        abr.SetIsCharging(false);
        //        //abr.SetTimer(0);
        //    }
        //}

        //if (mc.abilities[1] is AbilityChargeableAttack)
        //{
        //    AbilityChargeableAttack attack = (AbilityChargeableAttack)mc.abilities[1];

        //    if (attack.GetIsAutoCharging() || attack.GetIsCharging())
        //    {
        //        attack.SetIsAutoCharging(false);
        //        attack.SetIsCharging(false);
        //        attack.SetInputBufferTimer(0);
        //        attack.ChargeDown();
        //        //attack.SetTimer(0);
        //    }
        //}

        foreach (AbilityScript ab in mc.abilities)
        {
            if (ab is AbilityChargeable abc)
            {
                if (abc.GetIsCharging() || abc.GetIsAutoCharging())
                {
                    abc.SetIsAutoCharging(false);
                    abc.ChargeDown();
                    abc.SetInputBufferTimer(0);
                    abc.SetIsCharging(false);
                    //abr.SetTimer(0);
                }
            }
        }
    }

    protected float GetDistanceToNearestWarrior()
    {
        float distToNearestWarrior = float.MaxValue;
        foreach (GameObject warrior in warriors)
        {
            float distanceToWarrior = Vector3.Distance(transform.position, warrior.transform.position);
            if (distanceToWarrior < distToNearestWarrior)
            {
                distToNearestWarrior = distanceToWarrior;
            }
        }
        return distToNearestWarrior;
    }

    protected void MoveTo(Vector2 targetPos)
    {
        // Debug.Log("MoveTo: " + targetPos);
        if (targetPos != Vector2.zero)
        {
            //usingKeyboard = true;
            mc.movementDirection = new Vector3(targetPos.x, 0, targetPos.y).normalized;
            //Debug.Log("GROUND CLIP TEST: DIR = " + mc.movementDirection);
            mc.aimingDirection = mc.movementDirection;
        }


        rb.velocity = GM.isPlaying ? mc.movementDirection * mc.monsterSpeed : Vector3.zero;
        //rb.velocity = isCharging ? rb.velocity * chargeMoveSpeedMult : rb.velocity;
        if (rb.velocity != Vector3.zero)
        {
            Quaternion newRotation = Quaternion.LookRotation(mc.movementDirection.normalized, Vector3.up);
            transform.rotation = newRotation;
        }

        if (mc.movementDirection != Vector3.zero && GM.isPlaying)
        {
            mc.ANIM.SetBool("isWalking", true);
        }
        else
        {
            mc.ANIM.SetBool("isWalking", false);
        }

    }

    protected void Shoot()
    {
        if (mc.BP.ballOwner == gameObject)
        {
            Debug.Log("Shoot!");

            // Prevent ball from getting kicked "through" walls
            if (mc != null && mc.BP != null && mc.IsWallBetweenBallAndPlayer())
            {
                Debug.Log("Correcting ball position before kick");
                mc.BP.gameObject.transform.position =
                    new Vector3(transform.position.x, mc.BP.gameObject.transform.position.y, transform.position.z); // Ignore Y axis
            }

            // Make monster look at goal
            Quaternion newRotation =
                Quaternion.LookRotation((warriorGoal.transform.position - transform.position).normalized, Vector3.up);
            transform.rotation = newRotation;

            // Debug.Log("ballOwner set to null");
            mc.BP.ballOwner = null;
            mc.BP.previousKicker = gameObject;
            mc.BP.lastKicker = gameObject;
            canPickUpBall = false;
            StartCoroutine(SetPickUpBallTrue());
            // Debug.Log(transform.forward);
            float distFromGoalMultiplier = Vector3.Distance(warriorGoal.transform.position, transform.position) / (maxShootingRange / 2f);
            mc.BP.GetComponent<Rigidbody>().AddForce(transform.forward * aiShootSpeed);// * distFromGoalMultiplier);
            //audioPlayer.PlaySoundRandomPitch(audioPlayer.Find("pass"));
        }
    }

    IEnumerator SetPickUpBallTrue()
    {
        yield return new WaitForSeconds(0.2f);
        canPickUpBall = true;
    }

    public bool GetCanPickUpBall()
    {
        return canPickUpBall;
    }

    protected bool BallGoingTowardOwnGoal()
    {
        GameObject ball = mc.BP.gameObject;
        if (ball == null) return false;

        Rigidbody ballRB = ball.GetComponent<Rigidbody>();
        Vector3 ballToGoal = (monsterGoal.transform.position - ball.transform.position).normalized;
        float minVelocity = 1f;
        float minDistance = 8f;
        float directionalAlignmentThreshold = 0.5f;

        // If ball velocity is high enough, is within certain distance to own goal, and velocity is toward own goal, return true
        if (ballRB.velocity.magnitude > minVelocity && Vector3.Distance(ball.transform.position, monsterGoal.transform.position) < minDistance
            && Vector3.Dot(ballRB.velocity.normalized, ballToGoal) > directionalAlignmentThreshold)
        {
            // Debug.Log("Ball is going toward monster goal");
            return true;
        }

        return false;
    }

    protected void LookInDirection(Vector3 dir)
    {
        Debug.Log("LookInDirection: " + dir);
        Quaternion newRotation = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = newRotation;
    }

    protected void SetupFixedUpdate()
    {
        if (stateChanged)
        {
            Debug.Log("State changed");
            //StopCoroutines();
            stateChanged = false;
        }
    }

    #endregion General Methods

    #region Default Behavior Methods

    protected IEnumerator DefendGoal()
    {
        while (true)
        {
            Vector3 dir = (GetDefendGoalPosition() - transform.position).normalized;
            mc.movementDirection = new Vector3(dir.x, 0, dir.z); // Stand in between goal and ball owner
            //Debug.Log("GROUND CLIP TEST: DIR = " + mc.movementDirection);
            //defendGoalCoroutine = null;
            yield return null;
            yield return new WaitForSeconds(defendGoalDelay);
        }
    }

    // Defend goal position is in the middle of the ballOwner/ball and the goal
    protected Vector3 GetDefendGoalPosition()
    {
        if (mc.BP == null) return new Vector3(transform.position.x, transform.position.y, transform.position.z);
        GameObject targetToDefend = mc.BP.ballOwner;
        if (targetToDefend == null) targetToDefend = mc.BP.gameObject;
        Vector3 vec = targetToDefend.transform.position - monsterGoal.transform.position;
        Vector3 dir = vec.normalized;
        float distance = vec.magnitude;
        Vector3 defendPos = monsterGoal.transform.position + (dir * distance / 2);
        Vector3 defendPosIgnoreY = new Vector3(defendPos.x, transform.position.y, defendPos.z);
        // Debug.Log(defendPos);
        return defendPosIgnoreY;
    }

    protected void StartDefendGoal()
    {
        // if (isPerformingAbility) return;
        if (defendGoalCoroutine == null)
        {
            Debug.Log("Start Defend Goal");
            StopCoroutines();
            defendGoalCoroutine = StartCoroutine(DefendGoal());
        }
    }

    protected void StartPursuing()
    {
        if (isPerformingAbility) return;
        if (pursueCoroutine == null)
        {
            Debug.Log("Start pursuing");
            StopCoroutines();
            pursueCoroutine = StartCoroutine(PursuePlayer());
        }
    }

    private IEnumerator PursuePlayer()
    {
        while (true)
        {
            Debug.Log("Pursuing player");
            
            // Ensure the ball owner is valid before pursuing
            if (mc.BP.ballOwner != null)
            {
                Debug.Log("Pursuing player, ball owner is valid");
                Vector3 targetPosition = mc.BP.ballOwner.transform.position;
                targetPosition = new Vector3(targetPosition.x, transform.position.y, targetPosition.z); // Ignore y
                float distanceToPlayer = Vector3.Distance(targetPosition, transform.position);

                // Check if the monster is too close; stop if within minimum distance
                if (distanceToPlayer > minPursueDistance)
                {
                    // Calculate target direction
                    Vector3 targetDirection = (targetPosition - transform.position).normalized;
                    Vector3 targetDirectionIgnoreY = new Vector3(targetDirection.x, 0, targetDirection.z);

                    // Smoothly update the movement direction using linear interpolation
                    mc.movementDirection = targetDirectionIgnoreY;
                    //Debug.Log("GROUND CLIP TEST: DIR = " + mc.movementDirection);
                    //Vector3.Lerp(mc.movementDirection, targetDirection, Time.deltaTime * pursuitSmoothingFactor);
                }
                else
                {
                    // Stop moving if too close to the player
                    mc.movementDirection = Vector3.zero;
                }
            }
            else
            {
                // If ball owner is null, stop movement
                mc.movementDirection = Vector3.zero;
            }

            yield return null; // Continue to next frame
            yield return new WaitForSeconds(pursueDelay);
        }
    }

    // ROAM METHODS
    private IEnumerator Roam()
    {
        while (true)
        {
            Debug.Log("Monster roaming");

            // Determine a random position within the left half of the field
            float randomX = UnityEngine.Random.Range(leftBoundary, midFieldPoint);
            float randomZ = UnityEngine.Random.Range(-fieldDepth, fieldDepth);
            Vector3 randomTargetPosition = new Vector3(randomX, transform.position.y, randomZ);

            // If there is a ball owner, bias the random target position toward it
            if (mc.BP.ballOwner != null)
            {
                Vector3 ballOwnerPosition = mc.BP.ballOwner.transform.position;

                // Calculate the bias factor (range: 0 = no bias, 1 = full bias)
                float biasFactor = 0.35f; // Adjust this value to control how strongly it biases toward the ball owner
                randomTargetPosition = Vector3.Lerp(randomTargetPosition, new Vector3(ballOwnerPosition.x, transform.position.y, ballOwnerPosition.z), biasFactor);
                randomTargetPosition = new Vector3(randomTargetPosition.x, transform.position.y, randomTargetPosition.z); // Ignore y
            }

            // Move toward the random target position
            while (Vector3.Distance(transform.position, randomTargetPosition) > stoppingDistance)
            {
                if (mc.isStunned) yield break;

                // Calculate direction and move toward the target position
                Vector3 directionToTarget = (randomTargetPosition - transform.position).normalized;
                Vector3 directionToTargetIgnoreY = new Vector3(directionToTarget.x, transform.position.y, directionToTarget.z);
                mc.movementDirection = directionToTargetIgnoreY;
                rb.velocity = mc.movementDirection * mc.monsterSpeed;
                //Debug.Log("GROUND CLIP TEST: DIR = " + mc.movementDirection);

                // Rotate the monster to face the direction it's moving
                Quaternion newRotation = Quaternion.LookRotation(directionToTargetIgnoreY, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * rotationSpeed);

                yield return null;
            }

            // Prevent monster from moving once it reached its spot
            mc.movementDirection = Vector3.zero;
            rb.velocity = Vector3.zero;

            // Pause briefly before picking a new random position
            yield return new WaitForSeconds(waitInPlaceTime);
        }
    }

    protected void StartRoaming()
    {
        if (isPerformingAbility) return;
        if (roamCoroutine == null)
        {
            Debug.Log("Start roaming");
            StopCoroutines();
            roamCoroutine = StartCoroutine(Roam());
        }
    }

    protected void WiggleTowardGoal()
    {
        if (isPerformingAbility) return;
        // Debug.Log("WiggleTowardGoal");
        Vector3 goalPosition = warriorGoal.transform.position;

        // Update the timer
        redirectionTimer += Time.deltaTime;

        // Check if it's time to update the random offset
        if (redirectionTimer >= redirectionInterval)
        {
            // Debug.Log("new offset");
            // Reset the timer
            redirectionTimer = 0f;

            // Generate a new random offset for "wiggle" effect
            Vector3 newRandomOffset = new Vector3(
                UnityEngine.Random.Range(-wiggleOffset, wiggleOffset),  // Random x offset
                0,                          // Keep y as zero for ground-based movement
                UnityEngine.Random.Range(-wiggleOffset, wiggleOffset)   // Random z offset
            );

            // Smooth transition to the new offset
            currentRandomOffset = Vector3.Lerp(currentRandomOffset, newRandomOffset, redirectionSmoothness);
        }

        // Calculate the base direction toward the goal
        Vector3 toGoal = (goalPosition - transform.position).normalized;
        Vector3 toGoalIgnoreY = new Vector3(toGoal.x, 0, toGoal.z);

        // Apply smoothed offset to movement direction
        mc.movementDirection = Vector3.Lerp(mc.movementDirection, (toGoalIgnoreY + currentRandomOffset).normalized, Time.deltaTime * smoothFactor);
        //Debug.Log("GROUND CLIP TEST: DIR = " + mc.movementDirection);

        // Update walking animation if applicable
        mc.ANIM.SetBool("isWalking", rb.velocity != Vector3.zero);
    }

    #endregion Default Behavior Methods

    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log("AiMonsterController start");
        
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("AiMonsterController update");
    }

    IEnumerator PrintWarriors()
    {
        Debug.Log("Print Warriors");
        while (true)
        {
            yield return new WaitForSeconds(1f);
            Debug.Log("Warriors: " + warriors.Count);
        }
    }
}
