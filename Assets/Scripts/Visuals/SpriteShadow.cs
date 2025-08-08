using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteShadow : MonoBehaviour
{
    [SerializeField] private SpriteRenderer mainRenderer;
    [SerializeField] private SpriteRenderer shadowRenderer;
    [SerializeField] private float shadowScaleMultiplier = 1.05f; // How much bigger than the monster
    [SerializeField] private Vector2 shadowOffset = new Vector2(0f, -0.05f); // Position offset for drop effect

    void Awake()
    {
        if (mainRenderer == null)
            mainRenderer = GetComponentInParent<SpriteRenderer>();

        if (shadowRenderer == null)
            shadowRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (mainRenderer == null || shadowRenderer == null) return;

        shadowRenderer.color = new Color(0f, 0f, 0f, 0.5f);

        //Set sorting layer behind main
        shadowRenderer.sortingLayerID = mainRenderer.sortingLayerID;
        shadowRenderer.sortingOrder = mainRenderer.sortingOrder - 1;

        shadowRenderer.transform.localScale = Vector3.one * shadowScaleMultiplier;

        shadowRenderer.transform.localPosition = shadowOffset;

        shadowRenderer.flipX = mainRenderer.flipX;
        shadowRenderer.flipY = mainRenderer.flipY;
    }

    void LateUpdate()
    {
        if (mainRenderer == null || shadowRenderer == null) return;

        //Update sprite and flip
        shadowRenderer.sprite = mainRenderer.sprite;
    }
}