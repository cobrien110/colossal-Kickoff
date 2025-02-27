using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodiumSequencer : MonoBehaviour
{
    public GameObject podium;
    //public Transform podiumLocation;
    public GameObject[] spawnPoints;
    public float podiumShowTime = 5f;

    private GameplayManager GM;
    private UIManager UI;
    private StatTracker ST;
    private BallProperties BP;
    private MultipleTargetCamera MTC;
    private GameObject monster;
    private GameObject[] warriors;
    public ParticleSystem monPart;
    public ParticleSystem warPart;
    private AudioPlayer ScoreJingle;
    // Start is called before the first frame update
    void Start()
    {
        GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
        UI = GameObject.Find("Canvas").GetComponent<UIManager>();
        ST = GameObject.Find("Stat Tracker").GetComponent<StatTracker>();
        MTC = GameObject.Find("Main Camera").GetComponent<MultipleTargetCamera>();
        ScoreJingle = GameObject.FindGameObjectWithTag("Jukebox2").GetComponent<AudioPlayer>();
    }

    public void StartPodiumSequence(int winner)
    {
        Debug.Log("Podium Sequence Starting");
        // spawn podium object
        podium.SetActive(true);

        // get player refs
        monster = GameObject.FindGameObjectWithTag("Monster");
        //STOP MONSTER FROM USING ABILITIES?

        warriors = GameObject.FindGameObjectsWithTag("Warrior");
        

        // make ball uninteractible
        BP = GameObject.FindGameObjectWithTag("Ball").GetComponent<BallProperties>();
        if (BP != null)
        {
            BP.isInteractable = false;
            BP.SetSR(false);
            MTC.RemoveTarget(BP.gameObject.transform);
        }

        // move players
        if (winner == 0)
        {
            // move warriors to podium
            for (int i = 0; i < warriors.Length; i++)
            {
                warriors[i].transform.position = spawnPoints[i].transform.position;
                warriors[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
            // move monster to ground
            monster.transform.position = spawnPoints[6].transform.position;
            monster.GetComponent<Rigidbody>().velocity = Vector3.zero;
            // play respective theme
        } else
        {
            // move monster to podium (throne?)
            monster.transform.position = spawnPoints[0].transform.position;
            monster.GetComponent<Rigidbody>().velocity = Vector3.zero;
            // move warriors to ground
            for (int i = 0; i < warriors.Length; i++)
            {
                warriors[i].transform.position = spawnPoints[3 + i].transform.position;
                warriors[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
            // play respective theme
        }

        // make humans invuln
        // move ball far away
        // rain down confetti
        AudioPlayer ParticleAudio;
        if (winner == 0)
        {
            warPart.Play();
            ParticleAudio = warPart.GetComponent<AudioPlayer>();
            ParticleAudio.PlaySoundRandomPitch(ParticleAudio.Find("goalConfetti"));
        } else
        {
            monPart.Play();
            ParticleAudio = monPart.GetComponent<AudioPlayer>();
            ParticleAudio.PlaySoundRandomPitch(ParticleAudio.Find("goalConfetti"));
        }

        // play sound effect
        string songName = winner == 0 ? "humanScore" : "monsterScore";
        //ScoreJingle.PlaySoundSpecificPitch(ScoreJingle.Find(songName), 1.41421f);

        StartCoroutine(EndPodiumSequence());
    }

    private IEnumerator EndPodiumSequence()
    {
        Debug.Log("Podium sequence will end in: " + podiumShowTime);
        yield return new WaitForSeconds(podiumShowTime);
        Debug.Log("Podium sequence ending");
        // freeze time
        Time.timeScale = 0;
        // show scoreboard
        UI.ShowStatsScoreboard(true);

        if (ST != null) UI.ShowMVP(true, ST.GetMVP());
    }

    public UIManager GetUI()
    {
        return UI;
    }
}
