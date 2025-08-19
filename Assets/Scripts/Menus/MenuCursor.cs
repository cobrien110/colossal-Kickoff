using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;

public class MenuCursor : MonoBehaviour
{

    #region Variables
    [Header("Movement")]
    [SerializeField] private Rigidbody2D body;
    private float horizontal;
    private float vertical;
    //private float moveLimiter = 0.7f; //REDUNDENT
    [SerializeField] private float speed = 500.0f;
    private Vector3 savedPosition;
    private Vector3 screenMidpoint;

    [Header("Player Info")]
    public int playerNumber = -1;
    public int playerSlot = -1;
    public WarriorDesc WD = null;
    [SerializeField] private int menuSlot = -1; //REDUNDENT
    [SerializeField] private int warriorColor = -1; //REDUNDENT
    [SerializeField] private PlayerHolder PH;
    [SerializeField] private GameObject[] playerHolders;
    [SerializeField] private int colorIndex = -1; //REDUNDENT

    public enum SelectionType { None, Monster, Warrior }
    public SelectionType currentSelection = SelectionType.None;

    [Header("Scene References")]
    [SerializeField] private MenuController MC = null;
    [SerializeField] private InputManager IM = null;
    //[SerializeField] private VolumeManager VM = null; //REDUNDENT
    [SerializeField] private MonsterName MN = null;
    [SerializeField] private MonsterAbilityBlurb abilityBlurb = null; //REDUNDENT
    [SerializeField] private MonsterAbilityViewController monsterAbilityViewController = null;
    //[SerializeField] private WarriorDesc[] WDarr = null; //REDUNDENT

    [Header("UI")]
    [SerializeField] private Image cursorImage;
    [SerializeField] private PlayerSelectedDisplay[] playerMarkerIcons;
    [SerializeField] private Sprite[] cursorSprites;
    [SerializeField] private TMP_Dropdown thisDropdown = null;

    [Header("Input & Hover")]
    public InputAction cursorMove;
    public string hoveringItem = "null";
    public int hoveringID = -1;

    [Header("State Flags")]
    public bool hasSelected = false;
    public bool charConfirmed = false;
    [SerializeField] private bool selectedHighlightingAbilities = false;
    [SerializeField] private bool selectingProfile = false;

    [Header("Audio")]
    [SerializeField] private AudioPlayer AP;
    //private bool willPlaySelectSound = false; //REDUNDENT

    [Header("Player Input Actions")]
    [SerializeField] private InputAction moveAction;
    [SerializeField] private InputAction selectAction;
    [SerializeField] private InputAction deselectAction;
    [SerializeField] private InputAction leaveAction;
    [SerializeField] private InputAction changeAction;

    // Cached input values
    private Vector2 moveInput = Vector2.zero;
    private bool selectPressed = false;
    private bool deselectPressed = false;
    private bool leavePressed = false;
    private Vector2 changeInput = Vector2.zero;


    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        //Debug.Log("Height:" + Screen.height);
        //Debug.Log("Width:" + Screen.width);
        speed = 120.0f * (Screen.height / 100);
        screenMidpoint = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        hasSelected = false;
        transform.SetParent(GameObject.Find("Canvas").transform);
        transform.position = new Vector3(0, 0, 0);
        MC = GameObject.Find("MenuController").GetComponent<MenuController>();
        MC.newCursor();
        IM = GameObject.Find("InputManager").GetComponent<InputManager>();
        //VM = FindObjectOfType<VolumeManager>(true);
        MN = FindObjectOfType<MonsterName>(true);
        abilityBlurb = FindObjectOfType<MonsterAbilityBlurb>(true);
        monsterAbilityViewController = FindObjectOfType<MonsterAbilityViewController>(true);

        //WDarr = FindObjectsOfType<WarriorDesc>(true);
        //WarriorDesc holder = WDarr[1];
        //WDarr[1] = WDarr[2];
        //WDarr[2] = holder;

        if (MC.currentScreen == 2)
        {
            showCursor();
            findCharSelectItems();
        }
        else
        {
            hideCursor();
        }
        cursorImage.sprite = cursorSprites[playerNumber - 1];

        playerHolders = GameObject.FindGameObjectsWithTag("PlayerHolder");
        PlayerHolder temp = null;
        for (int i = 0; i < playerHolders.Length; i++)
        {
            temp = playerHolders[i].GetComponent<PlayerHolder>();
            if (temp.playerID == playerNumber - 1)
            {
                PH = temp;
            }
        }

        Debug.Log("Cursor " + playerNumber + " with PlayerHolder " + PH.playerID);
        transform.position = new Vector3(325, 185, 0);

        // play sound
        AP = GetComponent<AudioPlayer>();
        AP.PlaySoundRandomPitch(AP.Find("menuJoin"));

        // show connected
        MC.showConnected(playerNumber);
    }

    private void OnEnable()
    {
        cursorMove.Enable();
    }

    private void OnDisable()
    {
        cursorMove.Disable();
    }

    void FixedUpdate()
    {

        if ((!hasSelected) || MC.currentScreen == 3)
        {
            body.velocity = new Vector2(horizontal * speed, vertical * speed);
        }
        //bounding box
        if (transform.position.y > Screen.height)
        {
            transform.position = new Vector3(transform.position.x, Screen.height, 0);
        }
        if (transform.position.y < 0)
        {
            transform.position = new Vector3(transform.position.x, 0, 0);
        }
        if (transform.position.x < 0)
        {
            transform.position = new Vector3(0, transform.position.y, 0);
        }
        if (transform.position.x > Screen.width)
        {
            transform.position = new Vector3(Screen.width, transform.position.y, 0);
        }
    }

    /*private void Update()
    {
        // Only process input if this player's event system is active
        if (PH.thisES != null && PH.thisES.GetComponent<MultiplayerEventSystem>().currentPlayerNumber == playerNumber)
        {
            ReadInputs();

            HandleMovement();
            HandleSelect();
            HandleDeselect();
            HandleLeave();
            HandleChange();
        }
    }*/
    #endregion

    #region Cursor Control
    public void hideCursor()
    {
        cursorImage.enabled = false;
        body.velocity = new Vector2(0, 0);
    }

    public void showCursor()
    {
        transform.position = screenMidpoint;
        cursorImage.enabled = true;
    }
    #endregion

    #region Player Selection

    public void PlayerSelected(int value)
    {
        Debug.Log("PLAYER SELECTED CALLED");
        playerSlot = value;
        hasSelected = true;

        if (value == 0)
        {
            PH.teamName = "Monster";
            PH.monsterIndex = MN.monsterIndex;
        }
        else
        {
            PH.teamName = "Warrior";
            PH.warriorPosition = value;
        }
        hideCursor();
        MC.characterSelected(playerNumber, playerSlot);

        if (playerSlot == 0)
        {

            MN.selectName(MN.monsterNames[MN.monsterIndex]);
            thisDropdown = MC.monsterDrop;
        }
        else if (playerSlot == 1)
        {
            WD = GameObject.Find("Warrior1Color").GetComponent<WarriorDesc>();
            Debug.Log("Player Selected: 1");
            thisDropdown = MC.warriorDrop1;
        }
        else if (playerSlot == 2)
        {
            WD = GameObject.Find("Warrior2Color").GetComponent<WarriorDesc>();
            Debug.Log("Player Selected: 2");
            thisDropdown = MC.warriorDrop2;
        }
        else if (playerSlot == 3)
        {
            WD = GameObject.Find("Warrior3Color").GetComponent<WarriorDesc>();
            Debug.Log("Player Selected: 3");
            thisDropdown = MC.warriorDrop3;
        }
    }

    public void deselect()
    {
        if (selectedHighlightingAbilities)
        {
            selectedHighlightingAbilities = false;
        }
        if (hasSelected)
        {
            playerMarkerIcons[playerSlot].changeSprite(0);
            MC.characterUnselected(playerNumber, playerSlot);
            MN.unselectName(MN.monsterNames[MN.monsterIndex]);
        }

        //PH.RemoveEvents();
        PH.teamName = "";
        hasSelected = false;
        cursorImage.enabled = true;
        playerSlot = -1;
        PH.warriorPosition = -1;
        WD = null;

        if (thisDropdown != null)
        {
            thisDropdown.value = 0;

            List<GameObject> allSelected = new List<GameObject>();

            for (int i = 0; i < playerHolders.Length; i++)
            {
                Debug.Log("Player Holder " + i + ": " + playerHolders[i].GetComponentInChildren<MultiplayerEventSystem>().currentSelectedGameObject);
                allSelected.Add(playerHolders[i].GetComponentInChildren<MultiplayerEventSystem>().currentSelectedGameObject);
            }

            thisDropdown.Hide();

            for (int i = 0; i < playerHolders.Length; i++)
            {
                if (playerHolders[i] != PH.gameObject)
                {
                    Debug.Log("Player Holder " + i + ": " + allSelected[i]);
                    playerHolders[i].GetComponentInChildren<MultiplayerEventSystem>().SetSelectedGameObject(allSelected[i]);
                }
            }
        }

        thisDropdown = null;
        PH.RemoveEvents();
        PH.DefaultProfile();

        selectingProfile = false;

        MC.deselectOccured = true;
    }

    //find the icons that display who's selected which characters on screen
    public void findCharSelectItems()
    {
        Debug.Log("findcharselectitems running");
        playerMarkerIcons[0] = GameObject.Find("MonsterSelected").GetComponent<PlayerSelectedDisplay>();
        playerMarkerIcons[1] = GameObject.Find("Warrior1Selected").GetComponent<PlayerSelectedDisplay>();
        playerMarkerIcons[2] = GameObject.Find("Warrior2Selected").GetComponent<PlayerSelectedDisplay>();
        playerMarkerIcons[3] = GameObject.Find("Warrior3Selected").GetComponent<PlayerSelectedDisplay>();
    }

    #endregion

    #region Hovering

    public void StartHovering(string item, int ID)
    {
        hoveringItem = item;
        hoveringID = ID;
    }

    public void StopHovering()
    {
        hoveringItem = "null";
        hoveringID = -1;
    }

    #endregion

    #region Input Handling

    /// <summary>
    /// Reads player input for moving the cursor.
    /// Updates horizontal and vertical movement values.
    /// </summary>
    public void OnMove(InputAction.CallbackContext action)
    {
        // Get the x and y components from the input vector
        horizontal = action.ReadValue<Vector2>().x;
        vertical = action.ReadValue<Vector2>().y;
    }

    public void OnSelect(InputAction.CallbackContext action)
    {
        bool justSelectedChar = false;
        if (action.started)
        {
            if (MC.currentScreen == 2)
            {
                //Character Select
                if (!MC.canMoveToGame)
                {
                    if (!hasSelected)
                    {
                        if (hoveringItem.Equals("playerSelect") && !IM.IsSelected(hoveringID))
                        {
                            if (!playerMarkerIcons[hoveringID].isAssigned)
                            {
                                PlayerSelected(hoveringID);
                                playerMarkerIcons[hoveringID].changeSprite(playerNumber);
                            }
                        }
                    }
                    else if (PH.thisES.GetComponent<MultiplayerEventSystem>().currentSelectedGameObject == thisDropdown.gameObject)
                    {
                        List<GameObject> allSelected = new List<GameObject>();

                        for (int i = 0; i < playerHolders.Length; i++)
                        {
                            Debug.Log("Player Holder " + i + ": " + playerHolders[i].GetComponentInChildren<MultiplayerEventSystem>().currentSelectedGameObject);
                            allSelected.Add(playerHolders[i].GetComponentInChildren<MultiplayerEventSystem>().currentSelectedGameObject);
                        }

                        //If Player clicks dropdown box, open dropdown box
                        thisDropdown.Show();

                        for (int i = 0; i < playerHolders.Length; i++)
                        {
                            if (playerHolders[i] != PH.gameObject)
                            {
                                Debug.Log("Player Holder " + i + ": " + allSelected[i]);
                                playerHolders[i].GetComponentInChildren<MultiplayerEventSystem>().SetSelectedGameObject(allSelected[i]);
                            }
                        }

                        PH.SetEvents(thisDropdown.gameObject.transform.GetChild(3).GetChild(0).GetChild(0).GetChild(1).gameObject);
                    }
                    else if (PH.thisES.GetComponent<MultiplayerEventSystem>().currentSelectedGameObject != null && PH.thisES.GetComponent<MultiplayerEventSystem>().currentSelectedGameObject.name.StartsWith("Item"))
                    {
                        //If Player selects an item, do the following:
                        Debug.Log("Item!");

                        //...find the selected item's int value...
                        List<TMP_Dropdown.OptionData> options = thisDropdown.options;

                        string itemName = PH.thisES.GetComponent<MultiplayerEventSystem>().currentSelectedGameObject.name;
                        string resultString = Regex.Match(itemName, @"\d+").Value;
                        int itemInt = int.Parse(resultString);

                        //...set the item as active...
                        thisDropdown.value = itemInt;

                        List<GameObject> allSelected = new List<GameObject>();

                        for (int i = 0; i < playerHolders.Length; i++)
                        {
                            Debug.Log("Player Holder " + i + ": " + playerHolders[i].GetComponentInChildren<MultiplayerEventSystem>().currentSelectedGameObject);
                            allSelected.Add(playerHolders[i].GetComponentInChildren<MultiplayerEventSystem>().currentSelectedGameObject);
                        }

                        thisDropdown.Hide();

                        for (int i = 0; i < playerHolders.Length; i++)
                        {
                            if (playerHolders[i] != PH.gameObject)
                            {
                                Debug.Log("Player Holder " + i + ": " + allSelected[i]);
                                playerHolders[i].GetComponentInChildren<MultiplayerEventSystem>().SetSelectedGameObject(allSelected[i]);
                            }
                        }
                        PH.thisES.GetComponent<MultiplayerEventSystem>().SetSelectedGameObject(null);
                        PH.thisES.GetComponent<MultiplayerEventSystem>().playerRoot = null;
                        selectingProfile = false;

                        string profileName = thisDropdown.captionText.text;

                        //...if item is "No Profile", set Default options. Otherwise...
                        if (profileName.Equals("No Profile"))
                        {
                            PH.DefaultProfile();
                            return;
                        }

                        //...load the profile to the correct PlayerHolder
                        for (int i = 0; i < MC.savedProfiles.Count; i++)
                        {
                            if (MC.savedProfiles[i].Profile_Name.Equals(profileName))
                            {
                                PH.LoadProfile(MC.savedProfiles[i]);
                            }
                        }
                    }
                    else if (!charConfirmed)
                    {
                        charConfirmed = true;
                        MC.confirmCharacter(playerSlot);
                        // play sound
                        AP.PlaySoundRandomPitch(AP.Find("menuStart2"));
                        justSelectedChar = true;
                        AP.PlaySoundRandomPitch(AP.Find("menuSelect"));
                        //Temp Color code
                        if (playerSlot != 0)
                        {
                            PH.warriorColor = WD.getCurrentColor();
                        }
                    }
                }
                else if (playerNumber == 1)
                {
                    MC.loadGameplay(MC.stageSelection);
                }
                // play sound
            }
            else
            {
                // play sound
            }
        }
    }

    public void OnDeselect(InputAction.CallbackContext action)
    {
        if (action.started)
        {
            //Deselect Character
            if ((hasSelected) && (MC.currentScreen == 2))
            {
                charConfirmed = false;
                Debug.Log("Deselecting");
                deselect();
            }
            //Back to stage select
            else if (!hasSelected && (playerNumber == 1) && (MC.currentScreen == 2))
            {
                //MC.backToStageSelect();
            }
            //Back to top menu
            else if ((playerNumber == 1) && (MC.currentScreen == 3))
            {
                MC.returnToTop();
                //go back from stage select to stage select
            }
            else if (MC.currentScreen == 1)
            {
                //options menu
                MC.returnToTop();
                //}
            }

            // play sound
            if (MC.currentScreen == 2) AP.PlaySoundRandomPitch(AP.Find("menuCancel"));
        }
    }

    public void OnLeave(InputAction.CallbackContext action)
    {
        Leave();
    }

    public void OnChange(InputAction.CallbackContext action)
    {
        if (MC.currentScreen == 2)
        { //CHARACTER SELECT
            if (hasSelected && action.started)
            {
                float lrChangeDir = action.ReadValue<Vector2>().x;
                float udChangeDir = action.ReadValue<Vector2>().y;

                if (Mathf.Abs(lrChangeDir) > Mathf.Abs(udChangeDir))
                {
                    if (selectedHighlightingAbilities)
                    {
                        if (playerSlot == 0 && !selectingProfile)
                        {
                            if (lrChangeDir > 0)
                            {
                                monsterAbilityViewController.scrollRight();
                            }
                            else if (lrChangeDir < 0)
                            {
                                monsterAbilityViewController.scrollLeft();
                            }
                        }
                    }
                    else
                    {
                        if (playerSlot == 0 && !selectingProfile)
                        {
                            if (lrChangeDir > 0)
                            {
                                MN.pageRight();
                                monsterAbilityViewController.pageRight();
                            }
                            else if (lrChangeDir < 0)
                            {
                                MN.pageLeft();
                                monsterAbilityViewController.pageLeft();
                            }
                            PH.monsterIndex = MN.monsterIndex;
                            AP.PlaySoundRandomPitch(AP.Find("menuSwitch"));
                        }
                        else
                        {
                            int i = playerSlot - 1;
                            if (lrChangeDir > 0)
                            {
                                //WDarr[i].pageRight();
                            }
                            else if (lrChangeDir < 0)
                            {
                                //WDarr[i].pageLeft();
                            }
                            //AP.PlaySoundRandomPitch(AP.Find("menuSwitch"));
                        }
                    }
                }
                else
                {
                    //UP & DOWN
                    //Debug.Log(udChangeDir);
                    if (playerSlot == 0)
                    {
                        if (!selectedHighlightingAbilities && udChangeDir > 0 && !selectingProfile)
                        {
                            // Going up to profiles
                            selectingProfile = true;
                            MN.unselectName();

                            PH.thisES.GetComponent<MultiplayerEventSystem>().SetSelectedGameObject(thisDropdown.gameObject);
                            PH.thisES.GetComponent<MultiplayerEventSystem>().playerRoot = thisDropdown.gameObject;
                        }
                        else if (udChangeDir < 0 && selectingProfile && (PH.thisES.GetComponent<MultiplayerEventSystem>().currentSelectedGameObject == thisDropdown.gameObject))
                        {
                            // Coming Up or Down from profiles
                            PH.thisES.GetComponent<MultiplayerEventSystem>().SetSelectedGameObject(null);
                            PH.thisES.GetComponent<MultiplayerEventSystem>().playerRoot = null;
                            selectingProfile = false;

                            if (udChangeDir > 0)
                            {
                                selectedHighlightingAbilities = true;
                            }
                            else
                            {
                                selectedHighlightingAbilities = false;
                            }
                            monsterAbilityViewController.pageUpDown(selectedHighlightingAbilities);
                        }
                        else if (!selectingProfile)
                        {
                            //Swapping between highlighting abilities or not
                            selectedHighlightingAbilities = !selectedHighlightingAbilities;
                            monsterAbilityViewController.pageUpDown(selectedHighlightingAbilities);
                        }
                    }
                    else
                    {
                        if (udChangeDir > 0 && !selectingProfile)
                        {
                            selectingProfile = true;

                            PH.thisES.GetComponent<MultiplayerEventSystem>().SetSelectedGameObject(thisDropdown.gameObject);
                            PH.thisES.GetComponent<MultiplayerEventSystem>().playerRoot = thisDropdown.gameObject;
                        }
                        else if (udChangeDir < 0 && selectingProfile && (PH.thisES.GetComponent<MultiplayerEventSystem>().currentSelectedGameObject == thisDropdown.gameObject))
                        {
                            // Coming Up or Down from profiles
                            PH.thisES.GetComponent<MultiplayerEventSystem>().SetSelectedGameObject(null);
                            PH.thisES.GetComponent<MultiplayerEventSystem>().playerRoot = null;
                            selectingProfile = false;
                        }
                    }
                }
            }
        }
    }


    #endregion

    #region beta input handling
    /// <summary>
    /// Reads raw input values from assigned InputActions.
    /// </summary>
    private void ReadInputs()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        changeInput = changeAction.ReadValue<Vector2>();
        selectPressed = selectAction.triggered;
        deselectPressed = deselectAction.triggered;
        leavePressed = leaveAction.triggered;
    }

    /// <summary>
    /// Handles cursor movement input.
    /// </summary>
    private void HandleMovement()
    {
        horizontal = moveInput.x;
        vertical = moveInput.y;
        // Apply cursor movement logic here if needed
    }

    /// <summary>
    /// Handles selection input logic.
    /// </summary>
    private void HandleSelect()
    {
        if (!selectPressed) return;

        // Character select screen check
        if (MC.currentScreen == 2)
        {
            if (!MC.canMoveToGame)
            {
                // Add your character/profile/item selection logic here
            }
            else if (playerNumber == 1)
            {
                MC.loadGameplay(MC.stageSelection);
            }
        }
    }

    /// <summary>
    /// Handles deselect/back input logic.
    /// </summary>
    private void HandleDeselect()
    {
        if (!deselectPressed) return;

        if (hasSelected && MC.currentScreen == 2)
        {
            charConfirmed = false;
            deselect();
        }
        else if (playerNumber == 1 && MC.currentScreen == 3)
        {
            MC.returnToTop();
        }
    }

    /// <summary>
    /// Handles leave input logic.
    /// </summary>
    private void HandleLeave()
    {
        if (!leavePressed) return;
        Leave();
    }

    /// <summary>
    /// Handles horizontal/vertical page changes or ability highlighting.
    /// </summary>
    private void HandleChange()
    {
        if (MC.currentScreen != 2 || !hasSelected) return;

        float lr = changeInput.x;
        float ud = changeInput.y;

        // Add your horizontal/vertical navigation logic here
    }

    #endregion

    #region Player State / Exit

    public void Leave()
    {
        if (!hasSelected && !charConfirmed)
        {
            MC.hideConnected(playerNumber);
            IM.cursorList.Remove(this.gameObject);
            Destroy(PH.gameObject);
            Destroy(this.gameObject);
        }
    }

    public string GetGamepadName()
    {
        return PH.gamepadName;
    }

    #endregion

}
