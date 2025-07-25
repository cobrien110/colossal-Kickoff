using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.InputSystem;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.UI;
using Steamworks;

public class WarriorController : MonoBehaviour
{
    #region PlayerHolder Variables

    public int playerID;
    public int warriorPosition;
    public PlayerHolder PH = null;

    #endregion

    #region Control Scheme Variables

    [SerializeField] private bool invertControls = false;

    /*
     * 0: Hold and release Right-Stick to Kick
     * 1: Hold and release Right-Trigger to Kick
     */
    [SerializeField] private int kickMode = 0;
    private bool isKickCharging = false;

    #endregion

    public Rigidbody rb;
    [SerializeField] public GameObject Ball = null;
    public BallProperties BP = null;
    [SerializeField] private AimVisualizer AV = null;
    [SerializeField] private GameObject aura;

    public const float baseMovementSpeed = 2.85f;
    [SerializeField] public float warriorSpeed = baseMovementSpeed;
    public Vector3 movementDirection;
    public Vector3 aimingDirection;
    private Vector3 rightStickInput;

    [SerializeField] private GameObject ballPosition;

    [Header("Stats")]
    public int healthMax = 2;
    [SerializeField] private int health = 2;
    [SerializeField] private float respawnTime = 2f;
    private float respawnTimer;
    [SerializeField] private float respawnInvincibilityTime = 1.5f;
    [SerializeField] private float damageInvincibilityTime = 0.35f;
    [SerializeField] private bool willBeStunnedOnHit = false;
    private bool isDead = false;
    public bool isInvincible = false;
    [HideInInspector] public bool isWinner = false;
    [HideInInspector] public bool canRespawn = true;
    [SerializeField] protected float passSpeed = 5.0f;
    [SerializeField] private float kickSpeed = 5.0f;
    [SerializeField] private float chargeMultiplier = 0.5f;
    [SerializeField] private float maxCharge = 2f;
    [SerializeField] protected float chargeMoveSpeedMult = 0.2f;
    private float kickCharge = 1f;
    protected bool isCharging;
    public bool superKicking = false;
    private float chargeSpeed;
    [SerializeField] private float slideHitboxRadius = 2f;
    private float baseHitboxRadius;
    [SerializeField] private float auraLingerDuration = 1f;

    //Add setting to adjust this, replace deadzone?
    private float kickingSensitivity = 0.90f;

    private float slideCooldown = 2f;
    [SerializeField] private float slideSpeedRegular = 230f;
    [SerializeField] private float slideCooldownRegular = 2f;
    [SerializeField] private float slideDurationRegular = 1f;
    [SerializeField] private float slideSpeedDodge = 100f;
    [SerializeField] private float slideCooldownDodge = 3f;
    [SerializeField] private float slideDurationDodge = 0.5f;
    public bool isSliding = false;
    private bool isJuking = false;
    private bool jukeKickReady = false;
    private float lastSlideTime = -1f;
    [HideInInspector] public bool isStunned = false;

    //Temp Controller Scheme Swap
    public InputAction warriorControls;

    //Make True If Using Keyboard For Movement
    public bool usingKeyboard = false;

    [SerializeField] private GameplayManager GM = null;
    private WarriorUI WUI = null;
    private UIManager UM = null;
    private StatTracker ST = null;
    [SerializeField] private Transform respawnBox;
    private AudioPlayer audioPlayer;
    public GameObject WarriorSpawner = null;
    [SerializeField] GameObject spriteObject;
    private Vector3 spriteScale;
    [SerializeField] public Animator ANIM;
    private MultipleTargetCamera MTC;
    [SerializeField] private ParticleSystem PS;
    [SerializeField] private ParticleSystem PSCurse;
    //public Sprite[] ringColors;
    public SpriteRenderer ring;
    public SpriteRenderer transparentRing;
    public Image callForPass;
    //public SpriteRenderer playerRend;
    //public Color curseColor;
    private CommentatorSoundManager CSM;
    public int playerNum;
    [SerializeField] public GameObject goreParticleObj;
    [SerializeField] public GameObject pinataParticleObj;
    [SerializeField] private GameObject dodgeTextPrefab;
    [SerializeField] private GameObject superTextPrefab;
    [SerializeField] private GameObject superTextFullPrefab;
    [SerializeField] private GameObject[] tauntTextPrefabs;
    private bool canSpawnText = true;
    [SerializeField] private CapsuleCollider capsuleCollider;

    private bool shouldShake1 = true;
    private bool shouldShake2 = true;
    [SerializeField] private float shakeIntensity = 1.0f;

    //Mummy Curse
    [HideInInspector] public bool isCursed = false;
    [SerializeField] private GameObject Mummy = null;
    private bool isBomb = false;
    private float bombCooldown = 3f;
    private float bombTimer = 0f;
    [SerializeField] private GameObject BombVisual = null;
    private AiMummyManager aiMummyManager;

    // If there is no ball owner yet a warrior or monster is on top of ball, OnTriggerStay will wait this long until making that character pick up ball
    private float pickupBallCooldown = 0.25f;
    [SerializeField] private float pickupBallTimer = 0f;
    private float lastCallForPassTime;
    [SerializeField] private float callForPassCooldown = 1f;

    // fancy respawn animation
    private Transform jumpInLocation;
    private float jumpInTime = 2f;
    private float elapsedJumpTime = 0f;
    private float arcHeight = 3f;
    private bool fancySpawnStarted = false;
    //private float jumpRandomMod = 5f;

    // Call For Pass
    private float passWindowTimer;
    [SerializeField] private float passWindowDuration = 1f;
    private static bool kickHappened;
    private Coroutine callForPassWindowCheck = null;

    // Call For Pass - Gravity
    [SerializeField] private float gravityFieldDuration = 0.5f; // How long the field lasts
    [SerializeField] private float gravityForce = 20f; // Strength of pull
    [SerializeField] private float gravityRadius = 5f; // Radius of effect
    private Coroutine gravityCoroutine;

    private bool canReadAimInput = true;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
        UM = GameObject.Find("Canvas").GetComponent<UIManager>();
        ST = GameObject.Find("Stat Tracker").GetComponent<StatTracker>();
        Ball = GameObject.Find("Ball");
        AV = GetComponentInChildren<AimVisualizer>();
        BP = (BallProperties)Ball.GetComponent("BallProperties");
        MTC = GameObject.Find("Main Camera").GetComponent<MultipleTargetCamera>();
        MTC.AddTarget(transform);
        CSM = GameObject.Find("CommentatorSounds").GetComponent<CommentatorSoundManager>();
        WUI = GetComponentInChildren<WarriorUI>();
        audioPlayer = GetComponent<AudioPlayer>();
        respawnBox = GameObject.FindGameObjectWithTag("RespawnBox").transform;
        health = healthMax;
        spriteScale = spriteObject.transform.localScale;
        transform.rotation = new Quaternion(0f, .5f, 0f, 0f);
        baseHitboxRadius = capsuleCollider.radius;

        // fancy respawn
        jumpInLocation = GameObject.FindGameObjectWithTag("JumpInPoint").transform;
        jumpInTime = 1f;
    }

    private void Start()
    {
        if (this.gameObject.GetComponent<WarriorAiController>() != null)
        {
            WUI.SetAI();
        }
        UM = GameObject.Find("Canvas").GetComponent<UIManager>();
        pickupBallTimer = pickupBallCooldown;
        chargeSpeed = GM.warriorKickChargeSpeed;

        // Instantiate mummy manager variable
        GameObject monsterObj = GameObject.FindGameObjectWithTag("Monster");
        if (monsterObj != null)
        {
            aiMummyManager = monsterObj.GetComponent<AiMummyManager>();
            if (aiMummyManager == null)
            {
                Debug.LogWarning("AiMummyManager component not found on GameObject tagged 'Monster'.");
            }
        }
        else
        {
            Debug.LogWarning("No GameObject with tag 'Monster' found.");
        }

        if (PH != null) SetKickSen(PH.deadzoneSensitivity);
    }

    // Temp Controller Scheme Swap
    private void OnEnable()
    {
        warriorControls.Enable();
    }

    // Temp Controller Scheme Swap
    private void OnDisable()
    {
        warriorControls.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        InvincibilityFlash();
        if (GM.isPlaying && !isDead && !GM.isPaused)
        {
            //if (GetComponent<WarriorAiController>() != null) return;
            Dribbling();
            Passing();
            Kicking();
            RotateWhileCharging();
            //if (Input.GetKey(KeyCode.E)) {
            //    Sliding();
            //}
            

            if ((isStunned) && BP.ballOwner == this.gameObject)
            {
                // Debug.Log("ballOwner set to null");
                BP.ballOwner = null;
            }

            //if (superKicking && isCharging && GM.passMeter == GM.passMeterMax)
            if (superKicking && isCharging && BP.canFullSuperKick)
            {
                //Debug.Log("Should start glowing");
                float glowAmount = kickCharge - 1.0f;
                BP.StartBallGlow(glowAmount);
                if (kickCharge >= maxCharge && shouldShake2)
                {
                    shouldShake2 = false;
                    StartCoroutine(MTC.ScreenShake(shakeIntensity * 2));
                }
                else if (kickCharge > (maxCharge / 2) + 0.5f && shouldShake1)
                {
                    shouldShake1 = false;
                    StartCoroutine(MTC.ScreenShake(shakeIntensity));
                }
            }
            else if (BP.ballOwner == gameObject)
            {
                BP.StopBallGlow();
                //MTC.isShaking = false;
                shouldShake1 = true;
                shouldShake2 = true;
            }
        }
        Respawn();
        FancyRespawnAnimation();
        //Particles
        if (health < healthMax && !isDead && PS != null)
        {
            if (!PS.isPlaying) PS.Play();
        } else if (PS != null)
        {
            PS.time = 0;
            PS.Stop();
        }
        if (isCursed)
        {
            if (!PSCurse.isPlaying) PSCurse.Play();
        } else
        {
            PSCurse.Stop();
        }

        //Bomb Curse
        if (isBomb)
        {
            bombTimer += Time.deltaTime;
            if (bombTimer > bombCooldown)
            {
                Instantiate(BombVisual, new Vector3(transform.position.x, 1.3f, transform.position.z), Quaternion.identity);
                Die();
                Debug.Log("Explode");
                Collider[] objectsInRange = Physics.OverlapSphere(transform.position, 0.5f);
                foreach (Collider obj in objectsInRange)
                {
                    if (obj.GetComponent<WarriorController>() != null)
                    {
                        obj.GetComponent<WarriorController>().Damage(1);
                        Debug.Log("Player was hurt by bomb");
                    }
                    
                }
                bombTimer = 0;
                isBomb = false;
            }
        }

        // If warrior from getting pushed up by wall
        if (!isDead && transform.position.y != 0 && !GM.isGameOver)
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            //Debug.Log("Fixed warrior y position");
        }

    }

    private void FixedUpdate()
    {
        if (GetComponent<WarriorAiController>() != null) return;
        if (isDead) return;
        Movement();
    }

    //CURSE OF RA
    private void OnTriggerEnter(Collider other)
    {
        /*
        if (isCursed) return;
        
        AIMummy mummy = other.gameObject.GetComponent<AIMummy>();
        if ((other.tag.Equals("Mummy")) && (mummy != null))
        {
            isCursed = true;
            Debug.Log("This player is cursed");
            Instantiate(goreParticleObj, transform.position, Quaternion.identity);
            //playerRend.color = curseColor;
        }
        */

    }

    void Movement()
    {
        if (isSliding || isStunned) return;
        float horizontalInput = 0f;
        float verticalInput = 0f;

        // Check for WASD keys
        //if (Input.GetKey(KeyCode.W))
        //{
        //    verticalInput = 1f;
        //}
        //else if (Input.GetKey(KeyCode.S))
        //{
        //    verticalInput = -1f;
        //}
        //if (Input.GetKey(KeyCode.D))
        //{
        //    horizontalInput = 1f;
        //}
        //else if (Input.GetKey(KeyCode.A))
        //{
        //    horizontalInput = -1f;
        //}
        Vector2 keyBoardInputs = new Vector2(horizontalInput, verticalInput);
        if (keyBoardInputs != Vector2.zero)
        {
            usingKeyboard = true;
            movementDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;
            aimingDirection = movementDirection;
        } else if (usingKeyboard) 
        {
            movementDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;
        } 
        if (isCursed)
        {
            //movementDirection *= -1;
        }

        rb.velocity = GM.isPlaying ? movementDirection * warriorSpeed : Vector3.zero;
        rb.velocity = isCharging ? rb.velocity * chargeMoveSpeedMult : rb.velocity;
        if (rb.velocity != Vector3.zero && !isCharging) 
        {
            Quaternion newRotation = Quaternion.LookRotation(movementDirection.normalized, Vector3.up);
            transform.rotation = newRotation;
        }

        if (movementDirection != Vector3.zero && GM.isPlaying)
        {
            ANIM.SetBool("isWalking", true);
        } else
        {
            ANIM.SetBool("isWalking", false);
        }
    }

    void Dribbling()
    {
        if (BP.ballOwner == gameObject)
        {
            //UM.ShowChargeBar(true);
            WUI.ShowChargeBar(true);
            UM.UpdateChargeBarText("Warrior");
            Ball.transform.position = ballPosition.transform.position; // new Vector3(transform.position.x, 2, transform.position.z);
        } else
        {
            // BP.ballOwner = null; // this code was messing up the monsters ability to dribble
            WUI.ShowChargeBar(false);
            WUI.UpdateChargeBar(0f);
        }
    }

    void Passing()
    {
        if(Input.GetKeyDown(KeyCode.P) && BP.ballOwner == gameObject)
        {
            Debug.Log("Pass!");

            // Debug.Log("ballOwner set to null");
            BP.ballOwner = null;
            Debug.Log(transform.forward);
            BP.GetComponent<Rigidbody>().AddForce(transform.forward * passSpeed);
            audioPlayer.PlaySoundRandomPitch(audioPlayer.Find("pass"));
        }
    }

    public bool IsWallBetweenBallAndPlayer()
    {
        Vector3 direction = (BP.transform.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, BP.transform.position);

        // Define the layers to check using a LayerMask
        int layerMask = LayerMask.GetMask("InvisibleWall", "Ground");

        // Perform the raycast
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, distance, layerMask))
        {
            return true; // Something is blocking the path
        }

        return false; // No obstacles in the way
    }

    void Kicking()
    {
        //if (((rightStickInput.magnitude < kickingSensitivity && !usingKeyboard) || /*Input.GetKeyUp(KeyCode.Space)*/false) && BP.ballOwner == gameObject && kickCharge != 1)
        if (isKickCharging == false && BP.ballOwner == gameObject && kickCharge != 1)
        {
            Debug.Log("Kick!");

            // Prevent ball from getting kicked "through" walls
            if (BP != null && IsWallBetweenBallAndPlayer())
            {
                Debug.Log("Correcting ball position before kick");
                BP.gameObject.transform.position =
                    new Vector3(transform.position.x, BP.gameObject.transform.position.y, transform.position.z); // Ignore Y axis
            }

            // Debug.Log("ballOwner set to null");
            BP.ballOwner = null;
            BP.lastKicker = gameObject;
            BP.previousKicker = gameObject;
            //GM.isPassing = true;

            // For CallForPass gravity field
            kickHappened = true;
            StartCoroutine(ResetKickHappened());

            if (GM.passIndicator)
            {
                //BP.SetBallColor(Color.blue);
            }

            Debug.Log(kickCharge);
            float kickForce = kickSpeed * (kickCharge * chargeMultiplier);
            if (superKicking && GM.passMeter > 0f)
            {
                if (BP.canFullSuperKick)
                {
                    kickForce = kickForce * (2f);
                    BP.isSuperKick = true;
                    BP.isFullSuperKick = true;
                    GM.passMeter = 0f;
                    UM.UpdateWarriorContestBar(0f);
                }
                else
                {
                    kickForce = kickForce * (0.9f + GM.passMeter);
                    BP.isSuperKick = true;
                    GM.passMeter = 0f;
                    UM.UpdateWarriorContestBar(0f);
                }
                StopSuperKick();
            }

            //Juke Kick
            if (jukeKickReady)
            {
                kickForce = kickForce * (1.3f);
                jukeKickReady = false;
            }
            //Debug.Log(kickForce);

            if (aimingDirection == Vector3.zero)
            {
                aimingDirection = gameObject.transform.forward;
            }

            Vector3 forceToAdd = aimingDirection * kickForce;
            BP.GetComponent<Rigidbody>().AddForce(forceToAdd);

            ANIM.Play("WarriorKick");

            WUI.UpdateChargeBar(0f);
            PlayKickSound(kickCharge);

            StartCoroutine(KickDelay());
        }
        //if (((rightStickInput.magnitude >= kickingSensitivity && !usingKeyboard) || /*Input.GetKey(KeyCode.Space)*/false) && BP.ballOwner == gameObject)
        if (isKickCharging && BP.ballOwner == gameObject)
        {
            if (kickCharge <= maxCharge)
            {
                //Juke Kick Var
                if (isJuking && !jukeKickReady)
                {
                    jukeKickReady = true;
                    Invoke("UpdateJukeKickReady", 1.0f);
                    Debug.Log("JUKE KICK CHARGING");
                }

                //Debug.Log(kickCharge);
                WUI.UpdateChargeBar((kickCharge - 1) / (maxCharge - 1));

                //Charge Speed
                kickCharge += Time.deltaTime * chargeSpeed;
                isCharging = true;
                ANIM.SetBool("isChargingKick", true);
            }

            if (kickCharge > maxCharge)
            {
                //UM.UpdateChargeBar(1f);
            }

        }
        else
        {
            kickCharge = 1f;
            isCharging = false;
            aimingDirection = Vector3.zero;
            ANIM.SetBool("isChargingKick", false);
        }
    }

    void RotateWhileCharging()
    {
        if (isCharging)
        {
            if (aimingDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(aimingDirection);
                rb.rotation = targetRotation;
            }
        }
    }

    private void UpdateJukeKickReady()
    {
        Debug.Log("JUKE KICK NOT READY");
        jukeKickReady = false;
    }

    public void Sliding()
    {
        if (!CanSlide())
        {
            Debug.Log("Cannot Slide");
            return;
        }
  
        Debug.Log(gameObject.name + ": Sliding");
        isSliding = true;
        isInvincible = true;

        // Increase hitbox for sliding
        capsuleCollider.radius *= slideHitboxRadius;

        // Check if this is a dodge slide or a regular slide
        float slideDuration;
        float slideSpeed;
        // AudioClip audioClip;
        string anim;
        ForceMode forceMode;
        if (BP.ballOwner == gameObject)
        {
            Debug.Log("Dodge slide");
            // Dodge slide
            slideDuration = slideDurationDodge;
            slideSpeed = slideSpeedDodge;
            slideCooldown = slideCooldownDodge;
            forceMode = ForceMode.Force;
            //ANIM.SetBool("isJuking", true);
            isJuking = true;

            // audioClip = ???
            anim = "isJuking";
            ANIM.Play(anim);
        } else
        {
            Debug.Log("Regular slide");
            // Regular slide
            slideDuration = slideDurationRegular;
            slideSpeed = slideSpeedRegular;
            slideCooldown = slideCooldownRegular;
            forceMode = ForceMode.Force;

            // audioClip = ???
            anim = "isSliding";
        }

        Debug.Log("slideSpeed: " + slideSpeed);

        // Add force in direction of the player input for this warrior (movementDirection)
        Vector3 slideVelocity = movementDirection.normalized * slideSpeed;
        rb.AddForce(slideVelocity, forceMode);
        audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find("slide"), 0.5f); // Maybe replace argument with "audioClip" variable

        // Set isSliding to false after a delay
        Invoke("StopSliding", slideDuration);

        // If owner is kicking, if kicking
        if (BP.ballOwner != null && BP.ballOwner == gameObject && kickCharge > 1)
        {
            Debug.Log("Cancel Kick");

            // Cancel kick
            isCharging = false;
            kickCharge = 1;
            aimingDirection = Vector3.zero;
            WUI.UpdateChargeBar(0f); // Update ui
            rightStickInput = Vector3.zero;
            superKicking = false; // Reset superKicking if true

            // Disable aim input for a moment to allow the right stick to be released without causing a kick to occur
            canReadAimInput = false;
            Invoke("ResetCanReadAimInput", 0.45f);

        }

        // Update the last slide time
        lastSlideTime = Time.time;
        ANIM.SetBool(anim, true); // Maybe replace argument with "anim" variable

    }

    public void ResetSlideCooldown()
    {
        lastSlideTime = -1f;
    }

    private void ResetCanReadAimInput()
    {
        canReadAimInput = true;
    }

    void StopSliding()
    {
        // Debug.Log("No longer sliding");
        ANIM.SetBool("isSliding", false);
        ANIM.SetBool("isJuking", false);
        // ANIM.SetBool("isDodging", false);
        isSliding = false;
        isJuking = false;
        isInvincible = false;
        capsuleCollider.radius = baseHitboxRadius;
    }

    public void ResetPlayer()
    {
        Debug.Log("Resetting player to " + WarriorSpawner);
        transform.position = this.WarriorSpawner.transform.position;
        transform.rotation = new Quaternion(0f, .5f, 0f, 0f);

        Debug.Log(transform.position + "    " + this.WarriorSpawner.transform.position);
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        health = healthMax;
        Debug.Log("health reset to: " + healthMax);
        isStunned = false;
        //rb.rotation = Quaternion.identity;
    }

    IEnumerator KickDelay()
    {
        yield return new WaitForSeconds(0.1f);
        ANIM.SetBool("isKicking", false);
        BP.lastKicker = null;
    }

    void Respawn()
    {
        if (respawnTimer < respawnTime && isDead)
        {
            respawnTimer += Time.deltaTime;
            if (respawnTimer >= respawnTime)
            {
                UM.PlayerIconGreyout(false, warriorPosition);
            }

            //UM.UpdatePlayerRespawnBar(1 - (respawnTimer / respawnTime), playerNum);


        } else if (isDead && canRespawn)
        {
            isDead = false;
            canSpawnText = true;
            respawnTimer = 0;
            MTC.AddTarget(transform);
            ResetPlayer();
            fancySpawnStarted = false;
            elapsedJumpTime = 0;
            DisableAura();

            // Update list of warriors in AiMonsterController if appropriate
            AiMonsterController aiMonsterController = FindObjectOfType<MonsterController>().GetComponent<AiMonsterController>();
            if (aiMonsterController != null)
            {
                // Debug.Log("respawn");
                // Debug.Log("Before: " + aiMonsterController.warriors.Count);
                aiMonsterController.warriors.Add(gameObject);
                // Debug.Log("After: " + aiMonsterController.warriors.Count);
            }
        }
    }

    public void RespawnEarly()
    {
        Debug.Log("Respawning Early");
        isDead = false;
        isInvincible = false;
        respawnTimer = 0;
        MTC.AddTarget(transform);
        //ResetPlayer();
        fancySpawnStarted = false;
        elapsedJumpTime = 0;
        health = healthMax;
        ANIM.SetBool("isDead", false);

        // Update list of warriors in AiMonsterController if appropriate
        AiMonsterController aiMonsterController = FindObjectOfType<MonsterController>().GetComponent<AiMonsterController>();
        if (aiMonsterController != null)
        {
            // Debug.Log("respawn");
            // Debug.Log("Before: " + aiMonsterController.warriors.Count);
            aiMonsterController.warriors.Add(gameObject);
            // Debug.Log("After: " + aiMonsterController.warriors.Count);
        }
    }

    void FancyRespawnAnimation()
    {
        if (jumpInLocation == null || !canRespawn) return;

        if (respawnTimer >= respawnTime - 1 && isDead)
        {
            if (!fancySpawnStarted)
            {
                /*
                Vector3 randomizedPoint = Random.onUnitSphere * jumpRandomMod;
                randomizedPoint.z = 0f;
                Debug.Log("random point:" + randomizedPoint);
                */
                transform.position = jumpInLocation.position;// + randomizedPoint;
                fancySpawnStarted = true;
                ANIM.Play("WarriorJump");
                ANIM.SetBool("isDead", true);
            }

            if (elapsedJumpTime < jumpInTime)
            {
                elapsedJumpTime += Time.deltaTime;
            }
            float t = elapsedJumpTime / jumpInTime;

            // Linear interpolation between start and end
            Vector3 horizontalPosition = Vector3.Lerp(jumpInLocation.position, WarriorSpawner.transform.position, t);

            // Parabolic height calculation
            float arc = arcHeight * Mathf.Sin(t * Mathf.PI);

            // Apply the arc to the Y-axis
            transform.position = new Vector3(horizontalPosition.x, horizontalPosition.y + arc, horizontalPosition.z);
        } else
        {
            ANIM.SetBool("isDead", false);
        }
    }

    public float GetCurrentRespawnTime()
    {
        return respawnTimer;
    }

    public void Die()
    {
        Debug.Log("Die");
        Vector3 deathPosition = this.transform.position;
        if (isInvincible && isJuking && canSpawnText && GetComponent<WarriorAiController>() == null)
        {
            // TRIGGER "SMOOTH" flair text if a player dodges an instant death attack
            if (canSpawnText) {
                Instantiate(dodgeTextPrefab, transform.position, Quaternion.identity);
                canSpawnText = false;
                StartCoroutine(TextSpawnReset());

                // Steam Achievement Stuff
                if (SteamManager.Initialized)
                {
                    Debug.Log("Getting Stats: " + SteamUserStats.RequestCurrentStats());
                    Debug.Log("Setting Achievement: " + SteamUserStats.SetAchievement("SMOOTH_JUKE"));
                    Debug.Log("Storing Stats: " + SteamUserStats.StoreStats());
                }
            }
        }
        if (isInvincible || isWinner) return;
        
        // Chance for AiWarrior to dodge if slide is off cooldown
        if (GetComponent<WarriorAiController>() != null // Ensure this is an AI Warrior
            && Random.value < GetComponent<WarriorAiController>().GetDodgeChance() // Get chance to dodge
            && CanSlide()) // ensure warrior can slide
        {
            Debug.Log("Dodge!");
            Sliding();
            return;
        }

        Debug.Log("PLAYER THAT DIED: (" + this + ")");
        //ST.UpdateWDeaths(int.Parse(this.name.Substring(0,1)));
        

        if (playerNum == 1)
        {
            UM.PlayerIconGreyout(true, warriorPosition);
            ST.UpdateWDeaths(1);
            UM.UpdateWarriorDeathsSB(1);
        }
        if (playerNum == 2)
        {
            UM.PlayerIconGreyout(true, warriorPosition);
            ST.UpdateWDeaths(2);
            UM.UpdateWarriorDeathsSB(2);
        }
        if (playerNum == 3)
        {
            UM.PlayerIconGreyout(true, warriorPosition);
            ST.UpdateWDeaths(3);
            UM.UpdateWarriorDeathsSB(3);
        }

        ST.UpdateMKills();
        UM.UpdateMonsterKillsSB();
        WUI.ShowCallForPass(false);

        StopSuperKick();
        isDead = true;
        isInvincible = true;
        isBomb = false;
        bombTimer = 0;
        PS.Stop();

        if (BP != null && BP.ballOwner != null && BP.ballOwner.Equals(gameObject))
        {
            // Debug.Log("ballOwner set to null");
            BP.ballOwner = null;
            // stop ball from counting as a pass
            BP.SetPassTimer(BP.passTimeFrame);
        }

        // Gore
        int goreMode = PlayerPrefs.GetInt("goreMode", 0);
        if (goreParticleObj != null && goreMode == 0) {
            GameObject obj = Instantiate(goreParticleObj, transform.position, Quaternion.identity);
            SpriteBurst burst = obj.GetComponent<SpriteBurst>();
            if (burst != null) burst.spriteColor = GetColor();
        } else if (goreParticleObj != null && goreMode == 1) {
            Instantiate(pinataParticleObj, transform.position, Quaternion.identity);
        }
        //Instantiate(particleObj, transform.position, Quaternion.identity);

        // pump up minotaur boost state if active
        AbilityMinotaurBoost minotaurBoost = GameObject.FindGameObjectWithTag("Monster").GetComponent<AbilityMinotaurBoost>();
        if (minotaurBoost != null && minotaurBoost.GetActive())
        {
            minotaurBoost.AddBonus();
        }

        // soul orb spawn if fighting gasha
        AbilityGashaPassive AGP = GameObject.FindGameObjectWithTag("Monster").GetComponent<AbilityGashaPassive>();
        if (AGP != null)
        {
            AGP.AddAndSpawnOrb(AGP.bonusOnKill, transform.position);
        }

        transform.position = respawnBox.position;
        MTC.RemoveTarget(transform);
        health = healthMax;
        PlayDeathSound();
        CSM.PlayDeathSound(true);
        StopAllCoroutines();
        
        //Curse
        if (isCursed)
        {
            Debug.Log("Cursed Mummy Should Spawn");
            if (aiMummyManager != null)
            {
                aiMummyManager.CurseMummySpawn(deathPosition);
                // Instantiate(Mummy, deathPosition, Quaternion.identity);
            }

        }

        // Update list of warriors in AiMonsterController if appropriate
        AiMonsterController aiMonsterController = FindObjectOfType<MonsterController>().GetComponent<AiMonsterController>();
        if (aiMonsterController != null)
        {
            // Debug.Log("Die");
            // Debug.Log("Before: " + aiMonsterController.warriors.Count);
            aiMonsterController.warriors.Remove(gameObject);
            // Debug.Log("After: " + aiMonsterController.warriors.Count);
        }

        //Respawn();
        respawnTimer = 0f;
        isCursed = false;
        StartCoroutine(SetInvincibility(false, respawnTime + respawnInvincibilityTime));
    }

    public void Damage(int amount)
    {
        if (isInvincible || isWinner) return;
        health -= amount;
        if (health <= 0)
        {
            Die();
        } else
        {
            audioPlayer.PlaySoundRandomPitch(audioPlayer.Find("damage"));
            StartCoroutine(SetInvincibility(true, 0.1f));
            StartCoroutine(SetInvincibility(false, damageInvincibilityTime + 0.1f));
            if (willBeStunnedOnHit) Stun(0.1f);
        }
    }

    public void DamageWithInstantInvincibility(int amount)
    {
        if (isInvincible) return;
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
        else
        {
            audioPlayer.PlaySoundRandomPitch(audioPlayer.Find("damage"));
            SetInvincibility(true);
            StartCoroutine(SetInvincibility(false, damageInvincibilityTime));
            if (willBeStunnedOnHit) Stun(0.1f);
        }
    }

    public void SetIsSliding(bool isSliding)
    {
        this.isSliding = isSliding;
    }

    public void Stun(float stunTime)
    {
        if (isStunned) return;
        if (isSliding) return;
        isStunned = true;
        rb.velocity = Vector3.zero;
        //audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find("minotaurStun"), 0.5f);
        //CSM.PlayDeathSound(false);
        StartCoroutine(ResetStun(stunTime));
    }

    private IEnumerator ResetStun(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        isStunned = false;
    }

    private bool CanCallForPass()
    {
        return
            ((Time.time - lastCallForPassTime) >= callForPassCooldown) // Ensure call for pass is off cooldown
            && callForPassWindowCheck == null; // Ensure callForPassWindow check coroutine isn't already going
    }

    /**
     *  The Following Code Is For Controller Inputs
     **/

    public void OnMove(InputAction.CallbackContext context)
    {
        //Debug.Log("OnMove");
        if (!usingKeyboard) movementDirection = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y).normalized;
        if (isCursed) movementDirection *= -1;
        usingKeyboard = false;
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (!canReadAimInput) return;
        
        rightStickInput = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);

        if (invertControls)
        {
            rightStickInput.x = -rightStickInput.x;
            rightStickInput.z = -rightStickInput.z;
        }

        if (rightStickInput.magnitude > kickingSensitivity && !usingKeyboard)
        {
            aimingDirection = rightStickInput.normalized;

            if (kickMode == 0)
            {
                isKickCharging = true;
            }
            //Debug.Log("AIMING DIR " + aimingDirection);
            //Debug.Log("RIGHT STICK MAG " + rightStickInput.magnitude);
        }
        else if (kickMode == 0 && rightStickInput.magnitude <= kickingSensitivity && !usingKeyboard)
        {
            isKickCharging = false;
        }
        usingKeyboard = false;
    }

    public Vector3 GetAimDirection()
    {
        return aimingDirection;
    }

    public void OnSlide(InputAction.CallbackContext context)
    {
        if (GM.isPlaying && !isDead && !GM.isPaused) Sliding();
    }

    public void OnCallForPass(InputAction.CallbackContext context)
    {
        if (!GM.isPlaying || isDead || GM.isPaused // Ensure game is playing
            || !CanCallForPass()
            || BP == null || BP.ballOwner == null|| BP.ballOwner.GetComponent<WarriorController>() == null // Ensure a warrior has the ball
            || BP.ballOwner.Equals(gameObject)) // and ballowner isn't itself
            return;

        Debug.Log("OnCallForPass");
        lastCallForPassTime = Time.time;

        // Queue some kind of UI element to indicate calling for pass
        if (audioPlayer != null) audioPlayer.PlaySoundRandomPitch(audioPlayer.Find("callForPass1"));
        WUI.ShowCallForPass(true);

        // Create a temporary "gravity" effect around this player  
        if (BP != null && BP.ballOwner != null
            && BP.ballOwner.GetComponent<WarriorController>() != null)
        {
            Debug.Log("Ball owner is a warrior teammate");

            callForPassWindowCheck = StartCoroutine(PassWindowCheck());

            // For AI teammates...
            // If ball owner is a teammate, have them pass me the ball
            if (BP.ballOwner.GetComponent<WarriorAiController>() != null)
            {
                BP.ballOwner.GetComponent<WarriorAiController>().CallForPassing(GetComponent<WarriorController>());
            }

        }
        
    }

    private IEnumerator PassWindowCheck()
    {
        // Debug.Log("Start PassWindowCheck");
        // Debug.Log("passWindowTimer: " + passWindowTimer);
        // Debug.Log("passWindowDuration: " + passWindowDuration);
        while (passWindowTimer < passWindowDuration)
        {
            Debug.Log("pass window timer: " + passWindowTimer + ", duration: " + passWindowDuration);
            // Debug.Log("Tick");
            // Check if kick happened
            if (kickHappened)
            {
                // Enable gravity field
                Debug.Log("Enable gravity field from pass window");
                EnableGravityField();
                break;
            }

            passWindowTimer += Time.deltaTime;
            yield return null;
        }

        passWindowTimer = 0f;
        WUI.ShowCallForPass(false);
        callForPassWindowCheck = null;
    }

    private IEnumerator ResetKickHappened()
    {
        yield return new WaitForSeconds(0.1f); // however long to allow kickHappened to be true, before reseting back to false
        kickHappened = false;
    }

    private void EnableGravityField()
    {
        if (gravityCoroutine != null) StopCoroutine(gravityCoroutine); // Prevent overlapping
        gravityCoroutine = StartCoroutine(GravityFieldCoroutine());
    }

    public float GetRespawnTime()
    {
        return respawnTime;
    }

    private IEnumerator GravityFieldCoroutine()
    {
        if (BP == null) yield break; // Safety check

        float timer = 0f;
        Rigidbody ballRb = BP.GetComponent<Rigidbody>();

        if (ballRb == null) yield break; // Safety check

        while (timer < gravityFieldDuration
            && BP != null && BP.ballOwner != gameObject) // Ensure gravity field ends early if this warrior gets the ball
        {
            Vector3 toWarrior = transform.position - ballRb.position; // Direction to warrior
            float distance = toWarrior.magnitude;

            if (distance < gravityRadius) // Only pull if within range
            {
                Vector3 pullForce = toWarrior.normalized * gravityForce;
                ballRb.AddForce(pullForce, ForceMode.Acceleration);
            }

            timer += Time.deltaTime;
            yield return null;
        }
    }

    public void SetKickHappened(bool kickHappened)
    {
        WarriorController.kickHappened = kickHappened;
        StartCoroutine(ResetKickHappened());
    }


    public void OnInvert(InputAction.CallbackContext context)
    {
        // Method is now used for Pausing the game. Renaming would cause greater issues not worth the effort.
        if (GM.isPlaying)
        {
            GM.PauseGame(playerID);
        } else if (UM.GetTimeRemaining() < 0)
        {
            GM.MenuReturn();
        }
    }

    public void OnKick(InputAction.CallbackContext context)
    {
        if (kickMode == 1)
        {
            if (context.started)
            {
                isKickCharging = true;
            }
            if (context.canceled)
            {
                isKickCharging = false;
            }
        }
    }

    public void OnSuperKick(InputAction.CallbackContext context)
    {
        if (!superKicking && GM.passMeter > 0 && isCharging && !isDead)
        {
            superKicking = true;
            aura.SetActive(true);
            AV.SuperKickColor(Color.red);
            //if (GM.passMeter < 1)
            if (!BP.canFullSuperKick)
            {
                Instantiate(superTextPrefab, transform.position, Quaternion.identity);
                canSpawnText = false;
                StartCoroutine(TextSpawnReset());
            } else
            {

                Instantiate(superTextFullPrefab, transform.position, Quaternion.identity);
                canSpawnText = false;
                StartCoroutine(TextSpawnReset());

            }
            
        }
    }

    public void OnTaunt(InputAction.CallbackContext context)
    {
        if (!canSpawnText || GM.isGameOver) return;
        string tauntNum = context.control.displayName;
        int tauntStyle = 0;
        switch (tauntNum)
        {
            case "D-Pad Up":
                Debug.Log("Taunt 1 Active");
                tauntStyle = 0;
                break;
            case "D-Pad Right":
                Debug.Log("Taunt 2 Active");
                tauntStyle = 1;
                break;
            case "D-Pad Down":
                Debug.Log("Taunt 3 Active");
                tauntStyle = 2;
                break;
            case "D-Pad Left":
                Debug.Log("Taunt 4 Active");
                tauntStyle = 3;
                break;
            default:
                Debug.Log("Unknown Taunt");
                tauntStyle = 0;
                break;
        }

        Instantiate(tauntTextPrefabs[tauntStyle], transform.position, Quaternion.identity);
        canSpawnText = false;
        StartCoroutine(TextSpawnReset(1f));
    }

    public void StopSuperKick()
    {
        Debug.Log("StopSuperKick");
        superKicking = false;
        AV.RevertColor();
        Invoke("DisableAura", auraLingerDuration);
        BP.StopBallGlow();
    }

    private void DisableAura()
    {
        aura.SetActive(false);
    }

    /**
     *  The Following Code Is For Helper Methods
     **/
    public void SetColor(Color color)
    {
        //Debug.Log("Set color called with i = " + i);
        try
        {
            ring.color = color;
            transparentRing.color = new Color(color.r, color.g, color.b, 0.3f);
            callForPass.color = color;
        }
        catch
        {
            ring.color = color;
            transparentRing.color = new Color(color.r, color.g, color.b, 0.3f);
            callForPass.color = color;
        }

    }

    public Color GetColor()
    {
        return ring.color;
    }

    public Color GetSkinColor()
    {
        if (PH == null) return Color.white;
        return PH.skinColor;
    }

    public void SetKickSen(float sens)
    {
        kickingSensitivity = sens;
    }

    public float GetKickSens()
    {
        return kickingSensitivity;
    }

    IEnumerator SetInvincibility(bool invin, float time)
    {
        Debug.Log("Invincibility will be set to " + invin + " in " + time + " seconds");
        yield return new WaitForSeconds(time);
        isInvincible = invin;
    }

    public void SetInvincibility(bool invin)
    {
        isInvincible = invin;
    }

    public void InvincibilityFlash()
    {
        if (spriteObject == null) return;

        if (isDead)
        {
            spriteObject.transform.localScale = spriteScale;
            return;
        }

        if (isStunned && Time.frameCount % 2 == 0)
        {
            spriteObject.transform.localScale = Vector3.zero;
        }
        else if (isInvincible && Time.frameCount % 4 == 0 && !isSliding && canRespawn)
        {
            spriteObject.transform.localScale = Vector3.zero;
        } else 
        {
            spriteObject.transform.localScale = spriteScale;
        }
    }

    public bool IsSliding()
    {
        return isSliding;
    }

    public bool GetCursedStatus()
    {
        return isCursed;
    }

    void PlayKickSound(float charge)
    {
        if (charge >= maxCharge)
        {
            audioPlayer.PlaySoundRandomPitch(audioPlayer.Find("kick3"));
        }
        else if (charge >= maxCharge / 2f)
        {
            audioPlayer.PlaySoundRandomPitch(audioPlayer.Find("kick2"));
        }
        else
        {
            audioPlayer.PlaySoundRandomPitch(audioPlayer.Find("kick1"));
        }
    }

    void PlayDeathSound()
    {
        int i = Random.Range(1, 6);
        audioPlayer.PlaySoundSpecificPitch(audioPlayer.Find("warriorDeath" + i.ToString()), 1.75f);
    }

    public void BecomeBomb()
    {
        isBomb = true;
    }

    public void SetPlayerNum(int num)
    {
        this.playerNum = num;
    }

    public bool GetIsDead()
    {
        return isDead;
    }

    public bool CanSlide()
    {
        return !isStunned && !isCursed
            && (Time.time - lastSlideTime >= slideCooldown)
            && (movementDirection != Vector3.zero)
            && !isSliding
            && !superKicking;
    }

    private IEnumerator TextSpawnReset()
    {
        yield return new WaitForSeconds(0.15f);
        canSpawnText = true;
    }

    private IEnumerator TextSpawnReset(float time)
    {
        yield return new WaitForSeconds(1f);
        canSpawnText = true;
    }

    private void OnTriggerStay(Collider other)
    {
        BallProperties BP = other.GetComponent<BallProperties>();
        // Debug.Log("Other: " + other);

        if (BP == null)
        {
            // Debug.Log("No Ball found");
            //pickupBallTimer = pickupBallCooldown;
        }
        else if (BP.ballOwner != null)
        {
            // Debug.Log("Already have ball OR someone else has ball");
            pickupBallTimer = pickupBallCooldown;
        }
        // If ball hasn't been in warrior's colliders long enough
        else if (BP != null && pickupBallTimer > 0 && !isStunned)
        {
            // Count down timer
            // Debug.Log("Waiting to pick up ball");
            pickupBallTimer -= Time.deltaTime;
        }
        // If has been in warrior's collider long enough
        else if (BP != null && pickupBallTimer <= 0 && BP.isInteractable)
        {
            // if you were last kicker and ball is in singleMode, return
            if (BP.isInSingleOutMode && BP.previousKicker == gameObject) return;
            // Pick up ball
            Debug.Log("Pick up ball");
            pickupBallTimer = pickupBallCooldown;
            BP.GetComponent<Rigidbody>().velocity = Vector3.zero;
            BP.ballOwner = gameObject;
            BP.SetOwner(BP.ballOwner);
        }
    }

    public int GetHealth()
    {
        return health;
    }

    public void SetControlScheme()
    {
        switch (PH.controlScheme)
        {
            case 0:
                invertControls = false;
                kickMode = 0;
                break;
            case 1:
                invertControls = true;
                kickMode = 1;
                break;
            default:
                break;
        }
    }

    public Vector3 GetAnticipatedPosition(float inSeconds)
    {
        // Get the object's Rigidbody component
        Rigidbody rb = GetComponent<Rigidbody>();

        // If no Rigidbody is found, return the current position as a fallback
        if (rb == null) return transform.position;

        // Predict the future position based on rb velocity
        return transform.position + (rb.velocity * inSeconds);
    }
}
