using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ProfileButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public PlayerProfileManager PPManager;
    public MenuController MC;
    public PlayerProfile profile;

    public TMP_Text label;
    public Button button;

    private bool isSelected = false;

    private void Awake()
    {
        if (button == null) button = GetComponent<Button>();
        if (label == null) label = GetComponentInChildren<TMP_Text>();

        if (button != null)
            button.onClick.AddListener(OnClick);
    }

    public void Setup(PlayerProfile p, PlayerProfileManager PPMgr, MenuController menuMgr)
    {
        profile = p;
        PPManager = PPMgr;
        MC = menuMgr;
        if (label != null)
            label.text = p.Profile_Name;
    }

    private void OnClick()
    {
        MC.OpenExistingPPMenu(profile);
    }

    public void OnSelect(BaseEventData eventData)
    {

        isSelected = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {

        isSelected = false;
    }

    private void Update()
    {

        if (!isSelected) return;
        //X button on controller
        if (Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame)
        {
            MC.PromptDeleteProfile(profile);
        }
    }

}