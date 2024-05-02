using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    public bool isPlaying = false;
    [SerializeField] private UIManager UM = null;
    [SerializeField] private GameObject Ball = null;
    [SerializeField] private List<GameObject> playerList;
    [SerializeField] private MonsterController MC = null;
    [SerializeField] private WarriorController WC = null;
    [SerializeField] private MultipleTargetCamera MTC = null;
    private PlayerInputManager PIM = null;
    private GameObject BallSpawner = null;
    private GameObject[] WarriorSpawners = null;
    public GameObject warriorPrefab;
    public int passMeter = 0;
    public int passMeterMax = 100;

    Vector3 WarSpawnPos;
    private List<PlayerInput> playerInputs = new List<PlayerInput>();

    // Start is called before the first frame update
    void Start()
    {
        BallSpawner = GameObject.Find("BallSpawner");
        WarriorSpawners = GameObject.FindGameObjectsWithTag("WarriorSpawner");
        PIM = GameObject.Find("Warrior Manager").GetComponent<PlayerInputManager>();
        StartCoroutine(Kickoff());
    }

    // Update is called once per frame
    void Update()
    {
        if (passMeter > passMeterMax)
        {
            passMeter = passMeterMax;
        }
        UM.UpdatePassMeterText(passMeter);

        if (Input.GetKeyDown(KeyCode.LeftControl) && !isPlaying)
        {
            ResetGame();
        }
    }

    public void StartPlaying()
    {
        isPlaying = true;
        UM.StartTimer();
    }

    public void StopPlaying()
    {
        isPlaying = false;
        UM.StopTimer();
    }

    private IEnumerator Kickoff()
    {
        StartCoroutine(UM.Countdown());
        yield return new WaitForSeconds(1.8f);
        StartPlaying();
    }

    public void Reset()
    {
        StopPlaying();
        StartCoroutine(Kickoff());
        GameObject newBall = Instantiate(Ball, BallSpawner.transform.position, Quaternion.identity);
        Ball = newBall;
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].tag.Equals("Monster"))
            {
                MC = playerList[i].GetComponent<MonsterController>();
                MC.Ball = newBall;
                MC.BP = Ball.GetComponent<BallProperties>();
                MC.ResetPlayer();
            } else
            {
                WC = playerList[i].GetComponent<WarriorController>();
                WC.Ball = newBall;
                WC.BP = Ball.GetComponent<BallProperties>();
                WC.ResetPlayer();
            }
        }
        MultipleTargetCamera MTC = GameObject.Find("Main Camera").GetComponent<MultipleTargetCamera>();
        MTC.targets[0] = newBall.transform;
        FollowBall FB = GameObject.Find("BallPointer").GetComponent<FollowBall>();
        FB.BP = Ball.GetComponent<BallProperties>();
        passMeter = 0;
    }

    //isPlaying getter and setter
    public bool IsPlayingGet()
    {
        return isPlaying;
    }

    public void IsPlayingSet(bool set)
    {
        isPlaying = set;
    }

    public void AddPlayer(PlayerInput player)
    {
        playerInputs.Add(player);
        MTC.AddTarget(player.transform);
        NewPlayer();
        if (PIM != null) PIM.playerPrefab = warriorPrefab;
    }

    public void NewPlayer()
    {
        GameObject player;
        if (playerList.Count == 0 && GameObject.FindGameObjectWithTag("Monster"))
        {
            player = GameObject.FindGameObjectWithTag("Monster");
            MC = player.GetComponent<MonsterController>();
            playerList.Add(player);
        } else if (GameObject.FindGameObjectWithTag("Warrior"))
        {
            GameObject[] warriors = GameObject.FindGameObjectsWithTag("Warrior");
            player = warriors[warriors.Length - 1];
            WC = player.GetComponent<WarriorController>();
            WC.SetColor(playerList.Count);
            playerList.Add(player);
            WC.WarriorSpawner = WarriorSpawners[warriors.Length - 1];
        }
    }

    public void ResetGame()
    {
        Debug.Log("Resetting Game");
        isPlaying = true;
        UM.ShowGameOverText(false);
        GameObject ballTemp = Ball.gameObject;
        Reset();
        Destroy(ballTemp);
        UM.ResetScoreAndTime();

        // Resume Game
        Time.timeScale = 1;
    }
}
