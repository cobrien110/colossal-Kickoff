using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class MenuController : MonoBehaviour
{
    #region Variables
    //selected is updated by buttons when they become selected
    //public int selected;
    //camera that this can move around
    [SerializeField] private AsyncLoadManager ALM;
    [SerializeField] private MenuCamera menuCamera;
    [SerializeField] private EarthController EC;
    [SerializeField] private CreditsScrollingUI CSU; 

    //parent object containing all buttons from the main menu
    [SerializeField] private GameObject mainMenuButtons, quitGameButtons, creditsContent;

    [SerializeField] private TMP_Text settingsHeader;
    [SerializeField] private GameObject settingsButtons;
    [SerializeField] private GameObject gameplaySettings;
    [SerializeField] private GameObject audioSettings;
    [SerializeField] private GameObject controlSettings;

    [SerializeField] private GameObject characterSelect;
    [SerializeField] private GameObject stageSelect;
    [SerializeField] private GameObject sceneEventSystem;
    [SerializeField] private GameObject splashScreen;

    //Stage Images
    [SerializeField] private Image stageFade;
    [SerializeField] private GameObject allImages;
    //[SerializeField] private GameObject greeceImage;
    //[SerializeField] private GameObject canadaImage;
    //[SerializeField] private GameObject egyptImage;
    //[SerializeField] private GameObject mexicoImage;
    //[SerializeField] private GameObject japanImage;

    [SerializeField] private GameObject[] stageImages;
    [SerializeField] private Button[] stageButtons;
    [SerializeField] private GameObject stageSettings;
    [SerializeField] private Button stageSettingsButton;
    private Button lastStageButton;
    Navigation lastStageNavi = new Navigation();

    [SerializeField] private GameObject[] cursors;
    [SerializeField] private GameObject[] playerOptions;
    [SerializeField] private CharacterInfo[] characterInfos;
    [SerializeField] private GameObject readyText;
    [SerializeField] private GameObject hideWhenReady;
    [SerializeField] private GameObject showWhenReady;
    private List<CharacterInfo> confirmedInfos = new List<CharacterInfo>();
    private SceneManager SM;
    /**
    0: Top Menu
    1: Settings
    2: Character Select
    3: Stage Select
    4: Quit Screen
    **/
    public int currentScreen = 0;
    int effectsVolume, musicVolume, commentaryVolume, commentaryFrequency;
    //MENU INTERFACES
    [SerializeField] private Slider effectsSlider, musicSlider, comVolumeSlider, comFreqSlider;
    [SerializeField] private TMP_Dropdown goreDropdown;
    [SerializeField] private TMP_Dropdown goalDropdown;
    [SerializeField] private TMP_Dropdown overtimeDropdown;
    [SerializeField] private TMP_Dropdown kickchargeDropdown;
    [SerializeField] private Toggle screenshakeToggle;
    [SerializeField] private GameObject topFirstButton, settingsFirstButton, stageFirstButton, quitFirstButton, stageSettingsFirstButton, creditsBackButton;
    [SerializeField] private Button settingsControlsButton, settingsAudioButton, settingsGameplayButton, settingsBackButton;
    [SerializeField] private TMP_Text FXVolNum, MusicVolNum, CommVolNum, CommFreqNum;
    Navigation backNavi = new Navigation();
    Navigation controlsNavi = new Navigation();
    Navigation audioNavi = new Navigation();
    Navigation gameplayNavi = new Navigation();

    private int numPlayersConfirmed = 0;
    public bool canMoveToGame = false;
    public bool deselectOccured = false;

    //tracks the stage the game will move to when it starts
    public int stageSelection;

    //sound
    AudioPlayer AP;

    //connected player images
    [SerializeField] private GameObject p1Connected;
    [SerializeField] private GameObject p2Connected;
    [SerializeField] private GameObject p3Connected;
    [SerializeField] private GameObject p4Connected;
    #endregion

    #region Initialization

    void Start() {
        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            InputSystem.EnableDevice(Gamepad.all[i]);
        }

        GameObject currentES = EventSystem.current.gameObject;
        currentES.SetActive(false);
        currentES.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(topFirstButton);
        AP = GetComponent<AudioPlayer>();

        //Settings Navigation
        backNavi = settingsBackButton.navigation;
        backNavi.mode = Navigation.Mode.Explicit;
        controlsNavi = settingsControlsButton.navigation;
        controlsNavi.mode = Navigation.Mode.Explicit;
        audioNavi = settingsAudioButton.navigation;
        audioNavi.mode = Navigation.Mode.Explicit;
        gameplayNavi = settingsGameplayButton.navigation;
        gameplayNavi.mode = Navigation.Mode.Explicit;

        //Match Settings Navigation
        lastStageNavi = settingsBackButton.navigation;
        lastStageNavi.mode = Navigation.Mode.Explicit;

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

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
        Gamepad gamepad = Gamepad.current;
        if (splashScreen.activeInHierarchy && gamepad != null)
        {
            if (gamepad.buttonSouth.wasPressedThisFrame)
            {
                splashScreen.SetActive(false);
                mainMenuButtons.SetActive(true);
                if (AP != null) AP.PlaySoundRandomPitch(AP.Find("menuClick2"));
            }
        }

        if (gamepad != null)
        {
            if (gamepad.buttonEast.wasPressedThisFrame && !splashScreen.activeInHierarchy)
            {
                switch (currentScreen)
                {
                    case (1):
                        returnToTop();
                        break;
                    case (2):
                        if (!deselectOccured)
                        {
                            backToStageSelect();
                        }
                        break;
                    case (3):
                        if (stageSettings.activeSelf)
                        {
                            ShowStageSettings(false);
                        }
                        else
                        {
                            returnToTop();
                        }
                        break;
                    case (4):
                        returnToTop();
                        break;
                    case (5):
                        returnToTop();
                        break;
                    default:
                        break;
                }
                deselectOccured = false;
            }
        }
    }

    #endregion

    #region SceneLoading

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
                    ALM.BeginLoad("GreeceArena");
                    break;
                case 1:
                    //Canada
                    ALM.BeginLoad("CanadaArena");
                    break;
                case 2:
                    //Japan
                    ALM.BeginLoad("JapanArena");
                    break;
                case 3:
                    //Mexico
                    ALM.BeginLoad("MexicoStage");
                    break;
                case 4:
                    //Egypt
                    ALM.BeginLoad("EgyptArena");
                    break;
                default:
                   //Scene that hasn't been made yet
                    ALM.BeginLoad("GreeceArena");
                    break; 
            }
    }

    #endregion

    #region Menu Navigation

    public void OptionSelect(int optionID)
    {
        //findAllCursors();
        switch (optionID)
        {
            //VERSUS MATCH
            case 0:
                currentScreen = 3;
                menuCamera.goToVersusSetup();
                stageSelect.SetActive(true);
                mainMenuButtons.SetActive(false);
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(stageFirstButton);
                goalDropdown.value = PlayerPrefs.GetInt("goalBarriers", 0);
                //sound
                if (AP != null) AP.PlaySoundRandomPitch(AP.Find("menuClick2"));
                break;

            //SETTINGS
            case 1:
                currentScreen = 1;
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(settingsFirstButton);
                menuCamera.goToSettings();
                
                // Swapping Between Settings Here?
                settingsButtons.SetActive(true);

                mainMenuButtons.SetActive(false);
                goreDropdown.value = PlayerPrefs.GetInt("goreMode", 0);
                musicSlider.value = PlayerPrefs.GetFloat("musicVolume", 100) * 100;
                effectsSlider.value = PlayerPrefs.GetFloat("effectsVolume", 100) * 100;
                comVolumeSlider.value = PlayerPrefs.GetFloat("commentaryVolume", 100) * 100;
                comFreqSlider.value = PlayerPrefs.GetFloat("commentaryFrequency", 100) * 100;
                screenshakeToggle.isOn = (PlayerPrefs.GetInt("screenshake", 1) != 0);

                //sound
                if (AP != null) AP.PlaySoundRandomPitch(AP.Find("menuClick2"));
                break;

            //QUIT GAME
            case 2:
                currentScreen = 4;
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(quitFirstButton);
                menuCamera.goToQuitting();
                quitGameButtons.SetActive(true);
                mainMenuButtons.SetActive(false);
                //sound
                if (AP != null) AP.PlaySoundRandomPitch(AP.Find("menuClick2"));
                break;
                
            //CREDITS
            case 3:
                currentScreen = 5;
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(creditsBackButton);
                menuCamera.goToCredits();
                CSU.CreditsStart();
                //creditsContent.SetActive(true);
                mainMenuButtons.SetActive(false);
                //sound
                if (AP != null) AP.PlaySoundRandomPitch(AP.Find("menuClick2"));
                break;

            default:
                Debug.Log("Error: unknown menu option");
                break;
        }
    }

    public void returnToTop()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(topFirstButton);
        menuCamera.goToTitle();
        currentScreen = 0;
        mainMenuButtons.SetActive(true);
        stageSelect.SetActive(false);
        quitGameButtons.SetActive(false);
        settingsButtons.SetActive(false);
        creditsContent.SetActive(false);
        CSU.CreditsEnd();

        //sound
        if (AP != null) AP.PlaySoundRandomPitch(AP.Find("menuClick2"));
    }

    public void moveToStageSelect()
    {
        currentScreen = 3;
        //findAllCursors();
        /**for (int i = 0; i < cursors.Length; i++) {
            if (cursors[i].GetComponent<MenuCursor>().playerNumber != 1) {
                cursors[i].SetActive(false);
            }
        }**/
        mainMenuButtons.SetActive(false);
        characterSelect.SetActive(false);
        stageSelect.SetActive(true);

        //sound
        if (AP != null) AP.PlaySoundRandomPitch(AP.Find("menuClick2"));
    }

    /**public void backToCharSelect() {
        currentScreen = 2;
        findAllCursors();
        for (int i = 0; i < cursors.Length; i++) {
            cursors[i].SetActive(true);
        }
        Debug.Log("going back to character select");
        characterSelect.SetActive(true);
        stageSelect.SetActive(false);
    }**/

    public void backToStageSelect()
    {
        sceneEventSystem.GetComponent<EventSystem>().enabled = true;

        currentScreen = 3;
        /**findAllCursors();
        for (int i = 0; i < cursors.Length; i++) {
            cursors[i].SetActive(true);
        }**/
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(lastStageButton.gameObject);
        findAllCursors();
        for (int i = 0; i < cursors.Length; i++)
        {
            MenuCursor currentCursor = cursors[i].GetComponent<MenuCursor>();
            currentCursor.hideCursor();
            currentCursor.charConfirmed = false;
            currentCursor.deselect();
            currentCursor.Leave();
        }
        Debug.Log("going back to stage select");
        characterSelect.SetActive(false);

        GameObject[] playerHolders = GameObject.FindGameObjectsWithTag("PlayerHolder");
        for (int i = 0; i < playerHolders.Length; i++)
        {
            Destroy(playerHolders[i].gameObject);
        }

        stageSelect.SetActive(true);

        //sound
        if (AP != null) AP.PlaySoundRandomPitch(AP.Find("menuClose"));
    }

    //Change this to flip between screens with Dpad at some point
    public void SettingsSwap(int type)
    {
        //Add controller menu nav

        //if (settingsBackButton != null && settingsHeaderButton != null)

        //backNavi = settingsBackButton.navigation;
        //headerNavi = settingsHeaderButton.navigation;
        //Navigation navigation = new Navigation();

        //weird bug where if this isnt present, header on select up gets set to toggle screen shake???
        controlsNavi.selectOnUp = settingsBackButton;
        audioNavi.selectOnUp = settingsBackButton;
        gameplayNavi.selectOnUp = settingsBackButton;
        backNavi.selectOnDown = settingsGameplayButton;

        //Audio
        if (type == 0)
        {
            gameplaySettings.SetActive(false);
            controlSettings.SetActive(false);
            audioSettings.SetActive(true);
            //settingsHeader.text = "AUDIO";

            controlsNavi.selectOnDown = effectsSlider;
            settingsControlsButton.navigation = controlsNavi;
            audioNavi.selectOnDown = effectsSlider;
            settingsAudioButton.navigation = audioNavi;
            gameplayNavi.selectOnDown = effectsSlider;
            settingsGameplayButton.navigation = gameplayNavi;

            backNavi.selectOnUp = comFreqSlider;
            backNavi.selectOnDown = settingsAudioButton;
            settingsBackButton.navigation = backNavi;
        }
        //Controls
        else if (type == 1)
        {
            audioSettings.SetActive(false);
            gameplaySettings.SetActive(false);
            controlSettings.SetActive(true);
            //settingsHeader.text = "CONTROLS";

            controlsNavi.selectOnDown = settingsBackButton;
            settingsControlsButton.navigation = controlsNavi;
            audioNavi.selectOnDown = settingsBackButton;
            settingsAudioButton.navigation = audioNavi;
            gameplayNavi.selectOnDown = settingsBackButton;
            settingsGameplayButton.navigation = gameplayNavi;

            backNavi.selectOnUp = settingsControlsButton;
            backNavi.selectOnDown = settingsControlsButton;
            settingsBackButton.navigation = backNavi;
        }
        //Gameplay
        else if (type == 2)
        {
            controlSettings.SetActive(false);
            audioSettings.SetActive(false);
            gameplaySettings.SetActive(true);
            //settingsHeader.text = "GAMEPLAY";

            controlsNavi.selectOnDown = goreDropdown;
            settingsControlsButton.navigation = controlsNavi;
            audioNavi.selectOnDown = goreDropdown;
            settingsAudioButton.navigation = audioNavi;
            gameplayNavi.selectOnDown = goreDropdown;
            settingsGameplayButton.navigation = gameplayNavi;

            backNavi.selectOnUp = screenshakeToggle;
            backNavi.selectOnDown = settingsGameplayButton;
            settingsBackButton.navigation = backNavi;
        }

    }

    public void exitGame()
    {
        Debug.Log("Quitting game. Goodbye!");
        Application.Quit();
    }

    #endregion

    #region Character Selection

    public void characterSelected(int playerNumber, int playerSlot)
    {
        Debug.Log("Player " + playerNumber + " selected Character " + playerSlot);
        //todo: set it so playerNumber can control playerSlot's character options
        playerOptions[playerSlot].SetActive(true);
    }

    public void characterUnselected(int playerNumber, int playerSlot)
    {
        Debug.Log("Player " + playerNumber + " unselected Character " + playerSlot);
        //todo: reverse that thing from the last comment
        playerOptions[playerSlot].SetActive(false);
        //if (confirm)
        if (confirmedInfos.Contains(characterInfos[playerSlot]))
        {
            unconfirmCharacter(playerSlot);
        }
    }

    public void confirmCharacter(int playerSlot)
    {
        confirmedInfos.Add(characterInfos[playerSlot]);
        characterInfos[playerSlot].confirm();
        numPlayersConfirmed++;
        Debug.Log("players confirmed: " + numPlayersConfirmed + " - Time: " + Time.fixedTime);
        Debug.Log("players needed: " + cursors.Length);
        if (numPlayersConfirmed == cursors.Length)
        {
            canMoveToGame = true;
            readyText.SetActive(true);
            hideWhenReady.SetActive(false);
            showWhenReady.SetActive(true);
        }
    }

    public void unconfirmCharacter(int playerSlot)
    {
        confirmedInfos.Remove(characterInfos[playerSlot]);
        characterInfos[playerSlot].unconfirm();
        numPlayersConfirmed--;
        Debug.Log("players confirmed: " + numPlayersConfirmed + "Time: " + Time.fixedTime);
        if (canMoveToGame)
        {
            canMoveToGame = false;
            readyText.SetActive(false);
            hideWhenReady.SetActive(true);
            showWhenReady.SetActive(false);
        }
    }

    #endregion

    #region Player Cursors

    public void findAllCursors()
    {
        cursors = GameObject.FindGameObjectsWithTag("MenuCursor");
    }

    public void newCursor()
    {
        findAllCursors();
        canMoveToGame = false;
        readyText.SetActive(false);
    }

    #endregion

    #region Settings Management

    //Gameplay Settings
    public void setGore()
    {
        PlayerPrefs.SetInt("goreMode", goreDropdown.value);

        //sound
        if (AP != null) AP.PlaySoundRandomPitch(AP.Find("menuClick"));
    }

    public void setScreenshake()
    {
        if (screenshakeToggle.isOn)
        {
            PlayerPrefs.SetInt("screenshake", 1);
        }
        else
        {
            PlayerPrefs.SetInt("screenshake", 0);
        }

        //sound
        if (AP != null) AP.PlaySoundRandomPitch(AP.Find("menuClick"));
    }

    //Audio Settings
    public void setMusicVolume()
    {
        PlayerPrefs.SetFloat("musicVolume", musicSlider.value / 100f);
        MusicVolNum.text = Mathf.Round(musicSlider.value).ToString();
    }

    public void setEffectsVolume()
    {
        PlayerPrefs.SetFloat("effectsVolume", effectsSlider.value / 100f);
        FXVolNum.text = Mathf.Round(effectsSlider.value).ToString();

        //sound
        if (AP != null && !AP.isPlaying()) AP.PlaySoundRandomPitch(AP.Find("menuClick"));
    }

    public void setCommentaryVolume()
    {
        PlayerPrefs.SetFloat("commentaryVolume", comVolumeSlider.value / 100f);
        CommVolNum.text = Mathf.Round(comVolumeSlider.value).ToString();

        //sound
        if (AP != null && !AP.isPlaying())
        {
            AP.setUseComVol(true);
            AP.PlaySound(AP.Find("a_death_warrior3"));
        }
    }

    public void setCommentaryFrequency()
    {
        PlayerPrefs.SetFloat("commentaryFrequency", comFreqSlider.value / 100f);
        CommFreqNum.text = Mathf.Round(comFreqSlider.value).ToString();

        //sound
        if (AP != null && !AP.isPlaying()) AP.PlaySoundRandomPitch(AP.Find("menuClick"));
    }

    //Match Settings
    public void setGoalBarriers()
    {
        PlayerPrefs.SetInt("goalBarriers", goalDropdown.value);

        //sound
        if (AP != null) AP.PlaySoundRandomPitch(AP.Find("menuClick"));
    }

    public void SetOvertime()
    {
        PlayerPrefs.SetInt("overtime", overtimeDropdown.value);
        //sound
        if (AP != null) AP.PlaySoundRandomPitch(AP.Find("menuClick"));
    }

    public void SetKickCharge()
    {
        PlayerPrefs.SetInt("kickcharge", kickchargeDropdown.value);
        //sound
        if (AP != null) AP.PlaySoundRandomPitch(AP.Find("menuClick"));
    }
    #endregion

    #region Controller Connection

    public void showConnected(int playerNumber)
    {
        switch (playerNumber)
        {
            case 1:
                p1Connected.SetActive(true);
                break;
            case 2:
                p2Connected.SetActive(true);
                break;
            case 3:
                p3Connected.SetActive(true);
                break;
            case 4:
                p4Connected.SetActive(true);
                break;
            default:
                Debug.Log("Player Num Error");
                break;
        }
    }

    public void hideConnected(int playerNumber)
    {
        switch (playerNumber)
        {
            case 1:
                p1Connected.SetActive(false);
                break;
            case 2:
                p2Connected.SetActive(false);
                break;
            case 3:
                p3Connected.SetActive(false);
                break;
            case 4:
                p4Connected.SetActive(false);
                break;
            default:
                Debug.Log("Player Num Error");
                break;
        }
    }

    #endregion

    #region Stage Selection

    public void selectStage(int stageID) {
        stageSelection = stageID;
        currentScreen = 2;
        stageSelect.SetActive(false);
        characterSelect.SetActive(true);
        findAllCursors();
        for (int i = 0; i < cursors.Length; i++) {
            cursors[i].GetComponent<MenuCursor>().findCharSelectItems();
            cursors[i].GetComponent<MenuCursor>().showCursor();
        }

        //sceneEventSystem.SetActive(false);
        sceneEventSystem.GetComponent<EventSystem>().enabled = false;

        //set these just in case
        numPlayersConfirmed = 0;
        canMoveToGame = false;

        //sound
        if (AP != null) AP.PlaySoundRandomPitch(AP.Find("menuOpen"));
    }

    //Stage Image Handling
    public void StageImageSwap(int stageID)
    {
        Debug.Log("Swapped");
        //stageFade.CrossFadeAlpha(1, 1.0f, false);
        //stageFade.CrossFadeAlpha(0, 1.0f, false);
        for (int i = 0; i < stageImages.Length; i++)
        {
            if (i == stageID)
            {
                stageImages[i].SetActive(true);
            }
            else
            {
                stageImages[i].SetActive(false);
            }

        }
    }

    public void SetLastStageButton(int stageID)
    {
        lastStageButton = stageButtons[stageID];
        lastStageNavi.selectOnLeft = lastStageButton;
        lastStageNavi.selectOnRight = lastStageButton;
        stageSettingsButton.navigation = lastStageNavi;
    }

    public void ShowStageSettings(bool state)
    {
        if (state)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(stageSettingsFirstButton);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(lastStageButton.gameObject);
        }
        stageSettings.SetActive(state);
        stageSelect.SetActive(!state);
    }

    //private IEnumerator StageImage(int stageID)
    //{
    //    stageFade.CrossFadeAlpha
    //}

    #endregion

}
