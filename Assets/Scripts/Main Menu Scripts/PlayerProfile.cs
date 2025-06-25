using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a single player profile for input configuration, visual customization, and preferences.
/// Stores editable values such as control bindings, colors, and gameplay settings.
/// </summary>
[System.Serializable]
public class PlayerProfile
{
    /// <summary>The file path on disk where this profile is stored (not saved in the file itself).</summary>
    [System.NonSerialized]
    public string FilePath;

    /// <summary>The visible name of this profile.</summary>
    public string Profile_Name = "Default";

    /// <summary>Hex color used for the player's shirt.</summary>
    public string Shirt_Color = "#FF0000";

    /// <summary>Hex color used for the player's skin tone.</summary>
    public string Skin_Color = "#FABBA7";

    /// <summary>Deadzone sensitivity for analog input (range: 0 to 1).</summary>
    public float Deadzone = 0.3f;

    /// <summary>Kick mode type (0 = tap, 1 = charge, etc.).</summary>
    public int Kick_Mode = 0;

    public string Pause = "selectButton";
    public string Move = "leftStick";
    public string Aim = "rightStick";
    public string Kick = "rightTrigger";
    public string Super_Kick = "bButton";
    public string Call_For_Pass = "aButton";
    public string Dodge = "rightShoulder";
    public string Attack = "rightTrigger";
    public string Ability_1 = "leftTrigger";
    public string Ability_2 = "leftShoulder";
    public string Taunt_1 = "dpad.up";
    public string Taunt_2 = "dpad.right";
    public string Taunt_3 = "dpad.down";
    public string Taunt_4 = "dpad.left";

    /// <summary>
    /// Converts this PlayerProfile into a key-value dictionary, used for saving to text files.
    /// </summary>
    public Dictionary<string, string> ToDictionary()
    {
        return new Dictionary<string, string>
        {
            { "Profile_Name", Profile_Name },
            { "Shirt_Color", Shirt_Color },
            { "Skin_Color", Skin_Color },
            { "Deadzone", Deadzone.ToString("0.##") },
            { "Kick_Mode", Kick_Mode.ToString() },
            { "Pause", Pause },
            { "Move", Move },
            { "Aim", Aim },
            { "Kick", Kick },
            { "Super_Kick", Super_Kick },
            { "Call_For_Pass", Call_For_Pass },
            { "Dodge", Dodge },
            { "Attack", Attack },
            { "Ability_1", Ability_1 },
            { "Ability_2", Ability_2 },
            { "Taunt_1", Taunt_1 },
            { "Taunt_2", Taunt_2 },
            { "Taunt_3", Taunt_3 },
            { "Taunt_4", Taunt_4 }
        };
    }

    /// <summary>
    /// Populates the PlayerProfile's fields from a dictionary (parsed from a saved .txt file).
    /// </summary>
    /// <param name="data">Dictionary of key-value string pairs matching field names.</param>
    public void FromDictionary(Dictionary<string, string> data)
    {
        if (data.TryGetValue("Profile_Name", out var val)) Profile_Name = val;
        if (data.TryGetValue("Shirt_Color", out val)) Shirt_Color = val;
        if (data.TryGetValue("Skin_Color", out val)) Skin_Color = val;

        if (data.TryGetValue("Deadzone", out val) && float.TryParse(val, out var dz)) Deadzone = dz;
        if (data.TryGetValue("Kick_Mode", out val) && int.TryParse(val, out var km)) Kick_Mode = km;

        if (data.TryGetValue("Pause", out val)) Pause = val;
        if (data.TryGetValue("Move", out val)) Move = val;
        if (data.TryGetValue("Aim", out val)) Aim = val;
        if (data.TryGetValue("Kick", out val)) Kick = val;
        if (data.TryGetValue("Super_Kick", out val)) Super_Kick = val;
        if (data.TryGetValue("Call_For_Pass", out val)) Call_For_Pass = val;
        if (data.TryGetValue("Dodge", out val)) Dodge = val;
        if (data.TryGetValue("Attack", out val)) Attack = val;
        if (data.TryGetValue("Ability_1", out val)) Ability_1 = val;
        if (data.TryGetValue("Ability_2", out val)) Ability_2 = val;
        if (data.TryGetValue("Taunt_1", out val)) Taunt_1 = val;
        if (data.TryGetValue("Taunt_2", out val)) Taunt_2 = val;
        if (data.TryGetValue("Taunt_3", out val)) Taunt_3 = val;
        if (data.TryGetValue("Taunt_4", out val)) Taunt_4 = val;
    }
}