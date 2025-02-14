using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalWithBarrier : MonoBehaviour
{
    public bool startWithBariers = false;
    [Range(0,2)]
    public int respawnType = 0;
    public bool canBeScoredIn = false;
    public float maxHealth = 10;
    public float health;
    public BoxCollider col;
    public float maxBounceAngle = 45f;
    public float bounceForce = 4f;
    private float delayAfterInteraction = 0.25f;
    private float delayAfterDamage = 0.25f;
    [SerializeField] private float delayBeforeCanBeScored = 0.35f;
    private float canScoreTimer = 0f;
    private float timer;
    private float timerDamage;
    public GameObject[] barrierObjects;
    public float xPos = 7f;

    AudioPlayer AP;
    GameplayManager GM;
    public ParticleSystem GoalParticles;
    private AudioPlayer ParticleAudio;
    public AudioPlayer BarrierAudio;

    // Start is called before the first frame update
    void Start()
    {
        GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
        SetStats();

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

        if (transform.position.x < 0)
        {
            xPos = -xPos;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0 && canScoreTimer < delayBeforeCanBeScored)
        {
            canScoreTimer += Time.deltaTime;
        }

        if (health <= 0 && !canBeScoredIn
            && (timer >= delayAfterInteraction) && (canScoreTimer >= delayBeforeCanBeScored))
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
        
        if (health <= 0)
        {
            BarrierAudio.PlaySoundRandomPitch(BarrierAudio.Find("goalBarrierBreak"));
        } else
        {
            BarrierAudio.PlaySoundRandomPitch(BarrierAudio.Find("goalBarrierHit"));
        }
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
        // Fix ball position
        Vector3 tempPos = rb.transform.position;
        rb.transform.position = new Vector3(xPos,tempPos.y, tempPos.z);
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

    public void ResetTimers()
    {
        timer = delayAfterInteraction;
        timerDamage = delayAfterDamage;
    }

    public void Respawn()
    {
        if (!startWithBariers) return;
        if (respawnType == 0) // don't respawn
        {
            return;
        } else if (respawnType == 1) // respawn if fully gone
        {
            if (health <= 0)
            {
                health = maxHealth;
            }
        } else // completely respawn
        {
            health = maxHealth;
        }
    }

    public void PerformGoalEffects()
    {
        //yield return new WaitForSeconds(effectDelay);
        GoalParticles.Play();
        ParticleAudio = GoalParticles.GetComponent<AudioPlayer>();
        ParticleAudio.PlaySoundRandomPitch(ParticleAudio.Find("goalConfetti"));
    }

    public void SetStats()
    {
        if (GM == null) return;
        maxHealth = GM.barrierMaxHealth;
        startWithBariers = GM.barriersAreOn;
        respawnType = GM.barrierRespawnStyle;
        maxBounceAngle = GM.barrierBounceAngle;
        bounceForce = GM.barrierBounceForce;
    }
}
