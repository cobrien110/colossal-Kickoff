using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MonsterAbilityBlurb : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI displayedBlurb;


    void Start() {
        
    }

    public void setText(string input) {
        displayedBlurb.text = input;
    }

    public void unselectBlurbs() {
        displayedBlurb.color = new Color(0.6981132f, 0.6981132f, 0.6981132f, 1.0f);
    }

    public void selectBlurbs() {
        displayedBlurb.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

}