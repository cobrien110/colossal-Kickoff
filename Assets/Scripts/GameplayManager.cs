using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    [Header("Debug Properties")]
    public bool automaticAISpawn = true;
    public bool automaticStart = true;
    public bool passIndicator = true;

    [Header("Other Properties")]
    public bool isPlaying = false;
    public bool isPaused = false;
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
    private MusicPlayer MP = null;
    private GameObject BallSpawner = null;
    [SerializeField] private GameObject[] WarriorSpawners = null;
    public GameObject warriorPrefab;
    //public bool isPassing = false;
    public float passMeter = 0;
    public float passMeterMax = 1.0f;
    private int spawnCount = 0;
    private Transform lastGoalScoredIn;
    Vector3 WarSpawnPos;
    private List<PlayerInput> playerInputs = new List<PlayerInput>();

    public WarriorHolder WH = null;

    [Header("Goal Barrier Settings")]
    public bool barriersAreOn = false;
    [Range(0, 2)] public int barrierRespawnStyle = 0;
    public float barrierMaxHealth = 10;
    public float barrierBounceForce = 150f;
    public float barrierBounceAngle = 45f;
    

    // Start is called before the first frame update
    void Start()
    {
        BallSpawner = GameObject.Find("BallSpawner");
        Ball = GameObject.FindGameObjectWithTag("Ball");
        WarriorSpawners = GameObject.FindGameObjectsWithTag("WarriorSpawner");
        PIM = GameObject.Find("Player Spawn Manager").GetComponent<PlayerInputManager>();
        MP = GameObject.FindGameObjectWithTag("Jukebox").GetComponent<MusicPlayer>();
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
        //Debug.Log("Starting play");
        isPlaying = true;
        UM.StartTimer();

        //Debug.Log("Calling Unpause music");
        MP.UnPauseMusic();
    }

    public void StopPlaying()
    {
        isPlaying = false;
    }

    private IEnumerator Kickoff()
    {
        if (MC != null) MC.ResetAbilities();
        AbilityScript.canActivate = true;
        yield return new WaitForSeconds(0.75f);
        StartCoroutine(UM.Countdown());
        yield return new WaitForSeconds(3f);
        StartPlaying();
    }

    

    public void Reset()
    {
        //StopPlaying();
        UM.StopTimer();
        if (MP != null) MP.PauseMusic();
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
        UM.UpdateWarriorContestBar(passMeter);
        Debug.Log(playerList);
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

        // Reset Goal barriers
        GoalWithBarrier[] goals = GameObject.FindObjectsOfType<GoalWithBarrier>();
        foreach (GoalWithBarrier goal in goals)
        {
            goal.Respawn();
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
            

            //UM.ShowPlayerUI(true, warriors.Length);

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
        int playerNumberInput;
        for (int i = playerList.Count; i < 4; i++)
        {
            Debug.Log("SpawnAI Method Call Reference: " + i);

            if (GameObject.FindGameObjectWithTag("Monster") == null)
            {
                MonsterAI.name = "MonsterAI";
                Debug.Log(i);
                GameObject newMon = Instantiate(MonsterAI, new Vector3(5.25f, 0f, -2f), Quaternion.identity);
                //MonsterAI = GameObject.Find("MonsterAI(Clone)");
                MC = MonsterAI.GetComponent<MonsterController>();
                MC.monsterSpawner = GameObject.Find("MonsterSpawner");
                MC.transform.position = MC.monsterSpawner.transform.position;
                if (!CheckPlayerList(true, newMon)) playerList.Add(newMon);
                UM.ShowMonsterUI(true);
            } else
            {
                GameObject[] warriors = GameObject.FindGameObjectsWithTag("Warrior");
                playerNumberInput = warriors.Length + 1;
                Debug.Log("WarriorAI_"+ (playerNumberInput));
                WarriorAI.name = "WarriorAI_" + (playerNumberInput);
                GameObject newWar = Instantiate(WarriorAI, new Vector3(5.25f, 0f, -2f), Quaternion.identity);
                //WarriorAI = GameObject.Find("WarriorAI_" + (warriors.Length + 1) + "(Clone)");
                WC = newWar.GetComponent<WarriorController>();
                Debug.Log("My Spawner:" + spawnCount);
                WC.WarriorSpawner = WarriorSpawners[spawnCount++];
                WC.transform.position = WC.WarriorSpawner.transform.position;
                WC.SetColor(playerNumberInput);
                WC.SetPlayerNum(playerNumberInput);
                playerList.Add(newWar);
                Debug.Log("My Player Num:"+ WC.playerNum);             
                Debug.Log("My Name" + WarriorAI.name);
                Debug.Log("PLAYER ADDED");
                //UM.ShowPlayerUI(true, i);

            }
        }

        //WarriorAI.name = "2_WarriorAI";
        //Instantiate(WarriorAI, new Vector3(5.25f, 0f, 0f), Quaternion.identity);
        /*WarriorAI.name = "3_WarriorAI";
        Instantiate(WarriorAI, new Vector3(5.25f, 0f, 2f), Quaternion.identity);*/
    }

    private bool CheckPlayerList(bool isAddingMonster, GameObject thingToAdd)
    {
        // if adding monster, check to see if there are any human warriors
        if (isAddingMonster)
        {
            if (playerList.Count > 0)
            {
                // if there are human warriors add monster to front of list
                List<GameObject> tempList = new List<GameObject>();
                tempList.Add(thingToAdd);
                for (int i = 0; i < playerList.Count; i++)
                {
                    WC = playerList[i].GetComponent<WarriorController>();
                    WC.WarriorSpawner = WarriorSpawners[i];
                    WC.transform.position = WC.WarriorSpawner.transform.position;
                    tempList.Add(playerList[i]);
                }
                playerList = tempList;
                return true;
            }
        }
        return false;
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
