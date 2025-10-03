using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using Steamworks;
using System.IO;

public class MenuController : MonoBehaviour
{
    #region Variables
    //selected is updated by buttons when they become selected
    //public int selected;
    [Header("Managers and Controllers")]
    [SerializeField] private AsyncLoadManager ALM;
    [SerializeField] private MenuCamera menuCamera;
    [SerializeField] private EarthController EC;
    [SerializeField] private CreditsScrollingUI CSU;
    [SerializeField] private TVTextScroll TVT;
    [SerializeField] private ChangeChannel CC;
    [SerializeField] private StageTextBoxCrawl STBC;
    [SerializeField] private StatTracker ST;
    [SerializeField] private PlayerProfileManager PPM;
    private SceneManager SM;
    private AudioPlayer AP;

    [Header("Main Menu Elements")]
    // Parent object containing all buttons from the main menu
    [SerializeField] private GameObject splashScreen;
    [SerializeField] private Image splashLogo;
    [SerializeField] private TMP_Text splashText;
    [SerializeField] private Image pressA;
    private float startAlpha = 0f;
    private float endAlpha = 1f;
    private Color splashLogoCol = new Color(255, 255, 255);
    private Color pressTextCol = new Color(255, 255, 255);
    private Color pressACol = new Color(255, 255, 255);
    private bool fadeStart = false;

    [SerializeField] private GameObject mainMenuButtons;
    [SerializeField] private GameObject quitGameButtons;
    [SerializeField] private GameObject creditsContent;
    [SerializeField] private GameObject extrasContent;
    [SerializeField] private GameObject sandboxContent;
    [SerializeField] private GameObject tutorialContent;
    [SerializeField] private GameObject statsContent;
    [SerializeField] private GameObject changelogContent;

    [Header("Tutorial Elements")]
    [SerializeField] private Sprite[] tutorialImages;
    [SerializeField] private TMP_Text tutorialHeaderText;
    [SerializeField] private string[] tutorialHeaders;
    [SerializeField] private TMP_Text tutorialBlurbText;
    [SerializeField] private string[] tutorialBlurbs;
    [SerializeField] private Image tutorialSwapImage;
    private int tutorialIndex = 0;
    [SerializeField] private GameObject mainCanvasTutorial;

    [Header("Stats Elements")]
    [SerializeField] private TMP_Text mGoalsText;
    [SerializeField] private TMP_Text mKillsText;
    [SerializeField] private TMP_Text mAbiltiesText;
    [SerializeField] private TMP_Text mWinsText;
    [SerializeField] private TMP_Text wGoalsText;
    //[SerializeField] private TMP_Text wDeathsText;
    [SerializeField] private TMP_Text wAssistsText;
    [SerializeField] private TMP_Text wStealsText;
    [SerializeField] private TMP_Text wWinsText;
    private bool resetThisScene = false;

    [Header("Settings Menu Elements")]
    [SerializeField] private TMP_Text settingsHeader;
    [SerializeField] private GameObject SettingsMenu;
    [SerializeField] private GameObject SettingsButtons;
    [SerializeField] private GameObject gameplaySettings;
    [SerializeField] private GameObject audioSettings;
    [SerializeField] private GameObject controlSettings;
    [SerializeField] private GameObject warriorControls;
    [SerializeField] private GameObject monsterControls;
    [SerializeField] private TMP_Text teamControlsText;
    [SerializeField] private Selectable createNewProfileButton;

    //For building initial NAV
    private bool hasOpenedSettingsOnce = false;

    [Header("Stage Selection")]
    [SerializeField] private GameObject characterSelect;
    [SerializeField] private GameObject stageSelect;
    [SerializeField] private GameObject stageSettings;
    [SerializeField] private Button stageSettingsButton;
    private Button lastStageButton;
    Navigation lastStageNavi = new Navigation();

    [Header("UI Navigation & Buttons")]
    [SerializeField] private GameObject sceneEventSystem;
    [SerializeField] private GameObject readyText;
    [SerializeField] private GameObject hideWhenReady;
    [SerializeField] private GameObject showWhenReady;

    [Header("Stage Images & Buttons")]
    [SerializeField] private Image stageFade;
    [SerializeField] private GameObject allImages;
    [SerializeField] private GameObject[] stageImages;
    [SerializeField] private Button[] stageButtons;

    //[SerializeField] private GameObject greeceImage;
    //[SerializeField] private GameObject canadaImage;
    //[SerializeField] private GameObject egyptImage;
    //[SerializeField] private GameObject mexicoImage;
    //[SerializeField] private GameObject japanImage;

    [Header("Character Selection")]
    [SerializeField] private GameObject[] cursors;
    [SerializeField] private GameObject[] playerOptions;
    [SerializeField] private CharacterInfo[] characterInfos;
    private List<CharacterInfo> confirmedInfos = new List<CharacterInfo>();

    [SerializeField] private WarriorDesc WD1;
    [SerializeField] private WarriorDesc WD2;
    [SerializeField] private WarriorDesc WD3;
    public bool monsterAbilityCanHover = true;

    public TMP_Dropdown warriorDrop1;
    public TMP_Dropdown warriorDrop2;
    public TMP_Dropdown warriorDrop3;
    public TMP_Dropdown monsterDrop;

    [SerializeField] private GameObject[] monsterImages;
    [SerializeField] private MonsterName monsterNameScript;

    //Back button Util
    [SerializeField] private Image backButtonFillImage;
    [SerializeField] private float currentBackButtonFill = 0f;
    [SerializeField] private float maxBackButtonFill = 5f;

    private bool progressionActive = false;
    private Gamepad lockedGamepad = null;

    private Dictionary<int, int> slotToHoveringID = new Dictionary<int, int>();
    [SerializeField] private GameObject[] botElements;
    [SerializeField] private GameObject[] playerElements;

    [Header("Menu Navigation Controls")]
    [SerializeField] private GameObject topFirstButton;
    [SerializeField] private GameObject settingsFirstButton;
    [SerializeField] private GameObject stageFirstButton;
    [SerializeField] private GameObject quitFirstButton;
    [SerializeField] private GameObject stageSettingsFirstButton;
    [SerializeField] private GameObject extrasFirstButton;
    [SerializeField] private GameObject creditsBackButton;
    [SerializeField] private GameObject changelogBackButton;

    [SerializeField] private Button settingsControlsButton;
    [SerializeField] private Button settingsAudioButton;
    [SerializeField] private Button settingsGameplayButton;
    [SerializeField] private Button settingsBackButton;

    public bool skipMain = false;
    private bool canEnterGame = false;

    Navigation backNavi = new Navigation();
    Navigation controlsNavi = new Navigation();
    Navigation audioNavi = new Navigation();
    Navigation gameplayNavi = new Navigation();

    [Header("Audio & Volume Controls")]
    int effectsVolume, musicVolume, commentaryVolume, commentaryFrequency;
    [SerializeField] private Slider effectsSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider comVolumeSlider;
    [SerializeField] private Slider comFreqSlider;
    [SerializeField] private TMP_Text FXVolNum;
    [SerializeField] private TMP_Text MusicVolNum;
    [SerializeField] private TMP_Text CommVolNum;
    [SerializeField] private TMP_Text CommFreqNum;

    [Header("Gameplay Settings")]
    [SerializeField] private TMP_Dropdown goreDropdown;
    [SerializeField] private TMP_Dropdown goalDropdown;
    [SerializeField] private TMP_Dropdown overtimeDropdown;
    [SerializeField] private TMP_Dropdown kickchargeDropdown;
    [SerializeField] private Toggle screenshakeToggle;
    [SerializeField] private Toggle controlsToggle;
    [SerializeField] private Button controlTypeSwap;
    [SerializeField] private Toggle outlineToggle;
    [SerializeField] private Slider deadzoneSlider;
    [SerializeField] private TMP_Text DeadzoneAdjNum;

    [Header("Profile UI Limit Message")]
    [SerializeField] private TMP_Text limitReachedMessageText;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float messageDisplayTime = 2f;
    [SerializeField] private int MaxProfiles = 16;

    private Coroutine limitMessageCoroutine;

    [Header("Profile Delete UI")]
    [SerializeField] private GameObject deleteConfirmationScreen;
    [SerializeField] private GameObject deleteYesButton;
    [SerializeField] private GameObject deleteNoButton;
    [SerializeField] private CanvasGroup profileSelectionScreen;
    private PlayerProfile profileToDelete;

    [Header("Player Profile Settings")]
    [SerializeField] private GameObject playerProfileEditor;
    [SerializeField] private GameObject bindingsMenu;
    [SerializeField] private GameObject configMenu;
    [SerializeField] private GameObject shirtColorMenu;
    [SerializeField] private GameObject skinColorMenu;
    [SerializeField] private GameObject playerProfilesFirstButton;

    [SerializeField] private CanvasGroup playerProfileCanvasGroup;
    [SerializeField] private TMP_Dropdown bindingsFirstButton;
    [SerializeField] private TMP_Dropdown configFirstButton;
    [SerializeField] private Slider shirtFirstButton;
    [SerializeField] private Slider skinFirstButton;

    [SerializeField] private GameObject PPButtonPrefab;
    [SerializeField] private Transform PPButtonParent;
    [SerializeField] private GamepadScrolling GSScript;
    [SerializeField] private GridLayoutGroup playerButtonGrid;
    public List<PlayerProfile> savedProfiles = new List<PlayerProfile>();

    [Header("Player Connection Status")]
    [SerializeField] private GameObject p1Connected;
    [SerializeField] private GameObject p2Connected;
    [SerializeField] private GameObject p3Connected;
    [SerializeField] private GameObject p4Connected;
    [SerializeField] private GameObject p1Disconnected;
    [SerializeField] private GameObject p2Disconnected;
    [SerializeField] private GameObject p3Disconnected;
    [SerializeField] private GameObject p4Disconnected;

    [Header("Menu Navigation Elements")]
    [SerializeField] private Selectable[] settingsTabButtons;       
    [SerializeField] private Selectable[] gameplaySettingsNav;
    [SerializeField] private Selectable[] audioSettingsNav;
    [SerializeField] private Selectable[] controlsSettingsNav;
    //[SerializeField] private Selectable[] playerProfileNav;

    [Header("Game State Tracking")]
    public int currentScreen = 0;
    private int numPlayersConfirmed = 0; 
    public bool canMoveToGame = false;
    public bool deselectOccured = false;
    public bool playerRebinding = false;

    // Tracks the stage the game will move to when it starts
    public int stageSelection;
    #endregion

    #region Initialization

    void Start() {
        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            InputSystem.EnableDevice(Gamepad.all[i]);
        }

        StartCoroutine(FadeWait());
        GameObject currentES = EventSystem.current.gameObject;
        currentES.SetActive(false);
        currentES.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(topFirstButton);
        AP = GetComponent<AudioPlayer>();

        Color splashLogoCol = splashLogo.color;
        Color pressTextCol = splashText.color;
        Color pressACol = pressA.color;


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

        //Player Prefs set on start
        //Settings
        goreDropdown.value = PlayerPrefs.GetInt("goreMode", 0);
        screenshakeToggle.isOn = (PlayerPrefs.GetInt("screenshake", 1) != 0);
        outlineToggle.isOn = (PlayerPrefs.GetInt("ballOutlineMatchesTeam", 1) != 0);
        controlsToggle.isOn = (PlayerPrefs.GetInt("showControls", 1) != 0);
        deadzoneSlider.value = PlayerPrefs.GetFloat("kicksens", .7f) * 10;

        effectsSlider.value = PlayerPrefs.GetFloat("effectsVolume", 100) * 100;
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume", 100) * 100;
        comVolumeSlider.value = PlayerPrefs.GetFloat("commentaryVolume", 100) * 100;
        comFreqSlider.value = PlayerPrefs.GetFloat("commentaryFrequency", 100) * 100;
        
        //Match Settings
        goalDropdown.value = PlayerPrefs.GetInt("goalBarriers", 0);
        overtimeDropdown.value = PlayerPrefs.GetInt("overtime", 0);
        kickchargeDropdown.value = PlayerPrefs.GetInt("kickcharge", 0);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        TVT.WarningStart();

        if (skipMain)
        {
            splashScreen.SetActive(false);
            OptionSelect(0);
            skipMain = false;
        }

        // Steam Achievement Stuff
        if (SteamManager.Initialized)
        {
            Debug.Log("Getting Stats: " + SteamUserStats.RequestCurrentStats());
            Debug.Log("Setting Achievement: " + SteamUserStats.SetAchievement("CUTSCENE_ACHIEVEMENT"));
            Debug.Log("Storing Stats: " + SteamUserStats.StoreStats());
        }
    }

    void Update()
    {
        if (startAlpha < 1 && fadeStart)
            SplashFadeIn();

        // Loop through all connected gamepads
        foreach (var gamepad in Gamepad.all)
        {
            if (gamepad == null) continue;

            // Splash screen enter
            if (splashScreen.activeInHierarchy)
            {
                if (gamepad.buttonSouth.wasPressedThisFrame && canEnterGame)
                {
                    Debug.Log("Game Enter");
                    splashScreen.SetActive(false);
                    mainMenuButtons.SetActive(true);
                    PlayMenuClick();
                    return; // prevent double-input this frame
                }
            }

            // Back button actions
            if (gamepad.buttonEast.wasPressedThisFrame && !splashScreen.activeInHierarchy && !playerRebinding)
            {
                switch (currentScreen)
                {
                    case (1): // Settings Menu
                        returnToTop();
                        break;
                    case (2): // Character Select Screen
                        if (!deselectOccured && !progressionActive && lockedGamepad == null)
                        {
                            StartCoroutine(CharacterSelectProgression(gamepad));
                        }
                        break;
                    case (3): // Stage Select Screen
                        UpdateProfileOptions();
                        if (stageSettings.activeSelf)
                        {
                            ShowStageSettings(false); // IFF stage settings open
                        }
                        else
                        {
                            returnToTop();
                        }
                        break;
                    case (4): // Quit Confirmation Menu
                        returnToTop();
                        break;
                    case (5): // Credits
                        returnToTop();
                        break;
                    case (6): // Extras Menu
                        returnToTop();
                        break;
                    case (7): // Sandbox Mode
                        backToExtras();
                        break;
                    case (8): // Tutorial
                        backToExtras();
                        break;
                    case (9): // Stats
                        backToExtras();
                        break;
                    case (10): // Changelog
                        returnToTop();
                        break;
                    case (11): // Player Profile Editor
                        ReturnToSettings();
                        PPM.CloseProfile();
                        break;
                    case (12): // Player Profile Submenu (bindings, config, etc.)
                        disablePPWindow();
                        break;
                    case (13): // Delete Profile Confirmation
                        CancelDeleteProfile();
                        break;
                    default:
                        break;
                }
                deselectOccured = false;
                return; // prevent double-input this frame
            }

            // Shoulder navigation
            if (SettingsMenu.activeInHierarchy && !playerRebinding)
            {
                if (gamepad.rightShoulder.wasPressedThisFrame)
                {
                    if (gameplaySettings.activeInHierarchy)
                        SettingsSwap(0);
                    else if (audioSettings.activeInHierarchy)
                        SettingsSwap(1);
                    else
                        SettingsSwap(2);

                    return; // prevent double-input this frame
                }

                if (gamepad.leftShoulder.wasPressedThisFrame)
                {
                    if (gameplaySettings.activeInHierarchy)
                        SettingsSwap(1);
                    else if (audioSettings.activeInHierarchy)
                        SettingsSwap(2);
                    else
                        SettingsSwap(0);

                    return; // prevent double-input this frame
                }
            }

            // Tutorial swap
            if (tutorialContent.activeInHierarchy)
            {
                TutorialImageSwap(gamepad);
                return;
            }
        }

        // Non-gamepad input
        if (statsContent.activeInHierarchy && Input.GetKeyDown(KeyCode.R) && !resetThisScene)
        {
            ST.ResetSaveData();
            resetThisScene = true;
        }

        if (AP.getUseComVol() && !AP.isPlaying())
        {
            AP.setUseComVol(false);
        }
    }

    private void SplashFadeIn()
    {
        startAlpha = Mathf.MoveTowards(startAlpha, endAlpha, 2.0f * Time.deltaTime);
        splashLogoCol.a = startAlpha;
        splashLogo.color = splashLogoCol;
        pressTextCol.a = startAlpha;
        splashText.color = pressTextCol;
        pressACol.a = startAlpha;
        pressA.color = pressACol;

        //Debug.Log(startAlpha);
        if (startAlpha >= endAlpha) canEnterGame = true;
    }

    private IEnumerator FadeWait()
    {
        yield return new WaitForSeconds(0.25f);
        fadeStart = true;
    }

    private IEnumerator CharacterSelectProgression(Gamepad gamepad)
    {
        progressionActive = true;
        lockedGamepad = gamepad;

        while (progressionActive)
        {
            // Reset progress if the UI object is inactive
            if (backButtonFillImage != null && !backButtonFillImage.gameObject.activeInHierarchy)
            {
                currentBackButtonFill = 0f;
            }

            bool eastHeld = gamepad != null && gamepad.buttonEast.isPressed && !playerRebinding;

            if (eastHeld)
            {
                currentBackButtonFill += Time.deltaTime; // +1 per second
            }
            else
            {
                currentBackButtonFill -= 1.5f * Time.deltaTime; // -1.5 per second
            }

            currentBackButtonFill = Mathf.Clamp(currentBackButtonFill, 0f, maxBackButtonFill);

            // Update radial fill
            if (backButtonFillImage != null)
            {
                backButtonFillImage.fillAmount = currentBackButtonFill / maxBackButtonFill;
            }

            // Trigger when filled
            if (currentBackButtonFill >= maxBackButtonFill && !deselectOccured)
            {
                deselectOccured = true;
                backToStageSelect();
            }

            // Unlock if released AND progression hits zero
            if (!eastHeld && currentBackButtonFill <= 0f)
            {
                progressionActive = false;
                lockedGamepad = null;
            }

            yield return null;
        }
    }


    #endregion

    #region SceneLoading

    public void loadGameplay(int targetScene) {
        for (int i = 0; i < cursors.Length; i++)
            {
                string currentPlayer = "Player" + i;
                if (cursors[i].GetComponent<MenuCursor>() != null)
                {
                    MenuCursor currentCursor = cursors[i].GetComponent<MenuCursor>();
                    PlayerPrefs.SetInt(currentPlayer, currentCursor.playerSlot);
                }
                
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
                case -1:
                    //WarriorTutorial
                    ALM.BeginLoad("WarriorTutorial");
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
            // VERSUS MATCH
            case 0:
                Debug.Log("OptionSelectVersus");
                currentScreen = 3; // Stage Select
                menuCamera.goToVersusSetup();
                CC.SwitchToEarth();
                stageSelect.SetActive(true);
                mainMenuButtons.SetActive(false);
                TVT.WarningEnd();
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(stageFirstButton);

                UpdateProfileOptions();

                PlayMenuClick();
                break;

            // SETTINGS
            case 1:
                currentScreen = 1; // Settings Menu
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(settingsFirstButton);
                menuCamera.goToSettings();
                TVT.WarningEnd();
                SettingsMenu.SetActive(true);
                mainMenuButtons.SetActive(false);
                UpdateProfileOptions();

                //GSScript.ResizeGridContent(playerButtonGrid, savedProfiles.Count);
                if (!hasOpenedSettingsOnce)
                {
                    SettingsSwap(1);
                    hasOpenedSettingsOnce = true;
                }

                PlayMenuClick();
                break;

            // QUIT GAME
            case 2:
                currentScreen = 4; // Quit Confirmation
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(quitFirstButton);
                menuCamera.goToQuitting();
                quitGameButtons.SetActive(true);
                mainMenuButtons.SetActive(false);
                TVT.WarningEnd();


                PlayMenuClick();
                break;

            // CREDITS
            case 3:
                currentScreen = 5; // Credits Screen
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(creditsBackButton);
                menuCamera.goToCredits();
                CC.SwitchToCredits();
                CSU.CreditsStart();
                TVT.WarningEnd();
                mainMenuButtons.SetActive(false);


                PlayMenuClick();

                if (SteamManager.Initialized)
                {
                    Debug.Log("Getting Stats: " + SteamUserStats.RequestCurrentStats());
                    Debug.Log("Setting Achievement: " + SteamUserStats.SetAchievement("CREDITS_VIEWED"));
                    Debug.Log("Storing Stats: " + SteamUserStats.StoreStats());
                }
                break;

            // EXTRAS MENU
            case 4:
                currentScreen = 6; // Extras Main
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(extrasFirstButton);
                menuCamera.goToExtras();
                TVT.WarningEnd();
                extrasContent.SetActive(true);
                mainMenuButtons.SetActive(false);


                PlayMenuClick();
                break;

            // SANDBOX
            case 5:
                currentScreen = 7; // Sandbox
                menuCamera.goToSandbox();
                extrasContent.SetActive(false);
                sandboxContent.SetActive(true);
                TVT.WarningEnd();

                if (AP != null) AP.setUseComVol(false);
                if (AP != null) AP.PlaySoundRandomPitch(AP.Find("menuOpen"));
                break;

            // TUTORIAL
            case 6:
                currentScreen = 8; // Tutorial
                menuCamera.goToHowToZoom();
                tutorialIndex = 0;
                extrasContent.SetActive(false);

                tutorialContent.SetActive(true);
                tutorialSwapImage.sprite = tutorialImages[0];
                tutorialHeaderText.text = tutorialHeaders[0];
                tutorialBlurbText.text = tutorialBlurbs[0];

                mainCanvasTutorial.SetActive(true);
                TVT.WarningEnd();

                if (AP != null) AP.setUseComVol(false);
                if (AP != null) AP.PlaySoundRandomPitch(AP.Find("menuOpen"));
                break;

            // STATS
            case 7:
                currentScreen = 9; // Stats
                menuCamera.goToStatsZoom();
                extrasContent.SetActive(false);
                statsContent.SetActive(true);
                TVT.WarningEnd();

                if (AP != null) AP.setUseComVol(false);
                if (AP != null) AP.PlaySoundRandomPitch(AP.Find("menuOpen"));
                break;

            // CHANGELOG
            case 8:
                currentScreen = 10; // Changelog
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(changelogBackButton);
                changelogContent.SetActive(true);
                mainMenuButtons.SetActive(false);
                break;

            // DISCORD LINK
            case 9:
                string discordInviteURL = "https://discord.gg/yHKMHjEU9k";
                Application.OpenURL(discordInviteURL);
                break;

            

            default:
                Debug.Log("Error: unknown menu option");
                break;
        }
    }

    public void returnToTop()
    {
        Debug.Log("ReturnToTop");
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(topFirstButton);
        topFirstButton.GetComponentInChildren<TMP_Text>().fontSize = 27.5f;
        STBC.TurnOff();
        menuCamera.goToTitle();
        CC.SwitchToNews();
        currentScreen = 0;
        mainMenuButtons.SetActive(true);
        stageSelect.SetActive(false); 
        extrasContent.SetActive(false);
        quitGameButtons.SetActive(false);
        SettingsMenu.SetActive(false);
        creditsContent.SetActive(false);
        sandboxContent.SetActive(false);
        tutorialContent.SetActive(false);
        mainCanvasTutorial.SetActive(false);
        statsContent.SetActive(false);
        changelogContent.SetActive(false);
        CSU.CreditsEnd();
        TVT.WarningStart();

        //sound

        PlayMenuClick();
    }

    public void backToExtras() {
        Debug.Log("BackToExtras");
        currentScreen = 6;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(extrasFirstButton);
        menuCamera.goToExtras();
        TVT.WarningEnd();
        extrasContent.SetActive(true);
        sandboxContent.SetActive(false);
        tutorialContent.SetActive(false);
        mainCanvasTutorial.SetActive(false);
        statsContent.SetActive(false);
        //sound

        PlayMenuClick();
    }

    public void backToSettings()
    {
        Debug.Log("BackToSettings");
        currentScreen = 6;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(extrasFirstButton);
        extrasContent.SetActive(true);
        sandboxContent.SetActive(false);
        tutorialContent.SetActive(false);
        mainCanvasTutorial.SetActive(false);
        statsContent.SetActive(false);
        //sound

        PlayMenuClick();
    }

    public void disablePPWindow()
    {
        //Disable all player profile submenus
        bindingsMenu.SetActive(false);
        configMenu.SetActive(false);
        shirtColorMenu.SetActive(false);
        skinColorMenu.SetActive(false);

        //Reactivate the player profile editor root screen
        playerProfileEditor.SetActive(true);
        TogglePPCanvasGroup();

        currentScreen = 11;

        //Reset selection to a safe default (e.g. first button in the profile editor)
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(playerProfilesFirstButton);
    }

    public void PromptDeleteProfile(PlayerProfile profile)
    {
        profileToDelete = profile;
        deleteConfirmationScreen.SetActive(true);
        currentScreen = 13;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(deleteYesButton);

        PlayMenuClick();
    }

    public void ConfirmDeleteProfile()
    {
        if (profileToDelete != null && File.Exists(profileToDelete.FilePath))
        {
            File.Delete(profileToDelete.FilePath);
            Debug.Log("Deleted profile: " + profileToDelete.Profile_Name);
        }

        profileToDelete = null;

        deleteConfirmationScreen.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(createNewProfileButton.gameObject);
        currentScreen = 1;
        UpdateProfileOptions();
        PlayMenuClick();
    }

    public void CancelDeleteProfile()
    {
        profileToDelete = null;

        deleteConfirmationScreen.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(createNewProfileButton.gameObject);
        currentScreen = 1;
        PlayMenuClick();
    }

    private void SetCanvasGroupActive(CanvasGroup group, bool active)
    {
        group.interactable = active;
        group.blocksRaycasts = active;
    }

    public void moveToStageSelect()
    {
        Debug.Log("moveToStageSelect");
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
        stageFirstButton.gameObject.GetComponent<Selectable>().Select();
        //sound
        
                PlayMenuClick();
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
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(lastStageButton.gameObject);
        lastStageButton.GetComponent<Selectable>().Select();

        //sound
        PlayMenuClick("menuClose");
    }

    //Change this to flip between screens with Dpad at some point
    public void SettingsSwap(int type)
    {
        // Add controller menu nav
        // Weird bug where if this isn't present, header on select up gets set to toggle screen shake???
        controlsNavi.selectOnUp = settingsBackButton;
        audioNavi.selectOnUp = settingsBackButton;
        gameplayNavi.selectOnUp = settingsBackButton;
        backNavi.selectOnDown = settingsGameplayButton;

        switch (type)
        {
            case 0: // Audio
                gameplaySettings.SetActive(false);
                controlSettings.SetActive(false);
                audioSettings.SetActive(true);

                EventSystem.current.SetSelectedGameObject(settingsAudioButton.gameObject);
                //settingsHeader.text = "AUDIO";

                settingsTabButtons[0].navigation = BuildNav(settingsTabButtons[0].navigation, down: audioSettingsNav[1]);
                settingsTabButtons[1].navigation = BuildNav(settingsTabButtons[1].navigation, down: audioSettingsNav[1]);
                settingsTabButtons[2].navigation = BuildNav(settingsTabButtons[2].navigation, down: audioSettingsNav[1]);
                settingsTabButtons[3].navigation = BuildNav(settingsTabButtons[3].navigation, up: audioSettingsNav[3]);
                break;

            case 1: // Controls
                audioSettings.SetActive(false);
                gameplaySettings.SetActive(false);
                controlSettings.SetActive(true);

                EventSystem.current.SetSelectedGameObject(settingsControlsButton.gameObject);
                //settingsHeader.text = "CONTROLS";

                settingsTabButtons[0].navigation = BuildNav(settingsTabButtons[0].navigation, down: controlsSettingsNav[0]);
                settingsTabButtons[1].navigation = BuildNav(settingsTabButtons[1].navigation, down: controlsSettingsNav[0]);
                settingsTabButtons[2].navigation = BuildNav(settingsTabButtons[2].navigation, down: controlsSettingsNav[0]);
                settingsTabButtons[3].navigation = BuildNav(settingsTabButtons[3].navigation, up: controlsSettingsNav[0]);
                break;

            case 2: // Gameplay
                controlSettings.SetActive(false);
                audioSettings.SetActive(false);
                gameplaySettings.SetActive(true);

                EventSystem.current.SetSelectedGameObject(settingsGameplayButton.gameObject);
                //settingsHeader.text = "GAMEPLAY";

                settingsTabButtons[0].navigation = BuildNav(settingsTabButtons[0].navigation, down: gameplaySettingsNav[0]);
                settingsTabButtons[1].navigation = BuildNav(settingsTabButtons[1].navigation, down: gameplaySettingsNav[0]);
                settingsTabButtons[2].navigation = BuildNav(settingsTabButtons[2].navigation, down: gameplaySettingsNav[0]);
                settingsTabButtons[3].navigation = BuildNav(settingsTabButtons[3].navigation, up: gameplaySettingsNav[2]);
                break;
            default:
                Debug.LogWarning("Unknown settings tab type: " + type);
                break;
        }
    }

    public void SwapSettingsControlType()
    {
        if (warriorControls.activeInHierarchy)
        {
            warriorControls.SetActive(false);
            monsterControls.SetActive(true);
            teamControlsText.text = "MONSTERS";
        }
        else
        {
            warriorControls.SetActive(true);
            monsterControls.SetActive(false);
            teamControlsText.text = "WARRIORS";
        }
    }

    public void exitGame()
    {
        Debug.Log("Quitting game. Goodbye!");
        Application.Quit();
    }

    private void TutorialImageSwap(Gamepad gamepad)
    {
        if (gamepad.leftShoulder.wasPressedThisFrame && tutorialIndex >= 1)
        {
            tutorialIndex--;
            tutorialSwapImage.sprite = tutorialImages[tutorialIndex];
            tutorialHeaderText.text = tutorialHeaders[tutorialIndex];
            tutorialBlurbText.text = tutorialBlurbs[tutorialIndex];
        }

        if (gamepad.rightShoulder.wasPressedThisFrame && tutorialIndex < tutorialImages.Length - 1)
        {
            tutorialIndex++;
            tutorialSwapImage.sprite = tutorialImages[tutorialIndex];
            tutorialHeaderText.text = tutorialHeaders[tutorialIndex];
            tutorialBlurbText.text = tutorialBlurbs[tutorialIndex];
        } 
    }

    public void UpdateStatsText()
    {
        mGoalsText.text = "Goals: " + ST.saveData.mGoals;
        mKillsText.text = "Kills: " + ST.saveData.kills;
        mAbiltiesText.text = "Abilities: " + ST.saveData.abilities;
        mWinsText.text = "Wins: " + ST.saveData.mWins;

        wGoalsText.text = "Goals: " + ST.saveData.wGoals;
        //wDeathsText.text = "Deaths: " + ST.saveData.deaths;
        wAssistsText.text = "Assists: " + ST.saveData.assists;
        wStealsText.text = "Steals: " + ST.saveData.steals;
        wWinsText.text = "Wins: " + ST.saveData.wWins;
    }

    #endregion

    #region Player Profile Logic

    public void OpenNewPPMenu()
    {
        var savedProfiles = PPM.LoadAllProfiles();

        if (savedProfiles.Count < MaxProfiles)
        {
            currentScreen = 11;
            playerProfileEditor.SetActive(true);
            SettingsButtons.SetActive(false);

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(playerProfilesFirstButton);

            PPM.CreateNewProfile(savedProfiles);
        }
        else
        {
            if (limitMessageCoroutine != null) 
                StopCoroutine(limitMessageCoroutine);
            limitMessageCoroutine = StartCoroutine(ShowLimitReachedMessage());
        }
        PlayMenuClick();
    }

    private IEnumerator ShowLimitReachedMessage()
    {
        if (limitReachedMessageText == null) yield break;

        CanvasGroup canvasGroup = limitReachedMessageText.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = limitReachedMessageText.gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0f;
        limitReachedMessageText.gameObject.SetActive(true);

        //Fade in
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }

        yield return new WaitForSeconds(messageDisplayTime);

        //Fade out
        t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }

        limitReachedMessageText.gameObject.SetActive(false);
    }

    public void OpenExistingPPMenu(PlayerProfile profile)
    {
        currentScreen = 11;
        playerProfileEditor.SetActive(true);
        SettingsButtons.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(playerProfilesFirstButton);

        PPM.LoadProfile(profile); //Load the selected profile into the system

        PlayMenuClick();
    }

    public void OpenBindingsMenu()
    {
        TogglePPCanvasGroup();
        playerProfilesFirstButton = EventSystem.current.currentSelectedGameObject ?? playerProfilesFirstButton;
        currentScreen = 12;

        bindingsMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(bindingsFirstButton.gameObject);
        PlayMenuClick("menuClick2");
    }

    public void OpenConfigMenu()
    {
        TogglePPCanvasGroup();
        playerProfilesFirstButton = EventSystem.current.currentSelectedGameObject ?? playerProfilesFirstButton;
        currentScreen = 12;
        configMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(configFirstButton.gameObject);
        PlayMenuClick("menuClick2");
    }

    public void OpenShirtColorMenu()
    {
        TogglePPCanvasGroup();
        playerProfilesFirstButton = EventSystem.current.currentSelectedGameObject ?? playerProfilesFirstButton;
        currentScreen = 12;

        shirtColorMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(shirtFirstButton.gameObject);
        PlayMenuClick("menuClick2");
    }

    public void OpenSkinColorMenu()
    {
        TogglePPCanvasGroup();
        playerProfilesFirstButton = EventSystem.current.currentSelectedGameObject ?? playerProfilesFirstButton;
        currentScreen = 12;

        skinColorMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(skinFirstButton.gameObject);
        PlayMenuClick("menuClick2");
    }

    public void ReturnToSettings()
    {
        UpdateProfileOptions();
        playerProfilesFirstButton = EventSystem.current.currentSelectedGameObject ?? playerProfilesFirstButton;
        playerProfileEditor.SetActive(false);
        SettingsButtons.SetActive(true);

        currentScreen = 1;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(settingsControlsButton.gameObject);

        //GSScript.ResizeGridContent(playerButtonGrid, savedProfiles.Count);

        //sound
        PlayMenuClick();
    }

    private void TogglePPCanvasGroup()
    {
        playerProfileCanvasGroup.interactable = !playerProfileCanvasGroup.interactable;
        playerProfileCanvasGroup.blocksRaycasts = !playerProfileCanvasGroup.blocksRaycasts;
    }

    #endregion

    #region Character Selection

    public void characterSelected(int playerNumber, int playerSlot)
    {
        Debug.Log("Player " + playerNumber + " selected Character " + playerSlot);
        //todo: set it so playerNumber can control playerSlot's character options
        if (playerSlot == 0)
        { 
            monsterAbilityCanHover = false;
            monsterDrop.gameObject.SetActive(true);
        }
        
        else
        {
            playerOptions[playerSlot].SetActive(true);
        }
    }

    public void characterUnselected(int playerNumber, int playerSlot)
    {
        Debug.Log("Player " + playerNumber + " unselected Character " + playerSlot);
        //todo: reverse that thing from the last comment
        //playerOptions[playerSlot].SetActive(false);
        if (playerSlot == 0)
        {
            monsterAbilityCanHover = true;
            monsterDrop.gameObject.SetActive(false);
        }
        
        else
        {
            playerOptions[playerSlot].SetActive(false);
        }
        //if (confirm)
        if (confirmedInfos.Contains(characterInfos[playerSlot]))
        {
            unconfirmCharacter(playerSlot);
        }
    }

    public void confirmCharacter(int playerSlot, int hoveringID)
    {
        slotToHoveringID[playerSlot] = hoveringID;

        confirmedInfos.Add(characterInfos[playerSlot]);
        characterInfos[playerSlot].confirm();
        numPlayersConfirmed++;
        Debug.Log("players confirmed: " + numPlayersConfirmed + " - Time: " + Time.fixedTime);
        findAllCursors();
        Debug.Log("players needed: " + cursors.Length);

        if (numPlayersConfirmed == cursors.Length)
        {
            canMoveToGame = true;
            readyText.SetActive(true);
            hideWhenReady.SetActive(false);
            monsterImages[monsterNameScript.monsterIndex].SetActive(true);
            showWhenReady.SetActive(true);

            //Update UI elements all at once
            UpdateBotAndPlayerElements();
        }
    }

    public void unconfirmCharacter(int playerSlot)
    {
        if (!slotToHoveringID.ContainsKey(playerSlot))
            return;

        confirmedInfos.Remove(characterInfos[playerSlot]);
        characterInfos[playerSlot].unconfirm();
        numPlayersConfirmed--;
        Debug.Log("players confirmed: " + numPlayersConfirmed + "Time: " + Time.fixedTime);

        slotToHoveringID.Remove(playerSlot);

        if (canMoveToGame)
        {
            canMoveToGame = false;
            readyText.SetActive(false);
            hideWhenReady.SetActive(true);
            monsterImages[monsterNameScript.monsterIndex].SetActive(false);
            showWhenReady.SetActive(false);

            ResetBotAndPlayerElements();
        }
    }

    /// <summary>
    /// Sets the active states of botElements and playerElements
    /// based on which hoveringIDs are assigned.
    /// </summary>
    private void UpdateBotAndPlayerElements()
    {
        //First, default everything to bot
        for (int i = 0; i < botElements.Length; i++)
        {
            botElements[i].SetActive(true);
            playerElements[i].SetActive(false);
        }

        // Always force playerElements[0] on
        playerElements[0].SetActive(true);

        foreach (var kvp in slotToHoveringID)
        {
            int hoveringID = kvp.Value;
            if (hoveringID >= 0 && hoveringID < playerElements.Length)
            {
                botElements[hoveringID].SetActive(false);
                playerElements[hoveringID].SetActive(true);
            }
        }
    }

    /// <summary>
    /// Resets to "all bots" but keeps playerElements[0] on.
    /// </summary>
    private void ResetBotAndPlayerElements()
    {
        for (int i = 0; i < botElements.Length; i++)
        {
            botElements[i].SetActive(true);
            playerElements[i].SetActive(false);
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
        if (AP != null) AP.setUseComVol(false);
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
        if (AP != null) AP.setUseComVol(false);
        if (AP != null) AP.PlaySoundRandomPitch(AP.Find("menuClick"));
    }

    public void SetShowControls()
    {
        if (controlsToggle.isOn)
        {
            PlayerPrefs.SetInt("showControls", 1);
        }
        else
        {
            PlayerPrefs.SetInt("showControls", 0);
        }

        //sound
        PlayMenuClick("menuClick");
    }

    public void SetBallOutline()
    {
        if (outlineToggle.isOn)
        {
            PlayerPrefs.SetInt("ballOutlineMatchesTeam", 1);
        }
        else
        {
            PlayerPrefs.SetInt("ballOutlineMatchesTeam", 0);
        }

        //sound

        PlayMenuClick("menuClick");
    }

    public void setDeadzoneAdjustment()
    {
        PlayerPrefs.SetFloat("kicksens", deadzoneSlider.value / 10);
        DeadzoneAdjNum.text = (deadzoneSlider.value / 10).ToString();
        //sound
        if (AP != null) AP.setUseComVol(false);
        if (AP != null && !AP.isPlaying()) AP.PlaySoundRandomPitch(AP.Find("menuClick"));
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
        if (AP != null) AP.setUseComVol(false);
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

        if (PlayerPrefs.GetFloat("commentaryVolume") == 0f)
        {
            // Steam Achievement Stuff
            if (SteamManager.Initialized)
            {
                Debug.Log("Getting Stats: " + SteamUserStats.RequestCurrentStats());
                Debug.Log("Setting Achievement: " + SteamUserStats.SetAchievement("COMMENTATOR_OFF"));
                Debug.Log("Storing Stats: " + SteamUserStats.StoreStats());
            }
        }
    }

    public void setCommentaryFrequency()
    {
        PlayerPrefs.SetFloat("commentaryFrequency", comFreqSlider.value / 100f);
        CommFreqNum.text = Mathf.Round(comFreqSlider.value).ToString();

        //sound
        if (AP != null) AP.setUseComVol(false);
        if (AP != null && !AP.isPlaying()) AP.PlaySoundRandomPitch(AP.Find("menuClick"));

        if (PlayerPrefs.GetFloat("commentaryFrequency") == 0f)
        {
            // Steam Achievement Stuff
            if (SteamManager.Initialized)
            {
                Debug.Log("Getting Stats: " + SteamUserStats.RequestCurrentStats());
                Debug.Log("Setting Achievement: " + SteamUserStats.SetAchievement("COMMENTATOR_OFF"));
                Debug.Log("Storing Stats: " + SteamUserStats.StoreStats());
            }
        }
    }

    //Match Settings
    public void setGoalBarriers()
    {
        PlayerPrefs.SetInt("goalBarriers", goalDropdown.value);

        // give achievement for turning off
        if (goalDropdown.value == 6)
        {
            if (SteamManager.Initialized)
            {
                Debug.Log("Getting Stats: " + SteamUserStats.RequestCurrentStats());
                Debug.Log("Setting Achievement: " + SteamUserStats.SetAchievement("NO_BARRIERS"));
                Debug.Log("Storing Stats: " + SteamUserStats.StoreStats());
            }
        }

        //sound
        PlayMenuClick("menuClick");
    }

    public void SetOvertime()
    {
        PlayerPrefs.SetInt("overtime", overtimeDropdown.value);
        //sound
        PlayMenuClick("menuClick");
    }

    public void SetKickCharge()
    {
        PlayerPrefs.SetInt("kickcharge", kickchargeDropdown.value);
        //sound
        PlayMenuClick("menuClick");
    }
    #endregion

    #region Controller Connection

    public void showConnected(int playerNumber)
    {
        switch (playerNumber)
        {
            case 1:
                p1Connected.SetActive(true);
                p1Disconnected.SetActive(false);
                break;
            case 2:
                p2Connected.SetActive(true);
                p2Disconnected.SetActive(false);
                break;
            case 3:
                p3Connected.SetActive(true);
                p3Disconnected.SetActive(false);
                break;
            case 4:
                p4Connected.SetActive(true);
                p4Disconnected.SetActive(false);
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
                p1Disconnected.SetActive(true);
                break;
            case 2:
                p2Connected.SetActive(false);
                p2Disconnected.SetActive(true);
                break;
            case 3:
                p3Connected.SetActive(false);
                p3Disconnected.SetActive(true);
                break;
            case 4:
                p4Connected.SetActive(false);
                p4Disconnected.SetActive(true);
                break;
            default:
                Debug.Log("Player Num Error");
                break;
        }
    }

    private void UpdateProfileOptions()
    {
        savedProfiles = PPM.LoadAllProfiles();
        PPM.SyncProfileButtonsWithList(savedProfiles, PPButtonParent, PPButtonPrefab);
        

        // Below code is for adding Profile options on Character Select dropdowns
        warriorDrop1.ClearOptions();
        warriorDrop2.ClearOptions();
        warriorDrop3.ClearOptions();
        monsterDrop.ClearOptions();

        List<string> allProfileNames = new List<string>();
        allProfileNames.Add("No Profile");
        for (int i = 0; i < savedProfiles.Count; i++)
        {
            allProfileNames.Add(savedProfiles[i].Profile_Name);
        }

        warriorDrop1.AddOptions(allProfileNames);
        warriorDrop2.AddOptions(allProfileNames);
        warriorDrop3.AddOptions(allProfileNames);
        monsterDrop.AddOptions(allProfileNames);
    }

    //DEFUNCT NOW
    /*public void UpdateCreateButtonState()
    {
        bool limitReached = savedProfiles.Count >= MaxProfiles;

        if (createNewProfileButton != null)
        {
            var button = createNewProfileButton.GetComponent<Button>();
            if (button != null)
            {
                button.interactable = !limitReached;
            }
        }
    }*/

    #endregion

    #region Stage Selection

    public void selectStage(int stageID) {

        if (stageID == -1)
        {
            stageSelection = Random.Range(0, 5);
        } else
        {
            stageSelection = stageID;
        }

        currentScreen = 2;
        STBC.TurnOff();
        stageSelect.SetActive(false);
        characterSelect.SetActive(true);

        // Code for reseting sprite colors to black
        WD1.ResetColor();
        //WD1.ResetSliders();
        WD2.ResetColor();
        //WD2.ResetSliders();
        WD3.ResetColor();
        //WD3.ResetSliders();


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

    public void SetLastStageButton(int stageID)
    {
        lastStageButton = stageButtons[stageID];
        lastStageNavi.selectOnLeft = lastStageButton;
        lastStageNavi.selectOnRight = lastStageButton;
        stageSettingsButton.navigation = lastStageNavi;
    }

    public void ShowStageSettings(bool state)
    {
        stageSettings.SetActive(state);
        stageSelect.SetActive(!state);

        if (state)
        {
            EventSystem.current.SetSelectedGameObject(null);
            stageSettingsFirstButton.gameObject.GetComponent<Selectable>().Select();
            EventSystem.current.SetSelectedGameObject(stageSettingsFirstButton);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
            lastStageButton.gameObject.GetComponent<Selectable>().Select();
            EventSystem.current.SetSelectedGameObject(lastStageButton.gameObject);
        }
    }

    //private IEnumerator StageImage(int stageID)
    //{
    //    stageFade.CrossFadeAlpha
    //}

    #endregion

    private Navigation BuildNav(Navigation baseNav, Selectable up = null, Selectable down = null, Selectable left = null, Selectable right = null)
    {
        baseNav.mode = Navigation.Mode.Explicit;

        if (up != null) baseNav.selectOnUp = up;
        if (down != null) baseNav.selectOnDown = down;
        if (left != null) baseNav.selectOnLeft = left;
        if (right != null) baseNav.selectOnRight = right;

        return baseNav;
    }

    private void PlayMenuClick(string soundName = "menuClick2", bool useComVol = false)
    {
        if (AP == null) return;

        AP.setUseComVol(useComVol);
        AP.PlaySoundRandomPitch(AP.Find(soundName));
    }
}
