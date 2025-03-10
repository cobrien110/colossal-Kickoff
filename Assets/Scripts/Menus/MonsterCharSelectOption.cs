using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterCharSelectOption: MonoBehaviour
{
    [SerializeField] private Sprite[] monsterSprites;
    [SerializeField] private float[] monsterScales;
    [SerializeField] private Vector3[] monsterPositions;
    [SerializeField] private Image image;
    private BoxCollider2D BC;
    private Vector2 sizeBC;

    void Start() {
        image = GetComponent<Image>();
        BC = GetComponent<BoxCollider2D>();
        sizeBC = BC.size;
        image.sprite = monsterSprites[0];
        image.transform.localScale = new Vector3(monsterScales[0], monsterScales[0], 1f);
        //image.transform.position = monsterPositions[0];
    }

    public void updateSprite(int index) {
        image.sprite = monsterSprites[index];
        image.transform.localScale = new Vector3(monsterScales[index], monsterScales[index], 1f);
        BC.size = new Vector2(sizeBC.x / monsterScales[index], sizeBC.y / monsterScales[index]);
        //image.transform.localPosition = monsterPositions[index];
    }
}
