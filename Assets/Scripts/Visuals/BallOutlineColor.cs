using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallOutlineColor : MonoBehaviour
{
    /*
    [SerializeField] private Color monCol;
    [SerializeField] private Color warCol;
    [SerializeField] private Color baseCol;
    */
    private FollowBall FB;
    private SpriteRenderer SR;
    [SerializeField] private BallProperties BP;
    [SerializeField] private Sprite[] outlines;
    bool isSpinning = false;
    [SerializeField] private float spinSpeed = 100f;
    [SerializeField] private float spinSpeedPass = 20f;
    private float currentSpeed = 100f;
    private Camera cam;
    private int counter;
    private float outlineChangeTimer = 0f;
    private float outlineChangeTime = 2.5f;
    // Start is called before the first frame update
    void Start()
    {
        FB = GameObject.FindAnyObjectByType<FollowBall>();
        SR = GetComponent<SpriteRenderer>();
        if (BP == null) BP = GetComponentInParent<BallProperties>();
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        currentSpeed = spinSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerPrefs.GetInt("ballOutlineMatchesTeam") == 1)
        {
            Color fbCol = Color.white;
            if (FB != null) fbCol = FB.SR.color;
            fbCol = new Color(fbCol.r, fbCol.g, fbCol.b, 1);
            if (SR != null && SR.color != fbCol)
            {
                SR.color = fbCol;
            }
        }

        // look at camera
        // Store the current z rotation (rolling/spinning) of the object
        float zRotation = transform.eulerAngles.z;

        // Make the object face the camera
        transform.LookAt(transform.position + cam.transform.forward, cam.transform.up);

        // Restore the original Z rotation
        Vector3 euler = transform.eulerAngles;
        euler.z = zRotation;
        transform.eulerAngles = euler;

        // set outline shape
        if (SR != null && outlines.Length > 1)
        {
            //Debug.Log("Outline " + counter + ", " + outlineChangeTimer);
            if (BP.GetRB().velocity.magnitude < 1f)
            {
                counter = 0;
                isSpinning = false;
            }
            else if (BP.isFullSuperKick)
            {
                counter = 2;
                isSpinning = true;
                currentSpeed = spinSpeed;
            }
            else if (BP.isSuperKick)
            {
                counter = 1;
                isSpinning = true;
                currentSpeed = spinSpeed;
            } else if (BP.GetRB().velocity.magnitude > 10f)
            {
                counter = 1;
                isSpinning = true;
                currentSpeed = spinSpeed;
            } else if (BP.GetIsInPassState())
            {
                counter = 3;
                isSpinning = true;
                currentSpeed = spinSpeedPass;
            }
            else
            {
                counter = 0;
                isSpinning = false;
            }
            

            SR.sprite = outlines[counter];

            if (isSpinning)
            {
                transform.RotateAround(transform.forward, currentSpeed * Time.deltaTime);
            }
            else
            {

                    transform.rotation = Quaternion.identity;

            }
        }
    }
}
