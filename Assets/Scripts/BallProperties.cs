using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BallProperties : MonoBehaviour
{
    [HideInInspector] public GameObject ballOwner = null;

    private UIManager UM = null;
    private GameplayManager GM = null;
    private StatTracker ST = null;
    private AudioPlayer audioPlayer;
    public Transform ballSpawnPoint;
    public GameObject lastKicker = null;
    public GameObject previousKicker = null;
    public GameObject playerTest = null;
    public float passBonus = 0.25f;
    public bool isSuperKick = false;
    [SerializeField] private float heightLockDelay = 3.5f;

    // Lighting Effects
    private Material SoccerUVS = null;
    private GameObject ChargeColorGO = null;
    private Light SceneLight = null;
    [SerializeField] private float sceneLightIntensity = 1.0f;
    [SerializeField] private Color tier1Color = Color.yellow;
    [SerializeField] private Color tier2Color = Color.red;

    public bool isInteractable = true;

    CommentatorSoundManager CSM;
    private SpriteRenderer SR;

    // Start is called before the first frame update
    void Start()
    {
        UM = GameObject.Find("Canvas").GetComponent<UIManager>();
        GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
        ST = GameObject.Find("Stat Tracker").GetComponent<StatTracker>();
        audioPlayer = GetComponent<AudioPlayer>();
        CSM = GameObject.Find("CommentatorSounds").GetComponent<CommentatorSoundManager>();
        SR = GetComponentInChildren<SpriteRenderer>();
        if (SR != null) SR.enabled = true;
        isInteractable = true;

        SoccerUVS = gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material;
        SoccerUVS.EnableKeyword("_EMISSION");
        ChargeColorGO = gameObject.transform.GetChild(2).gameObject;
        SceneLight = GameObject.Find("Directional Light").GetComponent<Light>();

        GameObject[] warriors = GameObject.FindGameObjectsWithTag("Warrior");
        for (int i = 0; i < warriors.Length; i++)
        {
            WarriorController WC = warriors[i].GetComponent<WarriorController>();
            //WarriorController WC = FindAnyObjectByType<WarriorController>();
            WC.Ball = this.gameObject;
            WC.BP = this;
        }

        Rigidbody RB = GetComponent<Rigidbody>();
        RB.constraints = RigidbodyConstraints.None;
        Invoke("LockHeight", heightLockDelay);

        /*MonsterController MC = GameObject.FindGameObjectWithTag("Monster").GetComponent<MonsterController>();
        MC.Ball = this.gameObject;
        MC.BP = this;*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        WarriorController wc = other.gameObject.GetComponent<WarriorController>();
        MonsterController mc = other.gameObject.GetComponent<MonsterController>();
        AiMinotaurController aiMC = other.gameObject.GetComponent<AiMinotaurController>();
        //if (aiMC != null && !aiMC.GetCanPickUpBall()) return; // To prevent issue with AiMino picking up ball right after kicking it

        AIMummy mummy = other.gameObject.GetComponent<AIMummy>();
        if ((other.tag.Equals("Warrior") || other.tag.Equals("Monster") || other.tag.Equals("Mummy"))
            && (ballOwner == null || ( (wc != null && wc.IsSliding()) || (mummy != null && mummy.IsSliding()) )) && isInteractable)
        {
            if (other.gameObject.Equals(lastKicker)) return;
            
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

            Debug.Log("Ball owner being set to: " + other.gameObject);
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
            if (previousKicker != null && previousKicker != other.gameObject && ballOwner.tag.Equals("Warrior") && previousKicker.tag.Equals("Warrior"))
            {
                GM.passMeter += passBonus;
                UM.UpdatePassMeter(GM.passMeter);
                UM.UpdateWarriorContestBar(GM.passMeter);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag.Equals("WarriorGoal") && isInteractable)
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

        if (other.tag.Equals("MonsterGoal") && isInteractable)
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
        }
    }

    private void ScoreBall(bool isWarriorGoal, Transform t)
    {
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
        ballOwner = null;
        isInteractable = false;
        //SR = GetComponentInChildren<SpriteRenderer>();
        if (SR != null) SR.enabled = false;
        Invoke("DestroyDelay", 3.05f);

        //GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
        GM.Reset();
    }

    public void ResetForOvertime()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.tag.Equals("Ground") || collision.gameObject.tag.Equals("InvisWall")) && ballOwner == null && isInteractable)
        {
            if (audioPlayer == null) return;
            string bouncePick = Random.Range(1, 3).ToString();
            audioPlayer.PlaySoundRandomPitch(audioPlayer.Find("bounce" + bouncePick));
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

    private void SetOwner(GameObject player)
    {
        playerTest = player;
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

    public void StartBallGlow(int tier)
    {
        ChargeColorGO.SetActive(true);
        if (tier == 1)
        {
            SceneLight.intensity = sceneLightIntensity / 1.5f;
            SoccerUVS.SetColor("_EmissionColor", tier1Color);
            ChargeColorGO.GetComponent<Light>().color = tier1Color;
        } else if (tier == 2)
        {
            SceneLight.intensity = sceneLightIntensity / 2f;
            SoccerUVS.SetColor("_EmissionColor", tier2Color);
            ChargeColorGO.GetComponent<Light>().color = tier2Color;
        }
    }

    public void StopBallGlow()
    {
        ChargeColorGO.SetActive(false);
        SceneLight.intensity = 1.0f;
        SoccerUVS.SetColor("_EmissionColor", Color.black);
    }
}
