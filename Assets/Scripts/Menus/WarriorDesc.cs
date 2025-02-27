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

    [SerializeField] private Slider redSlider;
    [SerializeField] private Slider greenSlider;
    [SerializeField] private Slider blueSlider;
    [SerializeField] private float redValue;
    [SerializeField] private float greenValue;
    [SerializeField] private float blueValue;

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
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (charInfo.confirmed) {
                MC.unconfirmCharacter(playerSlot);
            } else {
                MC.confirmCharacter(playerSlot);
            }
        }
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

    public void changeRed()
    {
        redValue = redSlider.value;
    }

    public void changeGreen()
    {
        greenValue = greenSlider.value;
    }

    public void changeBlue()
    {
        blueValue = blueSlider.value;
    }

    public void UpdateColor()
    {
        WCSO.updateColor(redValue, greenValue, blueValue);
    }

    public Color getCurrentColor()
    {
        return new Color(redValue, greenValue, blueValue);
    }
}
