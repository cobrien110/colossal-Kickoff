using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterAbilityIcon : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private GameObject highlightVisual;
    [SerializeField] private Sprite[] abilityIcons;

    public void highlight() {
        highlightVisual.SetActive(true);
    }

    public void unhighlight() {
        highlightVisual.SetActive(false);
    }

    public void setVisual(int input) {
        GetComponent<Image>().sprite = abilityIcons[input];
    }

}
