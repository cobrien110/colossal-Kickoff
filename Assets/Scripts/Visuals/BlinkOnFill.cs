using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkOnFill : MonoBehaviour
{
    [SerializeField] private Image targetImage;
    [SerializeField] private Color colorA = Color.white;
    [SerializeField] private Color colorB = new Color(1f, 0.5f, 0f);
    [SerializeField] private float blinkSpeed = 2f; //cycles per second

    private void Reset()
    {
        targetImage = GetComponent<Image>();
    }

    private void Update()
    {
        if (targetImage == null) return;

        if (targetImage.fillAmount > 0f)
        {
            float t = (Mathf.Sin(Time.deltaTime * blinkSpeed * Mathf.PI * 2f) + 1f) * 0.5f;
            targetImage.color = Color.Lerp(colorA, colorB, t);
        }
        else
        {
            targetImage.color = colorA; //reset to base color when not blinking
        }
    }
}