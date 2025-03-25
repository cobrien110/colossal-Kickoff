using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBlinker : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    //private bool blinkingStarted = false;
    private float blinkInterval = 0.2f; 
    private float elapsedTime = 0f;
    private float maxBlinkSpeed = 0.05f;
    private float blinkAcceleration = 0.02f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Invoke("StartBlinking", 2f);
        Invoke("Disappear", 3f);
    }

    void StartBlinking()
    {
        //blinkingStarted = true;
        InvokeRepeating("BlinkSprite", 0f, blinkInterval);
    }

    void BlinkSprite()
    {
        spriteRenderer.enabled = !spriteRenderer.enabled;
        elapsedTime += blinkInterval;

        blinkInterval -= blinkAcceleration;
        blinkInterval = Mathf.Max(blinkInterval, maxBlinkSpeed);

        if (elapsedTime >= 3f)
        {
            CancelInvoke("BlinkSprite");
        }
    }

    void Disappear()
    {
        Destroy(gameObject);
    }
}
