using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WarriorDesc : MonoBehaviour
{
    //[SerializeField] public string[] colorNames;
    //[SerializeField] private TextMeshProUGUI displayedName;
    [SerializeField] private WarriorCharSelectOption visual;
    [SerializeField] private CharacterInfo charInfo;
    [SerializeField] private MenuController MC;
    [SerializeField] private int playerSlot;

    [SerializeField] private float redValue = 0.0f;
    [SerializeField] private float greenValue = 0.0f;
    [SerializeField] private float blueValue = 0.0f;

    [SerializeField] private float skinRedValue = 1.0f;
    [SerializeField] private float skinGreenValue = 1.0f;
    [SerializeField] private float skinBlueValue = 1.0f;

    [SerializeField] private WarriorCharSelectOption WCSO;

    //public int warriorColorIndex;
    //private int numColors;
    void Start()
    {
        //numColors = colorNames.Length;
        //warriorColorIndex = 0;
    }

    //this is all placeholder, get rid of it when we have real controller input
    void Update() {
        //if (Input.GetKeyDown(KeyCode.UpArrow)) {
        //    pageLeft();
        //}
        //if (Input.GetKeyDown(KeyCode.DownArrow)) {
        //    pageRight();
        //}
        /*if (Input.GetKeyDown(KeyCode.Space)) {
            if (charInfo.confirmed) {
                MC.unconfirmCharacter(playerSlot);
            } else {
                MC.confirmCharacter(playerSlot);
            }
        }*/
    }

    //public void pageLeft() {
    //    if (!charInfo.confirmed) {
    //        if (warriorColorIndex > 0) {
    //            warriorColorIndex--;
    //        } else {
    //            warriorColorIndex = numColors - 1;
    //        }
    //        displayedName.text = colorNames[warriorColorIndex];
    //        visual.updateSprite(warriorColorIndex);
    //    }
    //}

    //public void pageRight() {
    //    if (!charInfo.confirmed) {
    //        if (warriorColorIndex < numColors - 1) {
    //            warriorColorIndex++;
    //        } else {
    //            warriorColorIndex = 0;
    //        }
    //        displayedName.text = colorNames[warriorColorIndex];
    //        visual.updateSprite(warriorColorIndex);
    //    }
    //}

    public void UpdateColor()
    {
        WCSO.updateColor(redValue, greenValue, blueValue);
    }

    public void UpdateSkinColor()
    {
        WCSO.updateSkinColor(skinRedValue, skinGreenValue, skinBlueValue);
    }

    public void SetColors(Color color)
    {
        redValue = color.r;
        greenValue = color.g;
        blueValue = color.b;
        UpdateColor();
    }

    public void SetSkinColors(Color color)
    {
        skinRedValue = color.r;
        skinGreenValue = color.g;
        skinBlueValue = color.b;
        UpdateSkinColor();
    }

    public void ResetColor()
    {
        WCSO.updateColor(0.0f, 0.0f, 1.0f);
        WCSO.updateSkinColor(1.0f, 1.0f, 1.0f);

        //switch (playerSlot)
        //{
        //    case 1:
        //        Debug.Log("RESETING COLORS FOR 1");
        //        WCSO.updateColor(1.0f, 0.0f, 0.0f);
        //        break;
        //    case 2:
        //        WCSO.updateColor(0.0f, 1.0f, 0.0f);
        //        break;
        //    case 3:
        //        WCSO.updateColor(0.0f, 0.0f, 1.0f);
        //        break;
        //    default:
        //        WCSO.updateColor(0.0f, 0.0f, 0.0f);
        //        break;
        //}
    }

    public Color getCurrentColor()
    {
        return new Color(redValue, greenValue, blueValue);
    }

    public Color getCurrentSkinColor()
    {
        return new Color(skinRedValue, skinGreenValue, skinBlueValue);
    }
}
