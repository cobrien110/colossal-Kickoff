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
    private bool superKickActive = false;
    private float startSpeed = 0f;

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
        float currentSpeed = rb.velocity.magnitude;

        if (currentSpeed < 0.2f)
        {
            emission.rateOverDistance = 0f;
            emission.rateOverTime = 0f;
        }
        else
        {
            emission.rateOverDistance = Mathf.Lerp(minRateOverDistance, maxRateOverDistance, currentSpeed / BP.maxSpeed); // Adjust density based on speed
        }


        if (BP.ballOwner != null &&
        BP.ballOwner.TryGetComponent(out WarriorController wc) &&
        wc.superKicking)
        {
            currentColor = superKickColor;
            superKickActive = true;
            startSpeed = 0f;
        }
        else if (superKickActive)
        {
            if (startSpeed == 0f && currentSpeed > BP.GetSuperKickMinStunSpeed())
            {
                startSpeed = currentSpeed;
            }

            if (startSpeed > 0f)
            {
                float t = Mathf.InverseLerp(startSpeed, BP.GetSuperKickMinStunSpeed(), currentSpeed);
                currentColor = Color.Lerp(superKickColor, normalColor, t);

                if (t >= 1f || Vector4.Distance(currentColor, normalColor) < 0.01f)
                {
                    currentColor = normalColor;
                    superKickActive = false;
                    startSpeed = 0f;
                }
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

