using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioButton : MonoBehaviour
{
    [SerializeField] private int audioButtonID;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "MenuCursor")
        {
            other.gameObject.GetComponent<MenuCursor>().StartHovering("audioOptions", audioButtonID);
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
