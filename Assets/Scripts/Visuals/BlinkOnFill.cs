using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkOnFill : MonoBehaviour
{
    [SerializeField] private Image targetImage;
    [SerializeField] private Color colorA = Color.white;
    [SerializeField] private Color colorB = new Color(1f, 0.5f, 0f);
    [SerializeField] private float blinkSpeed = 4f; //how fast it ping-pongs (cycles per second)

    private float lerpValue = 0f;
    private int direction = 1;

    private void Reset()
    {
        targetImage = GetComponent<Image>();
    }

    private void Update()
    {
        if (targetImage == null) return;

        if (targetImage.fillAmount > 0f)
        {
            lerpValue += direction * blinkSpeed * Time.deltaTime;

            //Ping-pong control
            if (lerpValue >= 1f)
            {
                lerpValue = 1f;
                direction = -1;
            }
            else if (lerpValue <= 0f)
            {
                lerpValue = 0f;
                direction = 1;
            }

            //Apply color based on lerp
            targetImage.color = Color.Lerp(colorA, colorB, lerpValue);
        }
        else
        {
            lerpValue = 0f;
            direction = 1;
            targetImage.color = colorA;
        }
    }
}