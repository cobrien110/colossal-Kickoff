using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MonsterName : MonoBehaviour
{
    [SerializeField] private string[] monsterNames;
    [SerializeField] private TextMeshProUGUI displayedName;
    [SerializeField] private MonsterCharSelectOption visual;
    [SerializeField] private CharacterInfo charInfo;
    [SerializeField] private MenuController MC;
    [SerializeField] private int playerSlot;
    public int monsterIndex;
    private int numMonsters;
    void Start()
    {
        numMonsters = monsterNames.Length;
        monsterIndex = 0;
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
            if (monsterIndex > 0) {
                monsterIndex--;
            } else {
                monsterIndex = numMonsters - 1;
            }
            displayedName.text = monsterNames[monsterIndex];
            visual.updateSprite(monsterIndex);
        }
    }

    public void pageRight() {
        if (!charInfo.confirmed) {
            if (monsterIndex < numMonsters - 1) {
                monsterIndex++;
            } else {
                monsterIndex = 0;
            }
            displayedName.text = monsterNames[monsterIndex];
            visual.updateSprite(monsterIndex);
        }
    }

    public void unselectName() {
        displayedName.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
    }

    public void selectName() {
        displayedName.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
    }

}
