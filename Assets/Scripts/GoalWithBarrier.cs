using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalWithBarrier : MonoBehaviour
{
    public bool isHumanGoal = false;
    public bool startWithBariers = false;
    [Range(0,2)]
    public int respawnType = 0;
    public bool canBeScoredIn = false;
    public float maxHealth = 10;
    public float health;
    public float maxBonusHealth = 5;
    public float bonusHealth = 0;
    public BoxCollider col;
    public float maxBounceAngle = 45f;
    public float bounceForce = 4f;
    private float delayAfterInteraction = 0.25f;
    private float delayAfterDamage = 0.25f;
    [SerializeField] private float delayBeforeCanBeScored = 0.25f;
    private float canScoreTimer = 0f;
    private float timer;
    private float timerDamage;
    public GameObject[] barrierObjects;
    public GameObject[] bonusBarrierObjects;
    private bool usingBonusBars = false;
    public float bonusHealthLossPerSecond = 0.33f;
    public float xPos = 7f;
    private bool wasJustScored = false;

    AudioPlayer AP;
    GameplayManager GM;
    public ParticleSystem GoalParticles;
    private AudioPlayer ParticleAudio;
    public AudioPlayer BarrierAudio;
    private AudioPlayer ScoreJingle;

    // Start is called before the first frame update
    void Start()
    {
        GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
        ScoreJingle = GameObject.FindGameObjectWithTag("Jukebox2").GetComponent<AudioPlayer>();
        Invoke("SetStats",0.05f);
        
        col = GetComponent<BoxCollider>();
        AP = GetComponent<AudioPlayer>();
        
        //col.isTrigger = false;

        if (transform.position.x < 0)
        {
            xPos = -xPos;
        }

        // turn off extra barriers
        foreach (GameObject barrier in bonusBarrierObjects)
        {
            barrier.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("health: " + health);
        // Debug.Log(gameObject.name + " - canBeScoredIn: " + canBeScoredIn);
        if (health <= 0 && canScoreTimer < delayBeforeCanBeScored)
        {
            canScoreTimer += Time.deltaTime;
        }

        if (health <= 0 && !canBeScoredIn
            /*&& (timer >= delayAfterInteraction) */&& (canScoreTimer >= delayBeforeCanBeScored))
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
        if (usingBonusBars) UpdateBarsBonus();
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

        // if ball is too slow, do not damage
        if (damage < 1)
        {
            Debug.Log("Ball moving too slowly to deal damage to " + gameObject.name);
        }

        Debug.Log(gameObject.name + " hit, taking " + damage + "damage");

        float remainder = 0f;
        if (usingBonusBars && bonusHealth > 0)
        {
            bonusHealth -= damage;
            if (bonusHealth < 0)
            {
                remainder = bonusHealth * -1;
                health -= remainder;
            }
        } else
        {
            health -= damage;
        }
        
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

        //remove super kick
        BallProperties bp = rb.GetComponent<BallProperties>();
        if (bp != null)
        {
            bp.isSuperKick = false;
            bp.isFullSuperKick = false;
            bp.SetPassTimer(bp.passTimeFrame);
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

    private void UpdateBarsBonus()
    {
        int num = bonusBarrierObjects.Length;
        if (num == 0) return;

        float threshold = (1f / num) * maxBonusHealth;
        //Debug.Log("threshold: " + threshold);

        for (int i = num - 1; i >= 0; i--)
        {
            //Debug.Log("Health: " + health + "    Threshold: " + i * threshold);
            if (bonusHealth <= i * threshold)
            {
                bonusBarrierObjects[i].SetActive(false);
            }
            else
            {
                bonusBarrierObjects[i].SetActive(true);
            }
        }

        bonusHealth -= Time.deltaTime * bonusHealthLossPerSecond;
    }

    public void ResetTimers()
    {
        timer = delayAfterInteraction;
        timerDamage = delayAfterDamage;
    }

    public void Respawn()
    {
        bonusHealth = 0;
        if (!startWithBariers) return;
        if (respawnType == 0) // don't respawn
        {
            return;
        } else if (respawnType == 1) // respawn if fully gone
        {
            if (health <= 0 && wasJustScored)
            {
                health = maxHealth;
                canBeScoredIn = false;
                wasJustScored = false;
            }
        } else // completely respawn
        {
            health = maxHealth;
            canBeScoredIn = false;
        }

        if (GM.overtimeStarted)
        {
            health = maxHealth;
            canBeScoredIn = false;
            wasJustScored = false;
        }

        timer = 0f;
        timerDamage = 0f;
        canScoreTimer = 0f;
    }

    public void PerformGoalEffects()
    {
        //yield return new WaitForSeconds(effectDelay);
        GoalParticles.Play();
        ParticleAudio = GoalParticles.GetComponent<AudioPlayer>();
        ParticleAudio.PlaySoundRandomPitch(ParticleAudio.Find("goalConfetti"));
        string songName = isHumanGoal ? "humanScore" : "monsterScore";
        ScoreJingle.PlaySound(ScoreJingle.Find(songName));
        wasJustScored = true;
    }

    public void SetStats()
    {
        if (GM == null) return;
        maxHealth = GM.barrierMaxHealth;
        startWithBariers = GM.barriersAreOn;
        respawnType = GM.barrierRespawnStyle;
        maxBounceAngle = GM.barrierBounceAngle;
        bounceForce = GM.barrierBounceForce;

        if (startWithBariers)
        {
            health = maxHealth;
        }
        else
        {
            health = 0;
        }
    }

    public void AddBonusHealth(float amountToAdd)
    {
        bonusHealth = amountToAdd;
        if (bonusHealth >= maxBonusHealth) bonusHealth = maxBonusHealth;
        BarrierAudio.PlaySoundRandomPitch(BarrierAudio.Find("goalBarrierCharge"));
    }

    public void SetBonusBars(bool isOn)
    {
        usingBonusBars = isOn;
    }
}
