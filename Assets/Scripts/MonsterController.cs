using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class MonsterController : MonoBehaviour
{  
    private Rigidbody rb;
    [SerializeField] public GameObject Ball = null;
    public BallProperties BP = null;
    public GameObject wallPrefab;

    [SerializeField] float monsterSpeed = 2f;
    [HideInInspector] public Vector3 movementDirection;
    private Vector3 aimingDirection;
    private Vector3 rightStickInput;

    //Make True If Using Keyboard For Movement
    public bool usingKeyboard = false;

    [SerializeField] private GameObject ballPosition;

    [Header("Stats")]
    [SerializeField] private float passSpeed = 5.0f;
    [SerializeField] private float kickSpeed = 5.0f;
    [SerializeField] private float chargeMultiplier = 0.5f;
    [SerializeField] private float maxChargeSeconds = 2f;
    [SerializeField] private float chargeMoveSpeedMult = 0.2f;
    private float kickCharge = 1f;
    private bool isCharging;
    [Header("Ability Stats")]
    [SerializeField] private float wallSpawnDistance = 2f;
    [SerializeField] private float wallCooldown = 5f;
    [SerializeField] public int shrapnelAmount = 5;
    [SerializeField] public int shrapnelDamage = 1;
    [SerializeField] public float shrapnelSpeed = 500f;
    [SerializeField] public float wallDuration = 8f;
    private float wallTimer;


    [SerializeField] private GameplayManager GM = null;
    private AudioPlayer audioPlayer;
    private GameObject monsterSpawner = null;
    public float attackRange = 1f;
    public LayerMask layerMask;


    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
        Ball = GameObject.Find("Ball");
        BP = (BallProperties) Ball.GetComponent("BallProperties");
        audioPlayer = GetComponent<AudioPlayer>();
        monsterSpawner = GameObject.Find("MonsterSpawner");
        transform.position = monsterSpawner.transform.position;
        wallTimer = wallCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        //Movement();
        if (GM.isPlaying)
        {  
            Dribbling();
            Passing();
            Kicking();

            if (Input.GetKey(KeyCode.Backspace))
            {
                Attack();
            }

            if (wallTimer >= wallCooldown && (Input.GetKeyDown(KeyCode.J)))
            {
                BuildWall();
            }        
        }

        // Cooldowns
        if (wallTimer < wallCooldown)
        {
            wallTimer += Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        //Movement();
    }

    void Movement()
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

        rb.velocity = GM.isPlaying ? movementDirection * monsterSpeed : Vector3.zero;
        rb.velocity = isCharging ? rb.velocity * chargeMoveSpeedMult : rb.velocity;
        if (rb.velocity != Vector3.zero) 
        {
            Quaternion newRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = newRotation;
        }
    }

    void Dribbling()
    {
        if (BP.ballOwner == gameObject)
        {
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
        if (((rightStickInput == Vector3.zero && !usingKeyboard) || Input.GetKeyUp(KeyCode.KeypadEnter)) && BP.ballOwner == gameObject && kickCharge != 1)
        {
            Debug.Log("Kick!");
            BP.ballOwner = null;
            BP.lastKicker = gameObject;
            Debug.Log(kickCharge);
            float kickForce = kickSpeed * (kickCharge * chargeMultiplier);
            Vector3 forceToAdd = aimingDirection * kickForce;
            BP.GetComponent<Rigidbody>().AddForce(forceToAdd);

            PlayKickSound(kickCharge);
            StartCoroutine(KickDelay());
        }
        if (((rightStickInput != Vector3.zero && !usingKeyboard) || Input.GetKey(KeyCode.KeypadEnter)) && BP.ballOwner == gameObject)
        {
            if (kickCharge <= maxChargeSeconds)
            {
                //Debug.Log(kickCharge);
                kickCharge += Time.deltaTime;
                isCharging = true;
            }
        }
        else
        {
            kickCharge = 1f;
            isCharging = false;
        }
    }

    void Attack()
    {
        Debug.Log("Attack!");
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, attackRange, layerMask))
        {
            // If ray hits something, handle the collision here
            Debug.Log("Raycast hit " + hit.collider.gameObject.name);
            if (hit.collider.gameObject.CompareTag("Warrior"))
            {
                WarriorController WC = hit.collider.GetComponent<WarriorController>();
                if (!WC.isInvincible) WC.Die();
            }
        }
        
    }

    void BuildWall()
    {
        wallTimer = 0f;
        Vector3 spawnLocation = Vector3.zero;
        Quaternion spawnRotation = Quaternion.identity;

        Vector3 dir = movementDirection;
        if (dir.Equals(Vector3.zero))
        {
            return;
        }
        spawnLocation = transform.position + (movementDirection * wallSpawnDistance);
        spawnRotation = Quaternion.LookRotation(dir, Vector3.up);

        audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find("minotaurCreateWall"), 0.2f);
        Instantiate(wallPrefab, spawnLocation, spawnRotation);
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

    public void OnWall(InputAction.CallbackContext context)
    {
        if (wallTimer >= wallCooldown && GM.isPlaying)
        {
            BuildWall();
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        Attack();
    }
}
