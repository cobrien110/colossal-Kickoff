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

    void Start()
    {
        rb = transform.parent.gameObject.GetComponent<Rigidbody>();
        ballTrail = GetComponent<ParticleSystem>();
        BP = transform.parent.gameObject.GetComponent<BallProperties>();
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
        // Debug.Log("ball speed: " + speed);
    }

    void LateUpdate()
    {
        ballTrail.transform.rotation = Quaternion.identity;
    }

}

