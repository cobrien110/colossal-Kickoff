using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StageSelectButton : MonoBehaviour
{
    /**
    0: Greece
    1: Canada
    2: Japan
    3: Mexico
    4: Egypt
    **/
    [SerializeField] private int myID;
    [SerializeField] private TextMeshProUGUI displayedName;
    [SerializeField] private string stageName;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "MenuCursor")
        {
            other.gameObject.GetComponent<MenuCursor>().StartHovering("stageSelect", myID);
            becomeSelected();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "MenuCursor")
        {
            other.gameObject.GetComponent<MenuCursor>().StopHovering();
            becomeUnselected();
        }
    }

    public void becomeSelected() {
        displayedName.text = stageName;
    }

    public void becomeUnselected() {
    }
}
