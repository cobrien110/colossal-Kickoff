using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.InputSystem;
using UnityEngine;
using Unity.VisualScripting;

public class WarriorController : MonoBehaviour
{  
    private Rigidbody rb;
    [SerializeField] public GameObject Ball = null;
    public BallProperties BP = null;

    [SerializeField] float warriorSpeed = 2f;
    private Vector3 movementDirection;
    private Vector3 aimingDirection;
    private Vector3 rightStickInput;

    [SerializeField] private GameObject ballPosition;

    [Header("Stats")]
    public int healthMax = 2;
    [SerializeField] private int health = 2;
    [SerializeField] private float respawnTime = 2f;
    [SerializeField] private float respawnInvincibilityTime = 1.5f;
    private bool isDead = false;
    public bool isInvincible = false;
    [SerializeField] private float passSpeed = 5.0f;
    [SerializeField] private float kickSpeed = 5.0f;
    [SerializeField] private float slideSpeed = 5.0f;
    [SerializeField] private float chargeMultiplier = 0.5f;
    [SerializeField] private float maxChargeSeconds = 2f;
    [SerializeField] private float chargeMoveSpeedMult = 0.2f;
    private float kickCharge = 1f;
    private bool isCharging;
    [SerializeField] private float slideCooldown = 1f;
    [SerializeField] private float slideDuration = 0.35f;
    private bool isSliding = false;
    private float lastSlideTime = -1f;

    //Make True If Using Keyboard For Movement
    public bool usingKeyboard = false;

    [SerializeField] private GameplayManager GM = null;
    [SerializeField] private Transform respawnBox;
    private AudioPlayer audioPlayer;
    private GameObject WarriorSpawner = null;
    [SerializeField] private Animator ANIM;
    private MultipleTargetCamera MTC;
    [SerializeField] private ParticleSystem PS;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
        Ball = GameObject.Find("Ball");
        BP = (BallProperties)Ball.GetComponent("BallProperties");
        MTC = GameObject.Find("Main Camera").GetComponent<MultipleTargetCamera>();
        //PS = GetComponent<ParticleSystem>();
        audioPlayer = GetComponent<AudioPlayer>();
        WarriorSpawner = GameObject.Find("WarriorSpawner");
        respawnBox = GameObject.FindGameObjectWithTag("RespawnBox").transform;
        transform.position = WarriorSpawner.transform.position;
        health = healthMax;
        //transform.position = WarriorSpawner.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (GM.isPlaying && !isDead)
        {  
            Dribbling();
            Passing();
            Kicking();
            if (Input.GetKey(KeyCode.E)) {
                Sliding();
            }
        }

        //Particles
        if (health < healthMax && !isDead && PS != null)
        {
            if (!PS.isPlaying) PS.Play();
        } else if (PS != null)
        {
            PS.Stop();
        }
    }

    private void FixedUpdate()
    {
        if (isDead) return;
        Movement();
    }

    void Movement()
    {
        if (isSliding) return;
        float horizontalInput = 0f;
        float verticalInput = 0f;

        // Check for WASD keys
        if (Input.GetKey(KeyCode.W))
        {
            verticalInput = 1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            verticalInput = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            horizontalInput = 1f;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            horizontalInput = -1f;
        }
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

        rb.velocity = GM.isPlaying ? movementDirection * warriorSpeed : Vector3.zero;
        rb.velocity = isCharging ? rb.velocity * chargeMoveSpeedMult : rb.velocity;
        if (rb.velocity != Vector3.zero) 
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
            Ball.transform.position = ballPosition.transform.position; // new Vector3(transform.position.x, 2, transform.position.z);
        } else
        {
            // BP.ballOwner = null; // this code was messing up the monsters ability to dribble
        }
    }

    void Passing()
    {
        if(Input.GetKeyDown(KeyCode.P) && BP.ballOwner == gameObject)
        {
            Debug.Log("Pass!");
            BP.ballOwner = null;
            Debug.Log(transform.forward);
            BP.GetComponent<Rigidbody>().AddForce(transform.forward * passSpeed);
            audioPlayer.PlaySoundRandomPitch(audioPlayer.Find("pass"));
        }
    }

    void Kicking()
    {
        if (((rightStickInput == Vector3.zero && !usingKeyboard) || Input.GetKeyUp(KeyCode.Space)) && BP.ballOwner == gameObject && kickCharge != 1)
        {
            Debug.Log("Kick!");
            BP.ballOwner = null;
            BP.lastKicker = gameObject;
            Debug.Log(kickCharge);
            float kickForce = kickSpeed * (kickCharge * chargeMultiplier);
            Vector3 forceToAdd = aimingDirection * kickForce; 
            BP.GetComponent<Rigidbody>().AddForce(forceToAdd);
            ANIM.Play("WarriorKick");

            PlayKickSound(kickCharge);
            
            StartCoroutine(KickDelay());
        }
        if (((rightStickInput != Vector3.zero && !usingKeyboard) || Input.GetKey(KeyCode.Space)) && BP.ballOwner == gameObject)
        {
            if (kickCharge <= maxChargeSeconds)
            {
                //Debug.Log(kickCharge);
                kickCharge += Time.deltaTime;
                isCharging = true;
                ANIM.SetBool("isChargingKick", true);
            }
        }
        else
        {
            kickCharge = 1f;
            isCharging = false;
            ANIM.SetBool("isChargingKick", false);
        }
    }


    void Sliding()
    {
        // Check if enough time has passed since the last slide
        if (Time.time - lastSlideTime >= slideCooldown)
        {
            if (movementDirection != Vector3.zero && BP.ballOwner != gameObject)
            {
                Debug.Log("Sliding");
                isSliding = true;
                isInvincible = true;

                // Add force in direction of the player input for this warrior (movementDirection)
                Vector3 slideVelocity = movementDirection.normalized * slideSpeed;
                rb.AddForce(slideVelocity);
                audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find("slide"), 0.5f);

                // Set isSliding to false after a delay
                Invoke("StopSliding", slideDuration);

                // Update the last slide time
                lastSlideTime = Time.time;
                ANIM.SetBool("isSliding", true);
            }
        }
    }

    void StopSliding()
    {
        Debug.Log("No longer sliding");
        ANIM.SetBool("isSliding", false);
        isSliding = false;
        isInvincible = false;
    }

    public bool IsSliding()
    {
        return isSliding;
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
        gameObject.transform.position = WarriorSpawner.transform.position;
        rb.velocity = Vector3.zero;
        health = healthMax;
        //rb.rotation = Quaternion.identity;
    }

    IEnumerator KickDelay()
    {
        yield return new WaitForSeconds(0.1f);
        ANIM.SetBool("isKicking", false);
        BP.lastKicker = null;
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);
        isDead = false;
        MTC.AddTarget(transform);
        ResetPlayer();
    }

    public void Die()
    {
        if (isInvincible) return;
        isDead = true;
        isInvincible = true;
        if (BP.ballOwner.Equals(this.gameObject) && BP.ballOwner != null)
        {
            BP.ballOwner = null;
        }
        transform.position = respawnBox.position;
        MTC.RemoveTarget(transform);
        StartCoroutine(Respawn());
        StartCoroutine(SetInvincibility(false, respawnTime + respawnInvincibilityTime));
        audioPlayer.PlaySoundSpecificPitch(audioPlayer.Find("pass"), .25f);
    }

    IEnumerator SetInvincibility(bool invin, float time)
    {
        yield return new WaitForSeconds(time);
        isInvincible = invin;
    }

    public void Damage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
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

    public void OnSlide(InputAction.CallbackContext context)
    {
        if (GM.isPlaying && !isDead) Sliding();
        /*
         * // Check if enough time has passed since the last slide
        if (Time.time - lastSlideTime >= slideCooldown)
        {
            if (movementDirection != Vector3.zero && BP.ballOwner != gameObject)
            {
                Debug.Log("Sliding");
                isSliding = true;

                // Add force in direction of the player input for this warrior (movementDirection)
                Vector3 slideVelocity = movementDirection.normalized * slideSpeed;
                rb.AddForce(slideVelocity);
                audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find("slide"), 0.5f);

                // Set isSliding to false after a delay
                Invoke("StopSliding", slideDuration);

                // Update the last slide time
                lastSlideTime = Time.time;
                ANIM.SetBool("isSliding", true);
            }
        }
         */
    }
}
