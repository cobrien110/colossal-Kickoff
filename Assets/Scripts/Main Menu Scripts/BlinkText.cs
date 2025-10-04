using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BlinkText : MonoBehaviour
{
    public float BlinkTime = 0.5f;
    private string textToBlink;
    private TextMeshProUGUI textComponent;
    private Image SR;
    private Color col;

    void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        SR = GetComponent<Image>();
        if (SR != null) col = SR.color;
        //Set the Text that will be blinked from the inspector of the text component
        if (textComponent != null) textToBlink = textComponent.text;
    }

    private void OnEnable()
    {
        StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        while (true)
        {
            if (SR == null && textComponent != null)
            {
                textComponent.text = textToBlink;
                yield return new WaitForSeconds(BlinkTime);
                textComponent.text = string.Empty;
                yield return new WaitForSeconds(BlinkTime - 0.15f);
            }
            else if (SR != null)
            {
                SR.color = col;
                yield return new WaitForSeconds(BlinkTime);
                SR.color = new Color(255, 255, 255, 0);
                yield return new WaitForSeconds(BlinkTime - 0.15f);
            }
        }
    }
}
