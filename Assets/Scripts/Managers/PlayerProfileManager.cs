using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor.UI;
using TMPro;

/// <summary>
/// Manages the creation, loading, saving, and editing of player profiles using a template file.
/// Handles disk persistence and communicates profile data with MenuController.
/// </summary>
public class PlayerProfileManager : MonoBehaviour
{
    [Header("Template & Storage")]
    [Tooltip("Script that generates randomized profile names.")]
    public RandomizeProfileName RPN;

    [Tooltip("Default profile template to base new profiles on.")]
    public TextAsset defaultProfileTemplate;

    [Tooltip("Subfolder under persistentDataPath where profiles are stored.")]
    public string profileFolderName = "Player Profiles";

    private string currentProfilePath;
    private PlayerProfile currentProfile;

    /// <summary>
    /// Returns true if a profile is currently loaded and active.
    /// </summary>
    public bool IsProfileLoaded => currentProfile != null;

    /// <summary>
    /// Creates a new profile using the default template and saves it to disk.
    /// </summary>
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

        currentProfile = new PlayerProfile();
        Dictionary<string, string> parsedData = ParseTextToDict(defaultProfileTemplate.text);
        currentProfile.FromDictionary(parsedData);

        SaveProfile();
    }

    /// <summary>
    /// Parses a block of text into a dictionary of key-value pairs.
    /// Used for reading template or profile file contents.
    /// </summary>
    /// <param name="text">The raw template or file content.</param>
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

    /// <summary>
    /// Saves the current profile to its assigned file path.
    /// </summary>
    private void SaveProfile()
    {
        if (!IsProfileLoaded || string.IsNullOrEmpty(currentProfilePath))
            return;

        var lines = new List<string>();
        foreach (var pair in currentProfile.ToDictionary())
        {
            lines.Add($"{pair.Key}: {pair.Value}");
        }

        File.WriteAllLines(currentProfilePath, lines);
    }

    /// <summary>
    /// Updates a specific field in the profile and immediately saves the change to disk.
    /// </summary>
    /// <param name="fieldName">The field name to update (must match profile key).</param>
    /// <param name="newValue">The new value to assign to the field.</param>
    public void UpdateProfileField(string fieldName, string newValue)
    {
        if (!IsProfileLoaded)
            return;

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

    /// <summary>
    /// Retrieves the current value of a field from the active profile.
    /// </summary>
    /// <param name="fieldName">The key to look up.</param>
    /// <returns>Field value as a string, or empty if not found.</returns>
    public string GetProfileField(string fieldName)
    {
        if (!IsProfileLoaded) return "";

        var dict = currentProfile.ToDictionary();
        return dict.TryGetValue(fieldName, out var val) ? val : "";
    }

    /// <summary>
    /// Scans the profile directory and returns a list of all saved PlayerProfile objects found.
    /// </summary>
    /// <returns>List of loaded PlayerProfile instances.</returns>
    public List<PlayerProfile> LoadAllProfiles()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, profileFolderName);

        if (!Directory.Exists(folderPath))
        {
            Debug.Log("No Player Profiles folder found.");
            return new List<PlayerProfile>();
        }

        List<PlayerProfile> profiles = new List<PlayerProfile>();
        string[] profileFiles = Directory.GetFiles(folderPath, "*.txt");

        //Start looking through and filling out
        foreach (string file in profileFiles)
        {
            string[] lines = File.ReadAllLines(file);
            Dictionary<string, string> data = new Dictionary<string, string>();

            foreach (string line in lines)
            {
                if (line.Contains(":"))
                {
                    var parts = line.Split(':');
                    if (parts.Length >= 2)
                    {
                        string key = parts[0].Trim();
                        string value = string.Join(":", parts, 1, parts.Length - 1).Trim();
                        data[key] = value;
                    }
                }
            }

            PlayerProfile profile = new PlayerProfile();
            profile.FromDictionary(data);
            profiles.Add(profile);
        }

        Debug.Log($"Loaded {profiles.Count} player profiles from disk.");
        return profiles;
    }

    /// <summary>
    /// Synchronizes UI buttons under a parent based on the provided PlayerProfile list.
    /// Adds missing buttons and removes buttons that no longer match any profile.
    /// </summary>
    /// <param name="profileList">The up-to-date list of PlayerProfile instances.</param>
    /// <param name="buttonParent">The transform under which profile buttons are placed.</param>
    /// <param name="buttonPrefab">A prefab that represents a profile button UI element.</param>
    public void SyncProfileButtonsWithList(List<PlayerProfile> profileList, Transform buttonParent, GameObject buttonPrefab)
    {
        if (buttonPrefab == null || buttonParent == null)
        {
            Debug.LogError("Missing buttonPrefab or buttonParent.");
            return;
        }

        //Build a hash set of current profile names
        HashSet<string> incomingNames = new HashSet<string>();
        foreach (var profile in profileList)
        {
            incomingNames.Add(profile.Profile_Name);
        }

        //Remove buttons that no longer match any known profile
        List<Transform> childrenToRemove = new List<Transform>();
        foreach (Transform child in buttonParent)
        {
            string childName = child.name;
            if (!incomingNames.Contains(childName))
            {
                childrenToRemove.Add(child);
            }
        }
        foreach (Transform child in childrenToRemove)
        {
            Destroy(child.gameObject);
        }

        //Track existing button names to avoid duplicates
        HashSet<string> existingNames = new HashSet<string>();
        foreach (Transform child in buttonParent)
        {
            existingNames.Add(child.name);
        }

        //Add new buttons for profiles that don't yet have one
        foreach (var profile in profileList)
        {
            if (!existingNames.Contains(profile.Profile_Name))
            {
                GameObject newButton = Instantiate(buttonPrefab, buttonParent);
                newButton.name = profile.Profile_Name;

                TMP_Text label = newButton.GetComponentInChildren<TMP_Text>();
                if (label != null)
                {
                    label.text = profile.Profile_Name;
                }

            }
        }

        Debug.Log("Profile UI sync complete. Total: " + profileList.Count + " profiles, " + buttonParent.childCount + "buttons.");
    }

    /// <summary>
    /// Returns the current profile's name, or "None" if no profile is loaded.
    /// </summary>
    public string GetCurrentProfileName()
    {
        return IsProfileLoaded ? currentProfile.Profile_Name : "None";
    }

    /// <summary>
    /// Clears the active profile and unloads the current file path.
    /// </summary>
    public void CloseProfile()
    {
        currentProfile = null;
        currentProfilePath = null;
    }

    /// <summary>
    /// Returns the currently active PlayerProfile object.
    /// </summary>
    public PlayerProfile GetActiveProfile()
    {
        return currentProfile;
    }
}