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
    [SerializeField] private GameObject WarriorAI = null;
    [SerializeField] private GameObject MonsterPlayer = null;
    private PlayerInputManager PIM = null;
    private GameObject BallSpawner = null;
    [SerializeField] private GameObject[] WarriorSpawners = null;
    public GameObject warriorPrefab;
    public float passMeter = 0;
    public float passMeterMax = 1.0f;
    private int spawnCount = 0;

    Vector3 WarSpawnPos;
    private List<PlayerInput> playerInputs = new List<PlayerInput>();

    // Start is called before the first frame update
    void Start()
    {
        BallSpawner = GameObject.Find("BallSpawner");
        WarriorSpawners = GameObject.FindGameObjectsWithTag("WarriorSpawner");
        PIM = GameObject.Find("Player Spawn Manager").GetComponent<PlayerInputManager>();
        Time.timeScale = 1;


        //WIP
        int count = 0;
        while (count < 2) {
            string currentPlayer = "Player" + count;
            Debug.Log(count + " Player Selected: " + PlayerPrefs.GetInt(currentPlayer));
            count++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (passMeter > passMeterMax)
        {
            passMeter = passMeterMax;
        }
        UM.UpdatePassMeter(passMeter);


        //Start Game for Expo
        if (Input.GetKeyDown(KeyCode.Return) && !isPlaying)
        {
            StartCoroutine(Kickoff());
            Debug.Log("Enter");
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            Instantiate(MonsterPlayer, new Vector3(-5.25f, 0f, 0f), Quaternion.identity);
        }

        if (Input.GetKeyDown(KeyCode.B) && !isPlaying)
        {
            SpawnAI();
            Debug.Log("Birthed");
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && !isPlaying)
        {
            ResetGame();
        }

        if (UM.GetTimeRemaining() == 0)
        {
            Time.timeScale = 0;
            StopPlaying();
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
        yield return new WaitForSeconds(2.4f);
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

        GameObject[] MinoWalls = GameObject.FindGameObjectsWithTag("MinoWall");
        if (MinoWalls.Length != 0)
        {
            for (int i = 0; i < MinoWalls.Length; i++)
            {
                try
                {
                    MinoWalls[i].GetComponent<DeleteAfterDelay>().Kill();
                }
                catch
                {
                    // NOTHING HAHA
                }

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

    public void AddPlayer(GameObject playerPrefab, int playerID)
    {
        //playerInputs.Add(player);
        // MTC.AddTarget(player.transform);

        PlayerInput p = PlayerInput.Instantiate(playerPrefab, controlScheme: "Xbox Control Scheme", pairWithDevice: Gamepad.all[playerID]);
        //MTC.AddTarget(p.transform);

        NewPlayer();
        //if (PIM != null) PIM.playerPrefab = warriorPrefab;
    }

    public void NewPlayer()
    {

        GameObject player;
        if (playerList.Count == 0 && GameObject.FindGameObjectWithTag("Monster"))
        {
            player = GameObject.FindGameObjectWithTag("Monster");
            MC = player.GetComponent<MonsterController>();
            playerList.Add(player);
            //UM.ShowMonsterUI(true);
        } else if (GameObject.FindGameObjectWithTag("Warrior"))
        {
            GameObject[] warriors = GameObject.FindGameObjectsWithTag("Warrior");
            player = warriors[warriors.Length - 1];
            WC = player.GetComponent<WarriorController>();
            WC.SetColor(playerList.Count);
            WC.playerNum = warriors.Length;
            playerList.Add(player);
            //UM.ShowPlayerUI(true, warriors.Length);
            //UM.ShowPassMeter(true);
            //if (warriors.Length == 1)
            //{
               
            //}

            try
            {
                WC.WarriorSpawner = WarriorSpawners[spawnCount++];
                WC.transform.position = WC.WarriorSpawner.transform.position;
            } catch
            {
                // Null Reference Catch
            }
            
        }
    }

    public void SpawnAI()
    {
        WarriorAI.name = "1_WarriorAI";
        Instantiate(WarriorAI, new Vector3(5.25f, 0f, -2f), Quaternion.identity);
        WarriorAI.name = "2_WarriorAI";
        Instantiate(WarriorAI, new Vector3(5.25f, 0f, 0f), Quaternion.identity);
        WarriorAI.name = "3_WarriorAI";
        Instantiate(WarriorAI, new Vector3(5.25f, 0f, 2f), Quaternion.identity);
    }

    public void ResetGame()
    {
        Debug.Log("Resetting Game");
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        return;
        
        /*
        //isPlaying = true;
        UM.ShowGameOverText(false, 3);
        GameObject ballTemp = Ball.gameObject;
        Reset();
        Destroy(ballTemp);
        UM.ResetScoreAndTime();

        // Resume Game
        Time.timeScale = 1;
        */
    }
}
