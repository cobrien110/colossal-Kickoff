using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Windows;

public class MonsterAbilityBlurb : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] displayedBlurb;

    public void setText(string input) {
        foreach (TextMeshProUGUI text in displayedBlurb)
        {
            text.text = input;
        }

    }

    public void unselectBlurbs() {
        foreach (TextMeshProUGUI text in displayedBlurb)
        {
            text.color = new Color(0.6981132f, 0.6981132f, 0.6981132f, 1.0f);
        }
    }

    public void selectBlurbs() {
        foreach (TextMeshProUGUI text in displayedBlurb)
        {
            text.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
    }

}