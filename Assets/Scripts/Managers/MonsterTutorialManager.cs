using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MonsterTutorialManager : MonoBehaviour
{
    bool initialized = false;
    private int currentObj = 1;
    private int currentSubset = 0;

    //Objective Flags
    private bool endTutorial = false;
    private int goalsScored = 0;

    [SerializeField] private GameplayManager GM;
    [SerializeField] private UIManager UM;
    [SerializeField] private MTutorialUIHelper TUI;
    [SerializeField] private StatTracker ST;
    [SerializeField] private GameObject PHPrefab;

    [SerializeField] private MonsterController MC;
    [SerializeField] private AbilitySphericalAttack ASA;
    [SerializeField] private AbilityMinotaurWall AMW;
    [SerializeField] private AbilityBullrush ABR;
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
            if (!endTutorial)
            {
                Time.timeScale = 1.0f;
                TUI.ShowSubset(currentSubset);
                TUI.ResumeGame();
            }
            else
            {
                GM.MenuReturn();
            }
        }
    }

    public void Initiate()
    {
        GM = GameObject.Find("Gameplay Manager").GetComponent<GameplayManager>();
        UM = GameObject.Find("Canvas").GetComponent<UIManager>();
        TUI = GameObject.Find("TutorialUIHelper").GetComponent<MTutorialUIHelper>();
        ST = GameObject.Find("Stat Tracker").GetComponent<StatTracker>();
        Debug.Log("M TUTORIAL INITIALIZED");
        initialized = true;
        // All Tutorial scripting can occur after this point.

        Instantiate(PHPrefab);
        MC = GameObject.Find("Minotaur(Clone)").GetComponent<MonsterController>();
        ASA = GameObject.Find("Minotaur(Clone)").GetComponent<AbilitySphericalAttack>();
        AMW = GameObject.Find("Minotaur(Clone)").GetComponent<AbilityMinotaurWall>();
        ABR = GameObject.Find("Minotaur(Clone)").GetComponent<AbilityBullrush>();
        Ball = GameObject.Find("Ball").GetComponent<BallProperties>();
    }

    private void CurrentObjective()
    {
        //Run
        if (currentObj == 1)
        {
            //Debug.Log("ON OBJECTIVE #1");
            if (MC.movementDirection != Vector3.zero && GM.isPlaying)
            {
                currentObj++;
                TUI.SetActiveObjective(currentObj);
            }
        }

        //Grab the ball
        if (currentObj == 2)
        {
            //Debug.Log("ON OBJECTIVE #2");
            if (Ball.ballOwner != null)
            {
                currentObj++;
                TUI.SetActiveObjective(currentObj);
            }
        }

        //Kick the ball
        if (currentObj == 3)
        {
            //Debug.Log(Ball.ballOwner);
            //Debug.Log("ON OBJECTIVE #3");
            if (Ball.lastKicker != null)
            {
                currentObj++;
                TUI.SetActiveObjective(currentObj);

                currentSubset++;
                //TUI.UpdateSupsetHolder();
                TUI.FadeStart();
            }
        }

        //Score a goal
        if (currentObj == 4)
        {
            //Debug.Log("ON OBJECTIVE #4");
            goalsScored = ST.GetWGoals(1) + ST.GetMGoals();
            //Debug.Log("Current Goals" + (goalsScored1));

            if (goalsScored >= 1)
            {
                currentObj++;
                TUI.SetActiveObjective(currentObj);

                currentSubset++;
                //TUI.UpdateSupsetHolder();
                TUI.DelayedFade();
            }
        }

        if (currentObj == 5)
        {
            if (ASA.attacked)
            {
                currentObj++;
                TUI.SetActiveObjective(currentObj);
            }
        }

        if (currentObj == 6)
        {
            if (AMW.usedWall)
            {
                currentObj++;
                TUI.SetActiveObjective(currentObj);
            }
        }

        if (currentObj == 7)
        {
            if (MC.isDashing)
            {
                currentObj++;
                TUI.SetActiveObjective(currentObj);
            }
        }

        //End Tutorial
        if (currentObj == 8 && !endTutorial)
        {
            Debug.Log("END");
            endTutorial = true;

            //Run Ending Sequence
            TUI.DelayedFade();
        }
    }
}
