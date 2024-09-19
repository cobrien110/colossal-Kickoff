using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectedDisplay : MonoBehaviour
{
    [SerializeField] private Sprite[] selectionSprites;
    public bool isAssigned = false;
    void Start()
    {
        isAssigned = false;
        GetComponent<Image>().sprite = selectionSprites[0];
    }
    public void changeSprite(int target) {
        GetComponent<Image>().sprite = selectionSprites[target];
        if (target != 0) {
            isAssigned = true;
        } else {
            isAssigned = false;
        }
    }

}
