using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectOption : MonoBehaviour
{
    [SerializeField] public int characterID;
    //number of cursors currently hovering over this menu option
    [SerializeField] private int cursorsOver = 0;
    [SerializeField] private UIDropShadow shadow;

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "MenuCursor") {
            other.gameObject.GetComponent<MenuCursor>().setSelected(characterID);
            cursorsOver++;
            shadow.enabled = true;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag == "MenuCursor") {
            cursorsOver--;
            if (cursorsOver <= 0) {
                shadow.enabled = false;
            }
        }
    }
    
}
