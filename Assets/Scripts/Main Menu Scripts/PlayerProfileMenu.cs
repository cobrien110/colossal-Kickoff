using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;


public class PlayerProfileMenu : MonoBehaviour
{
    [Header("Sub Menus")]
    [SerializeField] private GameObject bindingsMenu;
    [SerializeField] private GameObject configMenu;
    [SerializeField] private GameObject shirtColorMenu;
    [SerializeField] private GameObject skinColorMenu;
    [SerializeField] private GameObject playerProfileMenu;
    [SerializeField] private GameObject settingsMenu;

    [Header("First Selectable Buttons")]
    [SerializeField] private GameObject profileFirstButton;
    [SerializeField] private GameObject settingsFirstButton;

    public bool isRebinding = false;
    private Gamepad gamepad;

    void Update()
    {
        gamepad = Gamepad.current;

        if (gamepad != null && gamepad.buttonEast.wasPressedThisFrame && !isRebinding)
        {
            // Close any open submenu and return to profile menu
            if (bindingsMenu.activeSelf)
                CloseSubMenu(bindingsMenu);
            else if (configMenu.activeSelf)
                CloseSubMenu(configMenu);
            else if (shirtColorMenu.activeSelf)
                CloseSubMenu(shirtColorMenu);
            else if (skinColorMenu.activeSelf)
                CloseSubMenu(skinColorMenu);
            else if (playerProfileMenu.activeSelf)
                ReturnToSettings();
        }
    }

    public void OpenBindingsMenu()
    {
        ShowOnly(bindingsMenu);
    }

    public void OpenConfigMenu()
    {
        ShowOnly(configMenu);
    }

    public void OpenShirtColorMenu()
    {
        ShowOnly(shirtColorMenu);
    }

    public void OpenSkinColorMenu()
    {
        ShowOnly(skinColorMenu);
    }

    public void ReturnToSettings()
    {
        playerProfileMenu.SetActive(false);
        settingsMenu.SetActive(true);
        SetSelected(settingsFirstButton);
    }

    private void CloseSubMenu(GameObject submenu)
    {
        submenu.SetActive(false);
        playerProfileMenu.SetActive(true);
        SetSelected(profileFirstButton);
    }

    private void ShowOnly(GameObject menuToOpen)
    {
        bindingsMenu.SetActive(false);
        configMenu.SetActive(false);
        shirtColorMenu.SetActive(false);
        skinColorMenu.SetActive(false);

        //playerProfileMenu.SetActive(false);
        menuToOpen.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(menuToOpen.GetComponentInChildren<Selectable>()?.gameObject);
    }

    private void SetSelected(GameObject button)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(button);
    }
}