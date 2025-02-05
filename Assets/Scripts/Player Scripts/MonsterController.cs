using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class MonsterController : MonoBehaviour
{
    [HideInInspector] public Rigidbody rb;
    [SerializeField] public GameObject Ball = null;
    public BallProperties BP = null;
    public List<AbilityScript> abilities;
    public PassiveAbility[] passiveAbilities;
    //public GameObject wallPrefab;
    //public GameObject shrapnelPrefab;

    [SerializeField] public float monsterSpeed = 2f;
    [HideInInspector] public Vector3 movementDirection;
    public Vector3 aimingDirection;
    private Vector3 rightStickInput;

    //Temp Controller Scheme Swap
    public bool usingNewScheme = false;
    public InputAction monsterControls;

    //Make True If Using Keyboard For Movement
    public bool usingKeyboard = false;
    public bool invertControls = false;

    [SerializeField] private GameObject ballPosition;

    [Header("Stats")]
    [SerializeField] private float passSpeed = 5.0f;
    [SerializeField] private float kickSpeed = 5.0f;
    //[SerializeField] private float attackHitForce = 200f;
    [SerializeField] private float chargeMultiplier = 0.5f;
    [SerializeField] private float maxChargeSeconds = 2f;
    [SerializeField] private float chargeMoveSpeedMult = 0.2f;
    [SerializeField] private float stunTime = 3f;
    [SerializeField] private float stunSpeed = 0.2f;
    private float kickCharge = 1f;
    public bool isCharging;
    public bool isChargingAbility;
    [Header("Ability Stats")]
    //[SerializeField] private float dashSpeed = 500.0f;
    //[SerializeField] private float dashCooldown = 1f;
    //[SerializeField] private float dashDuration = 0.35f;
    //[SerializeField] private float maxDashChargeSeconds = 2f;
    //private float lastDashTime = -1f;
    [HideInInspector] public bool isDashing = false;
    //private float dashCharge = 1f;
    //private bool isChargingDash = false;
    [HideInInspector] public bool isStunned = false;
    [HideInInspector] public bool canBeStunned = true;
    public bool isIntangible = false;

    [SerializeField] private bool canMove = true;
    public GameplayManager GM = null;
    
    private UIManager UM = null;
    private PlayerAttachedUI PAUI = null;
    private StatTracker ST = null;

    public Animator ANIM;
    private AudioPlayer audioPlayer;
    public GameObject monsterSpawner = null;
    public LayerMask layerMask;
    private CommentatorSoundManager CSM;
    public GameObject spriteObject;
    private Vector3 spriteScale;
    private MultipleTargetCamera MTC;
    //public GameObject attackVisual;
    //private float attackVisualOffsetY = -0.3f;

    private bool shouldShake1 = true;
    private bool shouldShake2 = true;
    [SerializeField] private float shakeIntensity = 1.0f;

    // If there is no ball owner yet a warrior or monster is on top of wall, OnTriggerStay will wait this long until making that character pick up ball
    private float pickupBallCooldown = 0.5f;
    private float pickupBallTimer = 0f;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
        ST = GameObject.Find("Stat Tracker").GetComponent<StatTracker>();
        Ball = GameObject.Find("Ball");
        BP = (BallProperties) Ball.GetComponent("BallProperties");
        MTC = GameObject.Find("Main Camera").GetComponent<MultipleTargetCamera>();
        MTC.AddTarget(transform);
        CSM = GameObject.Find("CommentatorSounds").GetComponent<CommentatorSoundManager>();
        ANIM = GetComponentInChildren<Animator>();
        audioPlayer = GetComponent<AudioPlayer>();
        monsterSpawner = GameObject.Find("MonsterSpawner");
        transform.position = monsterSpawner.transform.position;
        //wallTimer = wallCooldown;
        spriteScale = spriteObject.transform.localScale;
        abilities = new List<AbilityScript> { null, null, null };
        passiveAbilities = GetComponents<PassiveAbility>();
        StartCoroutine(RemoveNullAbilities());

    }

    void Start()
    {
        UM = GameObject.Find("Canvas").GetComponent<UIManager>();
        PAUI = GetComponentInChildren<PlayerAttachedUI>();
        UM.ShowMonsterUI(true);
        pickupBallTimer = pickupBallCooldown;
    }

    // Temp Controller Scheme Swap
    private void OnEnable()
    {
        monsterControls.Enable();
    }

    // Temp Controller Scheme Swap
    private void OnDisable()
    {
        monsterControls.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("movementDirection: " + movementDirection);
        // Debug.Log("Input.GetAxis(Horizontal): " + Input.GetAxis("Horizontal"));
        // Debug.Log("Input.GetAxis(Vertical): " + Input.GetAxis("Vertical"));
        if (canMove)
        {
            //Movement();
        }
        //Movement();
        if (GM.isPlaying && !GM.isPaused)
        {  
            Dribbling();
            Passing();
            Kicking();
            RotateWhileCharging();
            Dash();
            
            //ResizeAttackVisual();
            

            if (Input.GetKey(KeyCode.Z))
            {
                abilities[1].Activate();
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                BuildWall();
            }
            
            /*
            if (isChargingDash)
            {
                ChargeDashing();
            }

            
            if (isChargingAttack)
            {
                ChargeAttack();

                if (!attackVisual.activeSelf) attackVisual.SetActive(true);
            } else
            {
                if (attackVisual.activeSelf) attackVisual.SetActive(false);
            }
            */

            // TESTING STUN
            if (Input.GetKeyDown(KeyCode.T))
            {
                //Stun();
            }

        } else { rb.velocity = new Vector3(0, 0, 0); } // ensure monster momentum is killed when not playing

        InvincibilityFlash();

        if ((isStunned || isIntangible) && BP.ballOwner == this.gameObject)
        {
            // Debug.Log("ballOwner set to null");
            BP.ballOwner = null;
        }

        //Temp Controller Scheme Swap
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            usingNewScheme = !usingNewScheme;
            invertControls = !invertControls;
        }


        //if (kickCharge >= maxChargeSeconds)
        //{
        //    BP.StartBallGlow(2);
        //    if (shouldShake2)
        //    {
        //        shouldShake2 = false;
        //        StartCoroutine(MTC.ScreenShake(shakeIntensity * 2));
        //    }
        //}
        //else if (kickCharge > (maxChargeSeconds / 2) + 0.5f)
        //{
        //    BP.StartBallGlow(1);
        //    if (shouldShake1)
        //    {
        //        shouldShake1 = false;
        //        StartCoroutine(MTC.ScreenShake(shakeIntensity));
        //    }
        //}
        if (isCharging)
        {
            float glowAmount = kickCharge - 1.0f;
            BP.StartBallGlow(glowAmount);
            if (kickCharge >= maxChargeSeconds && shouldShake2)
            {
                shouldShake2 = false;
                StartCoroutine(MTC.ScreenShake(shakeIntensity * 2));
            } else if (kickCharge > (maxChargeSeconds / 2) + 0.5f && shouldShake1)
            {
                shouldShake1 = false;
                StartCoroutine(MTC.ScreenShake(shakeIntensity));
            }
        }
        else
        {
            BP.StopBallGlow();
            //MTC.isShaking = false;
            shouldShake1 = true;
            shouldShake2 = true;
        }

    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            Movement();
        }
        
    }

    

    void Movement()
    {
        if (isDashing) return;

        // Ignore inputs if Warrior is AI
        // movementDirection will be set manually by AiMonsterController
        if (GetComponent<AiMonsterController>() == null)
        {
            float horizontalInput = 0f;
            float verticalInput = 0f;

            // Check for WASD keys
            if (Input.GetKey(KeyCode.UpArrow))
            {
                verticalInput = 1f;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                verticalInput = -1f;
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                horizontalInput = 1f;
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                horizontalInput = -1f;
            }
            Vector2 keyBoardInputs = new Vector2(horizontalInput, verticalInput);
            if (keyBoardInputs != Vector2.zero)
            {
                usingKeyboard = true;
                movementDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;
                aimingDirection = movementDirection;
            }
            else if (usingKeyboard)
            {
                movementDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;
            }
        }

        Vector3 dir = isStunned ? -movementDirection : movementDirection;
        rb.velocity = GM.isPlaying ? dir * monsterSpeed : Vector3.zero;
        rb.velocity = isCharging || isChargingAbility ? rb.velocity * chargeMoveSpeedMult : rb.velocity;
        rb.velocity = isStunned ? rb.velocity * stunSpeed : rb.velocity;
        if (rb.velocity != Vector3.zero && !isCharging) 
        {
            Quaternion newRotation = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = newRotation;
        }

        if (movementDirection != Vector3.zero && GM.isPlaying)
        {
            ANIM.SetBool("isWalking", true);
        }
        else
        {
            ANIM.SetBool("isWalking", false);
        }
    }

    void Dribbling()
    {
        if (BP.ballOwner == gameObject)
        {
            PAUI.ShowChargeBar(true);           
            
            //UM.ShowChargeBar(true);
            UM.UpdateChargeBarText("Monster");
            Ball.transform.position = ballPosition.transform.position; // new Vector3(transform.position.x, 2, transform.position.z);
        }
    }

    void Passing()
    {
        if (GetComponent<AiMonsterController>() != null)
        {
            // Debug.Log("Passing : skip method because monster is AI");
            return;
        }

        if(Input.GetKeyDown(KeyCode.RightShift) && BP.ballOwner == gameObject)
        {
            Debug.Log("Pass!");

            // 
            BP.ballOwner = null;
            Debug.Log(transform.forward);
            BP.GetComponent<Rigidbody>().AddForce(transform.forward * passSpeed);
            audioPlayer.PlaySoundRandomPitch(audioPlayer.Find("pass"));
        } else if (Input.GetKeyDown(KeyCode.RightShift))
        {
            Debug.Log("Attempt to pass failed, ballOwner: " + BP.ballOwner);
        }
    }

    void Kicking()
    {
        if (GetComponent<AiMonsterController>() != null)
        {
            // Debug.Log("Kicking : skip method because monster is AI");
            return;
        }

        if (!usingNewScheme)
        {
            if (((rightStickInput == Vector3.zero && !usingKeyboard) || Input.GetKeyUp(KeyCode.KeypadEnter)) && BP.ballOwner == gameObject && kickCharge != 1)
            {
                Debug.Log("Kick!");

                // 
                BP.ballOwner = null;
                BP.lastKicker = gameObject;
                Debug.Log(kickCharge);
                float kickForce = kickSpeed * (kickCharge * chargeMultiplier);
                Vector3 forceToAdd = aimingDirection * kickForce;
                BP.GetComponent<Rigidbody>().AddForce(forceToAdd);

                PAUI.ShowChargeBar(false);
                PAUI.UpdateChargeBar(0f);

                //Outdated
                //UM.ShowChargeBar(false);
                //UM.UpdateChargeBar(0f);

                PlayKickSound(kickCharge);
                StartCoroutine(KickDelay());
                ANIM.SetBool("isWindingUp", false);
                ANIM.Play("MinotaurAttack");
            }
            if (((rightStickInput != Vector3.zero && !usingKeyboard) || Input.GetKey(KeyCode.KeypadEnter)) && BP.ballOwner == gameObject)
            {
                if (kickCharge <= maxChargeSeconds)
                {
                    //Debug.Log(kickCharge);
                    PAUI.UpdateChargeBar((kickCharge - 1) / (maxChargeSeconds - 1));
                    //UM.UpdateChargeBar((kickCharge - 1) / (maxChargeSeconds - 1));
                    kickCharge += Time.deltaTime;
                    isCharging = true;
                    ANIM.SetBool("isWindingUp", true);
                }

                if (kickCharge > maxChargeSeconds)
                {
                    PAUI.UpdateChargeBar(1f);
                    //UM.UpdateChargeBar(1f);
                }
            }
            else
            {
                kickCharge = 1f;
                isCharging = false;
                aimingDirection = Vector3.zero;
            }
        } else
        {
            if (((monsterControls.phase == InputActionPhase.Canceled || monsterControls.WasReleasedThisFrame()) || Input.GetKeyUp(KeyCode.KeypadEnter)) && BP.ballOwner == gameObject && kickCharge != 1)
            {
                Debug.Log("Kick!");

                // Debug.Log("ballOwner set to null");
                BP.ballOwner = null;
                BP.lastKicker = gameObject;
                Debug.Log(kickCharge);
                float kickForce = kickSpeed * (kickCharge * chargeMultiplier);
                Vector3 forceToAdd = aimingDirection * kickForce;
                BP.GetComponent<Rigidbody>().AddForce(forceToAdd);

                PAUI.ShowChargeBar(false);
                PAUI.UpdateChargeBar(0f);

                //UM.ShowChargeBar(false);
                //UM.UpdateChargeBar(0f);
                PlayKickSound(kickCharge);
                StartCoroutine(KickDelay());
                ANIM.SetBool("isWindingUp", false);
                ANIM.Play("MinotaurAttack");
            }
            if ((monsterControls.IsInProgress() || Input.GetKey(KeyCode.KeypadEnter)) && BP.ballOwner == gameObject)
            {
                if (kickCharge <= maxChargeSeconds)
                {
                    //Debug.Log(kickCharge);
                    PAUI.UpdateChargeBar((kickCharge - 1) / (maxChargeSeconds - 1));
                    //UM.UpdateChargeBar((kickCharge - 1) / (maxChargeSeconds - 1));
                    kickCharge += Time.deltaTime;
                    isCharging = true;
                    ANIM.SetBool("isWindingUp", true);
                }

                if (kickCharge > maxChargeSeconds)
                {
                    PAUI.UpdateChargeBar(1f);
                    //UM.UpdateChargeBar(1f);
                }
            }
            else
            {
                kickCharge = 1f;
                isCharging = false;
                //aimingDirection = Vector3.zero;
            }
        }
    }

    void RotateWhileCharging()
    {
        if (isCharging)
        {
            if (aimingDirection !=  Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(aimingDirection);
                rb.rotation = targetRotation;
            }
        }
    }

    // Vector3 startAngle = transform.forward
    void Attack()
    {
        /*
        if (BP != null && BP.ballOwner != gameObject && GM.isPlaying)
        {    
            Debug.Log("Attack!");

            Debug.Log(attackCharge);
            Vector3 origin = new Vector3(transform.position.x, transform.position.y + attackVisualOffsetY, transform.position.z);
            Collider[] colliders = Physics.OverlapSphere(origin + transform.forward * attackRange, attackBaseRadius + attackCharge * attackChargeRate, layerMask);

            foreach (Collider col in colliders)
            {
                // Handle collision with each collider
                Debug.Log("SphereCast hit " + col.gameObject.name);
                if (col.gameObject.CompareTag("Warrior"))
                {
                    WarriorController WC = col.GetComponent<WarriorController>();
                    if (!WC.isInvincible)
                        WC.Die();
                    else
                        Debug.Log("Warrior is invincible");
                }
                if (col.gameObject.CompareTag("Ball") && BP.ballOwner == null)
                {
                    // SWIPE AWAY BALL - UNUSED FOR NOW
                    
                    Debug.Log("AXE HIT BALL!");
                    float kickForce = attackHitForce;
                    Vector3 posA = new Vector3(BP.gameObject.transform.position.x, 0f, BP.gameObject.transform.position.z);
                    Vector3 posB = new Vector3(transform.position.x, 0f, transform.position.z);
                    Vector3 dir = (posA - posB).normalized;
                    Vector3 forceToAdd = dir * kickForce;
                    BP.GetComponent<Rigidbody>().AddForce(forceToAdd);
                    
                }
            }
            
            if (attackCharge < maxAttackChargeSeconds)
            {
                audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find("minotaurAxeAttack"), 0.7f);
            } else
            {
                audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find("minotaurAxeAttackCharged"), 0.7f);
                if (canSpawnShrapnelOnAttack) SpawnShrapnel();
            }
            lastAttackTime = Time.time;
            attackCharge = 0;
            isChargingAttack = false;
            ANIM.Play("MinotaurAttack");

            StartCoroutine(MoveDelay());
        }
        */
    }

    void Dash()
    {
        /*
        if (BP.ballOwner == gameObject || isStunned) return; // ensure no dashing or dash charging when you have ball
        if (!GM.isPlaying)
        {
            isChargingDash = false;
            dashCharge = 0;
            return;
        }

        if (Time.time - lastDashTime >= dashCooldown)
        {
            // If R input is no longer true, dash
            if (Input.GetKeyUp(KeyCode.R))
            {
                // Check if enough time has passed since the last slide

                if (movementDirection != Vector3.zero && BP.ballOwner != gameObject && !isStunned)
                {
                    Debug.Log("Dashing");
                    isDashing = true;
                    ANIM.SetBool("isWindingUp", false);
                    ANIM.Play("MinotaurCharge");
                    audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find("minotaurDash"), 0.75f);

                    // Add force in direction of the player input for this warrior (movementDirection)
                    Vector3 dashVelocity = movementDirection.normalized * dashCharge * dashSpeed;
                    Debug.Log("Dash Charge: " + dashCharge);
                    rb.AddForce(dashVelocity);

                    Invoke("StopDashing", dashDuration);
                }
                else
                {
                    Debug.Log("Dash failed");
                }
                // Update the last dash time
                lastDashTime = Time.time;
                dashCharge = 0;
                isChargingDash = false;
            }
            else if (Input.GetKey(KeyCode.R)) // If it still is true, keep charging
            {
                isChargingDash = true;
            }
        }
        */
    }

    void ChargeDashing()
    {
        /*
        if (isStunned) return;
        if (dashCharge < maxDashChargeSeconds)
        {
            Debug.Log("Charging dash");
            ANIM.SetBool("isWindingUp", true);
            dashCharge += Time.deltaTime;
            isChargingDash = true;
            if (audioPlayer.source.clip == null || (!audioPlayer.isPlaying() && !audioPlayer.source.clip.name.Equals("minotaurDashCharge")))
            {
                audioPlayer.PlaySoundVolume(audioPlayer.Find("minotaurDashCharge"), 0.65f);
            }
        }
        */
    }

    void ChargeAttack()
    {
        /*
        if (isStunned) return;
        if (attackCharge < maxAttackChargeSeconds)
        {
            // Debug.Log("charging attack");
            if (audioPlayer.source.clip == null || audioPlayer.source.clip != audioPlayer.Find("minotaurAxeCharge"))
            {
                audioPlayer.PlaySoundVolume(audioPlayer.Find("minotaurAxeCharge"), 0.5f);
            }
            attackCharge += Time.deltaTime;
            isChargingAttack = true;
        }
        */
    }

    /*
    void StopDashing()
    {
        
        Debug.Log("No longer dashing");
        // ANIM.SetBool("isSliding", false);
        isDashing = false;
        
    }
    */

    void BuildWall()
    {
        abilities[0].Activate();
    }

    public void Stun()
    {
        if (isStunned || !canBeStunned) return;
        BP.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
        isStunned = true;
        audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find("minotaurStun"), 0.5f);
        CSM.PlayDeathSound(false);
        StartCoroutine(ResetStun());
    }

    private IEnumerator ResetStun()
    {
        yield return new WaitForSeconds(stunTime);
        isStunned = false;
    }

    void PlayKickSound(float charge)
    {
        if (charge >= maxChargeSeconds)
        {
            audioPlayer.PlaySoundRandomPitch(audioPlayer.Find("kick3"));
        }
        else if (charge >= maxChargeSeconds / 2f)
        {
            audioPlayer.PlaySoundRandomPitch(audioPlayer.Find("kick2"));
        } else
        {
            audioPlayer.PlaySoundRandomPitch(audioPlayer.Find("kick1"));
        }
    }

    public void ResetPlayer()
    {
        gameObject.transform.position = monsterSpawner.transform.position;
        isIntangible = false;
        for (int i = 0; i < abilities.Count; i++)
        {
            if (abilities[i] != null) abilities[i].Deactivate();
        }
        if (passiveAbilities.Length > 0)
        {
            for (int i = 0; i< passiveAbilities.Length; i++)
            {
                passiveAbilities[i].Deactivate();
            }
        }

        // If playing Quetz, reset it's segments
        AbilitySnakeSegments segments = GetComponent<AbilitySnakeSegments>();
        if (segments != null)
        {
            segments.ResetSegments();
        }
        // If playing Sphinx, reset mummies
        AiMummyManager AMM = GetComponent<AiMummyManager>();
        if (AMM != null)
        {
            AMM.ResetMummies();
        }
        // If playing Gasha, reset hand BP
        GashadokuroHand[] GH = GameObject.FindObjectsOfType<GashadokuroHand>();
        if (GH.Length > 0)
        {
            foreach (GashadokuroHand hand in GH)
            {
                hand.BP = BP;
            }
        }

        // Reset AiMonster
        AiMonsterController aiMonster = GetComponent<AiMonsterController>();
        if (aiMonster != null) aiMonster.SetIsPerformingAbility(false);
        if (isChargingAbility) isChargingAbility = false;
        if (isCharging) isCharging = false;
    }

    IEnumerator KickDelay()
    {
        Debug.Log(BP.lastKicker + " just kicked");
        yield return new WaitForSeconds(0.1f);
        BP.lastKicker = null;
        Debug.Log("Wait Done");
    }

    public IEnumerator MoveDelay()
    {
        canMove = false;
        ANIM.SetBool("isWalking", false);
        yield return new WaitForSeconds(1.0f);
        canMove = true;
    }

    public void InvincibilityFlash()
    {
        if (spriteObject == null) return;
        if (isStunned && Time.frameCount % 2 == 0)
        {
            spriteObject.transform.localScale = Vector3.zero;
        } 
        else if (isIntangible && Time.frameCount % 4 == 0)
        {
            //spriteObject.transform.localScale = Vector3.zero;
        }
        else
        {
            spriteObject.transform.localScale = spriteScale;
        }
    }

    /**
    *  The Following Code Is For Controller Inputs
    **/

    public void OnMove(InputAction.CallbackContext context)
    {
        // Ignore input if monster is ai
        if (GetComponent<AiMonsterController>() != null) return;

        //Debug.Log("OnMove");
        if (!usingKeyboard) movementDirection = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y).normalized;
        usingKeyboard = false;
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        // Ignore input if monster is ai
        if (GetComponent<AiMonsterController>() != null) return;

        rightStickInput = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);

        if (invertControls)
        {
            rightStickInput.x = -rightStickInput.x;
            rightStickInput.z = -rightStickInput.z;
        }

        if (rightStickInput != Vector3.zero && !usingKeyboard)
        {
            aimingDirection = rightStickInput.normalized;
        }
        usingKeyboard = false;
    }

    public Vector3 GetAimDirection()
    {
        return aimingDirection;
    }

    public Rigidbody GetRB()
    {
        return rb;
    }

    public Animator GetAnimator()
    {
        return ANIM;
    }

    public void OnWall(InputAction.CallbackContext context)
    {
        // Ignore input if monster is ai
        if (GetComponent<AiMonsterController>() != null) return;

        if (isStunned || (BP.ballOwner != null && BP.ballOwner == gameObject && !abilities[0].usableWhileDribbling)
            || (isIntangible && !abilities[0].usableWhileIntangible) || !GM.isPlaying || GM.isPaused) return; // ensure no dashing or dash charging when you have ball
        if (abilities[0] is AbilityChargeable) // Ability Chargeable
        {
            AbilityChargeable ab = (AbilityChargeable)abilities[0];
            ab.CheckInputs(context);
        }
        else if (abilities[0] is AbilityDelayed) // Ability Delayed
        {
            AbilityDelayed ab = (AbilityDelayed)abilities[0];
            ab.CheckInputs(context);
        }
        else // Regular Ability
        {
            abilities[0].Activate();
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        // Ignore input if monster is ai
        if (GetComponent<AiMonsterController>() != null) return;

        if (isStunned || (BP.ballOwner != null && BP.ballOwner == gameObject && !abilities[1].usableWhileDribbling)
            || (isIntangible && !abilities[1].usableWhileIntangible) || !GM.isPlaying || GM.isPaused) return; // ensure no dashing or dash charging when you have ball
        if (abilities[1] is AbilityChargeable) // Ability Chargeable
        {
            AbilityChargeable ab = (AbilityChargeable)abilities[1];
            ab.CheckInputs(context);
        } else if (abilities[1] is AbilityDelayed) // Ability Delayed
        {
            AbilityDelayed ab = (AbilityDelayed)abilities[1];
            ab.CheckInputs(context);
        }
        else // Regular Ability
        {
            abilities[1].Activate();
        }
    }

    public void OnCharge(InputAction.CallbackContext context)
    {
        // Ignore input if monster is ai
        if (GetComponent<AiMonsterController>() != null) return;

        if (isStunned || (BP.ballOwner != null && BP.ballOwner == gameObject && !abilities[2].usableWhileDribbling)
            || (isIntangible && !abilities[2].usableWhileIntangible) || !GM.isPlaying || GM.isPaused) return; // ensure no dashing or dash charging when you have ball
        if (abilities[2] is AbilityChargeable) // Ability Chargeable
        {
            AbilityChargeable ab = (AbilityChargeable)abilities[2];
            ab.CheckInputs(context);
        }
        else if (abilities[2] is AbilityDelayed) // Ability Delayed
        {
            AbilityDelayed ab = (AbilityDelayed)abilities[2];
            ab.CheckInputs(context);
        }
        else // Regular Ability
        {
            abilities[2].Activate();
        }
    }

    public void OnInvert(InputAction.CallbackContext context)
    {
        /* Old Invert code if we still want it */
        //invertControls = !invertControls;
        //usingNewScheme = !usingNewScheme;

        if (GM.isPlaying)
        {
            GM.PauseGame();
        } else if (UM.GetTimeRemaining() < 0)
        {
            GM.MenuReturn();
        }
    }

    IEnumerator RemoveNullAbilities()
    {
        yield return new WaitForSeconds(0.025f);

        for (int i = abilities.Count - 1; i >= 0; i--)
        {
            if (abilities[i] == null)
            {
                Debug.Log("Removed a null ability");
                abilities.RemoveAt(i);
            }
        }
        UM.UpdateAbilityIcons();
    }

    private void OnTriggerStay(Collider other)
    {
        BallProperties BP = other.GetComponent<BallProperties>();
        // Debug.Log("Other: " + other);

        if (BP != null) Debug.Log("BP.ballOwner: " + BP.ballOwner);
        if (BP == null || BP.ballOwner != null) return;
        // If ball hasn't been in warrior's colliders long enough
        if (BP != null && pickupBallTimer > 0)
        {
            // Count down timer
            Debug.Log("Waiting to pick up ball");
            pickupBallTimer -= Time.deltaTime;
        }
        // If has been in warrior's collider long enough
        else if (BP != null && pickupBallTimer <= 0)
        {
            // Pick up ball
            Debug.Log("Pick up ball");
            pickupBallTimer = pickupBallCooldown;
            BP.GetComponent<Rigidbody>().velocity = Vector3.zero;
            BP.ballOwner = gameObject;
            BP.SetOwner(BP.ballOwner);
        }
    }

}
