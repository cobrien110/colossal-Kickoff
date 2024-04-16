using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.InputSystem;
using UnityEngine;

public class WarriorController : MonoBehaviour
{  
    private Rigidbody rb;
    [SerializeField] public GameObject Ball = null;
    //[SerializeField] public GameObject lastKicked = null;

    public BallProperties BP = null;

    [SerializeField] float warriorSpeed = 2f;
    private Vector3 movementDirection;
    private Vector3 aimingDirection;
    private Vector3 rightStickInput;

    [SerializeField] private GameObject ballPosition;

    [SerializeField] private float passSpeed = 5.0f;
    [SerializeField] private float kickSpeed = 5.0f;
    [SerializeField] private float chargeMultiplier = 0.5f;
    [SerializeField] private float maxChargeSeconds = 2f;
    [SerializeField] private float chargeMoveSpeedMult = 0.2f;
    private float kickCharge = 1f;
    private bool isCharging;

    //Make True If Using Keyboard For Movement
    [Header("Click True If Using Keyboard For Movement")]
    public bool usingKeyboard = false;


    [SerializeField] private GameplayManager GM = null;
    private AudioPlayer audioPlayer;
    private GameObject WarriorSpawner = null;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        BP = (BallProperties) Ball.GetComponent("BallProperties");
        audioPlayer = GetComponent<AudioPlayer>();
        WarriorSpawner = GameObject.Find("WarriorSpawner");
        transform.position = WarriorSpawner.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (usingKeyboard)
        {
            Movement();
        }

        if (GM.isPlaying)
        {  
            Dribbling();
            Passing();
            Kicking();
        }
    }

    void Movement()
    {
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

        movementDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;
        if (usingKeyboard && movementDirection != Vector3.zero) aimingDirection = movementDirection;

        rb.velocity = GM.isPlaying ? movementDirection * warriorSpeed : Vector3.zero;
        rb.velocity = isCharging ? rb.velocity * chargeMoveSpeedMult : rb.velocity;
        if (rb.velocity != Vector3.zero) 
        {
            Quaternion newRotation = Quaternion.LookRotation(movementDirection.normalized, Vector3.up);
            transform.rotation = newRotation;
        }
    }

    void Dribbling()
    {
        if (BP.ballOwner == gameObject)
        {
            Ball.transform.position = ballPosition.transform.position; // new Vector3(transform.position.x, 2, transform.position.z);
        } else
        {
            BP.ballOwner = null;
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

            PlayKickSound(kickCharge);
            StartCoroutine(KickDelay());
        }
        if (((rightStickInput != Vector3.zero && usingKeyboard) || Input.GetKey(KeyCode.Space)) && BP.ballOwner == gameObject)
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
        movementDirection = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y).normalized;

        rb.velocity = GM.isPlaying ? movementDirection * warriorSpeed : Vector3.zero;
        rb.velocity = isCharging ? rb.velocity * chargeMoveSpeedMult : rb.velocity;
        if (rb.velocity != Vector3.zero)
        {
            Quaternion newRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = newRotation;
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        rightStickInput = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);
        if (rightStickInput != Vector3.zero)
        {
            aimingDirection = rightStickInput.normalized;
        }
    }
}
