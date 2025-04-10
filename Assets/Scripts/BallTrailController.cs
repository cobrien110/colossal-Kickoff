using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallTrailController : MonoBehaviour
{
    private ParticleSystem ballTrail;
    private Rigidbody rb;
    private BallProperties BP;
    [SerializeField] private float maxRateOverDistance = 50f;
    [SerializeField] private float minRateOverDistance = 10f;

    private Color normalColor;
    [SerializeField] private Color superKickColor = Color.red;
    [SerializeField] private float colorLerpSpeed = 2f;

    private Color currentColor;
    private bool wasSuperKicked = false;

    void Start()
    {
        rb = transform.parent.gameObject.GetComponent<Rigidbody>();
        ballTrail = GetComponent<ParticleSystem>();
        BP = transform.parent.gameObject.GetComponent<BallProperties>();

        // Save base color of trail
        if (ballTrail != null)
        {
            normalColor = ballTrail.main.startColor.color;
            currentColor = normalColor;
        }
    }

    void Update()
    {
        var emission = ballTrail.emission;
        float speed = rb.velocity.magnitude;

        if (speed < 0.2f)
        {
            emission.rateOverDistance = 0f;
            emission.rateOverTime = 0f;
        }
        else
        {
            emission.rateOverDistance = Mathf.Lerp(minRateOverDistance, maxRateOverDistance, speed / BP.maxSpeed); // Adjust density based on speed
        }


        // Change ball trail color on super kick
        if (BP.ballOwner != null && BP.ballOwner.GetComponent<WarriorController>() != null
        && BP.ballOwner.GetComponent<WarriorController>().superKicking)
        {
            Debug.Log("Superkicking: " + BP.ballOwner.GetComponent<WarriorController>().superKicking);

            currentColor = superKickColor;
            wasSuperKicked = true;
        }
        else if (wasSuperKicked)
        {
            // Smoothly lerp back to base color
            currentColor = Color.Lerp(currentColor, normalColor, Time.deltaTime * colorLerpSpeed);

            if (Vector4.Distance(currentColor, normalColor) < 0.01f)
            {
                currentColor = normalColor;
                wasSuperKicked = false;
            }
        }

        // Apply color to particle system
        var main = ballTrail.main;
        main.startColor = currentColor;

        // Debug.Log("ball speed: " + speed);
    }

    void LateUpdate()
    {
        ballTrail.transform.rotation = Quaternion.identity;
    }

}

