using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class RandomizeProfileName : MonoBehaviour
{
    [Header("Output")]
    public string profileName;
    public TMP_Text textField;

    [Header("Name Parts")]
    private string[] firstParts = { "Quick", "Brave", "Wacky", "Sneaky", "Mighty", "Tiny", "Cranky", "Zany", "Red", "Colossal", "Brilliant", "Ferocious", "Immortal", "stupid" };
    private string[] secondParts = { "Lizard", "Wizard", "Panther", "Knight", "Robot", "Dragon", "Slug", "Falcon", "Mino", "Akhlut", "Quetz", "Gasha", "Sphinx", "Warrior", "Commentator", "Offence", "Defence", "Goalie", "Combatant" };


    public void GenerateRandomName()
    {
        textField.text = GenerateAndGiveName();
    }

    public string NewProfileName(List<PlayerProfile> savedProfiles)
    {
        HashSet<string> existingNames = new HashSet<string>();
        foreach (var profile in savedProfiles)
        {
            existingNames.Add(profile.Profile_Name);
        }

        string baseName = GenerateAndGiveName();
        string finalName = baseName;

        int suffix = 2;
        while (existingNames.Contains(finalName))
        {
            finalName = baseName + suffix;
            suffix++;
        }

        if (textField != null)
            textField.text = finalName;

        return finalName;
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