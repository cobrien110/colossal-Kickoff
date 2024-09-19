using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuCursor : MonoBehaviour
{
    [SerializeField] Rigidbody2D body;
    private float horizontal;
    private float vertical;
    private float moveLimiter = 0.7f;
    [SerializeField] private float speed = 300.0f;
    public int playerNumber = -1;
    public int playerSlot = -1;
    [SerializeField] private int menuSlot = -1;
    [SerializeField] private MenuController MC = null;
    [SerializeField] private InputManager IM = null;

    public InputAction cursorMove;
    public string hoveringItem = "null";
    public int hoveringID = -1;

    private void Start()
    {
        transform.SetParent(GameObject.Find("Canvas").transform);
        MC = GameObject.Find("MenuController").GetComponent<MenuController>();
        IM = GameObject.Find("InputManager").GetComponent<InputManager>();
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
        body.velocity = new Vector2(horizontal * speed, vertical * speed);
    }

    public void PlayerSelected(int value) {
        playerSlot = value;
        Debug.Log("Player " + playerNumber + " selected Character " + playerSlot);
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
        if (hoveringItem.Equals("playerSelect") && !IM.IsSelected(hoveringID))
        {
            PlayerSelected(hoveringID);
        } else if (hoveringItem.Equals("menuSelect") && playerNumber == 1)
        {
            MC.OptionSelect(hoveringID);
        }
    }

    public void OnDeselect(InputAction.CallbackContext action)
    {
        if (playerSlot != -1)
        {
            playerSlot = -1;
        }
    }
}
