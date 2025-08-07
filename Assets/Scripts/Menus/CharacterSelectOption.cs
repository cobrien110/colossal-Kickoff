using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectOption : MonoBehaviour
{
    [SerializeField] public int characterID;
    [SerializeField] private int cursorsOver = 0;
    [SerializeField] private GameObject highlightObject;

    public bool canBeSelected = true;

    void Start()
    {
        if (highlightObject != null)
            highlightObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("MenuCursor"))
        {
            other.GetComponent<MenuCursor>().StartHovering("playerSelect", characterID);
            cursorsOver++;

            if (highlightObject != null)
                highlightObject.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (canBeSelected && other.CompareTag("MenuCursor"))
        {
            other.GetComponent<MenuCursor>().StopHovering();
            cursorsOver--;

            if (cursorsOver <= 0 && highlightObject != null)
                highlightObject.SetActive(false);
        }
    }
}