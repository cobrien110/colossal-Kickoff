using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MonsterName : MonoBehaviour
{
    [SerializeField] public string[] monsterNames;
    [SerializeField] public TextMeshProUGUI displayedName;
    [SerializeField] private MonsterCharSelectOption visual;
    [SerializeField] private CharacterInfo charInfo;
    [SerializeField] private MenuController MC;
    [SerializeField] private int playerSlot;
    public int monsterIndex;
    private int numMonsters;
    void Start()
    {
        //selectName();
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
            Debug.Log("PAGE LEFT SET ARROWS");
            displayedName.text = "< " + monsterNames[monsterIndex] + " >";
            visual.UpdateMonster(monsterIndex);
        }
    }

    public void pageRight() {
        if (!charInfo.confirmed) {
            if (monsterIndex < numMonsters - 1) {
                monsterIndex++;
            } else {
                monsterIndex = 0;
            }
            Debug.Log("PAGE RIGHT SET ARROWS");
            displayedName.text = "< " + monsterNames[monsterIndex] + " >";
            visual.UpdateMonster(monsterIndex);
        }
    }

    public void unselectName(string name) {
        //displayedName.color = new Color(0.6981132f, 0.6981132f, 0.6981132f, 1.0f);
        displayedName.text = name;
    }

    // Overload that just gets rid of the arrows on the current name
    public void unselectName()
    {
        displayedName.text = displayedName.text.TrimStart("< ".ToCharArray()).TrimEnd(" >".ToCharArray());
    }

    public void selectName(string name) {
        //displayedName.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        Debug.Log("SELECT NAME SET ARROWS");
        displayedName.text = "< " + name + " >";
    }

}
