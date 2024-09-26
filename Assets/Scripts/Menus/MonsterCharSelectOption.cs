using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterCharSelectOption: MonoBehaviour
{
    [SerializeField] private Sprite[] monsterSprites;

    void Start() {
        GetComponent<Image>().sprite = monsterSprites[0];
    }

    public void updateSprite(int index) {
        GetComponent<Image>().sprite = monsterSprites[index];
    }
}
