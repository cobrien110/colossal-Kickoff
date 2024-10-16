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
    public float passBonus = 25f;
    public bool isSuperKick = false;

    public bool isInteractable = true;

    CommentatorSoundManager CSM;

    // Start is called before the first frame update
    void Start()
    {
        UM = GameObject.Find("Canvas").GetComponent<UIManager>();
        GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
        ST = GameObject.Find("Stat Tracker").GetComponent<StatTracker>();
        audioPlayer = GetComponent<AudioPlayer>();
        CSM = GameObject.Find("CommentatorSounds").GetComponent<CommentatorSoundManager>();

        GameObject[] warriors = GameObject.FindGameObjectsWithTag("Warrior");
        for (int i = 0; i < warriors.Length; i++)
        {
            WarriorController WC = warriors[i].GetComponent<WarriorController>();
            //WarriorController WC = FindAnyObjectByType<WarriorController>();
            WC.Ball = this.gameObject;
            WC.BP = this;
        }

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
            }
        }

        if (other.tag.Equals("WarriorGoal") && isInteractable)
        {
            if (playerTest != null) Debug.Log("PLAYER (" + playerTest.name + ") SCORED");
            
            UM.MonsterPoint();
            ST.UpdateMGoals();
            UM.UpdateMonsterGoalsSB();
            ScoreBall(true);

            AudioPlayer goalAudio = other.GetComponent<AudioPlayer>();
            if (!goalAudio.isPlaying()) goalAudio.PlaySoundRandom(); 
        }

        if (other.tag.Equals("MonsterGoal") && isInteractable)
        {
            if (playerTest != null) Debug.Log("PLAYER (" + playerTest.name + ") SCORED");

            UM.WarriorPoint();

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
            ScoreBall(false);

            AudioPlayer goalAudio = other.GetComponent<AudioPlayer>();
            if (!goalAudio.isPlaying()) goalAudio.PlaySoundRandom();
        }
    }

    private void ScoreBall(bool isWarriorGoal)
    {
        /*Debug.Log(ballOwner);
        Debug.Log(player);
        
        if (isWarriorGoal)
        {
            UM.MonsterPoint();
            ST.UpdateMGoals();
        } else
        {
            UM.WarriorPoint();

            if (ballOwner.name.StartsWith('1'))
            {
                ST.UpdateWGoals(1);
            }
            if (ballOwner.name.StartsWith('2'))
            {
                ST.UpdateWGoals(2);
            }
            if (ballOwner.name.StartsWith('3'))
            {
                ST.UpdateWGoals(3);
            }
        }*/
        ballOwner = null;
        if (CSM != null)
        {
            CSM.PlayGoalSound(!isWarriorGoal);
        }
        Debug.Log("RESET");
        GM.Reset();
        AudioPlayer globalAudioPlayer = GameObject.Find("GlobalSoundPlayer").GetComponent<AudioPlayer>();
        globalAudioPlayer.PlaySound(globalAudioPlayer.Find("goal"));
        isInteractable = false;
        Invoke("DestroyDelay", 3f);

        // Reset mummies if applicable
        MonsterController mc = FindObjectOfType<MonsterController>();
        AiMummyManager aiMummyManager = mc.GetComponent<AiMummyManager>();
        if (aiMummyManager != null) aiMummyManager.ResetMummies(); 

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
}
