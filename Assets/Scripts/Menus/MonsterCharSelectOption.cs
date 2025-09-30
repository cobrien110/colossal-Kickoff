using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterCharSelectOption: MonoBehaviour
{
    [SerializeField] private Sprite[] monsterSprites;
    [SerializeField] private float[] monsterScales;
    [SerializeField] private float[] monsterOffsets;
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
        BC.size = new Vector2(sizeBC.x / (monsterScales[0] * 1.75f), sizeBC.y / (monsterScales[0] * 1.25f));
        BC.offset = new Vector2(monsterOffsets[0], -3.0f);
        //image.transform.position = monsterPositions[0];
    }

    public void updateSprite(int index) {
        image.sprite = monsterSprites[index];
        image.transform.localScale = new Vector3(monsterScales[index], monsterScales[index], 1f);
        BC.size = new Vector2(sizeBC.x / (monsterScales[index] * 1.75f), sizeBC.y / (monsterScales[index] * 1.50f));
        BC.offset = new Vector2(monsterOffsets[index], -3.0f);
        //image.transform.localPosition = monsterPositions[index];
    }
}
