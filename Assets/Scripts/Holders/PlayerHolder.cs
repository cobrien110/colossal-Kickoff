using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;

public class PlayerHolder : MonoBehaviour
{
    public int playerID = -1;
    public List<int> availableIDs = new List<int> { 0, 1, 2, 3 };

    public string teamName = "";
    public int monsterIndex = 0;
    public Gamepad thisGamepad;
    public string gamepadName;

    // The loaded playerProfile the player has selected.
    // If null, uses default values.
    public PlayerProfile playerProfile = null;

    /*
     * 0: Default. Right-Stick inverted to aim and release to kick.
     * 1: Right-Stick to aim. Hold Right-Trigger to charge and release to kick.
     */
    public int controlScheme;

    public string profileName;

    public float deadzoneSensitivity;

    public Color warriorColor;
    public Color skinColor;

    // Which of the three warriors was selected on the Main Menu. Keeping it separate from PlayerNum cause that whole thing is a mess.
    public int warriorPosition = -1;

    public string gameplaySceneName = null;

    public InputActionAsset InputMaster;
    public GameObject sceneES;
    public GameObject thisES;

    public WarriorDesc WD = null;

    private MonsterName monsterName;

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        sceneES = GameObject.Find("EventSystem");
        thisES = gameObject.GetComponentInChildren<EventSystem>().gameObject;
        //playerID = GameObject.FindGameObjectsWithTag("PlayerHolder").Length - 1;
        SetPlayerID();

        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            if (Gamepad.all[i].startButton.wasPressedThisFrame)
            {
                thisGamepad = Gamepad.all[i];
                gamepadName = thisGamepad.name;
            }
        }

        if (SceneManager.GetActiveScene().name.Equals("MainMenus"))
        {
            if (GameObject.Find("CharacterSelect") != null && GameObject.Find("ShowWhenReady") == null)
            {
                GameObject.Find("CursorHolder").GetComponent<CursorHolder>().spawnCursor(playerID, thisGamepad);
            } else
            {
                Destroy(this.gameObject);
            }
        }
        else if (SceneManager.GetActiveScene().name.Equals("WarriorTutorial"))
        {
            thisGamepad = Gamepad.all[0];
            gamepadName = thisGamepad.name;
            teamName = "Warrior";
            GameObject.Find("WarriorHolder").GetComponent<WarriorHolder>().spawnWarrior(playerID, warriorPosition, thisGamepad, warriorColor, skinColor);
        }
        else
        {
            if (thisES != null)
            {
                thisES.SetActive(false);
            }
            if (playerID == 0)
            {
                teamName = "Monster";
                GameObject.Find("MonsterHolder").GetComponent<MonsterHolder>().spawnMonster(playerID, thisGamepad);
            } else
            {
                teamName = "Warrior";
                GameObject.Find("WarriorHolder").GetComponent<WarriorHolder>().spawnWarrior(playerID, warriorPosition, thisGamepad, warriorColor, skinColor);

            }
        }

        monsterName = FindObjectOfType<MonsterName>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (sceneES != null)
        {
            sceneES.SetActive(false);
            sceneES.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (monsterName != null) monsterIndex = monsterName.monsterIndex;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Equals("MainMenus"))
        {
            //teamName = "";
            //monsterIndex = -1;
            //GameObject.Find("CursorHolder").GetComponent<CursorHolder>().spawnCursor(playerID, thisGamepad);
            GameObject.Find("MenuController").GetComponent<MenuController>().skipMain = true;
            Destroy(this.gameObject);
        }
        else
        {
            gameplaySceneName = scene.name;
            if (thisES != null)
            {
                thisES.SetActive(false);
            }
            if (teamName.Equals("Monster"))
            {
                MonsterHolder MH = GameObject.Find("MonsterHolder").GetComponent<MonsterHolder>();
                MH.monsterIndex = monsterIndex;
                MH.spawnMonster(playerID, thisGamepad);
            }
            else
            {
                GameObject.Find("WarriorHolder").GetComponent<WarriorHolder>().spawnWarrior(playerID, warriorPosition, thisGamepad, warriorColor, skinColor);

            }
        }
    }

    void SetPlayerID()
    {
        GameObject[] playerHolders = GameObject.FindGameObjectsWithTag("PlayerHolder");
        if (playerHolders.Length == 1)
        {
            playerID = 0;
        } else
        {
            for (int i = 0; i < playerHolders.Length; i++)
            {
                PlayerHolder PH = playerHolders[i].GetComponent<PlayerHolder>();
                if (playerHolders[i] != this.gameObject)
                {
                    availableIDs.Remove(PH.playerID);
                }
            }

            playerID = availableIDs[0];
        }
    }

    public void SetEvents(GameObject selectedObject)
    {
        gameObject.GetComponentInChildren<MultiplayerEventSystem>().SetSelectedGameObject(selectedObject.gameObject);

        //EventSystem.current = GetComponentInChildren<MultiplayerEventSystem>();
    }

    public void RemoveEvents()
    {
        Debug.Log("REMOVE EVENTS PLAYER HOLDER: " + playerID);
        GetComponentInChildren<MultiplayerEventSystem>().playerRoot = null;
        GetComponentInChildren<MultiplayerEventSystem>().SetSelectedGameObject(null);
    }

    public void LoadProfile(PlayerProfile profile)
    {
        Debug.Log("Loading Profile: " + profile.Profile_Name + ", for PlayerHolder: " + playerID);
        playerProfile = profile;

        // Setting Control Scheme
        controlScheme = profile.Kick_Mode;

        // Setting Jersey Color
        ColorUtility.TryParseHtmlString(profile.Shirt_Color, out warriorColor);

        // Setting Deadzone
        deadzoneSensitivity = profile.Deadzone;

        if (warriorPosition == 1)
        {
            WD = GameObject.Find("Warrior1Color").GetComponent<WarriorDesc>();
        } else if (warriorPosition == 2)
        {
            WD = GameObject.Find("Warrior2Color").GetComponent<WarriorDesc>();
        }
        else if (warriorPosition == 3)
        {
            WD = GameObject.Find("Warrior3Color").GetComponent<WarriorDesc>();
        }
        else
        {
            WD = null;
        }

            //WD = thisES.GetComponent<EventSystem>().currentSelectedGameObject.transform.parent.GetComponent<WarriorDesc>();
        if (WD != null)
        {
            WD.SetColors(warriorColor);
        }

        // Setting Skin Color
        ColorUtility.TryParseHtmlString(profile.Skin_Color, out skinColor);
        if (WD != null)
        {
            WD.SetSkinColors(skinColor);
        }
    }

    public void DefaultProfile()
    {
        // Loading Profile
        Debug.Log("Loading Profile: Default, for PlayerHolder: " + playerID);
        playerProfile = null;

        // Setting Control Scheme
        controlScheme = 1;

        // Setting Jersey Color
        warriorColor = Color.red;

        // Setting Skin Color
        skinColor = Color.white;

        // Setting Deadzone
        deadzoneSensitivity = 0.3f;

        if (WD != null)
        {
            WD.SetColors(warriorColor);
        }
            
        if (WD != null)
        {
            WD.SetSkinColors(skinColor);
        }
    }
}
