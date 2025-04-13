using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WarriorAbilityHover : MonoBehaviour
{
    [SerializeField] private int abilityIndex;
    [SerializeField] private TMP_Text textBox;
    [SerializeField] private GameObject highlight;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "MenuCursor")
        {
            SetText(abilityIndex);
            highlight.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "MenuCursor")
        {
            SetText(-1);
            highlight.SetActive(false);
        }
    }

    private void SetText(int index)
    {
        if (index == 0)
        {
            textBox.text = "SUPER KICK";
        }
        else if (index == 1)
        {
            textBox.text = "SLIDE JUKE";
        }
        else if (index == 2)
        {
            textBox.text = "CALL FOR PASS";
        }
        else
        {
            textBox.text = "BASE WARRIOR TEXT";
        }
    }

}
