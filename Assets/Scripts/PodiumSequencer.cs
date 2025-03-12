using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodiumSequencer : MonoBehaviour
{
    public GameObject podium;
    //public Transform podiumLocation;
    public GameObject[] spawnPoints;
    public GameObject[] walls;
    public float podiumShowTime = 5f;
    public float podiumMoveSpeed = 0.5f;
    public Transform moveTarget;

    private GameplayManager GM;
    private UIManager UI;
    private StatTracker ST;
    private BallProperties BP;
    private MonsterUI MUI;
    private MultipleTargetCamera MTC;
    private GameObject monster;
    private GameObject[] warriors;
    public ParticleSystem monPart;
    public ParticleSystem warPart;
    private AudioPlayer ScoreJingle;
    private JunkThrower JT;
    private bool isMoving = false;
    // Start is called before the first frame update
    void Start()
    {
        GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
        UI = GameObject.Find("Canvas").GetComponent<UIManager>();
        ST = GameObject.Find("Stat Tracker").GetComponent<StatTracker>();
        MTC = GameObject.Find("Main Camera").GetComponent<MultipleTargetCamera>();
        JT = GetComponentInChildren<JunkThrower>();
        ScoreJingle = GameObject.FindGameObjectWithTag("Jukebox2").GetComponent<AudioPlayer>();
        MUI = GameObject.FindWithTag("Monster").GetComponentInChildren<MonsterUI>();
    }

    private void LateUpdate()
    {
        if (isMoving) MovePodium();
    }

    public void StartPodiumSequence(int winner)
    {
        Debug.Log("Podium Sequence Starting");
        if (MUI != null)
        {
            MUI.ShowChargeBar(false);
            MUI.ShowDots(false);
        }

        UI.ShowPlayerScoredText(false);
        UI.ShowInGameUI(false);
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
            BP.ballOwner = null;
            BP.isInteractable = false;
            BP.SetSR(false);
            MTC.RemoveTarget(BP.gameObject.transform);
        }

        // move players
        if (winner == 0)
        {
            StartCoroutine(WarriorWin());
        } else
        {
            StartCoroutine(MonsterWin());
        }

        // move ball far away
        BP.gameObject.GetComponent<Rigidbody>().useGravity = false;
        BP.gameObject.transform.position = new Vector3(0, 100, 0);

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

        // remove deletable objects
        DeleteAfterDelay[] ObjectsToDelete = (DeleteAfterDelay[])FindObjectsByType(typeof(DeleteAfterDelay), FindObjectsSortMode.InstanceID);
        if (ObjectsToDelete.Length != 0)
        {
            for (int i = 0; i < ObjectsToDelete.Length; i++)
            {
                try
                {
                    ObjectsToDelete[i].Kill();
                }
                catch
                {
                    // NOTHING HAHA
                }

            }
        }
    }

    private IEnumerator WarriorWin()
    {
        // move warriors to podium and make them unkillable
        for (int i = 0; i < warriors.Length; i++)
        {
            WarriorController WC = warriors[i].GetComponent<WarriorController>();
            WC.canRespawn = false;
            WC.RespawnEarly();

            warriors[i].transform.position = spawnPoints[i].transform.position;
            warriors[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            WC.isWinner = true;
        }
        // move monster to ground
        monster.transform.position = spawnPoints[6].transform.position;
        monster.GetComponent<Rigidbody>().velocity = Vector3.zero;

        //stop Monster abilities
        MonsterController MC = monster.GetComponent<MonsterController>();
        MC.canUseAbilities = false;
        //quetz check
        AbilitySnakeSegments ASS = MC.GetComponent<AbilitySnakeSegments>();

        yield return new WaitForSeconds(3f);
        // start throwing tomatoes
        JT.isSpawning = true;
        JT.monster = monster;
        
            // play respective theme
    }

    private IEnumerator MonsterWin()
    {
        // move monster to podium
        monster.transform.position = spawnPoints[0].transform.position;
        monster.GetComponent<Rigidbody>().velocity = Vector3.zero;
        //quetz check
        MonsterController MC = monster.GetComponent<MonsterController>();
        AbilitySnakeSegments ASS = MC.GetComponent<AbilitySnakeSegments>();
        if (ASS != null) ASS.ResetSegments();

        // move warriors to ground
        for (int i = 0; i < warriors.Length; i++)
        {
            WarriorController WC = warriors[i].GetComponent<WarriorController>();
            WC.canRespawn = false;
            WC.RespawnEarly();
            //WC.isWinner = true;

            warriors[i].transform.position = spawnPoints[3 + i].transform.position;
            warriors[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
        }

        //stop Monster abilitie
        MC.canUseAbilities = false;

        yield return new WaitForSeconds(3f);
        // make warriors vulnerable
        for (int i = 0; i < warriors.Length; i++)
        {
            WarriorController WC = warriors[i].GetComponent<WarriorController>();
            WC.isWinner = false;

        }

        // Move the podium down
        isMoving = true;
        MC.canUseAbilities = true;
        // parent monster to podium to simulate movement
        monster.transform.parent = podium.transform;

        // make warriors scared(?)

        // play respective theme
    }

    private void MovePodium()
    {
        if (moveTarget != null) podium.transform.position = Vector3.MoveTowards(podium.transform.position, moveTarget.position, podiumMoveSpeed * Time.deltaTime);
        if (Vector3.Distance(podium.transform.position, moveTarget.position) < 0.1f) RemoveWalls();
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
        UI.ShowGameOverText(false, -1);
        UI.UpdateWinnerTextSB(ST.GetGameWinner());
        UI.GoldenWarriorStats();

        if (ST != null) UI.ShowMVP(true, ST.GetMVP());
    }

    public UIManager GetUI()
    {
        return UI;
    }

    private void RemoveWalls()
    {
        foreach (GameObject wall in walls)
        {
            wall.SetActive(false);
        }
    }
}
