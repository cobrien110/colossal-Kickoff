using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MenuCursor : MonoBehaviour
{
    [SerializeField] Rigidbody2D body;
    private float horizontal;
    private float vertical;
    private float moveLimiter = 0.7f;
    [SerializeField] private float speed = 500.0f;
    public int playerNumber = -1;
    public int playerSlot = -1;
    [SerializeField] private int menuSlot = -1;
    [SerializeField] private MenuController MC = null;
    [SerializeField] private InputManager IM = null;
    [SerializeField] private VolumeManager VM = null;
    [SerializeField] private MonsterName MN = null;
    [SerializeField] private WarriorDesc[] WDarr = null;
    [SerializeField] private PlayerSelectedDisplay[] playerMarkerIcons;
    [SerializeField] private Sprite[] cursorSprites;

    [SerializeField] private PlayerHolder PH;
    [SerializeField] private GameObject[] playerHolders;

    public InputAction cursorMove;
    public string hoveringItem = "null";
    public int hoveringID = -1;
    public bool hasSelected = false;
    private Vector3 savedPosition;
    private bool charConfirmed = false;


    private void Start()
    {
        hasSelected = false;
        transform.SetParent(GameObject.Find("Canvas").transform);
        transform.position = new Vector3(0, 0, 0);
        MC = GameObject.Find("MenuController").GetComponent<MenuController>();
        MC.findAllCursors();
        IM = GameObject.Find("InputManager").GetComponent<InputManager>();
        VM = FindObjectOfType<VolumeManager>(true);
        MN = FindObjectOfType<MonsterName>(true);
        WDarr = FindObjectsOfType<WarriorDesc>(true);
        WarriorDesc holder = WDarr[1];
        WDarr[1] = WDarr[2];
        WDarr[2] = holder;
        //findCharSelectItems();
        GetComponent<Image>().sprite = cursorSprites[playerNumber - 1];

        playerHolders = GameObject.FindGameObjectsWithTag("PlayerHolder");
        PH = playerHolders[playerHolders.Length - 1].GetComponent<PlayerHolder>();
        Debug.Log("Cursor " + playerNumber + " with PlayerHolder " + PH.playerID);
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
        }

        this.GetComponent<Image>().enabled = false;
        body.velocity = new Vector2(0, 0);
        //cursorMove.Disable();
        MC.characterSelected(playerNumber, playerSlot);
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
        if (action.started)
        {
            if (MC.currentScreen == 0) {
                //Top Menu
                if (hoveringItem.Equals("menuSelect") && playerNumber == 1) {
                    MC.OptionSelect(hoveringID);
                }
            } else if (MC.currentScreen == 2) {
                //Character Select
                if (!MC.canMoveToStageSelect) {
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
                    }
                } else if (playerNumber == 1) {
                    this.GetComponent<Image>().enabled = true;
                    savedPosition = transform.position;
                    MC.moveToStageSelect();
                }
            } else if (MC.currentScreen == 3) {
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
            //Back to top menu
            else if (!hasSelected && (playerNumber == 1) && (MC.currentScreen == 2))
            {
                Debug.Log("Return to top");
                MC.returnToTop();
            }
            //Back to character select
            else if ((playerNumber == 1) && (MC.currentScreen == 3))
            {
                //go back from stage select to character select
                transform.position = savedPosition;
                this.GetComponent<Image>().enabled = false;
                body.velocity = new Vector2(0, 0);
                MC.backToCharSelect();
            } else if (MC.currentScreen == 1) {
                //options menu
                if (hasSelected) {
                    VM.unselect();
                    hasSelected = false;
                    this.GetComponent<Image>().enabled = true;
                } else {
                    MC.returnToTop();
                }
            }
        }
    }

    public void deselect() {
            //cursorMove.Enable();
            if (hasSelected) {
                playerMarkerIcons[playerSlot].changeSprite(0);
                MC.characterUnselected(playerNumber, playerSlot);
            }

            PH.teamName = "";
            hasSelected = false;
            this.GetComponent<Image>().enabled = true;
            playerSlot = -1;
    }

    //find the icons that display who's selected which characters on screen
    public void findCharSelectItems() {
        playerMarkerIcons[0] = GameObject.Find("MonsterSelected").GetComponent<PlayerSelectedDisplay>();
        playerMarkerIcons[1] = GameObject.Find("Warrior1Selected").GetComponent<PlayerSelectedDisplay>();
        playerMarkerIcons[2] = GameObject.Find("Warrior2Selected").GetComponent<PlayerSelectedDisplay>();
        playerMarkerIcons[3] = GameObject.Find("Warrior3Selected").GetComponent<PlayerSelectedDisplay>();
    }

    public void OnChange(InputAction.CallbackContext action)
    {
        if (MC.currentScreen == 1) { //OPTIONS MENU
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
        } else if (MC.currentScreen == 2) { //CHARACTER SELECT
            if (hasSelected && action.started)
            {
                float changeDir = action.ReadValue<Vector2>().x;
                if (playerSlot == 0)
                {
                    if (changeDir > 0)
                    {
                        MN.pageRight();
                    }
                    else if (changeDir < 0)
                    {
                        MN.pageLeft();
                    }
                    PH.monsterIndex = MN.monsterIndex;
                } else
                {
                    int i = playerSlot - 1;
                    if (changeDir > 0)
                    {
                        WDarr[i].pageRight();
                    }
                    else if (changeDir < 0)
                    {
                        WDarr[i].pageLeft();
                    }
                }
            }   
        }
    }

    public void enterAudio(int optionSelected) {
        hasSelected = true;
        this.GetComponent<Image>().enabled = false;
        body.velocity = new Vector2(0, 0);
        VM.select(optionSelected);
    }
}
