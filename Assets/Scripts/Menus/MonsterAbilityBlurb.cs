using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MonsterAbilityBlurb : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI displayedBlurb;

    public void setText(string input) {
        displayedBlurb.text = input;
    }

    public void unselectBlurbs() {
        displayedBlurb.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
    }

    public void selectBlurbs() {
        displayedBlurb.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
    }

}