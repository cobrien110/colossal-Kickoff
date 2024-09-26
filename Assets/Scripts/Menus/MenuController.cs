using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    //selected is updated by buttons when they become selected
    public int selected;
    //camera that this can move around
    [SerializeField] private MenuCamera menuCamera;
    //parent object containing all buttons from the main menu
    [SerializeField] private GameObject mainMenuButtons;
    [SerializeField] private GameObject characterSelect;
    [SerializeField] private GameObject[] cursors;
    private SceneManager SM;

    void Update()
    {
        //if (Input.GetMouseButtonDown(0)) {
        //    switch (selected) {
        //        //VERSUS MATCH
        //        case 0:
        //        menuCamera.goToVersusSetup();
        //        mainMenuButtons.SetActive(false);
        //        characterSelect.SetActive(true);
        //        break;

        //        //SETTINGS
        //        case 1:
        //        menuCamera.goToSettings();
        //        mainMenuButtons.SetActive(false);
        //        break;

        //        //QUIT GAME
        //        case 2:
        //        Debug.Log("Quitting game. Goodbye!");
        //        Application.Quit();
        //        break;

        //        default:
        //        Debug.Log("Error: unknown menu option");
        //        break;
        //    }
        //}

        //WIP
        if (Input.GetKey(KeyCode.Return))
        {
            for (int i = 0; i < cursors.Length; i++)
            {
                string currentPlayer = "Player" + i;
                MenuCursor currentCursor = cursors[i].GetComponent<MenuCursor>();
                PlayerPrefs.SetInt(currentPlayer, currentCursor.playerSlot);
            }
            SceneManager.LoadScene("GameplayScene");
        }

    }

    public void OptionSelect(int optionID)
    {
        findAllCursors();
        switch (optionID)
        {
            //VERSUS MATCH
            case 0:
                menuCamera.goToVersusSetup();
                mainMenuButtons.SetActive(false);
                characterSelect.SetActive(true);
                for (int i = 0; i < cursors.Length; i++) {
                    cursors[i].GetComponent<MenuCursor>().findCharSelectItems();
                }
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

    private void findAllCursors() {
        cursors = GameObject.FindGameObjectsWithTag("MenuCursor");
    }
}
