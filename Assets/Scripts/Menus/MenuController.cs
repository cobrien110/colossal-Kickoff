using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    //selected is updated by buttons when they become selected
    public int selected;
    //camera that this can move around
    [SerializeField] private MenuCamera menuCamera;
    //parent object containing all buttons from the main menu
    [SerializeField] private GameObject mainMenuButtons;
    [SerializeField] private GameObject characterSelect;
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            switch (selected) {
                //VERSUS MATCH
                case 0:
                menuCamera.goToVersusSetup();
                mainMenuButtons.SetActive(false);
                characterSelect.SetActive(true);
                break;

                //SETTINGS
                case 1:
                menuCamera.goToSettings();
                mainMenuButtons.SetActive(false);
                break;

                //QUIT GAME
                case 2:
                Debug.Log("Quitting game. Goodbye!");
                Application.Quit();
                break;

                default:
                Debug.Log("Error: unknown menu option");
                break;
            }
        }
    }
}
