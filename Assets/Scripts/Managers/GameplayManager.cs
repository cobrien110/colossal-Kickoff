using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameplayManager : MonoBehaviour
{
    [Header("Debug Properties")]
    public bool automaticAISpawn = true;
    public bool automaticStart = true;
    public bool passIndicator = true;
    public bool debugMode;

    [Header("Game State")]
    public bool isPlaying = false;
    public bool isGameOver = false;
    public bool isPaused = false;
    public bool hasScored = false;
    public bool overtimeStarted = false;
    private bool podiumSequenceStarted = false;
    private int spawnCount = 0;

    [Header("Timers and Delays")]
    private float pauseDelay = 0.5f;
    private float pauseTimer = 0f;

    [Header("Game Time and Mechanics")]
    public int gameSeconds;
    public float warriorKickChargeSpeed;
    public float monsterKickChargeSpeed;

    [Header("Player Settings")]
    public int overtimeStyle = 0;
    public int chargeStyle = 0;
    public Color[] playerColors = new Color[4];
    private List<PlayerInput> playerInputs = new List<PlayerInput>();

    [Header("Passing System")]
    public float passMeter = 0;
    public float passMeterMax = 1.0f;

    [Header("Goal Barrier Settings")]
    public bool usePlayerPrefs = true;
    public bool barriersAreOn = false;
    [Range(0, 2)] public int barrierRespawnStyle = 0;
    public float barrierMaxHealth = 10;
    public float barrierBounceForce = 150f;
    public float barrierBounceAngle = 45f;

    [Header("Spawning System")]
    public GameObject warriorPrefab;
    private Transform lastGoalScoredIn;
    Vector3 WarSpawnPos;

    [Header("References")]
    [SerializeField] private UIManager UM = null;
    public SceneInfoManager SceneIM;
    [SerializeField] private GameObject Ball = null;
    [SerializeField] private AsyncLoadManager ALM = null;
    [SerializeField] private List<GameObject> playerList;
    [SerializeField] private MonsterController MC = null;
    [SerializeField] private WarriorController WC = null;
    //[SerializeField] private MultipleTargetCamera MTC = null;
    [SerializeField] private GameObject WarriorAI = null;
    [SerializeField] private GameObject MonsterAI = null;
    [SerializeField] private GameObject MonsterPlayer = null;
    private PlayerInputManager PIM = null;
    [SerializeField] private MusicPlayer MP = null;
    //private MusicPlayerResults MPr = null;
    private PodiumSequencer PS;
    private GameObject BallSpawner = null;
    [SerializeField] private GameObject[] WarriorSpawners = null;
    private AiMummyManager aiMummymanager;
    public WarriorHolder WH = null;


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            InputSystem.EnableDevice(Gamepad.all[i]);
        }

        BallSpawner = GameObject.Find("BallSpawner");
        Ball = GameObject.FindGameObjectWithTag("Ball");
        WarriorSpawners = GameObject.FindGameObjectsWithTag("WarriorSpawner");
        PIM = GameObject.Find("Player Spawn Manager").GetComponent<PlayerInputManager>();
        ALM = GameObject.Find("AsychLoader").GetComponent<AsyncLoadManager>();
        MP = GameObject.FindGameObjectWithTag("Jukebox").GetComponent<MusicPlayer>();
        //MPr = MP.GetComponentInChildren<MusicPlayerResults>();
        PS = GameObject.Find("PodiumSequencer").GetComponent<PodiumSequencer>();
        aiMummymanager = FindObjectOfType<AiMummyManager>();
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

        if (usePlayerPrefs)
        {
            GetBarrierPrefs();
            GetOvertimePrefs();
            GetChargePrefs();
        }
        SetPlayerColors();
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
        
        //if (Input.GetKeyDown(KeyCode.Slash))
        //{
        //    Instantiate(MonsterPlayer, new Vector3(-5.25f, 0f, 0f), Quaternion.identity);
        //}

        //if (Input.GetKeyDown(KeyCode.Dollar) && !isPlaying)
        //{
        //    SpawnAI();
        //    Debug.Log("Birthed");
        //}

        //if (Input.GetKeyDown(KeyCode.LeftControl) && !isPlaying)
        //{
        //    ResetGame();
        //}

        if (UM.GetTimeRemaining() <= 0 && !podiumSequenceStarted && UM.overtime == false)
        {
            // Before ending game, trigger Podium Sequence
            //StopPlaying();
            podiumSequenceStarted = true;
            if (PS == null) Time.timeScale = 0;
            else
            {
                StartPodiumSequence();
            }
            //EndMatch();
        }

        if (pauseTimer < pauseDelay)
        {
            pauseTimer += Time.unscaledDeltaTime;
        }
    }

    public void StartPlaying()
    {
        Debug.Log("Starting play");
        isPlaying = true;
        if (overtimeStyle == 1 && UM.overtime)
        {
            UM.SuddenDeathStart();
            OvertimeMusic();
            Debug.Log("Calling Unpause music");
            MP.UnPauseMusic();
        } else if (overtimeStyle == 0 && UM.overtime)
        {
            //UM.SuddenDeathStart();
            OvertimeMusic();
            Debug.Log("Calling Unpause music");
            MP.UnPauseMusic();
            UM.StartTimer();
        }
        else
        {
            UM.StartTimer();

            Debug.Log("Calling Unpause music");
            MP.UnPauseMusic();
        }
    }

    public void StopPlaying()
    {
        isPlaying = false;
    }

    public void StartPodiumSequence()
    {
        Debug.Log("GM calling start of podium sequence");
        isGameOver = true;
        overtimeStarted = false;
        MP.PlayResults();
        PS.StartPodiumSequence(PS.GetUI().CheckWinner());
        GameObject[] hazards = GameObject.FindGameObjectsWithTag("Hazard");
        foreach (GameObject g in hazards)
        {
            g.SetActive(false);
        }

        // Remove all mummies
        if (aiMummymanager != null) aiMummymanager.ResetMummies();
    }

    private IEnumerator Kickoff()
    {
        if (MC != null) MC.ResetAbilities();
        AbilityScript.canActivate = true;
        yield return new WaitForSeconds(0.75f);
        StartCoroutine(UM.Countdown());
        yield return new WaitForSeconds(3f);
        StartPlaying();
        hasScored = false;
    }

    public void Reset()
    {
        //StopPlaying();
        UM.StopTimer();
        hasScored = true;
        if ((MP != null && !overtimeStarted) || (MP != null && MP.GetComponent<MusicPlayerOvertime>() != null)) MP.PauseMusic();
        else Debug.Log("Failed to pause music");
        Invoke("FinalizeReset", 3f);

        MultipleTargetCamera MTC = GameObject.Find("Main Camera").GetComponent<MultipleTargetCamera>();
        if (lastGoalScoredIn != null) MTC.targets[0] = lastGoalScoredIn; 
    }

    public void ResetOvertime()
    {
        //StopPlaying();
        UM.StopTimer();
        hasScored = true;
        if (MP != null && MP.GetComponent<MusicPlayerOvertime>() != null) MP.PauseMusic();
        else Debug.Log("Failed to pause music");
        Invoke("FinalizeResetOvertime", 3f);

        MultipleTargetCamera MTC = GameObject.Find("Main Camera").GetComponent<MultipleTargetCamera>();
        if (lastGoalScoredIn != null) MTC.targets[0] = lastGoalScoredIn;
    }

    private void FinalizeReset()
    {
        StopPlaying();
        Vector3 spawnPosition = BallSpawner.transform.position;
        GameObject newBall = Instantiate(Ball, spawnPosition, Quaternion.identity);
        Ball = newBall;
        BallProperties BP = Ball.GetComponent<BallProperties>();
        StartCoroutine(Kickoff());
        BP.isSuperKick = false;
        BP.isFullSuperKick = false;
        passMeter = 0;
        UM.UpdateWarriorContestBar(passMeter);
        UM.ShowPlayerScoredText(false);
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

    private void FinalizeResetOvertime()
    {
        //StopPlaying();
        
        Vector3 spawnPosition = BallSpawner.transform.position;
        GameObject newBall = Instantiate(Ball, spawnPosition, Quaternion.identity);
        Ball = newBall;
        BallProperties BP = Ball.GetComponent<BallProperties>();

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
        MTC.targets[0] = null;
        //FollowBall FB = GameObject.Find("BallPointer").GetComponent<FollowBall>();
        //FB.BP = Ball.GetComponent<BallProperties>();

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

    private void GetBarrierPrefs()
    {
        int goalSetting = PlayerPrefs.GetInt("goalBarriers");
        if (goalSetting == 0)
        {
            // DEFAULT
            barrierRespawnStyle = 2;
            barriersAreOn = true;
        } else if (goalSetting == 1)
        {
            // HIGH HEALTH
            barrierMaxHealth *= 1.5f;
            barrierRespawnStyle = 2;
            barriersAreOn = true;
        } else if (goalSetting == 2)
        {
            // SINGLE HIT
            barrierMaxHealth = 1;
            barrierRespawnStyle = 2;
            barriersAreOn = true;
        } else if (goalSetting == 3) {
            // PERSISTANT
            barrierRespawnStyle = 1;
            barriersAreOn = true;
        } else if (goalSetting == 4)
        {
            // PERSISTANT HIGH HEALTH
            barrierMaxHealth *= 1.5f;
            barrierRespawnStyle = 1;
            barriersAreOn = true;
        } else if (goalSetting == 5)
        {
            // PERSISTANT SINGLE HIT
            // SINGLE HIT
            barrierMaxHealth = 1;
            barrierRespawnStyle = 1;
            barriersAreOn = true;
        } else
        {
            // NO BARRIERS
            barriersAreOn = false;
        }
    }

    private void GetOvertimePrefs()
    {
        overtimeStyle = PlayerPrefs.GetInt("overtime");
    }

    private void GetChargePrefs()
    {
        chargeStyle = PlayerPrefs.GetInt("kickcharge");
        if (chargeStyle == 0)
        {
            //STANDARD
            monsterKickChargeSpeed = 1.0f;
            warriorKickChargeSpeed = 1.0f;
        }
        else if (chargeStyle == 1)
        {
            //FAST
            monsterKickChargeSpeed = 1.5f;
            warriorKickChargeSpeed = 1.5f;
        }
        else
        {
            //SLOW
            monsterKickChargeSpeed = 0.5f;
            warriorKickChargeSpeed = 0.5f;
        }

    }

    public void AddPlayer(GameObject playerPrefab, int playerID, Gamepad gamepad)
    {
        WH = GameObject.Find("WarriorHolder").GetComponent<WarriorHolder>();
        //playerInputs.Add(player);
        // MTC.AddTarget(player.transform);

        PlayerInput p = PlayerInput.Instantiate(playerPrefab, controlScheme: "Xbox Control Scheme", pairWithDevice: gamepad);
        //MTC.AddTarget(p.transform);

        NewPlayer(p, playerID);
        //if (PIM != null) PIM.playerPrefab = warriorPrefab;
    }

    public void NewPlayer(PlayerInput p, int playerID)
    {
        GameObject player = p.gameObject;
        if (player.tag.Equals("Monster"))
        {
            MC = player.GetComponent<MonsterController>();
            MC.playerID = playerID;
            playerList.Add(player);
            //UM.ShowMonsterUI(true);
        } else if (player.tag.Equals("Warrior"))
        {
            GameObject[] warriors = GameObject.FindGameObjectsWithTag("Warrior");
            WC = player.GetComponent<WarriorController>();
            WC.playerID = playerID;
            //WC.SetColor(warriors.Length);
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
                //WC.SetColor(playerNumberInput);
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
        ALM.BeginLoad(SceneManager.GetActiveScene().name);
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

    public void PauseGame(int playerID)
    {
        if (!SceneManager.GetActiveScene().ToString().Equals("MainMenus") && isPlaying && !hasScored)
        {
            if (pauseTimer < pauseDelay || isGameOver) return;
            pauseTimer = 0f;
            GameObject[] playerHolders = GameObject.FindGameObjectsWithTag("PlayerHolder");
            if (isPaused)
            {
                Time.timeScale = 1;
                isPaused = false;
                UM.PauseScreen(isPaused, playerID);
                MP.UnPauseMusic();

                for (int i = 0; i < playerHolders.Length; i++)
                {
                    InputSystem.EnableDevice(playerHolders[i].GetComponent<PlayerHolder>().thisGamepad);
                }
            }
            else
            {
                Time.timeScale = 0;
                isPaused = true;
                UM.PauseScreen(isPaused, playerID);
                MP.PauseMusicNoFloor();
                
                for (int i = 0; i < playerHolders.Length; i++)
                {
                    PlayerHolder currentPH = playerHolders[i].GetComponent<PlayerHolder>();
                    if (currentPH.playerID == playerID)
                    {
                        EventSystem.current.SetSelectedGameObject(GameObject.Find("ButtonResume"));
                    }
                    else
                    {
                        InputSystem.DisableDevice(currentPH.thisGamepad);
                    }
                }
            }
        }
    }

    public void MenuReturn()
    {
        Time.timeScale = 1;
        Debug.Log("Back to Menu");
        ALM.BeginLoad("MainMenus");
    }

    public void SetLastScoredGoal(Transform t)
    {
        lastGoalScoredIn = t;
    }

    private void SetPlayerColors()
    {
        Debug.Log("FINDING COLOR");
        int count = 1;
        GameObject[] playerHolders = GameObject.FindGameObjectsWithTag("PlayerHolder");
        GameObject[] warriors = GameObject.FindGameObjectsWithTag("Warrior");
        PlayerHolder PH = null;

        foreach (GameObject holder in playerHolders)
        {
            PH = holder.GetComponent<PlayerHolder>();
            if (PH != null)
            {
                if (PH.teamName.Equals("Monster"))
                {
                    //do nothing atm
                }
                else
                {
                    playerColors[count] = PH.warriorColor;
                    count++;
                }
            }
        }

        count = 1;
        foreach (GameObject warrior in warriors)
        {
            WC = warrior.GetComponent<WarriorController>();
            if (WC != null)
            {
                if (warrior.GetComponent<WarriorAiController>() != null)
                {
                    //do nothing
                }
                else
                {
                    WC.SetColor(playerColors[count]);
                    count++;
                }
            }
        }
    }

    public void OvertimeMusic()
    {
        if (!overtimeStarted)
        {
            MP.SwitchToOvertime();
            overtimeStarted = true;
        }
    }

    public void SetMP(MusicPlayer m)
    {
        MP = m;
    }
}
