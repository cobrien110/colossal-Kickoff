using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    [SerializeField] private List<PlayerInput> playerInputs = new List<PlayerInput>();
    [SerializeField] private List<GameObject> cursorList = new List<GameObject>();
    [SerializeField] private MenuCursor MC = null;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void NewPlayer(GameObject cursorPrefab, int playerID)
    {
        PlayerInput.Instantiate(cursorPrefab, controlScheme: "Xbox Control Scheme", pairWithDevice: Gamepad.all[playerID]);
        GameObject newCursor;
        GameObject[] cursors = GameObject.FindGameObjectsWithTag("MenuCursor");
        newCursor = cursors[cursors.Length - 1];
        MC = newCursor.GetComponent<MenuCursor>();
        MC.playerNumber = cursors.Length;
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
