using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class WarriorTutorialManager : MonoBehaviour
{
    bool initialized = false;
    private int currentObj = 1;

    //Objective Flags
    private bool endTutorial = false;
    private int goalsScored = 0;

    [SerializeField] private GameplayManager GM;
    [SerializeField] private UIManager UM;
    [SerializeField] private TutorialUIHelper TUI;
    [SerializeField] private StatTracker ST;
    [SerializeField] private GameObject PHPrefab;

    [SerializeField] private WarriorController WC;
    [SerializeField] private BallProperties Ball;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (initialized)
        {
            UM.SetTimeRemaining(181);
        }

        if (!endTutorial) CurrentObjective();

        Gamepad gamepad = Gamepad.current;
        if (gamepad.startButton.wasPressedThisFrame && TUI.GetFadeEnd())
        {
            Time.timeScale = 1.0f;
            TUI.ResumeGame();
        }
    }

    public void Initiate()
    {
        GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
        UM = GameObject.Find("Canvas").GetComponent<UIManager>();
        TUI = GameObject.Find("TutorialUIHelper").GetComponent<TutorialUIHelper>();
        ST = GameObject.Find("Stat Tracker").GetComponent<StatTracker>();
        Debug.Log("WARRIOR TUTORIAL INITIALIZED");
        initialized = true;
        // All Tutorial scripting can occur after this point.

        Instantiate(PHPrefab);
        WC = GameObject.Find("Warrior(Clone)").GetComponent<WarriorController>();
        Ball = GameObject.Find("Ball").GetComponent<BallProperties>();
    }

    private void CurrentObjective()
    {
        //Run
        if (currentObj == 1)
        {
            Debug.Log("ON OBJECTIVE #1");
            if (WC.movementDirection != Vector3.zero)
            {
                currentObj++;
                TUI.SetActiveObjective(currentObj);
            }
        }

        //Grab the ball
        if (currentObj == 2)
        {
            Debug.Log("ON OBJECTIVE #2");
            if (Ball.ballOwner != null)
            {
                currentObj++;
                TUI.SetActiveObjective(currentObj);
            }
        }

        //Kick the ball
        if (currentObj == 3)
        {
            Debug.Log("ON OBJECTIVE #3");
            if (Ball.lastKicker != null)
            { 
                currentObj++;
                TUI.SetActiveObjective(currentObj);
                TUI.FadeStart(true);
            }
        }

        //Score a goal
        if (currentObj == 4)
        {
            Debug.Log("ON OBJECTIVE #4");
            goalsScored = ST.GetWGoals(1) + ST.GetMGoals();
            //Debug.Log("Current Goals" + (goalsScored1));

            if (goalsScored >= 1)
            {
                currentObj++;
                TUI.SetActiveObjective(currentObj);
            }
        }

        //End Tutorial
        if (currentObj == 5 && !endTutorial)
        {
            Debug.Log("END");
            endTutorial = true;
        }
    }
}
