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
            textBox.text = "Let loose a super-charged kick that stuns the Monster and pierces the goal barrier when fully charged! Charge it by passing to each other.";
        }
        else if (index == 1)
        {
            textBox.text = "Slide for brief invulnerability and to steal the ball from the Monster! While you have the ball, juke to avoid attacks!";
        }
        else if (index == 2)
        {
            textBox.text = "Call for a pass from your teammates! Computer players will automatically pass to you.";
        }
        else
        {
            textBox.text = "Team up as the 3 Warriors!";
        }
    }

}
