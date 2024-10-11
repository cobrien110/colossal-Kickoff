using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoreButton : MonoBehaviour
{
    [SerializeField] public int goreButtonID;
    [SerializeField] private Sprite[] goreButtonSprites;
    void Start()
    {
        if (PlayerPrefs.GetInt("goreMode", 0) == goreButtonID) {
            selectOption();
        }
    }

    public void selectOption() {
        GetComponent<Image>().sprite = goreButtonSprites[1];
    }

    public void unselectOption() {
        GetComponent<Image>().sprite = goreButtonSprites[0];
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "MenuCursor")
        {
            other.gameObject.GetComponent<MenuCursor>().StartHovering("goreSelect", goreButtonID);
            //becomeSelected();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "MenuCursor")
        {
            other.gameObject.GetComponent<MenuCursor>().StopHovering();
            //becomeUnselected();
        }
    }

}
