using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProfileButton : MonoBehaviour
{
    public PlayerProfileManager PPManager;
    public MenuController MC;
    public PlayerProfile profile;

    public TMP_Text label;
    public Button button;

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
}