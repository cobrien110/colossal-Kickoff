using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class RandomizeProfileName : MonoBehaviour
{
    [Header("UI Reference")]
    public Button generateButton;

    [Header("Output")]
    public string profileName;
    public TMP_Text textField;

    [Header("Name Parts")]
    private string[] firstParts = { "Quick", "Brave", "Wacky", "Sneaky", "Mighty", "Tiny", "Cranky", "Zany", "Red", "Colossal", "Brilliant", "Ferocious", "Immortal", "stupid" };
    private string[] secondParts = { "Lizard", "Wizard", "Panther", "Knight", "Robot", "Dragon", "Slug", "Falcon", "Mino", "Akhlut", "Quetz", "Gasha", "Sphinx", "Warrior", "Commentator", "Offence", "Defence", "Goalie", "Combatant" };

    void Start()
    {
        if (generateButton != null)
        {
            generateButton.onClick.AddListener(GenerateRandomName);
        }

        //GenerateRandomName();
    }

    public void GenerateRandomName()
    {
        textField.text = GenerateAndGiveName();
    }

    public string GenerateAndGiveName()
    {
        string stichedName = "";
        if (firstParts.Length == 0 || secondParts.Length == 0)
        {
            Debug.LogWarning("Name part arrays are empty.");
            return "DefaultName";
        }

        string first = firstParts[Random.Range(0, firstParts.Length)];
        string second = secondParts[Random.Range(0, secondParts.Length)];
        stichedName = first + second;

        Debug.Log("Generated profile name: " + stichedName);

        return stichedName;
    }
}