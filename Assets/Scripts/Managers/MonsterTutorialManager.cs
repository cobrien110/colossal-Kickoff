using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    [SerializeField] private TutorialUIHelper TUI;
    [SerializeField] private StatTracker ST;
    [SerializeField] private GameObject PHPrefab;

    [SerializeField] private MonsterController MC;
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
        TUI = GameObject.Find("TutorialUIHelper").GetComponent<TutorialUIHelper>();
        ST = GameObject.Find("Stat Tracker").GetComponent<StatTracker>();
        Debug.Log("WARRIOR TUTORIAL INITIALIZED");
        initialized = true;
        // All Tutorial scripting can occur after this point.

        Instantiate(PHPrefab);
        MC = GameObject.Find("Minotaur(Clone)").GetComponent<MonsterController>();
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
    }
}
