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

    public Color warriorColor;

    // Which of the three warriors was selected on the Main Menu. Keeping it separate from PlayerNum cause that whole thing is a mess.
    public int warriorPosition = -1;

    public string gameplaySceneName = null;

    public InputActionAsset InputMaster;
    public GameObject sceneES;
    public GameObject thisES;

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
            if (GameObject.Find("CharacterSelect") != null)
            {
                GameObject.Find("CursorHolder").GetComponent<CursorHolder>().spawnCursor(playerID, thisGamepad);
            } else
            {
                Destroy(this.gameObject);
            }
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
                GameObject.Find("WarriorHolder").GetComponent<WarriorHolder>().spawnWarrior(playerID, warriorPosition, thisGamepad, warriorColor);

            }
        }
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
                GameObject.Find("WarriorHolder").GetComponent<WarriorHolder>().spawnWarrior(playerID, warriorPosition, thisGamepad, warriorColor);

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

    public void SetEvents(WarriorDesc WD)
    {
        GetComponentInChildren<MultiplayerEventSystem>().playerRoot = WD.getRedSlider().gameObject.transform.parent.gameObject;
        GetComponentInChildren<MultiplayerEventSystem>().SetSelectedGameObject(WD.getRedSlider().gameObject);
    }

    public void RemoveEvents()
    {
        GetComponentInChildren<MultiplayerEventSystem>().playerRoot = null;
        GetComponentInChildren<MultiplayerEventSystem>().SetSelectedGameObject(null);
    }
}
