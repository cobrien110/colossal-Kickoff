using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterCharSelectOption: MonoBehaviour
{
    [SerializeField] private GameObject[] monsterObjects;
    private CharacterSelectOption characterSelectOption;

    private List<SpriteShadow>[] shadowObjects; // One list per monster
    private int currentIndex = 0;

    void Start()
    {
        characterSelectOption = GetComponent<CharacterSelectOption>();
        PopulateShadowObjects();
        UpdateMonster(currentIndex);
    }

    private void PopulateShadowObjects()
    {
        shadowObjects = new List<SpriteShadow>[monsterObjects.Length];

        for (int i = 0; i < monsterObjects.Length; i++)
        {
            if (monsterObjects[i] != null)
            {
                SpriteShadow[] shadows = monsterObjects[i].GetComponentsInChildren<SpriteShadow>(true);
                shadowObjects[i] = new List<SpriteShadow>(shadows);
            }
            else
            {
                shadowObjects[i] = new List<SpriteShadow>();
            }
        }
    }

    public void UpdateMonster(int index)
    {
        if (index < 0 || index >= monsterObjects.Length)
        {
            Debug.LogWarning("Monster index out of range.");
            return;
        }

        for (int i = 0; i < monsterObjects.Length; i++)
        {
            if (monsterObjects[i] != null)
                monsterObjects[i].SetActive(false);
        }

        if (monsterObjects[index] != null)
        {
            monsterObjects[index].SetActive(true);
            currentIndex = index;

            // Update the character select with new shadow set
            if (characterSelectOption != null)
                characterSelectOption.SetActiveShadows(shadowObjects[index]);
        }
    }
}