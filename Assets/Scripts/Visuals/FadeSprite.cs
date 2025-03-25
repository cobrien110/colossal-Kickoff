using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeSprite : MonoBehaviour
{
    public float clearTime = 2f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        //Store the original color of the sprite (with full opacity)
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            StartFade();
        }
    }

    public void StartFade()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;

        //Continue fading while time is less than the clearTime
        while (elapsedTime < clearTime)
        {
            elapsedTime += Time.deltaTime;

            //Calculate the new opacity (alpha) value and apply
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / clearTime);

            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        //Ensure the sprite is fully transparent at the end
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
    }
}
