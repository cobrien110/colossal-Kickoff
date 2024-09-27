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
    [SerializeField] private MonsterName MN = null;
    [SerializeField] private PlayerSelectedDisplay[] playerMarkerIcons;
    [SerializeField] private Sprite[] cursorSprites;

    public InputAction cursorMove;
    public string hoveringItem = "null";
    public int hoveringID = -1;
    public bool hasSelected = false;


    private void Start()
    {
        hasSelected = false;
        transform.SetParent(GameObject.Find("Canvas").transform);
        transform.position = new Vector3(0, 0, 0);
        MC = GameObject.Find("MenuController").GetComponent<MenuController>();
        MC.findAllCursors();
        IM = GameObject.Find("InputManager").GetComponent<InputManager>();
        MN = FindObjectOfType<MonsterName>(true);
        //findCharSelectItems();
        GetComponent<Image>().sprite = cursorSprites[playerNumber - 1];
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
        if (!hasSelected) {
            body.velocity = new Vector2(horizontal * speed, vertical * speed);
        }
    }

    public void PlayerSelected(int value) {
        playerSlot = value;
        hasSelected = true;
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
        if (!hasSelected) {
            if (hoveringItem.Equals("playerSelect") && !IM.IsSelected(hoveringID))
            {
                if (!playerMarkerIcons[hoveringID].isAssigned) {
                    PlayerSelected(hoveringID);
                    playerMarkerIcons[hoveringID].changeSprite(playerNumber);
                }
            } else if (hoveringItem.Equals("menuSelect") && playerNumber == 1)
            {
                MC.OptionSelect(hoveringID);
            }
        } else
        {
            MC.confirmCharacter(playerSlot);
        }
    }

    public void OnDeselect(InputAction.CallbackContext action)
    {
        if (hasSelected && action.started)
        {
            Debug.Log("Deselecting");
            deselect();
        }

        if (!hasSelected && (playerNumber == 1) && (MC.currentScreen == 2)) {
            Debug.Log("Return to top");
            MC.returnToTop();
        }

        if ((playerNumber == 1) && (MC.currentScreen == 3)) {
            //TODO: Return to Character Select
        }
    }

    public void deselect() {
            //cursorMove.Enable();
            if (hasSelected) {
                playerMarkerIcons[playerSlot].changeSprite(0);
                MC.characterUnselected(playerNumber, playerSlot);
            }
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
        if (hasSelected && action.started)
        {
            float changeDir = action.ReadValue<Vector2>().x;
            if (changeDir > 0)
            {
                MN.pageRight();
            } else if (changeDir < 0)
            {
                MN.pageLeft();
            }
        }
    }
}
