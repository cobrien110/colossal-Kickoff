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

    public bool isInteractable = true;

    // Start is called before the first frame update
    void Start()
    {
        UM = GameObject.Find("Canvas").GetComponent<UIManager>();
        GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
        audioPlayer = GetComponent<AudioPlayer>();

        GameObject[] warriors = GameObject.FindGameObjectsWithTag("Warrior");
        for (int i = 0; i < warriors.Length; i++)
        {
            WarriorController WC = warriors[i].GetComponent<WarriorController>();
            WC.Ball = this.gameObject;
            WC.BP = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Warrior") && ballOwner == null && isInteractable)
        {
            ballOwner = other.gameObject;
            audioPlayer.PlaySoundVolumeRandomPitch(audioPlayer.Find("catchPass"), 0.25f);
        }

        if (other.tag.Equals("WarriorGoal") && isInteractable)
        {
            ScoreBall(true);
        }

        if (other.tag.Equals("MonsterGoal") && isInteractable)
        {
            ScoreBall(false);
        }
    }

    private void ScoreBall(bool isWarriorGoal)
    {
        if (isWarriorGoal)
        {
            UM.warriorPoint();
        } else
        {
            UM.monsterPoint();
        }
        GM.Reset();
        AudioPlayer globalAudioPlayer = GameObject.Find("GlobalSoundPlayer").GetComponent<AudioPlayer>();
        globalAudioPlayer.PlaySound(globalAudioPlayer.Find("goal"));
        isInteractable = false;
        Destroy(this.gameObject);
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
}
