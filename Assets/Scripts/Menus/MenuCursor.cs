using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class MenuCursor : MonoBehaviour
{
    [SerializeField] Rigidbody2D body;
    private float horizontal;
    private float vertical;
    //private float moveLimiter = 0.7f;
    [SerializeField] private float speed = 500.0f;
    public int playerNumber = -1;
    public int playerSlot = -1;
    public WarriorDesc WD = null;
    [SerializeField] private int menuSlot = -1;
    [SerializeField] private int warriorColor = -1;
    [SerializeField] private MenuController MC = null;
    [SerializeField] private InputManager IM = null;
    //[SerializeField] private VolumeManager VM = null;
    [SerializeField] private MonsterName MN = null;
    [SerializeField] private MonsterAbilityBlurb abilityBlurb = null;
    [SerializeField] private MonsterAbilityViewController monsterAbilityViewController = null;
    //[SerializeField] private WarriorDesc[] WDarr = null;
    [SerializeField] private PlayerSelectedDisplay[] playerMarkerIcons;
    [SerializeField] private Sprite[] cursorSprites;

    [SerializeField] private PlayerHolder PH;
    [SerializeField] private GameObject[] playerHolders;
    [SerializeField] private int colorIndex = -1;

    public InputAction cursorMove;
    public string hoveringItem = "null";
    public int hoveringID = -1;
    public bool hasSelected = false;
    public bool charConfirmed = false;
    private Vector3 savedPosition;
    private Vector3 screenMidpoint;
    private bool selectedHighlightingAbilities = false;

    //sounds
    [SerializeField] private AudioPlayer AP;
    //private bool willPlaySelectSound = false;

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

        if (MC.currentScreen == 2) {
            showCursor();
            findCharSelectItems();
        } else {
            hideCursor();
        }
        GetComponent<Image>().sprite = cursorSprites[playerNumber - 1];

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
        transform.position = new Vector3 (325, 185, 0);

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

    void Update()
    {
        // Gives a value between -1 and 1
        //horizontal = Input.GetAxisRaw("Horizontal"); // -1 is left
        //vertical = Input.GetAxisRaw("Vertical"); // -1 is down
    }

    void FixedUpdate()
    {
        //if (horizontal != 0 && vertical != 0) // Check for diagonal movement
        //{
        //    // limit movement speed diagonally, so you move at 70% speed
        //    horizontal *= moveLimiter;
        //    vertical *= moveLimiter;
        //} 
        if ((!hasSelected) || MC.currentScreen == 3) {
            body.velocity = new Vector2(horizontal * speed, vertical * speed);
        }
        //Rect screenBounds = new Rect(0f, 0f, Screen.width, Screen.height); // Screen space bounds (assumes camera renders across the entire screen)
        //bounding box
        if (transform.position.y > Screen.height) {
            transform.position = new Vector3(transform.position.x, Screen.height, 0);
        }
        if (transform.position.y < 0) {
            transform.position = new Vector3(transform.position.x, 0, 0);
        }
        if (transform.position.x < 0) {
            transform.position = new Vector3(0, transform.position.y, 0);
        }
        if (transform.position.x > Screen.width) {
            transform.position = new Vector3(Screen.width, transform.position.y, 0);
        }
    }

    public void hideCursor() {
        this.GetComponent<Image>().enabled = false;
        body.velocity = new Vector2(0, 0);
    }

    public void showCursor() {
        transform.position = screenMidpoint;
        this.GetComponent<Image>().enabled = true;
    }

    public void PlayerSelected(int value) {
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
        //cursorMove.Disable();
        MC.characterSelected(playerNumber, playerSlot);

        if (playerSlot == 0)
        {
            //MC.monsterAbilityCanHover = false;
            MN.selectName(MN.monsterNames[MN.monsterIndex]);
        } else if (playerSlot == 1)
        {
            WD = GameObject.Find("Warrior1Color").GetComponent<WarriorDesc>();
        } else if (playerSlot == 2)
        {
            WD = GameObject.Find("Warrior2Color").GetComponent<WarriorDesc>();
        } else if (playerSlot == 3)
        {
            WD = GameObject.Find("Warrior3Color").GetComponent<WarriorDesc>();
        }

        if (playerSlot != 0)
        {
            PH.SetEvents(WD);
        }
    }

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

    public void OnMove(InputAction.CallbackContext action)
    {
        horizontal = action.ReadValue<Vector2>().x;
        vertical = action.ReadValue<Vector2>().y;
    }

    public void OnSelect(InputAction.CallbackContext action)
    {
        bool justSelectedChar = false;
        if (action.started)
        {
            /**if (MC.currentScreen == 0) {
                //Top Menu
                if (hoveringItem.Equals("menuSelect") && playerNumber == 1) {
                    MC.OptionSelect(hoveringID);
                }
            } else**/ 
            if (MC.currentScreen == 2) {
                //Character Select
                if (!MC.canMoveToGame) {
                    if (!hasSelected) {
                        if (hoveringItem.Equals("playerSelect") && !IM.IsSelected(hoveringID))
                        {
                            if (!playerMarkerIcons[hoveringID].isAssigned)
                            {
                                PlayerSelected(hoveringID);
                                playerMarkerIcons[hoveringID].changeSprite(playerNumber);
                            }
                        }
                    }
                    else if (!charConfirmed) {
                        charConfirmed = true;
                        MC.confirmCharacter(playerSlot);
                        // play sound
                        AP.PlaySoundRandomPitch(AP.Find("menuStart2"));
                        justSelectedChar = true;
                        AP.PlaySoundRandomPitch(AP.Find("menuSelect"));
                        //Temp Color code
                        if (playerSlot != 0)
                        {
                            //colorIndex = WDarr[playerSlot - 1].warriorColorIndex;
                            //switch (colorIndex)
                            //{
                            //    case 0:
                            //        PH.warriorColor = Color.red;
                            //        break;
                            //    case 1:
                            //        PH.warriorColor = Color.green;
                            //        break;
                            //    case 2:
                            //        PH.warriorColor = Color.blue;
                            //        break;
                            //    case 3:
                            //        PH.warriorColor = Color.yellow;
                            //        break;
                            //    case 4:
                            //        PH.warriorColor = Color.magenta;
                            //        break;
                            //    default:
                            //        PH.warriorColor = Color.black;
                            //        break;
                            //}
                            PH.warriorColor = WD.getCurrentColor();
                        }
                    }
                } else if (playerNumber == 1) {
                    MC.loadGameplay(MC.stageSelection);
                }
                // play sound
                //if (!justSelectedChar) AP.PlaySoundRandomPitch(AP.Find("menuSelect"));
            } else
            {
                // play sound
                //AP.PlaySoundRandomPitch(AP.Find("menuClick"));
            }
            
            /**else if (MC.currentScreen == 3) {
                //Stage Select
                if (hoveringItem.Equals("stageSelect") && playerNumber == 1) {
                    //TODO: Loading Gameplay and grabbing controller info and all that junk
                    MC.loadGameplay(hoveringID);
                }
            } else if (MC.currentScreen == 1) {
                //Options Menu
                if (hoveringItem.Equals("goreSelect") && playerNumber == 1) {
                    MC.setGore(hoveringID);
                }
                if (hoveringItem.Equals("audioOptions") && playerNumber == 1) {
                    if (!hasSelected) {
                        enterAudio(hoveringID);
                    }
                }
            }
            **/
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
            } else if (MC.currentScreen == 1) {
                //options menu
                /**if (hasSelected) {
                    //VM.unselect();
                    hasSelected = false;
                    this.GetComponent<Image>().enabled = true;
                } else {**/
                    MC.returnToTop();
                //}
            }

            // play sound
            if (MC.currentScreen == 2) AP.PlaySoundRandomPitch(AP.Find("menuCancel"));
        }
    }

    public void deselect() {
        //cursorMove.Enable();
        if (selectedHighlightingAbilities)
        {
            selectedHighlightingAbilities = false;
            //monsterAbilityViewController.pageUpDown(false);
        }
        if (hasSelected)
        {
            playerMarkerIcons[playerSlot].changeSprite(0);
            MC.characterUnselected(playerNumber, playerSlot);
            MN.unselectName(MN.monsterNames[MN.monsterIndex]);
        }

        PH.RemoveEvents();
        PH.teamName = "";
        hasSelected = false;
        this.GetComponent<Image>().enabled = true;
        playerSlot = -1;
        PH.warriorPosition = -1;
        WD = null;
        MC.deselectOccured = true;
    }

    //find the icons that display who's selected which characters on screen
    public void findCharSelectItems() {
        Debug.Log("findcharselectitems running");
        playerMarkerIcons[0] = GameObject.Find("MonsterSelected").GetComponent<PlayerSelectedDisplay>();
        playerMarkerIcons[1] = GameObject.Find("Warrior1Selected").GetComponent<PlayerSelectedDisplay>();
        playerMarkerIcons[2] = GameObject.Find("Warrior2Selected").GetComponent<PlayerSelectedDisplay>();
        playerMarkerIcons[3] = GameObject.Find("Warrior3Selected").GetComponent<PlayerSelectedDisplay>();
    }

    public void OnChange(InputAction.CallbackContext action)
    {
        /**if (MC.currentScreen == 1) { //OPTIONS MENU
            if (hasSelected && action.started) {
                float changeDir = action.ReadValue<Vector2>().x;
                if (changeDir > 0)
                {
                    VM.pageRight();
                }
                else if (changeDir < 0)
                {
                    VM.pageLeft();
                }
            }
        } else**/ if (MC.currentScreen == 2) { //CHARACTER SELECT
            if (hasSelected && action.started)
            {
                float lrChangeDir = action.ReadValue<Vector2>().x;
                float udChangeDir = action.ReadValue<Vector2>().y;

                if (Mathf.Abs(lrChangeDir) > Mathf.Abs(udChangeDir)) {
                    if (selectedHighlightingAbilities)
                    {
                        if (playerSlot == 0)
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
                    } else
                    {
                        if (playerSlot == 0)
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
                } else {
                    //UP & DOWN
                    //Debug.Log(udChangeDir);
                    if (playerSlot == 0)
                    {
                        selectedHighlightingAbilities = !selectedHighlightingAbilities;
                        monsterAbilityViewController.pageUpDown(selectedHighlightingAbilities);
                        /**if (selectedHighlightingAbilities) {
                            abilityBlurb.selectBlurbs();
                            MN.unselectName();
                        } else {
                            abilityBlurb.unselectBlurbs();
                            MN.selectName();
                        }**/
                    }
                }
            }   
        }
    }

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

    public void OnLeave(InputAction.CallbackContext action)
    {
        Leave();
    }

    public string GetGamepadName()
    {
        return PH.gamepadName;
    }

    /**public void enterAudio(int optionSelected) {
        hasSelected = true;
        this.GetComponent<Image>().enabled = false;
        body.velocity = new Vector2(0, 0);
        VM.select(optionSelected);
    }**/
}
