using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButton : MonoBehaviour, IPointerEnterHandler
{
    //each button saves its own positions it changes between
    //when it's selected or not
    [SerializeField] private float unselectedXPos;
    [SerializeField] private float unselectedYPos;
    [SerializeField] private float selectedXPos;
    [SerializeField] private float selectedYPos;
    //array exists so all other buttons can become unselected
    //when a button becomes selected
    [SerializeField] private GameObject[] otherButtons;
    [SerializeField] private MenuController menuController;
    /**
    myID:
    0: Versus Match
    1: Settings
    2: Quit Game
    **/
    [SerializeField] private int myID;

    //These buttons contain code for having themselves become highlighted or not
    //MenuController handles actually selecting those options and moving
    //between menus
    void Start() {
        becomeUnselected();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        becomeSelected();
        for (int i = 0; i < otherButtons.Length; i++) {
            otherButtons[i].GetComponent<MenuButton>().becomeUnselected();
        }
    }
    public void becomeSelected() {
        GetComponent<RectTransform>().anchoredPosition = new Vector3(selectedXPos, selectedYPos, 0);
        menuController.selected = myID;
    }

    public void becomeUnselected() {
        GetComponent<RectTransform>().anchoredPosition = new Vector3(unselectedXPos, unselectedYPos, 0);
    }
}
