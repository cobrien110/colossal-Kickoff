using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarriorCharSelectOption: MonoBehaviour
{
    [SerializeField] private GameObject warriorObject;
    [SerializeField] private Shader thisShader;
    [SerializeField] private Material thisMaterial;

    private List<SpriteShadow> shadowObjectList = new List<SpriteShadow>();

    void Awake()
    {
        if (warriorObject == null)
        {
            Debug.LogError("Warrior object not assigned.");
            return;
        }
        // Create material instance with shader
        thisMaterial = new Material(thisShader);

        // Get all shadows and assign them to a list
        SpriteShadow[] shadows = warriorObject.GetComponentsInChildren<SpriteShadow>(true);
        foreach (var shadow in shadows)
        {
            shadowObjectList.Add(shadow);
        }

        // Assign first shadow (or null) to CharacterSelectOption script if available
        CharacterSelectOption characterSelect = GetComponent<CharacterSelectOption>();
        if (characterSelect != null)
        {
            characterSelect.SetActiveShadows(shadowObjectList);
        }
    }

    public void updateColor(float red, float green, float blue)
    {
        if (thisMaterial != null)
            thisMaterial.SetColor("_ShirtColor", new Color(red, green, blue));
    }

    public void updateSkinColor(float red, float green, float blue)
    {
        if (thisMaterial != null)
            thisMaterial.SetColor("_SkinColor", new Color(red, green, blue));
    }
}