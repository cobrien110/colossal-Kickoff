using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarriorCharSelectOption: MonoBehaviour
{
    [SerializeField] private Sprite[] warriorSprites;

    void Start() {
        GetComponent<Image>().sprite = warriorSprites[0];
    }

    public void updateSprite(int index) {
        GetComponent<Image>().sprite = warriorSprites[index];
    }

    public void updateColor(float red, float green, float blue)
    {
        gameObject.GetComponent<Image>().material.color = new Color(red, green, blue);
    }
}
