using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WarriorDesc : MonoBehaviour
{
    [SerializeField] private string[] colorNames;
    [SerializeField] private TextMeshProUGUI displayedName;
    [SerializeField] private WarriorCharSelectOption visual;
    [SerializeField] private CharacterInfo charInfo;
    [SerializeField] private MenuController MC;
    [SerializeField] private int playerSlot;
    private int warriorColorIndex;
    private int numColors;
    void Start()
    {
        numColors = colorNames.Length;
        warriorColorIndex = 0;
    }

    //this is all placeholder, get rid of it when we have real controller input
    void Update() {
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            pageLeft();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            pageRight();
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (charInfo.confirmed) {
                MC.unconfirmCharacter(playerSlot);
            } else {
                MC.confirmCharacter(playerSlot);
            }
        }
    }

    public void pageLeft() {
        if (!charInfo.confirmed) {
            if (warriorColorIndex > 0) {
                warriorColorIndex--;
            } else {
                warriorColorIndex = numColors - 1;
            }
            displayedName.text = colorNames[warriorColorIndex];
            visual.updateSprite(warriorColorIndex);
        }
    }

    public void pageRight() {
        if (!charInfo.confirmed) {
            if (warriorColorIndex < numColors - 1) {
                warriorColorIndex++;
            } else {
                warriorColorIndex = 0;
            }
            displayedName.text = colorNames[warriorColorIndex];
            visual.updateSprite(warriorColorIndex);
        }
    }

}