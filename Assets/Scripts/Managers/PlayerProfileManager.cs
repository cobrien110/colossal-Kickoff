using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor.UI;
using TMPro;

public class PlayerProfileManager : MonoBehaviour
{
    [Header("Template & Storage")]
    public RandomizeProfileName RPN;
    public TextAsset defaultProfileTemplate;
    public string profileFolderName = "Player Profiles";

    private string currentProfilePath;
    private PlayerProfile currentProfile;

    public bool IsProfileLoaded => currentProfile != null;

    public void CreateNewProfile()
    {
        if (defaultProfileTemplate == null)
        {
            Debug.LogError("No profile template assigned.");
            return;
        }

        string profileName = RPN.GenerateAndGiveName();
        string folderPath = Path.Combine(Application.persistentDataPath, profileFolderName);
        Directory.CreateDirectory(folderPath);

        currentProfilePath = Path.Combine(folderPath, profileName + ".txt");

        //Create and populate profile
        currentProfile = new PlayerProfile();
        Dictionary<string, string> parsedData = ParseTextToDict(defaultProfileTemplate.text);
        currentProfile.FromDictionary(parsedData);

        SaveProfile(); //Save to disk immediately
    }

    private Dictionary<string, string> ParseTextToDict(string text)
    {
        var dict = new Dictionary<string, string>();
        string[] lines = text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            if (line.Contains(":"))
            {
                var parts = line.Split(':');
                if (parts.Length >= 2)
                {
                    string key = parts[0].Trim();
                    string value = string.Join(":", parts, 1, parts.Length - 1).Trim();
                    dict[key] = value;
                }
            }
        }

        return dict;
    }

    private void SaveProfile()
    {
        if (!IsProfileLoaded || string.IsNullOrEmpty(currentProfilePath)) return;

        var lines = new List<string>();
        foreach (var pair in currentProfile.ToDictionary())
        {
            lines.Add($"{pair.Key}: {pair.Value}");
        }

        File.WriteAllLines(currentProfilePath, lines);
    }

    public void UpdateProfileField(string fieldName, string newValue)
    {
        if (!IsProfileLoaded) return;

        //Update dictionary and reload back into class
        var dict = currentProfile.ToDictionary();
        if (dict.ContainsKey(fieldName))
        {
            dict[fieldName] = newValue;
            currentProfile.FromDictionary(dict);
            SaveProfile();
        }
        else
        {
            Debug.LogWarning($"Profile field not found: {fieldName}");
        }
    }

    public string GetProfileField(string fieldName)
    {
        if (!IsProfileLoaded) return "";

        var dict = currentProfile.ToDictionary();
        return dict.TryGetValue(fieldName, out var val) ? val : "";
    }

    public string GetCurrentProfileName()
    {
        return IsProfileLoaded ? currentProfile.Profile_Name : "None";
    }

    public void CloseProfile()
    {
        currentProfile = null;
        currentProfilePath = null;
    }

    public PlayerProfile GetActiveProfile()
    {
        return currentProfile;
    }
}