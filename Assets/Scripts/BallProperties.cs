using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public bool isFullSuperKick = false;
    public bool isInSingleOutMode = false;
    public float passTimeFrame = .5f;
    private float passTimer = 0f;
    [SerializeField] private float heightLockDelay = 3.5f;
    [SerializeField] private float superKickMinStunSpeed = 1.2f;

    // Lighting Effects
    private Material SoccerUVS = null;
    private GameObject ChargeColorGO = null;
    private Light SceneLight = null;
    [SerializeField] private float sceneLightIntensity = 1.0f;
    [SerializeField] private Color tier1Color = Color.yellow;
    [SerializeField] private Color tier2Color = Color.red;
    [SerializeField] private Gradient colorGradient;
    private bool isGlowing = false;

    public bool isInteractable = true;

    CommentatorSoundManager CSM;
    private SpriteRenderer SR;
    private Rigidbody RB;
    private Vector3 previousVelocity;

    public float maxSpeed = 15f;

    private GameObject assistingPlayer;
    private bool isAssisting;

    private const float ballRadius = 0.15f;
    private Vector3 previousPosition;
    private Vector3 calculatedVelocity;

    private float intangibleTime = 0.3f;


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

        // Set ball rolling speed and direction based on velocity
        if (ballOwner != null)
        {
            // Calculate velocity manually
            calculatedVelocity = (transform.position - previousPosition) / Time.fixedDeltaTime;
        }
        else
        {
            // Use Rigidbody velocity when physics is applied
            calculatedVelocity = RB.velocity;
        }

        // Apply rotation based on calculated velocity
        if (calculatedVelocity.sqrMagnitude > 0.001f)
        {
            Vector3 rotationAxis = Vector3.Cross(Vector3.up, calculatedVelocity.normalized);
            float rotationSpeed = calculatedVelocity.magnitude / ballRadius;

            RB.angularVelocity = rotationAxis * rotationSpeed;
        }

        // Store the current position for the next frame's calculation
        previousPosition = transform.position;
    }

    private void Update()
    {
        if (isFullSuperKick && GM.passIndicator && passTimer <= passTimeFrame)
        {
            SetBallColor(Color.red);
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

        if (isInSingleOutMode && GM.isPlaying)
        {
            SetBallColor(Color.black);
        }
        
        if (ballOwner == null)
        {
            passTimer += Time.deltaTime;
            if (isGlowing)
            {
                StopBallGlow();
            }
        } else
        {
            passTimer = 0f;
        }
    }

    private void LateUpdate()
    {
        previousVelocity = RB.velocity;
    }

    public void ReapplyLastVelocity()
    {
        RB.velocity = previousVelocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        WarriorController wc = other.gameObject.GetComponent<WarriorController>();
        MonsterController mc = other.gameObject.GetComponent<MonsterController>();
        AiMinotaurController aiMC = other.gameObject.GetComponent<AiMinotaurController>();
        AIMummy mummy = other.gameObject.GetComponent<AIMummy>();
        if ((other.tag.Equals("Warrior") || other.tag.Equals("Monster") || other.tag.Equals("Mummy"))
            && (ballOwner == null || ( (wc != null && wc.IsSliding()) || (mummy != null && mummy.IsSliding()))))
        {
            if (!isInteractable) return;
            // if the ball just hit the goal, prevent that person from touching the ball
            if (isInSingleOutMode && previousKicker == other.gameObject) return;

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

            if (mc != null && !mc.isStunned && isFullSuperKick && passTimer <= passTimeFrame
                && RB.velocity.magnitude > superKickMinStunSpeed)
            {
                // Full super kick hit monster
                if (mc.isIntangible) return;
                mc.Stun();
                isSuperKick = false;
                isFullSuperKick = false;
                ReapplyLastVelocity();
                return;
            } else if (mc != null && (mc.isStunned || mc.isIntangible))
            {
                return;
            }

            if (mummy != null && isFullSuperKick && passTimer <= passTimeFrame
                && RB.velocity.magnitude > superKickMinStunSpeed)
            {
                // Full super kick hit mummy
                if (!mummy.IsSliding())
                {
                    Debug.Log("Super kick hit " + mummy.name);
                    mummy.Die(true);
                    ReapplyLastVelocity();
                    return;
                }
            }

            if (wc != null)
            {
                wc.StopSuperKick();
            }
            
            Debug.Log("ballOwner before: " + ballOwner);
            
            bool isASteal = false;
            AIMummy mummyToKill = null; // Used to kill mummy when stolen from
            if (ballOwner != null)
            {
                isASteal = true;

                // If a mummy is being stolen from, queue it to die
                mummyToKill = ballOwner.GetComponent<AIMummy>();
            }

            // If this is a warrior picking up the ball, reset slide cooldown (to allow them to dodge/juke)
            if (wc != null) wc.ResetSlideCooldown();

            Debug.Log("Ball owner being set to: " + other.gameObject);
            RB.velocity = Vector3.zero;
            ballOwner = other.gameObject;
            SetOwner(ballOwner);

            if (wc != null && wc.IsSliding() && isASteal)
            {
                if (wc.playerNum == 1)
                {
                    ST.UpdateWSteals(1);
                    UM.UpdateWarriorStealsSB(1);
                    Debug.Log("Stole your ball haha");
                }
                if (wc.playerNum == 2)
                {
                    ST.UpdateWSteals(2);
                    UM.UpdateWarriorStealsSB(2);
                }
                if (wc.playerNum == 3)
                {
                    ST.UpdateWSteals(3);
                    UM.UpdateWarriorStealsSB(3);
                }
            }

            audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find("catchPass"), 0.25f);
            isSuperKick = false;
            isFullSuperKick = false;

            // GET SUCCESSFUL PASS
            if (previousKicker != null && previousKicker != other.gameObject && ballOwner.tag.Equals("Warrior") && previousKicker.tag.Equals("Warrior"))
            {
                if (isSuperKick || isFullSuperKick)
                {
                    isSuperKick = false;
                    isFullSuperKick = false;
                }
                if (passTimer <= passTimeFrame)
                {
                    GM.passMeter += passBonus;
                    UM.UpdateWarriorContestBar(GM.passMeter);
                    //GM.isPassing = false;

                    //Assist stat code
                    assistingPlayer = previousKicker;
                    Debug.Log("" + assistingPlayer.GetComponent<WarriorController>().playerNum + 
                        " is assisting " + ballOwner.GetComponent<WarriorController>().playerNum);
                    StartCoroutine(Assisting());
                }
            }

            // If a mummy was stolen from, kill mummy
            if (mummyToKill != null) mummyToKill.Die(true);
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

                //stat tracking monster points
                UM.MonsterPoint();
                ST.UpdateMGoals();
                UM.UpdateMonsterGoalsSB();

                //Monster Scores
                if (playerTest.GetComponent<MonsterController>() != null)
                {
                    int i = playerTest.GetComponent<MonsterController>().playerID;
                    if (playerTest.GetComponent<AiMonsterController>() != null)
                    {
                        UM.UpdatePlayerScoredText(0);
                    }
                    else
                    {
                        UM.UpdatePlayerScoredText(i + 1);
                    }
                    UM.ShowPlayerScoredText(true);
                }
                //Warrior OwnGoals
                else if (playerTest.GetComponent<WarriorController>() != null)
                {
                    int i = playerTest.GetComponent<WarriorController>().playerID;
                    if (playerTest.GetComponent<WarriorAiController>() != null)
                    {
                        UM.UpdatePlayerScoredText(0);
                    }
                    else
                    {
                        i = i * -1;
                        UM.UpdatePlayerScoredText(i - 1);
                    }
                    UM.ShowPlayerScoredText(true);
                }
                
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
                    previousKicker = ballOwner;
                    float damageToDeal = GWB.maxHealth / 2f;
                    if (damageToDeal < 1) damageToDeal = 1;
                    GWB.TakeBallDamage(damageToDeal);

                    // Debug.Log("ballOwner set to null");
                    ballOwner = null;
                }

                Debug.Log("Reject ball - warrior goal");
                if (GWB.health <= 0 && previousKicker.GetComponent<MonsterController>() != null && RB.velocity.magnitude >= 10f)
                {
                    GWB.canBeScoredIn = true;
                    GWB.health = 0;
                } else
                {
                    GWB.RejectBall(RB);
                }
                
                //isInteractable = false;
                isInSingleOutMode = true;
                Invoke("EndIntangibility", intangibleTime);
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

                //Warrior Scores
                if (playerTest.GetComponent<WarriorController>() != null)
                {
                    int i = playerTest.GetComponent<WarriorController>().playerID;
                    if (playerTest.GetComponent<WarriorAiController>() != null)
                    {
                        UM.UpdatePlayerScoredText(0);
                    }
                    else
                    {
                        UM.UpdatePlayerScoredText(i + 1);
                    }
                    UM.ShowPlayerScoredText(true);
                }
                //Monster OwnGoals
                else if (playerTest.GetComponent<MonsterController>() != null)
                {
                    int i = playerTest.GetComponent<MonsterController>().playerID;
                    if (playerTest.GetComponent<AiMonsterController>() != null)
                    {
                        UM.UpdatePlayerScoredText(0);
                    }
                    else
                    {
                        i = i * -1;
                        UM.UpdatePlayerScoredText(i - 1);
                    }
                    UM.ShowPlayerScoredText(true);
                }

                //Stat tracking warrior points
                if (playerTest.GetComponent<WarriorController>() != null)
                {
                    if (playerTest.GetComponent<WarriorController>().playerNum == 1)
                    {
                        ST.UpdateWGoals(1);
                        UM.UpdateWarriorGoalsSB(1);
                    }
                    if (playerTest.GetComponent<WarriorController>().playerNum == 2)
                    {
                        ST.UpdateWGoals(2);
                        UM.UpdateWarriorGoalsSB(2);
                    }
                    if (playerTest.GetComponent<WarriorController>().playerNum == 3)
                    {
                        ST.UpdateWGoals(3);
                        UM.UpdateWarriorGoalsSB(3);
                    }
                }

                if (isAssisting && assistingPlayer.GetComponent<WarriorController>() != null)
                {
                    int playerNum = assistingPlayer.GetComponent<WarriorController>().playerNum;
                    ST.UpdateWAssists(playerNum);
                    UM.UpdateWarriorAssistsSB(playerNum);
                }

                ScoreBall(false, other.transform);

                AudioPlayer goalAudio = other.GetComponent<AudioPlayer>();
                if (!goalAudio.isPlaying()) goalAudio.PlaySoundRandom();
            }
            else if (GWB != null && !GWB.canBeScoredIn)
            {
                // prevent goal if it would be an own goal while the ball is still being held
                if (ballOwner != null && ballOwner.GetComponent<MonsterController>() != null) return;
                //else if (ballOwner != null) GWB.TakeBallDamage(1000);

                // If being shot, not dribbled, reduce goal health
                if (ballOwner == null) GWB.TakeBallDamage(RB.velocity.magnitude);

                // otherwise hurt goal and bounce ball away
                else
                {
                    //GWB.RejectBall(RB);
                    previousKicker = ballOwner;
                    float damageToDeal = GWB.maxHealth / 2f;
                    if (damageToDeal < 1) damageToDeal = 1;
                    GWB.TakeBallDamage(damageToDeal);

                    // Debug.Log("ballOwner set to null");
                    ballOwner = null;
                }

                Debug.Log("Reject ball - Monster goal");
                if (isFullSuperKick)
                {
                    if (GWB.bonusHealth > 0)
                    {
                        GWB.bonusHealth = 0;
                        GWB.RejectBall(RB);
                    } else
                    {
                        GWB.canBeScoredIn = true;
                        GWB.health = 0;
                    }
                }
                else
                {
                    GWB.RejectBall(RB);
                }
                //isInteractable = false;
                isInSingleOutMode = true;
                Invoke("EndIntangibility", intangibleTime);
            }
            
        }

        if (other.gameObject.tag.Equals("MinoWall"))
        {
            isSuperKick = false;
            isFullSuperKick = false;

            MinoWall wall = other.gameObject.GetComponent<MinoWall>();
            if (wall != null && isInSingleOutMode)
            {
                wall.EndEarly();
                ReapplyLastVelocity();
            }
        }
    }

    private void EndIntangibility()
    {
        isInSingleOutMode = false;
    }

    private void ScoreBall(bool isWarriorGoal, Transform t)
    {
        //Sudden Death Goal
        if (GM.overtimeStyle == 1 && UM.overtime)
        {
            Debug.Log("I am in the right place for OT");
            GameObject scorer = previousKicker;
            if (ballOwner != null) scorer = ballOwner;
            if (scorer.GetComponent<AIMummy>()) scorer = GameObject.FindGameObjectWithTag("Monster");
            // Play goal effects
            try
            {
                GoalWithBarrier goal = t.gameObject.GetComponent<GoalWithBarrier>();
                goal.PerformGoalEffects();
                //Debug.Log("Previous kicker");
                if (scorer != null) MTC.FocusOn(scorer.transform);
                StartCoroutine(MTC.ScreenShake(2.0f));
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
            GM.SetLastScoredGoal(t);
            ballOwner = null;
            isInteractable = false;
            previousKicker = null;
            if (SR != null) SR.enabled = false;
            Invoke("DestroyDelay", 3.05f);

            GM.ResetOvertime();
        }
        //Regulation Goal
        else
        {
            Debug.Log("I am in the wrong place for OT");
            GameObject scorer = previousKicker;
            if (ballOwner != null) scorer = ballOwner;
            if (scorer.GetComponent<AIMummy>()) scorer = GameObject.FindGameObjectWithTag("Monster");
            // Play goal effects
            try
            {
                GoalWithBarrier goal = t.gameObject.GetComponent<GoalWithBarrier>();
                goal.PerformGoalEffects();
                //Debug.Log("Previous kicker");
                if (scorer != null) MTC.FocusOn(scorer.transform);
                StartCoroutine(MTC.ScreenShake(2.0f));
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
            globalAudioPlayer.PlaySoundRandomPitch(globalAudioPlayer.Find("goal"));

            // Reset mummies if applicable
            MonsterController mc = FindObjectOfType<MonsterController>();
            if (mc != null)
            {
                AiMummyManager aiMummyManager = mc.GetComponent<AiMummyManager>();
                if (aiMummyManager != null) aiMummyManager.ResetMummies();
            }

            mc.ResetAbilities();

            AbilityScript.canActivate = false;
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

    public void SetSR(bool b)
    {
        SR.enabled = b;
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
            isFullSuperKick = false;

            MinoWall wall = collision.gameObject.GetComponent<MinoWall>();
            if (wall != null && isInSingleOutMode)
            {
                Destroy(wall);
                ReapplyLastVelocity();
            }
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
        isGlowing = true;
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
        isGlowing = false;
        if (ChargeColorGO != null) ChargeColorGO.SetActive(false);
        if (SceneLight != null) SceneLight.intensity = 1.0f;
        if (SoccerUVS != null) SoccerUVS.SetColor("_EmissionColor", Color.black);
    }

    public void SetBallColor(Color ballColor)
    {
        MeshRenderer MR = GetComponentInChildren<MeshRenderer>();
        MR.material.SetColor("_BaseColor", ballColor);
    }

    private IEnumerator Assisting()
    {
        Debug.Log("Assist ready");
        isAssisting = true;
        yield return new WaitForSeconds(3.0f);
        Debug.Log("Assist no longer ready");
        isAssisting = false;
        assistingPlayer = null;
    }

    public Vector3 GetAnticipatedPosition(float inSeconds)
    {
        // Get the object's Rigidbody component
        Rigidbody rb = GetComponent<Rigidbody>();

        // If no Rigidbody is found, return the current position as a fallback
        if (rb == null) return transform.position;

        if (ballOwner == null)
        {
            // Predict the future position based on rb velocity
            return transform.position + (rb.velocity * inSeconds);
        } else
        {
            // There is a ballOwner, so can't use rb velocity
            // Predict the future position based on calculated velocity
            return transform.position + (calculatedVelocity * inSeconds);
        }
    }

    public void SetPassTimer(float f)
    {
        passTimer = f;
    }

    public Rigidbody GetRB()
    {
        return RB;
    }

}
