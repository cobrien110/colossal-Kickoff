using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MonsterController : MonoBehaviour
{  
    private Rigidbody rb;
    [SerializeField] public GameObject Ball = null;
    public BallProperties BP = null;
    public GameObject wallPrefab;

    [SerializeField] float monsterSpeed = 2f;
    private Vector3 movementDirection;

    [SerializeField] private GameObject ballPosition;

    [Header("Stats")]
    [SerializeField] private float passSpeed = 5.0f;
    [SerializeField] private float kickSpeed = 5.0f;
    [SerializeField] private float chargeMultiplier = 0.5f;
    [SerializeField] private float maxChargeSeconds = 2f;
    [SerializeField] private float chargeMoveSpeedMult = 0.2f;
    private float kickCharge = 1f;
    private bool isCharging;
    [SerializeField] private float wallSpawnDistance = 2f;
    [SerializeField] private float wallCooldown = 5f;
    private float wallTimer;


    [SerializeField] private GameplayManager GM = null;
    private AudioPlayer audioPlayer;
    private GameObject monsterSpawner = null;
    public float attackRange = 1f;
    public LayerMask layerMask;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
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
            Attack();
            BuildWall();
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

        movementDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

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
        if (Input.GetKeyUp(KeyCode.KeypadEnter) && BP.ballOwner == gameObject)
        {
            Debug.Log("Kick!");
            BP.ballOwner = null;
            Debug.Log(kickCharge);
            BP.GetComponent<Rigidbody>().AddForce(transform.forward * kickSpeed * (kickCharge * chargeMultiplier));

            PlayKickSound(kickCharge);
        }
        if (Input.GetKey(KeyCode.KeypadEnter) && BP.ballOwner == gameObject)
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
        if (Input.GetKeyDown(KeyCode.Backspace)) {
            Debug.Log("Attack!");
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, attackRange, layerMask))
            {
                // If ray hits something, handle the collision here
                Debug.Log("Raycast hit " + hit.collider.gameObject.name);
            }
        }
        
    }

    void BuildWall()
    {
        if (wallTimer < wallCooldown)
        {
            wallTimer += Time.deltaTime;
        }
        if (wallTimer >= wallCooldown && Input.GetKeyDown(KeyCode.J))
        {
            wallTimer = 0f;
            Vector3 spawnLocation = transform.position + (movementDirection * wallSpawnDistance);
            audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find("minotaurCreateWall"), 0.2f);
            Instantiate(wallPrefab, spawnLocation, transform.rotation);
        }
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
}
