using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectOption : MonoBehaviour
{
    [SerializeField] public int characterID;
    //number of cursors currently hovering over this menu option
    [SerializeField] private int cursorsOver = 0;

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "MenuCursor") {
            cursorsOver++;
            Debug.Log("turn on backdrop glow");
        }
    }
    void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag == "MenuCursor") {
            cursorsOver--;
            if (cursorsOver <= 0) {
                Debug.Log("turn on backdrop glow");
            }
        }
    }
    
}
