using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{

    [SerializeField] private List<PlayerInput> playerInputs = new List<PlayerInput>();
    [SerializeField] public List<GameObject> cursorList = new List<GameObject>();
    [SerializeField] private MenuCursor MC = null;
    //[SerializeField] private MenuController menuController;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void NewPlayer(GameObject cursorPrefab, int playerID, Gamepad gamepad)
    {
        PlayerInput.Instantiate(cursorPrefab, controlScheme: "Xbox Control Scheme", pairWithDevice: gamepad);
        GameObject newCursor;
        GameObject[] cursors = GameObject.FindGameObjectsWithTag("MenuCursor");
        newCursor = cursors[cursors.Length - 1];
        MC = newCursor.GetComponent<MenuCursor>();
        //MC.playerNumber = cursors.Length;
        MC.playerNumber = playerID + 1;
        cursorList.Add(newCursor);
    }

    /*
     * Returns true if character is already selected
     */
    public bool IsSelected(int value)
    {
        for (int i = 0; i < cursorList.Count; i++)
        {
            int selected = cursorList[i].GetComponent<MenuCursor>().playerSlot;
            if (value == selected)
            {
                return true;
            }
        }

        return false;
    }
}
