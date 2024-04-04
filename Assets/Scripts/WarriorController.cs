using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorController : MonoBehaviour
{  
    private Rigidbody rb;
    [SerializeField] public GameObject Ball = null;
    public BallProperties BP = null;

    [SerializeField] float warriorSpeed = 2f;
    private Vector3 movementDirection;

    [SerializeField] private GameObject ballPosition;

    [SerializeField] private float passSpeed = 5.0f;

    [SerializeField] private GameplayManager GM = null;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        BP = (BallProperties) Ball.GetComponent("BallProperties");
    }

    // Update is called once per frame
    void Update()
    {
        if (GM.isPlaying)
        {
            Movement();
            Dribbling();
            Passing();
        }
    }

    void Movement()
    {
        movementDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        rb.velocity = movementDirection * warriorSpeed;
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
        }
    }

    void Passing()
    {
        if(Input.GetKeyDown(KeyCode.Space) && BP.ballOwner == gameObject)
        {
            Debug.Log("Pass!");
            BP.ballOwner = null;
            Debug.Log(transform.forward);
            BP.GetComponent<Rigidbody>().AddForce(transform.forward * passSpeed);
        }
    }
}
