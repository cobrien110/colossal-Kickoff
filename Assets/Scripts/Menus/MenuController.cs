using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    //selected is updated by buttons when they become selected
    //public int selected;
    //camera that this can move around
    [SerializeField] private MenuCamera menuCamera;
    //parent object containing all buttons from the main menu
    [SerializeField] private GameObject mainMenuButtons;
    [SerializeField] private GameObject settingsButtons;
    [SerializeField] private GameObject characterSelect;
    [SerializeField] private GameObject stageSelect;
    [SerializeField] private GameObject[] cursors;
    [SerializeField] private GameObject[] playerOptions;
    [SerializeField] private CharacterInfo[] characterInfos;
    [SerializeField] private GameObject readyText;
    [SerializeField] private GoreButton[] goreButtons;
    private List<CharacterInfo> confirmedInfos = new List<CharacterInfo>();
    private SceneManager SM;
    public int currentScreen = 0;
    /**
    0: Top Menu
    1: Settings
    2: Character Select
    3: Stage Select
    **/
    private bool monsterConfirmed = false;
    public bool canMoveToStageSelect = false;

    void Update()
    {
        //if (Input.GetMouseButtonDown(0)) {
        //    switch (selected) {
        //        //VERSUS MATCH
        //        case 0:
        //        menuCamera.goToVersusSetup();
        //        mainMenuButtons.SetActive(false);
        //        characterSelect.SetActive(true);
        //        break;

        //        //SETTINGS
        //        case 1:
        //        menuCamera.goToSettings();
        //        mainMenuButtons.SetActive(false);
        //        break;

        //        //QUIT GAME
        //        case 2:
        //        Debug.Log("Quitting game. Goodbye!");
        //        Application.Quit();
        //        break;

        //        default:
        //        Debug.Log("Error: unknown menu option");
        //        break;
        //    }
        //}

    }

    public void loadGameplay(int targetScene) {
        for (int i = 0; i < cursors.Length; i++)
            {
                string currentPlayer = "Player" + i;
                MenuCursor currentCursor = cursors[i].GetComponent<MenuCursor>();
                PlayerPrefs.SetInt(currentPlayer, currentCursor.playerSlot);
            }
            switch (targetScene) {
                case 0:
                    //Greece
                    SceneManager.LoadScene("GreeceArena");
                    break;
                case 1:
                    //Canada
                    SceneManager.LoadScene("CanadaArena");
                    break;
                case 2:
                    //Japan
                    SceneManager.LoadScene("JapanArena");
                    break;
                case 3:
                    //Mexico
                    SceneManager.LoadScene("MexicoStage");
                    break;
                case 4:
                    //Egypt
                    SceneManager.LoadScene("EgyptArena");
                    break;
                default:
                   //Scene that hasn't been made yet
                    SceneManager.LoadScene("GreeceArena");
                    break; 
            }
    }

    public void OptionSelect(int optionID)
    {
        findAllCursors();
        switch (optionID)
        {
            //VERSUS MATCH
            case 0:
                currentScreen = 2;
                menuCamera.goToVersusSetup();
                mainMenuButtons.SetActive(false);
                characterSelect.SetActive(true);
                for (int i = 0; i < cursors.Length; i++) {
                    cursors[i].GetComponent<MenuCursor>().findCharSelectItems();
                }
                break;

            //SETTINGS
            case 1:
                currentScreen = 1;
                menuCamera.goToSettings();
                settingsButtons.SetActive(true);
                mainMenuButtons.SetActive(false);
                break;

            //QUIT GAME
            case 2:
                Debug.Log("Quitting game. Goodbye!");
                Application.Quit();
                break;

            default:
                Debug.Log("Error: unknown menu option");
                break;
        }
    }

    public void returnToTop() {
        menuCamera.goToMainMenu();
        currentScreen = 0;
        for (int i = 0; i < cursors.Length; i++)
        {
            cursors[i].GetComponent<MenuCursor>().deselect();
        }
        for (int i = 0; i < characterInfos.Length; i++) {
            unconfirmCharacter(i);
        }
        mainMenuButtons.SetActive(true);
        characterSelect.SetActive(false);
        settingsButtons.SetActive(false);
    }

    public void findAllCursors() {
        if (canMoveToStageSelect) {
            canMoveToStageSelect = false;
            readyText.SetActive(false);
        }
        cursors = GameObject.FindGameObjectsWithTag("MenuCursor");
    }

    public void characterSelected(int playerNumber, int playerSlot) {
        Debug.Log("Player " + playerNumber + " selected Character " + playerSlot);
        //todo: set it so playerNumber can control playerSlot's character options
        playerOptions[playerSlot].SetActive(true);
    }

    public void characterUnselected(int playerNumber, int playerSlot) {
        Debug.Log("Player " + playerNumber + " unselected Character " + playerSlot);
        //todo: reverse that thing from the last comment
        playerOptions[playerSlot].SetActive(false);
        unconfirmCharacter(playerSlot);
    }

    public void confirmCharacter(int playerSlot) {
        confirmedInfos.Add(characterInfos[playerSlot]);
        characterInfos[playerSlot].confirm();
        if (playerSlot == 0) {
            monsterConfirmed = true;
            canMoveToStageSelect = true;
            readyText.SetActive(true);
        }
    }

    public void unconfirmCharacter(int playerSlot) {
        confirmedInfos.Remove(characterInfos[playerSlot]);
        characterInfos[playerSlot].unconfirm();
        if (playerSlot == 0 && canMoveToStageSelect) {
            canMoveToStageSelect = false;
            readyText.SetActive(false);
        }
    }

    public void moveToStageSelect() {
        currentScreen = 3;
        //findAllCursors();
        for (int i = 0; i < cursors.Length; i++) {
            if (cursors[i].GetComponent<MenuCursor>().playerNumber != 1) {
                cursors[i].SetActive(false);
            }
        }
        characterSelect.SetActive(false);
        stageSelect.SetActive(true);
    }

    public void backToCharSelect() {
        currentScreen = 2;
        findAllCursors();
        for (int i = 0; i < cursors.Length; i++) {
            cursors[i].SetActive(true);
        }
        Debug.Log("going back to character select");
        characterSelect.SetActive(true);
        stageSelect.SetActive(false);
    }

    public void setGore(int value) {
        PlayerPrefs.SetInt("goreMode", value);
        for (int i = 0; i < goreButtons.Length; i++) {
            if (goreButtons[i].goreButtonID == value) {
                goreButtons[i].selectOption();
            } else {
                goreButtons[i].unselectOption();
            }
        }
    }
}
