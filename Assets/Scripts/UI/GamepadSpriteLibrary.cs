using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Input/Gamepad Sprite Library")]
public class GamepadSpriteLibrary : ScriptableObject
{
    [System.Serializable]
    public class GamepadBindingSprite
    {
        public string inputName;
        public Sprite sprite;
    }

    public List<GamepadBindingSprite> spriteMappings;

    private Dictionary<string, Sprite> lookup;

    public void Initialize()
    {
        lookup = new Dictionary<string, Sprite>();
        foreach (var mapping in spriteMappings)
        {
            if (!lookup.ContainsKey(mapping.inputName.ToLower()))
            {

                lookup.Add(mapping.inputName.ToLower(), mapping.sprite);
            }
        }
    }

    public Sprite GetSpriteForInput(string input)
    {
        if (lookup == null) Initialize();
        lookup.TryGetValue(input.ToLower(), out Sprite sprite);
        return sprite;
    }
}