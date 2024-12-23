using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalWithBarrier : MonoBehaviour
{
    public bool startWithBariers = false;
    public bool canBeScoredIn = false;
    public float maxHealth = 10;
    public float health;
    public BoxCollider col;
    public float maxBounceAngle = 45f;
    public float bounceForce = 4f;
    private float delayAfterInteraction = 0.25f;
    private float delayAfterDamage = 0.25f;
    private float timer;
    private float timerDamage;
    public GameObject[] barrierObjects;

    AudioPlayer AP; 
    // Start is called before the first frame update
    void Start()
    {
        if (startWithBariers)
        {
            health = maxHealth;
        } else
        {
            health = 0;
        }
        
        col = GetComponent<BoxCollider>();
        AP = GetComponent<AudioPlayer>();
        //col.isTrigger = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0 && !canBeScoredIn)
        {
            SetCanScore(true);
        }
        if (timer < delayAfterInteraction)
        {
            timer += Time.deltaTime;
        }
        if (timerDamage < delayAfterDamage)
        {
            timerDamage += Time.deltaTime;
        }

        UpdateBars();
    }

    void SetCanScore(bool b)
    {
        //col.isTrigger = b;
        canBeScoredIn = b;
    }

    public void TakeBallDamage(float damage)
    {
        if (timerDamage < delayAfterDamage) return;
        timerDamage = 0f;

        Debug.Log(gameObject.name + " hit by ball with " + damage + " velocity");

        // if ball is too slow, do not damage
        if (damage < 2) return;
        health -= damage;
    }

    public void RejectBall(Rigidbody rb)
    {
        if (timer < delayAfterInteraction) return;
        timer = 0;
        Debug.Log("Attempting to reject ball");
        rb.velocity = Vector3.zero;

        // Bounce the ball away in a random direction
        // Calculate the base direction to the origin
        Vector3 pos = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 baseDirection = (Vector3.zero - pos).normalized;

        // Create a random rotation within the cone angle
        float halfAngle = maxBounceAngle / 2f;
        float randomYaw = Random.Range(-halfAngle, halfAngle); // Rotation around the Y-axis
        float randomPitch = Random.Range(-halfAngle, halfAngle); // Rotation around the X-axis

        // Combine the rotations into a Quaternion
        Quaternion randomRotation = Quaternion.Euler(randomPitch, randomYaw, 0);

        // Rotate the base direction by the random rotation
        Vector3 dir = randomRotation * baseDirection;
        //Debug.Log(dir);

        // add force to the ball
        Vector3 force = dir * bounceForce;
        rb.AddForce(force);
    }

    public void Restart()
    {
        if (startWithBariers)
        {
            health = maxHealth;
            timer = 0f;
            timerDamage = 0f;
        }
        
    }

    private void UpdateBars()
    {
        int num = barrierObjects.Length;
        if (num == 0) return;

        float threshold = (1f / num) * maxHealth;
        //Debug.Log("threshold: " + threshold);

        for (int i = num-1; i >= 0; i--)
        {
            //Debug.Log("Health: " + health + "    Threshold: " + i * threshold);
            if (health <= i * threshold)
            {
                barrierObjects[i].SetActive(false);
            } else
            {
                barrierObjects[i].SetActive(true);
            }
        }
    }
}
