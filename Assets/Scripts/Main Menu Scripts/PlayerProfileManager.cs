using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor.UI;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Manages the creation, loading, saving, and editing of player profiles using a template file.
/// Handles disk persistence and communicates profile data with MenuController.
/// </summary>
public class PlayerProfileManager : MonoBehaviour
{
    [Header("Template & Storage")]
    [Tooltip("Script that generates randomized profile names.")]
    public RandomizeProfileName RPN;
    public MenuController MC;
    private AudioPlayer AP;

    [Tooltip("Default profile template to base new profiles on.")]
    public TextAsset defaultProfileTemplate;

    [Tooltip("Subfolder under persistentDataPath where profiles are stored.")]
    public string profileFolderName = "Player Profiles";

    private string currentProfilePath;
    public PlayerProfile currentProfile;

    private Dictionary<ProfileButton, PlayerProfile> activeButtons = new Dictionary<ProfileButton, PlayerProfile>();

    [Header("UI Content Updating")]
    [SerializeField] Selectable createNewProfileButton;
    [SerializeField] Selectable playerTabButton;
    [SerializeField] Selectable backButton;

    [Header("Player Profile Menu Elements")]
    [SerializeField] private GameObject profileNameButton;

    [SerializeField] private HSVSlider shirtColorScript;
    [SerializeField] private HSVSlider skinColorScript;

    [SerializeField] private GameObject kickModeDropdown;
    [SerializeField] private Slider sliderDeadzoneAdjustment;
    [SerializeField] private TMP_Text deadzoneAdjNum;

    [SerializeField] private GameObject movementStickDropdown;
    [SerializeField] private GameObject aimingStickDropdown;

    [SerializeField] private List<RebindingUI> rebindButtons;


    /// <summary>
    /// Returns true if a profile is currently loaded and active.
    /// </summary>
    public bool IsProfileLoaded => currentProfile != null;

    /// <summary>
    /// Creates a new profile using the default template and saves it to disk.
    /// </summary>
    public void CreateNewProfile(List<PlayerProfile> savedProfiles)
    {
        if (defaultProfileTemplate == null)
        {
            Debug.LogError("No profile template assigned.");
            return;
        }

        string fileID = GenerateUniqueFilename();
        string profileName = RPN.NewProfileName(savedProfiles);

        string folderPath = Path.Combine(Application.persistentDataPath, profileFolderName);
        Directory.CreateDirectory(folderPath);

        string fullPath = Path.Combine(folderPath, fileID + ".txt");

        currentProfile = new PlayerProfile();
        currentProfile.FilePath = fullPath;

        Dictionary<string, string> parsedData = ParseTextToDict(defaultProfileTemplate.text);
        currentProfile.FromDictionary(parsedData);

        //Save new name
        currentProfile.Profile_Name = profileName;

        //Save to disk
        SaveProfile();

        LoadInProfileUI();
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
            profile.FilePath = file;
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

        // Clean existing buttons
        foreach (Transform child in buttonParent)
        {
            Destroy(child.gameObject);
        }

        activeButtons.Clear();
        List<Selectable> buttonSelectables = new List<Selectable>();

        foreach (var profile in profileList)
        {
            GameObject buttonObj = Instantiate(buttonPrefab, buttonParent);
            ProfileButton profileButton = buttonObj.GetComponent<ProfileButton>();
            if (profileButton != null)
            {
                profileButton.Setup(profile, this, MC);
                activeButtons.Add(profileButton, profile);
                
                Selectable sel = buttonObj.GetComponent<Selectable>();
                if (sel != null)
                {
                    buttonSelectables.Add(sel);
                }
            }
            else
            {
                Debug.LogWarning("Profile prefab missing ProfileButton component.");
            }
        }
        //build nav
        UpdateButtonNav(buttonSelectables);
        Debug.Log($"Synced {activeButtons.Count} profile buttons.");
    }

    private void UpdateButtonNav(List<Selectable> buttons)
    {
        if (buttons == null || buttons.Count == 0) return;

        int columnCount = buttons.Count / 3 + (buttons.Count % 3 == 0 ? 0 : 1);

        for (int i = 0; i < buttons.Count; i++)
        {
            var nav = new Navigation { mode = Navigation.Mode.Explicit };

            // Determine column and row
            int col = i / 3;
            int row = i % 3;

            // Horizontal
            // LEFT
            if (col == 0)
            {
                nav.selectOnLeft = createNewProfileButton;
            }
            else
            {
                int leftIndex = (col - 1) * 3 + row;
                if (leftIndex < buttons.Count)
                    nav.selectOnLeft = buttons[leftIndex];
            }

            // RIGHT
            int rightIndex = (col + 1) * 3 + row;
            if (rightIndex < buttons.Count)
            {
                nav.selectOnRight = buttons[rightIndex];
            }

            // Vertical
            if (row == 0)
            {
                nav.selectOnUp = playerTabButton;
                if (i + 1 < buttons.Count) nav.selectOnDown = buttons[i + 1];
            }
            else if (row == 1)
            {
                nav.selectOnUp = buttons[i - 1];
                if (i + 1 < buttons.Count) nav.selectOnDown = buttons[i + 1];
            }
            else if (row == 2)
            {
                nav.selectOnUp = buttons[i - 1];
                nav.selectOnDown = backButton;
            }

            buttons[i].navigation = nav;
        }

        //sets the button that can interact with profiles first
        var createNav = createNewProfileButton.navigation;
        createNav.mode = Navigation.Mode.Explicit;
        createNav.selectOnRight = buttons[0]; //Point to first button in list
        createNewProfileButton.navigation = createNav;

        Debug.Log("Profile button navigation updated.");
    }

    /// <summary>
    /// Returns the current profile's name, or "None" if no profile is loaded.
    /// </summary>
    public string GetCurrentProfileName()
    {
        return IsProfileLoaded ? currentProfile.Profile_Name : "None";
    }

    private string GenerateUniqueFilename()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, profileFolderName);
        Directory.CreateDirectory(folderPath);

        const int length = 16;
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        int attempts = 0;
        string filename;

        do
        {
            filename = "";
            for (int i = 0; i < length; i++)
            {
                filename += chars[Random.Range(0, chars.Length)];
            }

            attempts++;
            if (attempts > 1000)
            {
                Debug.LogError("Could not generate a unique filename after 1000 attempts.");
                return null;
            }

        } while (File.Exists(Path.Combine(folderPath, filename + ".txt")));

        return filename;
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

    /// <summary>
    /// Loads the given profile as the active profile.
    /// </summary>
    /// <param name="profile">Profile to load into memory.</param>
    public void LoadProfile(PlayerProfile profile)
    {
        if (profile == null)
        {
            Debug.LogWarning("Tried to load null profile.");
            return;
        }

        currentProfile = profile;

        LoadInProfileUI();

        Debug.Log($"Loaded profile: {profile.Profile_Name}");
    }

    #region button mechanics

    public void ChangeBinding(string field, string newValue)
    {
        UpdateProfileField(field, newValue);


        //sound
        if (AP != null) AP.setUseComVol(false);
        if (AP != null && !AP.isPlaying()) AP.PlaySoundRandomPitch(AP.Find("menuClick"));
    }

    public void SetBindingsFromProfile()
    {
        foreach (var rebind in rebindButtons)
        {
            string input = GetProfileField(rebind.bindingKey);
            rebind.SetBindingDisplay(input);
        }
    }

    public void SetShirtColor()
    {
        if (ColorUtility.TryParseHtmlString(currentProfile.Shirt_Color, out Color shirtColor))
        {
            shirtColorScript.SetColor(shirtColor);
        }
        else
        {
            Debug.LogWarning("Invalid shirt color in profile: " + currentProfile.Shirt_Color);
        }
    }

    public void ChangeShirtColor(Color newColor)
    {
        string hex = "#" + ColorUtility.ToHtmlStringRGB(newColor);
        UpdateProfileField("Shirt_Color", hex);

        //sound
        if (AP != null) AP.setUseComVol(false);
        if (AP != null && !AP.isPlaying()) AP.PlaySoundRandomPitch(AP.Find("menuClick"));
    }

    public void SetSkinColor()
    {
        if (ColorUtility.TryParseHtmlString(currentProfile.Skin_Color, out Color skinColor))
        {
            skinColorScript.SetColor(skinColor);
        }
        else
        {
            Debug.LogWarning("Invalid skin color in profile: " + currentProfile.Skin_Color);
        }
    }

    public void ChangeSkinColor(Color newColor)
    {
        string hex = "#" + ColorUtility.ToHtmlStringRGB(newColor);
        UpdateProfileField("Skin_Color", hex);

        //sound
        if (AP != null) AP.setUseComVol(false);
        if (AP != null && !AP.isPlaying()) AP.PlaySoundRandomPitch(AP.Find("menuClick"));
    }

    public void ChangeProfileName()
    {
        string newName = RPN.GenerateAndGiveName();
        UpdateProfileField("Profile_Name", newName);
        profileNameButton.GetComponentInChildren<TMP_Text>().text = newName;

        //sound
        if (AP != null) AP.setUseComVol(false);
        if (AP != null && !AP.isPlaying()) AP.PlaySoundRandomPitch(AP.Find("menuClick"));
    }

    public void SetProfileName()
    {
        if (currentProfile != null && profileNameButton != null)
        {
            profileNameButton.GetComponentInChildren<TMP_Text>().text = currentProfile.Profile_Name;
        }
    }

    public void ChangeDeadzoneAdjustment()
    {
        float newVale = sliderDeadzoneAdjustment.value / 10;
        UpdateProfileField("Deadzone", newVale.ToString());
        deadzoneAdjNum.text = (newVale).ToString();

        //sound
        if (AP != null) AP.setUseComVol(false);
        if (AP != null && !AP.isPlaying()) AP.PlaySoundRandomPitch(AP.Find("menuClick"));
    }

    public void SetDeadzoneAdjustment()
    {
        if (currentProfile != null && sliderDeadzoneAdjustment != null)
        {
            float deadzone = Mathf.Clamp01(currentProfile.Deadzone);
            float sliderVal = Mathf.Clamp(deadzone * 10f, sliderDeadzoneAdjustment.minValue, sliderDeadzoneAdjustment.maxValue);
            sliderDeadzoneAdjustment.value = sliderVal;

            if (deadzoneAdjNum != null)
            {
                deadzoneAdjNum.text = deadzone.ToString("0.0");
            }
        }
    }

    public void ChangeKickModeDropdown()
    {
        if (kickModeDropdown.TryGetComponent(out TMP_Dropdown dropdown))
        {
            int selectedMode = dropdown.value;
            UpdateProfileField("Kick_Mode", selectedMode.ToString());

            // sound
            if (AP != null) AP.setUseComVol(false);
            if (AP != null && !AP.isPlaying()) AP.PlaySoundRandomPitch(AP.Find("menuClick"));
        }
        else
        {
            Debug.LogWarning("KickModeDropdown does not have a TMP_Dropdown component.");
        }
    }

    public void SetKickModeDropdown()
    {
        if (currentProfile != null && kickModeDropdown.TryGetComponent(out TMP_Dropdown dropdown))
        {
            int mode = Mathf.Clamp(currentProfile.Kick_Mode, 0, dropdown.options.Count - 1);
            dropdown.value = mode;
        }
    }

    public void ChangeMovementStickDropdown()
    {
        if (movementStickDropdown.TryGetComponent(out TMP_Dropdown dropdown))
        {
            int selected = dropdown.value;
            string selectedValue = selected == 0 ? "leftStick" : "rightStick";
            string otherValue = selected == 0 ? "rightStick" : "leftStick";

            UpdateProfileField("Move", selectedValue);

            //Update aiming dropdown to opposite
            if (aimingStickDropdown.TryGetComponent(out TMP_Dropdown otherDropdown))
            {
                otherDropdown.value = selected == 0 ? 1 : 0;
                UpdateProfileField("Aim", otherValue);
            }

            //sound
            if (AP != null) AP.setUseComVol(false);
            if (AP != null && !AP.isPlaying()) AP.PlaySoundRandomPitch(AP.Find("menuClick"));
        }
    }

    public void ChangeAimingStickDropdown()
    {
        if (aimingStickDropdown.TryGetComponent(out TMP_Dropdown dropdown))
        {
            int selected = dropdown.value;
            string selectedValue = selected == 0 ? "leftStick" : "rightStick";
            string otherValue = selected == 0 ? "rightStick" : "leftStick";

            UpdateProfileField("Aim", selectedValue);

            //Update movement dropdown to opposite
            if (movementStickDropdown.TryGetComponent(out TMP_Dropdown otherDropdown))
            {
                otherDropdown.value = selected == 0 ? 1 : 0;
                UpdateProfileField("Move", otherValue);
            }

            //sound
            if (AP != null) AP.setUseComVol(false);
            if (AP != null && !AP.isPlaying()) AP.PlaySoundRandomPitch(AP.Find("menuClick"));
        }
    }

    public void SetMovementStickDropdown()
    {
        if (currentProfile != null && movementStickDropdown.TryGetComponent(out TMP_Dropdown dropdown))
        {
            int index = currentProfile.Move == "leftStick" ? 0 : 1;
            dropdown.value = index;

            // Also update aiming stick to opposite
            if (aimingStickDropdown.TryGetComponent(out TMP_Dropdown otherDropdown))
            {
                otherDropdown.value = index == 0 ? 1 : 0;
            }
        }
    }

    public void SetAimingStickDropdown()
    {
        if (currentProfile != null && aimingStickDropdown.TryGetComponent(out TMP_Dropdown dropdown))
        {
            int index = currentProfile.Aim == "leftStick" ? 0 : 1;
            dropdown.value = index;

            // Also update movement stick to opposite
            if (movementStickDropdown.TryGetComponent(out TMP_Dropdown otherDropdown))
            {
                otherDropdown.value = index == 0 ? 1 : 0;
            }
        }
    }

    public void LoadInProfileUI()
    {
        SetBindingsFromProfile();
        SetShirtColor();
        SetSkinColor();
        SetProfileName();
        SetDeadzoneAdjustment();
        SetKickModeDropdown();
        SetMovementStickDropdown();
    }

    #endregion
}