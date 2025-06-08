using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadButtonDetector : MonoBehaviour
{
    void Update()
    {
        // Check if a gamepad is connected
        if (Gamepad.current == null) return;

        var gamepad = Gamepad.current;

        // Check for button presses and print them
        if (gamepad.aButton.wasPressedThisFrame) Debug.Log("Pressed: A Button");
        if (gamepad.bButton.wasPressedThisFrame) Debug.Log("Pressed: B Button");
        if (gamepad.xButton.wasPressedThisFrame) Debug.Log("Pressed: X Button");
        if (gamepad.yButton.wasPressedThisFrame) Debug.Log("Pressed: Y Button");

        if (gamepad.leftShoulder.wasPressedThisFrame) Debug.Log("Pressed: Left Shoulder");
        if (gamepad.rightShoulder.wasPressedThisFrame) Debug.Log("Pressed: Right Shoulder");

        if (gamepad.leftTrigger.wasPressedThisFrame) Debug.Log("Pressed: Left Trigger");
        if (gamepad.rightTrigger.wasPressedThisFrame) Debug.Log("Pressed: Right Trigger");

        if (gamepad.dpad.up.wasPressedThisFrame) Debug.Log("Pressed: D-Pad Up");
        if (gamepad.dpad.down.wasPressedThisFrame) Debug.Log("Pressed: D-Pad Down");
        if (gamepad.dpad.left.wasPressedThisFrame) Debug.Log("Pressed: D-Pad Left");
        if (gamepad.dpad.right.wasPressedThisFrame) Debug.Log("Pressed: D-Pad Right");

        if (gamepad.startButton.wasPressedThisFrame) Debug.Log("Pressed: Start");
        if (gamepad.selectButton.wasPressedThisFrame) Debug.Log("Pressed: Select");

        if (gamepad.leftStickButton.wasPressedThisFrame) Debug.Log("Pressed: Left Stick Button");
        if (gamepad.rightStickButton.wasPressedThisFrame) Debug.Log("Pressed: Right Stick Button");
    }
}