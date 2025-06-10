using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class TutorialManager : MonoBehaviour
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
    [SerializeField] private InputActionAsset inputMaster;
    public float deadzoneValue = 0.3f;

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
    [SerializeField] private WarriorController WC = null;
    //[SerializeField] private MultipleTargetCamera MTC = null;
    [SerializeField] private GameObject WarriorAI = null;
    [SerializeField] private GameObject MonsterAI = null;
    [SerializeField] private GameObject MonsterPlayer = null;
    private PlayerInputManager PIM = null;
    [SerializeField] private MusicPlayer MP = null;
    //private MusicPlayerResults MPr = null;
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
        aiMummymanager = FindObjectOfType<AiMummyManager>();
        Time.timeScale = 1;

        //WIP
        int count = 0;
        while (count < 2)
        {
            string currentPlayer = "Player" + count;
            Debug.Log(count + " Player Selected: " + PlayerPrefs.GetInt(currentPlayer));
            count++;
        }

        if (usePlayerPrefs)
        {
            GetInputPrefs();
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

        if (pauseTimer < pauseDelay)
        {
            pauseTimer += Time.unscaledDeltaTime;
        }
    }

    public void StartPlaying()
    {
        Debug.Log("Starting play");
        MP.UnPauseMusic();
        isPlaying = true;
    }

    public void StopPlaying()
    {
        isPlaying = false;
    }

    private IEnumerator Kickoff()
    {
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
        Invoke("FinalizeReset", 3f);

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
        BP.canFullSuperKick = false;
        passMeter = 0;
        UM.UpdateWarriorContestBar(passMeter);
        UM.ShowPlayerScoredText(false);
        Debug.Log(playerList);
        for (int i = 0; i < playerList.Count; i++)
        {
            WC = playerList[i].GetComponent<WarriorController>();
            WC.Ball = newBall;
            WC.BP = BP;
            WC.ResetPlayer();
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


    private void GetInputPrefs()
    {
        deadzoneValue = PlayerPrefs.GetFloat("deadzoneValue");
        inputMaster.FindActionMap("Player").FindAction("Aim").ApplyParameterOverride("StickDeadzone:min", deadzoneValue);
        inputMaster.FindActionMap("Monster").FindAction("Aim").ApplyParameterOverride("StickDeadzone:min", deadzoneValue);
        Debug.Log("DEADZONE VALUE: " + inputMaster.FindActionMap("Player").FindAction("Aim").GetParameterValue("StickDeadzone:min"));
    }

    public void AddPlayer(GameObject playerPrefab, int playerID, int warriorPosition, Gamepad gamepad)
    {
        WH = GameObject.Find("WarriorHolder").GetComponent<WarriorHolder>();
        //playerInputs.Add(player);
        // MTC.AddTarget(player.transform);

        PlayerInput p = PlayerInput.Instantiate(playerPrefab, controlScheme: "Xbox Control Scheme", pairWithDevice: gamepad);
        //MTC.AddTarget(p.transform);

        NewPlayer(p, playerID, warriorPosition);
        //if (PIM != null) PIM.playerPrefab = warriorPrefab;
    }

    public void NewPlayer(PlayerInput p, int playerID, int warriorPosition)
    {
        GameObject player = p.gameObject;
        if (player.tag.Equals("Warrior"))
        {
            GameObject[] warriors = GameObject.FindGameObjectsWithTag("Warrior");
            WC = player.GetComponent<WarriorController>();
            WC.playerID = playerID;
            WC.warriorPosition = warriorPosition;
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
            }
            catch
            {
                // Null Reference Catch
            }

            UM.SetPlayerPortrait(false, WC);
        }
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


    public void SetMP(MusicPlayer m)
    {
        MP = m;
    }
}
