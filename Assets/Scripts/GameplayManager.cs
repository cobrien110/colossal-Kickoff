using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    public bool isPlaying = false;
    public bool isPaused = false;
    public bool automaticAISpawn = true;
    public bool automaticStart = true;
    [SerializeField] private UIManager UM = null;
    [SerializeField] private GameObject Ball = null;
    [SerializeField] private List<GameObject> playerList;
    [SerializeField] private MonsterController MC = null;
    [SerializeField] private WarriorController WC = null;
    [SerializeField] private MultipleTargetCamera MTC = null;
    [SerializeField] private GameObject WarriorAI = null;
    [SerializeField] private GameObject MonsterAI = null;
    [SerializeField] private GameObject MonsterPlayer = null;
    private PlayerInputManager PIM = null;
    private GameObject BallSpawner = null;
    [SerializeField] private GameObject[] WarriorSpawners = null;
    public GameObject warriorPrefab;
    public float passMeter = 0;
    public float passMeterMax = 1.0f;
    private int spawnCount = 0;
    private Transform lastGoalScoredIn;

    Vector3 WarSpawnPos;
    private List<PlayerInput> playerInputs = new List<PlayerInput>();

    public WarriorHolder WH = null;

    // Start is called before the first frame update
    void Start()
    {
        BallSpawner = GameObject.Find("BallSpawner");
        Ball = GameObject.FindGameObjectWithTag("Ball");
        WarriorSpawners = GameObject.FindGameObjectsWithTag("WarriorSpawner");
        PIM = GameObject.Find("Player Spawn Manager").GetComponent<PlayerInputManager>();
        Time.timeScale = 1;

        if (automaticAISpawn && playerList.Count < 4)
        {
            SpawnAI();
        }

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
        //UM.UpdatePassMeter(passMeter);


        //Start Game for Expo
        if (automaticStart && !isPlaying)
        {
            automaticStart = false;
            StartCoroutine(Kickoff());
        }
        else if (Input.GetKeyDown(KeyCode.Return) && !isPlaying)
        {
            StartCoroutine(Kickoff());
            Debug.Log("Enter");
        }

        //Add inputfield.isfocused here when adding console
        
        if (Input.GetKeyDown(KeyCode.Slash))
        {
            Instantiate(MonsterPlayer, new Vector3(-5.25f, 0f, 0f), Quaternion.identity);
        }

        if (Input.GetKeyDown(KeyCode.Dollar) && !isPlaying)
        {
            SpawnAI();
            Debug.Log("Birthed");
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && !isPlaying)
        {
            ResetGame();
        }

        if (UM.GetTimeRemaining() <= 0 && isPlaying)
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
    }

    private IEnumerator Kickoff()
    {
        yield return new WaitForSeconds(0.75f);
        StartCoroutine(UM.Countdown());
        yield return new WaitForSeconds(2.4f);
        StartPlaying();
    }

    

    public void Reset()
    {
        //StopPlaying();
        UM.StopTimer();
        Invoke("FinalizeReset", 3f);

        MultipleTargetCamera MTC = GameObject.Find("Main Camera").GetComponent<MultipleTargetCamera>();
        if (lastGoalScoredIn != null) MTC.targets[0] = lastGoalScoredIn; 
    }

    private void FinalizeReset()
    {
        StopPlaying();
        GameObject newBall = Instantiate(Ball, BallSpawner.transform.position, Quaternion.identity);
        Ball = newBall;
        BallProperties BP = Ball.GetComponent<BallProperties>();
        StartCoroutine(Kickoff());
        BP.isSuperKick = false;
        passMeter = 0;
        UM.UpdatePassMeter(passMeter);
        UM.UpdateWarriorContestBar(passMeter);

        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].tag.Equals("Monster"))
            {
                MC = playerList[i].GetComponent<MonsterController>();
                MC.Ball = newBall;
                MC.BP = BP;
                MC.ResetPlayer();
            }
            else
            {
                WC = playerList[i].GetComponent<WarriorController>();
                WC.Ball = newBall;
                WC.BP = BP;
                WC.ResetPlayer();
            }
        }

        MultipleTargetCamera MTC = GameObject.Find("Main Camera").GetComponent<MultipleTargetCamera>();
        MTC.targets[0] = newBall.transform;
        FollowBall FB = GameObject.Find("BallPointer").GetComponent<FollowBall>();
        FB.BP = Ball.GetComponent<BallProperties>();

        // These lines of code should delete any objects in the scene that have the DELETEAFTERDELAY script attatched
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

    //isPlaying getter and setter
    public bool IsPlayingGet()
    {
        return isPlaying;
    }

    public void IsPlayingSet(bool set)
    {
        isPlaying = set;
    }

    public GameObject GetBall()
    {
        return Ball;
    }

    public void AddPlayer(GameObject playerPrefab, int playerID, Gamepad gamepad)
    {
        WH = GameObject.Find("WarriorHolder").GetComponent<WarriorHolder>();
        //playerInputs.Add(player);
        // MTC.AddTarget(player.transform);

        PlayerInput p = PlayerInput.Instantiate(playerPrefab, controlScheme: "Xbox Control Scheme", pairWithDevice: gamepad);
        //MTC.AddTarget(p.transform);

        NewPlayer(p);
        //if (PIM != null) PIM.playerPrefab = warriorPrefab;
    }

    public void NewPlayer(PlayerInput p)
    {
        GameObject player = p.gameObject;
        if (player.tag.Equals("Monster"))
        {
            MC = player.GetComponent<MonsterController>();
            playerList.Add(player);
            //UM.ShowMonsterUI(true);
        } else if (player.tag.Equals("Warrior"))
        {
            GameObject[] warriors = GameObject.FindGameObjectsWithTag("Warrior");
            WC = player.GetComponent<WarriorController>();
            WC.SetColor(warriors.Length);
            WC.playerNum = warriors.Length;
            playerList.Add(player);
            UM.ShowPlayerUI(true, warriors.Length);
            //UM.ShowPassMeter(true);
            //if (warriors.Length == 1)
            //{

            //}

            MaterialPropertyBlock MPB = new MaterialPropertyBlock();
            MPB.SetColor("_Color", WH.warriorColors[warriors.Length - 1]);
            player.GetComponentInChildren<SpriteRenderer>().SetPropertyBlock(MPB);

            try
            {
                WarriorSpawners = GameObject.FindGameObjectsWithTag("WarriorSpawner");
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
        for (int i = playerList.Count; i < 4; i++)
        {
            Debug.Log("SpawnAI Method Call Reference: " + i);

            if (GameObject.FindGameObjectWithTag("Monster") == null)
            {
                MonsterAI.name = "MonsterAI";
                Instantiate(MonsterAI, new Vector3(5.25f, 0f, -2f), Quaternion.identity);
                MonsterAI = GameObject.Find("MonsterAI(Clone)");
                MC = MonsterAI.GetComponent<MonsterController>();
                MC.monsterSpawner = GameObject.Find("MonsterSpawner");
                MC.transform.position = MC.monsterSpawner.transform.position;
                playerList.Add(MonsterAI);
                UM.ShowMonsterUI(true);
            } else
            {
                WarriorAI.name = i + "_WarriorAI";
                Instantiate(WarriorAI, new Vector3(5.25f, 0f, -2f), Quaternion.identity);
                WarriorAI = GameObject.Find(i + "_WarriorAI(Clone)");
                WC = WarriorAI.GetComponent<WarriorController>();
                WC.WarriorSpawner = WarriorSpawners[spawnCount++];
                WC.transform.position = WC.WarriorSpawner.transform.position;
                WC.SetColor(playerList.Count);
                playerList.Add(WarriorAI);
                UM.ShowPlayerUI(true, i);
            }
        }

        //WarriorAI.name = "2_WarriorAI";
        //Instantiate(WarriorAI, new Vector3(5.25f, 0f, 0f), Quaternion.identity);
        /*WarriorAI.name = "3_WarriorAI";
        Instantiate(WarriorAI, new Vector3(5.25f, 0f, 2f), Quaternion.identity);*/
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

    public void PauseGame()
    {
        if (!SceneManager.GetActiveScene().ToString().Equals("MainMenus") && isPlaying)
        {
            if (isPaused)
            {
                Time.timeScale = 1;
                isPaused = false;
                UM.PauseScreen(isPaused);
            }
            else
            {
                Time.timeScale = 0;
                isPaused = true;
                UM.PauseScreen(isPaused);
            }
        }
    }

    public void MenuReturn()
    {
        Time.timeScale = 1;
        Debug.Log("Back to Menu");
        SceneManager.LoadScene("MainMenus");
    }

    public void SetLastScoredGoal(Transform t)
    {
        lastGoalScoredIn = t;
    }
}
