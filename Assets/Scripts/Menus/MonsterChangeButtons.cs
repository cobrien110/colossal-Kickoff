using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterChangeButtons : MonoBehaviour
{
    [SerializeField] public int characterID = 0;

    public bool canBeSelected = true;

    [Header("Blink Settings")]
    [SerializeField] private Image targetImage;
    [SerializeField] private Color colorA = Color.white;
    [SerializeField] private Color colorB = new Color(1f, 0.5f, 0f); // orange
    [SerializeField] private float blinkSpeed = 4f; //how fast it ping-pongs

    private float lerpValue = 0f;
    private int direction = 1;
    private bool blinking = false;

    private void Reset()
    {
        targetImage = GetComponent<Image>();
    }

    private void Update()
    {
        if (blinking && targetImage != null)
        {
            lerpValue += direction * blinkSpeed * Time.deltaTime;

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

            targetImage.color = Color.Lerp(colorA, colorB, lerpValue);
        }
        else if (targetImage != null)
        {
            lerpValue = 0f;
            direction = 1;
            targetImage.color = colorA;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "MenuCursor")
        {
            other.gameObject.GetComponent<MenuCursor>().StartHovering("monsterChange", characterID);
            blinking = true; 
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (canBeSelected)
        {
            if (other.gameObject.tag == "MenuCursor")
            {
                other.gameObject.GetComponent<MenuCursor>().StopHovering();
                blinking = false; 
            }
        }
    }
}