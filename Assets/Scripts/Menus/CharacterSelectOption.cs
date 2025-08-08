using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectOption : MonoBehaviour
{
    [SerializeField] public int characterID;
    [SerializeField] private int cursorsOver = 0;

    //Shadows to enable/disable on hover
    private List<GameObject> activeShadows = new List<GameObject>();

    public bool canBeSelected = true;

    public void SetActiveShadows(List<SpriteShadow> shadowComponents)
    {
        activeShadows.Clear();
        foreach (var shadow in shadowComponents)
        {
            if (shadow != null)
                activeShadows.Add(shadow.gameObject);
        }

        // Ensure shadows start off
        LoopThroughShadows(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("MenuCursor"))
        {
            other.GetComponent<MenuCursor>().StartHovering("playerSelect", characterID);
            cursorsOver++;

            LoopThroughShadows(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (canBeSelected && other.CompareTag("MenuCursor"))
        {
            other.GetComponent<MenuCursor>().StopHovering();
            cursorsOver--;

            if (cursorsOver <= 0)
            {
                LoopThroughShadows(false);
            }
        }
    }
    
    private void LoopThroughShadows(bool state)
    {
        foreach (var shadowGO in activeShadows)
        {
            if (shadowGO != null)
                shadowGO.SetActive(state);
        }
    }

}
