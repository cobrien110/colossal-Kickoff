using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallProperties : MonoBehaviour
{
    public GameObject ballOwner = null;

    private UIManager UM = null;
    private GameplayManager GM = null;
    private StatTracker ST = null;
    private MultipleTargetCamera MTC = null;
    private AudioPlayer audioPlayer;
    public Transform ballSpawnPoint;
    public GameObject lastKicker = null;
    public GameObject previousKicker = null;
    public GameObject playerTest = null;
    public float passBonus = 0.25f;
    public bool isSuperKick = false;
    public float passTimeFrame = .5f;
    private float passTimer = 0f;
    [SerializeField] private float heightLockDelay = 3.5f;

    // Lighting Effects
    private Material SoccerUVS = null;
    private GameObject ChargeColorGO = null;
    private Light SceneLight = null;
    [SerializeField] private float sceneLightIntensity = 1.0f;
    [SerializeField] private Color tier1Color = Color.yellow;
    [SerializeField] private Color tier2Color = Color.red;
    [SerializeField] private Gradient colorGradient;

    public bool isInteractable = true;

    CommentatorSoundManager CSM;
    private SpriteRenderer SR;
    private Rigidbody RB;

    [SerializeField] private float maxSpeed = 15f;

    // Start is called before the first frame update
    void Start()
    {
        UM = GameObject.Find("Canvas").GetComponent<UIManager>();
        GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
        ST = GameObject.Find("Stat Tracker").GetComponent<StatTracker>();
        MTC = GameObject.Find("Main Camera").GetComponent<MultipleTargetCamera>();
        audioPlayer = GetComponent<AudioPlayer>();
        CSM = GameObject.Find("CommentatorSounds").GetComponent<CommentatorSoundManager>();
        SR = GetComponentInChildren<SpriteRenderer>();
        if (SR != null) SR.enabled = true;
        isInteractable = true;

        SoccerUVS = gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material;
        SoccerUVS.EnableKeyword("_EMISSION");
        ChargeColorGO = gameObject.transform.GetChild(2).gameObject;
        SceneLight = GameObject.Find("Directional Light").GetComponent<Light>();

        // Gradient Set Up
        GradientColorKey[] colorKey = new GradientColorKey[2];
        colorKey[0].color = tier1Color;
        colorKey[0].time = 0.0f;
        colorKey[1].color = tier2Color;
        colorKey[1].time = 2.0f;

        GradientAlphaKey[] alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 1.0f;
        alphaKey[1].time = 1.0f;

        colorGradient.SetKeys(colorKey, alphaKey);

        GameObject[] warriors = GameObject.FindGameObjectsWithTag("Warrior");
        for (int i = 0; i < warriors.Length; i++)
        {
            WarriorController WC = warriors[i].GetComponent<WarriorController>();
            //WarriorController WC = FindAnyObjectByType<WarriorController>();
            WC.Ball = this.gameObject;
            WC.BP = this;
        }

        RB = GetComponent<Rigidbody>();
        RB.constraints = RigidbodyConstraints.None;
        Invoke("LockHeight", heightLockDelay);

        /*MonsterController MC = GameObject.FindGameObjectWithTag("Monster").GetComponent<MonsterController>();
        MC.Ball = this.gameObject;
        MC.BP = this;*/
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Check if the object's velocity exceeds the maximum speed
        if (RB != null && RB.velocity.magnitude > maxSpeed)
        {
            Debug.Log("Ball reached max speed. Capping its speed.");

            // Cap the velocity to the maximum speed, preserving direction
            RB.velocity = RB.velocity.normalized * maxSpeed;
        }
    }

    private void Update()
    {
        if (isSuperKick && GM.passIndicator)
        {
            SetBallColor(Color.yellow);
        }
        else if (previousKicker != null && previousKicker.tag.Equals("Warrior") && ballOwner == null && GM.isPlaying
            && GM.passIndicator && passTimer <= passTimeFrame)
        {
            SetBallColor(Color.blue);
        }
        else
        {
            SetBallColor(Color.white);
        }
        
        if (ballOwner == null)
        {
            passTimer += Time.deltaTime;
        } else
        {
            passTimer = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        WarriorController wc = other.gameObject.GetComponent<WarriorController>();
        MonsterController mc = other.gameObject.GetComponent<MonsterController>();
        AiMinotaurController aiMC = other.gameObject.GetComponent<AiMinotaurController>();
        //if (aiMC != null && !aiMC.GetCanPickUpBall()) return; // To prevent issue with AiMino picking up ball right after kicking it

        AIMummy mummy = other.gameObject.GetComponent<AIMummy>();
        if ((other.tag.Equals("Warrior") || other.tag.Equals("Monster") || other.tag.Equals("Mummy"))
            && (ballOwner == null || ( (wc != null && wc.IsSliding()) || (mummy != null && mummy.IsSliding()))))
        {
            if (!isInteractable) return;
            if (GM.passIndicator)
            {
                //SetBallColor(Color.white);
            }

            //if (other.gameObject.Equals(lastKicker)) return;
            if (ballOwner == other.gameObject)
            {
                Debug.Log("Collider is already ballOwner, ignore interaction");
                return;
            }

            // If mummy tries to steal ball from sliding warrior, don't allow it
            if (mummy != null && ballOwner != null && ballOwner.GetComponent<WarriorController>() != null
                && ballOwner.GetComponent<WarriorController>().IsSliding())
            {
                // Debug.Log("Mummy can't steal from sliding warrior");
                return;
            }

            if (mc != null && !mc.isStunned && isSuperKick)
            {
                if (mc.isIntangible) return;
                mc.Stun();
                isSuperKick = false;
                return;
            } else if (mc != null && (mc.isStunned || mc.isIntangible))
            {
                return;
            }

            Debug.Log("ballOwner: " + ballOwner);
            Debug.Log("Ball owner being set to: " + other.gameObject);
            RB.velocity = Vector3.zero;
            ballOwner = other.gameObject;
            SetOwner(ballOwner);

            if (wc != null && wc.IsSliding())
            {
                if (GetOwner().name.StartsWith('1'))
                {
                    ST.UpdateWSteals(1);
                    UM.UpdateWarriorStealsSB(1);
                }
                if (GetOwner().name.StartsWith('2'))
                {
                    ST.UpdateWSteals(2);
                    UM.UpdateWarriorStealsSB(2);
                }
                if (GetOwner().name.StartsWith('3'))
                {
                    ST.UpdateWSteals(3);
                    UM.UpdateWarriorStealsSB(3);
                }
            }

            audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find("catchPass"), 0.25f);
            isSuperKick = false;

            // GET SUCCESSFUL PASS
            if (previousKicker != null && previousKicker != other.gameObject && ballOwner.tag.Equals("Warrior") && previousKicker.tag.Equals("Warrior"))
            {
                if (isSuperKick)
                {
                    isSuperKick = false;
                }
                if (passTimer <= passTimeFrame)
                {
                    GM.passMeter += passBonus;
                    UM.UpdateWarriorContestBar(GM.passMeter);
                    //GM.isPassing = false;
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        GoalWithBarrier GWB = other.GetComponent<GoalWithBarrier>();
        if (other.tag.Equals("WarriorGoal") && isInteractable)
        {
            if (GWB != null && GWB.canBeScoredIn) // attempt to score if goal has no barrier
            {
                // prevent goal if it would be an own goal while the ball is still being held
                if (ballOwner != null && ballOwner.GetComponent<WarriorController>() != null) return;

                if (playerTest != null) Debug.Log("PLAYER (" + playerTest.name + ") SCORED");

                UM.MonsterPoint();
                ST.UpdateMGoals();
                UM.UpdateMonsterGoalsSB();
                ScoreBall(true, other.transform);

                AudioPlayer goalAudio = other.GetComponent<AudioPlayer>();
                if (!goalAudio.isPlaying()) goalAudio.PlaySoundRandom();
            }
            else if (GWB != null && !GWB.canBeScoredIn)
            {
                // prevent goal if it would be an own goal while the ball is still being held
                if (ballOwner != null && ballOwner.GetComponent<WarriorController>() != null) return;
                //else if (ballOwner != null) GWB.TakeBallDamage(1000);

                // If being shot, not dribbled, reduce goal health
                if (ballOwner == null) GWB.TakeBallDamage(RB.velocity.magnitude);

                // otherwise destroy goal and bounce ball away
                else
                {
                    //GWB.RejectBall(RB);
                    GWB.TakeBallDamage(1000);

                    // Debug.Log("ballOwner set to null");
                    ballOwner = null;
                }


                GWB.RejectBall(RB);
                
            }
        }

        if (other.tag.Equals("MonsterGoal") && isInteractable)
        {
            if (GWB != null && GWB.canBeScoredIn) // attempt to score if goal has no barrier
            {
                // prevent goal if it would be an own goal while the ball is still being held
                if (ballOwner != null && ballOwner.GetComponent<MonsterController>() != null) return;

                if (playerTest != null) Debug.Log("PLAYER (" + playerTest.name + ") SCORED");

                UM.WarriorPoint();

                if (playerTest != null)
                {
                    if (playerTest.name.StartsWith('1'))
                    {
                        ST.UpdateWGoals(1);
                        UM.UpdateWarriorGoalsSB(1);
                    }
                    if (playerTest.name.StartsWith('2'))
                    {
                        ST.UpdateWGoals(2);
                        UM.UpdateWarriorGoalsSB(2);
                    }
                    if (playerTest.name.StartsWith('3'))
                    {
                        ST.UpdateWGoals(3);
                        UM.UpdateWarriorGoalsSB(3);
                    }
                }

                ScoreBall(false, other.transform);

                AudioPlayer goalAudio = other.GetComponent<AudioPlayer>();
                if (!goalAudio.isPlaying()) goalAudio.PlaySoundRandom();
            } else if (GWB != null && !GWB.canBeScoredIn)
            {
                // prevent goal if it would be an own goal while the ball is still being held
                if (ballOwner != null && ballOwner.GetComponent<MonsterController>() != null) return;
                //else if (ballOwner != null) GWB.TakeBallDamage(1000);

                // If being shot, not dribbled, reduce goal health
                if (ballOwner == null) GWB.TakeBallDamage(RB.velocity.magnitude);

                // otherwise destroy goal and bounce ball away
                else
                {
                    //GWB.RejectBall(RB);
                    GWB.TakeBallDamage(1000);

                    // Debug.Log("ballOwner set to null");
                    ballOwner = null;
                }

                
                GWB.RejectBall(RB);
                
            }
            
        }
    }

    private void ScoreBall(bool isWarriorGoal, Transform t)
    {
        GameObject scorer = previousKicker;
        if (ballOwner != null) scorer = ballOwner;
        // Play goal effects
        try
        {
            GoalWithBarrier goal = t.gameObject.GetComponent<GoalWithBarrier>();
            goal.PerformGoalEffects();
            //Debug.Log("Previous kicker");
            if (scorer != null) MTC.FocusOn(scorer.transform);
            //ballOwner = null;
        }
        catch
        {
            Debug.LogWarning("Something went wrong trying to play fancy goal effects.");
        }
        if (CSM != null)
        {
            CSM.PlayGoalSound(!isWarriorGoal);
        }
        

        Debug.Log("RESET");
        // Update the last scored ball for the delayed start
        GM.SetLastScoredGoal(t);
        ResetBall();

        AudioPlayer globalAudioPlayer = GameObject.Find("GlobalSoundPlayer").GetComponent<AudioPlayer>();
        globalAudioPlayer.PlaySound(globalAudioPlayer.Find("goal"));

        // Reset mummies if applicable
        MonsterController mc = FindObjectOfType<MonsterController>();
        if (mc != null)
        {
            AiMummyManager aiMummyManager = mc.GetComponent<AiMummyManager>();
            if (aiMummyManager != null) aiMummyManager.ResetMummies();
        }
    }

    public void ResetBall()
    {
        Debug.Log("Reset ball has been called");

        // Debug.Log("ballOwner set to null");
        ballOwner = null;
        isInteractable = false;
        previousKicker = null;
        //SR = GetComponentInChildren<SpriteRenderer>();
        if (SR != null) SR.enabled = false;
        Invoke("DestroyDelay", 3.05f);

        //GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
        GM.Reset();
    }

    public void ResetPreviousKicker()
    {
        previousKicker = null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.tag.Equals("Ground") || collision.gameObject.tag.Equals("InvisWall")) && ballOwner == null && isInteractable)
        {
            string bouncePick = Random.Range(1, 3).ToString();
            if (audioPlayer != null) audioPlayer.PlaySoundRandomPitch(audioPlayer.Find("bounce" + bouncePick));
        }

        if (collision.gameObject.tag.Equals("MinoWall") || collision.gameObject.tag.Equals("InvisWall"))
        {
            isSuperKick = false;
        }
    }

    private void DestroyDelay()
    {
        Destroy(this.gameObject);
    }

    public void SetOwner(GameObject player)
    {
        playerTest = player;

        // reset goal timers to allow for immediate interaction
        GoalWithBarrier[] goals = GameObject.FindObjectsOfType<GoalWithBarrier>();
        foreach (GoalWithBarrier goal in goals)
        {
            goal.ResetTimers();
        }
    }

    private GameObject GetOwner()
    {
        return playerTest;
    }

    private void LockHeight()
    {
        Rigidbody RB = GetComponent<Rigidbody>();
        RB.constraints = RigidbodyConstraints.FreezePositionY;
    }

    public void StartBallGlow(float tier)
    {
        ChargeColorGO.SetActive(true);

        //if (tier == 1)
        //{
        //    SceneLight.intensity = sceneLightIntensity / 1.5f;
        //    SoccerUVS.SetColor("_EmissionColor", tier1Color);
        //    ChargeColorGO.GetComponent<Light>().color = tier1Color;
        //} else if (tier == 2)
        //{
        //    SceneLight.intensity = sceneLightIntensity / 2f;
        //    SoccerUVS.SetColor("_EmissionColor", tier2Color);
        //    ChargeColorGO.GetComponent<Light>().color = tier2Color;
        //}
        float intensity = sceneLightIntensity - (tier / 4);
        //Debug.Log("Light Intensity: " + intensity);
        SceneLight.intensity = intensity;
        Color glowColor = colorGradient.Evaluate(tier / 2);
        SoccerUVS.SetColor("_EmissionColor", glowColor);
        ChargeColorGO.GetComponent<Light>().color = glowColor;
    }

    public void StopBallGlow()
    {
        if (ChargeColorGO != null) ChargeColorGO.SetActive(false);
        if (SceneLight != null) SceneLight.intensity = 1.0f;
        if (SoccerUVS != null) SoccerUVS.SetColor("_EmissionColor", Color.black);
    }

    public void SetBallColor(Color ballColor)
    {
        MeshRenderer MR = GetComponentInChildren<MeshRenderer>();
        MR.material.SetColor("_BaseColor", ballColor);
    }
}
