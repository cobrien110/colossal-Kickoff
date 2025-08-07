using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterCharSelectOption: MonoBehaviour
{
    [SerializeField] private GameObject[] monsterObjects;
    private int currentIndex = 0;

    void Start()
    {
        UpdateMonster(currentIndex);
    }

    public void UpdateMonster(int index)
    {
        if (index < 0 || index >= monsterObjects.Length)
        {
            Debug.LogWarning("Monster index out of range.");
            return;
        }

        //Disable all monsters
        for (int i = 0; i < monsterObjects.Length; i++)
        {
            if (monsterObjects[i] != null)
                monsterObjects[i].SetActive(false);
        }

        //Enable the selected monster
        if (monsterObjects[index] != null)
        {
            monsterObjects[index].SetActive(true);
            currentIndex = index;
        }
    }
}