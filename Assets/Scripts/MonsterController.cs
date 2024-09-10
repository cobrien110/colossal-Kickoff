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
    private Rigidbody rb;
    [SerializeField] public GameObject Ball = null;
    public BallProperties BP = null;
    public GameObject wallPrefab;
    public GameObject shrapnelPrefab;

    [SerializeField] float monsterSpeed = 2f;
    [HideInInspector] public Vector3 movementDirection;
    private Vector3 aimingDirection;
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
    [SerializeField] private float attackHitForce = 200f;
    [SerializeField] private float chargeMultiplier = 0.5f;
    [SerializeField] private float maxChargeSeconds = 2f;
    [SerializeField] private float chargeMoveSpeedMult = 0.2f;
    [SerializeField] private float stunTime = 3f;
    [SerializeField] private float stunSpeed = 0.2f;
    private float kickCharge = 1f;
    private bool isCharging;
    [Header("Ability Stats")]
    [SerializeField] private float wallSpawnDistance = 2f;
    [SerializeField] private float wallCooldown = 5f;
    [SerializeField] public int shrapnelAmount = 5;
    [SerializeField] public int shrapnelDamage = 1;
    [SerializeField] public float shrapnelSpeed = 500f;
    [SerializeField] public float shrapnelSpreadAngle = 35f;
    [SerializeField] public float wallDuration = 8f;
    private float wallTimer;
    [SerializeField] private float dashSpeed = 500.0f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float dashDuration = 0.35f;
    [SerializeField] private float maxDashChargeSeconds = 2f;
    private float lastDashTime = -1f;
    private bool isDashing = false;
    private float dashCharge = 1f;
    private bool isChargingDash = false;
    [HideInInspector] public bool isStunned = false;
    public float attackRange = 1f;
    [SerializeField] private float attackCooldown = 1f;
    private float lastAttackTime = -1f;
    [SerializeField] private float attackBaseRadius = 0.5f;
    private bool isChargingAttack = false;
    private float attackCharge = 0f;
    [SerializeField] private float attackChargeRate = 1f;
    [SerializeField] private float maxAttackChargeSeconds = 2f;
    [SerializeField] private bool canSpawnShrapnelOnAttack = true;

    [SerializeField] private bool canMove = true;
    [SerializeField] private GameplayManager GM = null;
    [SerializeField] private UIManager UM = null;
    [SerializeField] private Animator ANIM;
    private AudioPlayer audioPlayer;
    private GameObject monsterSpawner = null;
    public LayerMask layerMask;
    private CommentatorSoundManager CSM;
    public GameObject spriteObject;
    private Vector3 spriteScale;
    public GameObject attackVisual;
    private float attackVisualOffsetY = -0.3f;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
        UM = GameObject.Find("Canvas").GetComponent<UIManager>();
        Ball = GameObject.Find("Ball");
        BP = (BallProperties) Ball.GetComponent("BallProperties");
        CSM = GameObject.Find("CommentatorSounds").GetComponent<CommentatorSoundManager>();
        audioPlayer = GetComponent<AudioPlayer>();
        monsterSpawner = GameObject.Find("MonsterSpawner");
        transform.position = monsterSpawner.transform.position;
        wallTimer = wallCooldown;
        spriteScale = spriteObject.transform.localScale;
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
        if (canMove)
        {
            //Movement();
        }
        //Movement();
        if (GM.isPlaying)
        {  
            Dribbling();
            Passing();
            Kicking();
            RotateWhileCharging();
            Dash();
            ResizeAttackVisual();

            if (Input.GetKey(KeyCode.Backspace))
            {
                Attack();
            }

            if (wallTimer >= wallCooldown && (Input.GetKeyDown(KeyCode.J)))
            {
                BuildWall();
            }
            
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

            // TESTING STUN
            if (Input.GetKeyDown(KeyCode.T))
            {
                Stun();
            }

        } else { rb.velocity = new Vector3(0, 0, 0); } // ensure monster momentum is killed when not playing
        InvincibilityFlash();
        // Cooldowns
        if (wallTimer < wallCooldown)
        {
            wallTimer += Time.deltaTime;
            UM.UpdateMonsterAbility1Bar(1-(wallTimer/wallCooldown));
        }

        if (Time.time - lastAttackTime < attackCooldown)
        {
            UM.UpdateMonsterAbility2Bar(1-((Time.time - lastAttackTime) / attackCooldown));
        }

        if (Time.time - lastDashTime < dashCooldown)
        {
            UM.UpdateMonsterAbility3Bar(1 - ((Time.time - lastDashTime) / dashCooldown));
        }



        if (isStunned && BP.ballOwner == this.gameObject)
        {
            BP.ballOwner = null;
        }

        //Temp Controller Scheme Swap
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            usingNewScheme = !usingNewScheme;
            invertControls = !invertControls;
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
        Vector3 dir = isStunned ? -movementDirection : movementDirection;
        rb.velocity = GM.isPlaying ? dir * monsterSpeed : Vector3.zero;
        rb.velocity = isCharging || isChargingDash ? rb.velocity * chargeMoveSpeedMult : rb.velocity;
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
            if (isChargingAttack)
            {
                isChargingAttack = false;
                attackCharge = 0;
            }

            UM.ShowChargeBar(true);
            UM.UpdateChargeBarText("Monster");
            Ball.transform.position = ballPosition.transform.position; // new Vector3(transform.position.x, 2, transform.position.z);
        }
    }

    void Passing()
    {
        if(Input.GetKeyDown(KeyCode.RightShift) && BP.ballOwner == gameObject)
        {
            Debug.Log("Pass!");
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
        if (!usingNewScheme)
        {
            if (((rightStickInput == Vector3.zero && !usingKeyboard) || Input.GetKeyUp(KeyCode.KeypadEnter)) && BP.ballOwner == gameObject && kickCharge != 1)
            {
                Debug.Log("Kick!");
                BP.ballOwner = null;
                BP.lastKicker = gameObject;
                Debug.Log(kickCharge);
                float kickForce = kickSpeed * (kickCharge * chargeMultiplier);
                Vector3 forceToAdd = aimingDirection * kickForce;
                BP.GetComponent<Rigidbody>().AddForce(forceToAdd);

                UM.ShowChargeBar(false);
                UM.UpdateChargeBar(0f);
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
                    UM.UpdateChargeBar((kickCharge - 1) / (maxChargeSeconds - 1));
                    kickCharge += Time.deltaTime;
                    isCharging = true;
                    ANIM.SetBool("isWindingUp", true);
                }

                if (kickCharge > maxChargeSeconds)
                {
                    UM.UpdateChargeBar(1f);
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
                BP.ballOwner = null;
                BP.lastKicker = gameObject;
                Debug.Log(kickCharge);
                float kickForce = kickSpeed * (kickCharge * chargeMultiplier);
                Vector3 forceToAdd = aimingDirection * kickForce;
                BP.GetComponent<Rigidbody>().AddForce(forceToAdd);

                UM.ShowChargeBar(false);
                UM.UpdateChargeBar(0f);
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
                    UM.UpdateChargeBar((kickCharge - 1) / (maxChargeSeconds - 1));
                    kickCharge += Time.deltaTime;
                    isCharging = true;
                    ANIM.SetBool("isWindingUp", true);
                }

                if (kickCharge > maxChargeSeconds)
                {
                    UM.UpdateChargeBar(1f);
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

    }

    void SpawnShrapnel()
    {
        if (shrapnelPrefab == null) return;
        Vector3 pos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        GameObject shrap = Instantiate(shrapnelPrefab, pos, Quaternion.LookRotation(transform.forward, Vector3.up));
        WallShrapnel WS = shrap.GetComponent<WallShrapnel>();
        WS.damage = shrapnelDamage;
        WS.speed = shrapnelSpeed;
    }

    private void OnDrawGizmos()
    {
        Vector3 direction = transform.forward;
        Gizmos.color = Color.red;
        Vector3 origin = new Vector3(transform.position.x, transform.position.y + attackVisualOffsetY, transform.position.z);
        Gizmos.DrawWireSphere(origin + direction * attackRange, attackBaseRadius + attackCharge * attackChargeRate);
    }

    void Dash()
    {
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
    }

    void ChargeDashing()
    {
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
    }

    void ChargeAttack()
    {
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
    }

    void StopDashing()
    {
        Debug.Log("No longer dashing");
        // ANIM.SetBool("isSliding", false);
        isDashing = false;
    }


    void BuildWall()
    {
        if (isStunned) return;
        wallTimer = 0f;
        Vector3 spawnLocation = Vector3.zero;
        Quaternion spawnRotation = Quaternion.identity;

        Vector3 dir = movementDirection;
        if (dir.Equals(Vector3.zero))
        {
            dir = rb.transform.forward.normalized;
            // return;
        }
        spawnLocation = transform.position + (dir * wallSpawnDistance);
        spawnRotation = Quaternion.LookRotation(dir, Vector3.up);

        audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find("minotaurCreateWall"), 0.2f);
        Instantiate(wallPrefab, spawnLocation, spawnRotation);
        ANIM.Play("MinotaurWall");
    }

    public void Stun()
    {
        if (isStunned) return;
        BP.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
        isStunned = true;
        audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find("minotaurStun"), 0.5f);
        CSM.PlayDeathSound(false);
        StartCoroutine(ResetStun());
    }

    private void ResizeAttackVisual()
    {
        attackVisual.transform.localScale = new Vector3(attackBaseRadius * 2f + attackCharge * attackChargeRate * 2f,
            0.05f, attackBaseRadius * 2f + attackCharge * attackChargeRate * 2f);
        Vector3 dir = transform.forward * attackRange;
        attackVisual.transform.position = new Vector3(transform.position.x, attackVisual.transform.position.y, transform.position.z) + dir;
        if (BP.ballOwner != null && BP.ballOwner.Equals(gameObject))
        {
            attackVisual.transform.localScale = Vector3.zero;
        }
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
    }

    IEnumerator KickDelay()
    {
        Debug.Log(BP.lastKicker + " just kicked");
        yield return new WaitForSeconds(0.1f);
        BP.lastKicker = null;
        Debug.Log("Wait Done");
    }

    private void OnTriggerEnter(Collider collider)
    {
        // Debug.Log("Monster Collision with: " + collider.gameObject.name);
        if (isDashing && collider.tag.Equals("Warrior"))
        {
            Debug.Log("Dash killed warrior");
            collider.gameObject.GetComponent<WarriorController>().Die();
        }
    }

    IEnumerator MoveDelay()
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
        //Debug.Log("OnMove");
        if (!usingKeyboard) movementDirection = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y).normalized;
        usingKeyboard = false;
    }

    public void OnAim(InputAction.CallbackContext context)
    {
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

    public void OnWall(InputAction.CallbackContext context)
    {
        if (wallTimer >= wallCooldown && GM.isPlaying)
        {
            BuildWall();
            StartCoroutine(MoveDelay());
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (isStunned || (BP.ballOwner != null && BP.ballOwner == gameObject)) return; // ensure no dashing or dash charging when you have ball
        if (!GM.isPlaying)
        {
            isChargingAttack = false;
            attackCharge = 0;
            return;
        }

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            // If input is no longer true, attack
            if (context.action.WasReleasedThisFrame() && attackCharge != 0)
            {
                Attack();
            }
            else if (context.action.IsInProgress() && Time.time - lastAttackTime >= attackCooldown) // If it still is true, keep charging
            {
                Debug.Log("Is Charging Attack");
                isChargingAttack = true;
            } else
            {
                Debug.Log("Not attack and not charging");
                isChargingAttack = false;
                attackCharge = 0;
            }
        }
    }

    public void OnCharge(InputAction.CallbackContext context)
    {
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
            if (context.action.WasReleasedThisFrame() && dashCharge != 0)
            {
                // Check if enough time has passed since the last slide

                if (movementDirection != Vector3.zero && BP.ballOwner != gameObject)
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
                    // audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find("slide"), 0.5f); replace with different sound

                    Invoke("StopDashing", dashDuration);

                    // ANIM.SetBool("isSliding", true);
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
            else if (context.action.IsInProgress()) // If it still is true, keep charging
            {
                isChargingDash = true;
            }
        }
    }

    public void OnInvert(InputAction.CallbackContext context)
    {
        invertControls = !invertControls;
        usingNewScheme = !usingNewScheme;
    }

}
