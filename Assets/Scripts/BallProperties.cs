using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BallProperties : MonoBehaviour
{
    [HideInInspector] public GameObject ballOwner = null;

    private UIManager UM = null;
    private GameplayManager GM = null;
    private AudioPlayer audioPlayer;
    public Transform ballSpawnPoint;
    public GameObject lastKicker = null;
    public GameObject previousKicker = null;
    public int passBonus = 25;
    public bool isSuperKick = false;

    public bool isInteractable = true;

    CommentatorSoundManager CSM;

    // Start is called before the first frame update
    void Start()
    {
        UM = GameObject.Find("Canvas").GetComponent<UIManager>();
        GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
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
        if ((other.tag.Equals("Warrior") || other.tag.Equals("Monster"))
            && (ballOwner == null || (wc != null && wc.IsSliding())) && isInteractable)
        {
            if (other.gameObject.Equals(lastKicker)) return;
            
            if (mc != null && !mc.isStunned && isSuperKick)
            {
                mc.Stun();
                isSuperKick = false;
                return;
            } else if (mc != null && mc.isStunned)
            {
                return;
            }
            
            Debug.Log("Ball owner being set to: " + other.gameObject);
            ballOwner = other.gameObject;
            audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find("catchPass"), 0.25f);
            isSuperKick = false;
            if (previousKicker != null && previousKicker != other.gameObject && ballOwner.tag.Equals("Warrior") && previousKicker.tag.Equals("Warrior"))
            {
                GM.passMeter += passBonus;
            }
        }

        if (other.tag.Equals("WarriorGoal") && isInteractable)
        {
            ScoreBall(true);
            AudioPlayer goalAudio = other.GetComponent<AudioPlayer>();
            if (!goalAudio.isPlaying()) goalAudio.PlaySoundRandom();
        }

        if (other.tag.Equals("MonsterGoal") && isInteractable)
        {
            ScoreBall(false);
            AudioPlayer goalAudio = other.GetComponent<AudioPlayer>();
            if (!goalAudio.isPlaying()) goalAudio.PlaySoundRandom();
        }
    }

    private void ScoreBall(bool isWarriorGoal)
    {
        if (isWarriorGoal)
        {
            UM.MonsterPoint();
        } else
        {
            UM.WarriorPoint();
        }
        ballOwner = null;
        if (CSM != null)
        {
            CSM.PlayGoalSound(!isWarriorGoal);
        }
        GM.Reset();
        AudioPlayer globalAudioPlayer = GameObject.Find("GlobalSoundPlayer").GetComponent<AudioPlayer>();
        globalAudioPlayer.PlaySound(globalAudioPlayer.Find("goal"));
        isInteractable = false;
        Invoke("DestroyDelay", 3f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Ground") && ballOwner == null && isInteractable)
        {
            if (audioPlayer == null) return;
            string bouncePick = Random.Range(1, 3).ToString();
            audioPlayer.PlaySoundRandomPitch(audioPlayer.Find("bounce" + bouncePick));
        }
    }

    private void DestroyDelay()
    {
        Destroy(this.gameObject);
    }
}
