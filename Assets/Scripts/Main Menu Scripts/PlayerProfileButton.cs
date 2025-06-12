using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProfileButton : MonoBehaviour
{
    public PlayerProfileManager manager;
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

    public void Setup(PlayerProfile p, PlayerProfileManager mgr)
    {
        profile = p;
        manager = mgr;
        if (label != null)
            label.text = p.Profile_Name;
    }

    private void OnClick()
    {
        manager.LoadProfile(profile);
    }
}