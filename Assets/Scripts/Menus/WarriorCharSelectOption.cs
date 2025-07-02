using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarriorCharSelectOption: MonoBehaviour
{
    [SerializeField] private Sprite[] warriorSprites;
    [SerializeField] private Shader thisShader;
    [SerializeField] private Material thisMaterial;

    void Awake() {
        GetComponent<Image>().sprite = warriorSprites[0];

        thisMaterial = new Material(thisShader);
        gameObject.GetComponent<Image>().material = thisMaterial;
    }

    public void updateSprite(int index) {
        GetComponent<Image>().sprite = warriorSprites[index];
    }

    public void updateColor(float red, float green, float blue)
    {
        thisMaterial.SetColor("_ShirtColor", new Color(red, green, blue));
    }

    public void updateSkinColor(float red, float green, float blue)
    {
        thisMaterial.SetColor("_SkinColor", new Color(red, green, blue));
    }
}
