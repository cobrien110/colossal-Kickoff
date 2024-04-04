using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallProperties : MonoBehaviour
{
    [HideInInspector] public GameObject ballOwner = null;
    private UIManager UM = null;
    private GameplayManager GM = null;
    private AudioPlayer audioPlayer;

    // Start is called before the first frame update
    void Start()
    {
        UM = GameObject.Find("Canvas").GetComponent<UIManager>();
        GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
        audioPlayer = GetComponent<AudioPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Warrior"))
        {
            ballOwner = other.gameObject;
        }

        if (other.tag.Equals("WarriorGoal"))
        {
            UM.monsterPoint();
            GM.Reset();
            Destroy(this.gameObject);
        }

        if (other.tag.Equals("MonsterGoal"))
        {
            UM.warriorPoint();
            GM.Reset();
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Ground"))
        {
            string bouncePick = Random.Range(1, 3).ToString();
            audioPlayer.PlaySoundRandomPitch(audioPlayer.Find("bounce" + bouncePick));
        }
    }
}
