using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class RebindingUI : MonoBehaviour
{
    public string bindingKey;
    public Image bindingImage;
    public TMP_Text bindingText;
    public TMP_Text rebindingStatusText;
    public PlayerProfileManager profileManager;
    public MenuController MC;

    [Header("Library")]
    public GamepadSpriteLibrary spriteLibrary;

    private bool isRebinding = false;

    public void SetBindingDisplay(string input)
    {
        if (string.IsNullOrEmpty(input) || input == "none")
        {
            bindingImage.enabled = false;
            bindingText.text = "[NONE]";
        }
        else
        {
            Sprite icon = spriteLibrary?.GetSpriteForInput(input);
            if (icon != null)
            {
                bindingImage.sprite = icon;
                bindingImage.enabled = true;
                bindingText.text = "";
            }
            else
            {
                bindingImage.enabled = false;
                bindingText.text = "[" + input + "]";
            }
        }
    }

    public void StartRebinding()
    {
        if (isRebinding) return;

        isRebinding = true;
        MC.playerRebinding = true;
        bindingText.text = "Rebinding...";
        bindingImage.enabled = false;

        StartCoroutine(WaitForGamepadInput());
    }

    private IEnumerator WaitForGamepadInput()
    {
        yield return null;

        var gamepad = Gamepad.current;
        if (gamepad == null)
        {
            Debug.LogWarning("No gamepad connected.");
            yield break;
        }

        while (isRebinding)
        {
            if (CheckButton(gamepad.buttonSouth, "buttonSouth")) yield break;
            if (CheckButton(gamepad.buttonEast, "buttonEast")) yield break;
            if (CheckButton(gamepad.buttonWest, "buttonWest")) yield break;
            if (CheckButton(gamepad.buttonNorth, "buttonNorth")) yield break;

            if (CheckButton(gamepad.startButton, "startButton")) yield break;
            if (CheckButton(gamepad.selectButton, "selectButton")) yield break;

            if (CheckButton(gamepad.leftShoulder, "leftShoulder")) yield break;
            if (CheckButton(gamepad.rightShoulder, "rightShoulder")) yield break;

            if (CheckButton(gamepad.leftStickButton, "leftStick")) yield break;
            if (CheckButton(gamepad.rightStickButton, "rightStick")) yield break;

            if (CheckButton(gamepad.leftTrigger, "leftTrigger")) yield break;
            if (CheckButton(gamepad.rightTrigger, "rightTrigger")) yield break;

            if (CheckButton(gamepad.dpad.up, "dpad.up")) yield break;
            if (CheckButton(gamepad.dpad.down, "dpad.down")) yield break;
            if (CheckButton(gamepad.dpad.left, "dpad.left")) yield break;
            if (CheckButton(gamepad.dpad.right, "dpad.right")) yield break;

            yield return null;
        }
    }

    private bool CheckButton(ButtonControl button, string name)
    {
        if (button.wasPressedThisFrame)
        {
            ApplyBinding(name);
            return true;
        }
        return false;
    }

    private void ApplyBinding(string newInput)
    {
        isRebinding = false;
        MC.playerRebinding = false;
        profileManager.ChangeBinding(bindingKey, newInput);
        SetBindingDisplay(newInput);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        StartRebinding();
    }
}