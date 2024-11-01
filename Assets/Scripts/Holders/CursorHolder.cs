using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorHolder : MonoBehaviour
{
    [SerializeField] private GameObject cursorPrefab;
    [SerializeField] private InputManager IM;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void spawnCursor(int playerID, Gamepad gamepad)
    {
        //PlayerInput.Instantiate(cursorPrefab, controlScheme: "Xbox Control Scheme", pairWithDevice: Gamepad.all[0]);
        IM.NewPlayer(cursorPrefab, playerID, gamepad);
    }
}
