using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAbilityViewController : MonoBehaviour
{
    [SerializeField] private MonsterAbilityBlurb blurb;
    [SerializeField] private MonsterAbilityIcon[] abilityIcons;
    [SerializeField] private MonsterName name;
    [SerializeField] private string[] monsterBlurbs;
    [SerializeField] private string[] minotaurAbilityBlurbs;
    [SerializeField] private string[] akhlutAbilityBlurbs;
    [SerializeField] private string[] gashaAbilityBlurbs;
    [SerializeField] private string[] quetzalAbilityBlurbs;
    [SerializeField] private string[] sphinxAbilityBlurbs;
    private int abilityHighlighted = 0;
    public bool selectedHighlightingAbilities = false;
    // Start is called before the first frame update

    //scroll right
    //scroll left
    //page right
    //page left
    //reset to ability 0


    void Start()
    {
        unhighlightAll();
    }

    public void scrollLeft() {
        if (abilityHighlighted < 1) {
            abilityHighlighted = 3;
        } else {
            abilityHighlighted--;
        }
        updateVisuals();
    }

    public void scrollRight() {
        if (abilityHighlighted > 2) {
            abilityHighlighted = 0;
        } else {
            abilityHighlighted++;
        }
        updateVisuals();
    }

    public void pageLeft() {
        updateVisuals();
    }
    public void pageRight() {
        updateVisuals();
    }

    public void pageUpDown(bool highlightingAbilities) {
        selectedHighlightingAbilities = highlightingAbilities;
        if (selectedHighlightingAbilities) {
            blurb.selectBlurbs();
            name.unselectName();
        } else {
            blurb.unselectBlurbs();
            name.selectName();
        }
        updateVisuals();
    }

    // Update is called once per frame
    void updateVisuals()
    {
        if (!selectedHighlightingAbilities) {
            blurb.setText(monsterBlurbs[name.monsterIndex]);
            abilityHighlighted = 0;
            unhighlightAll();
            for (int i = 0; i < abilityIcons.Length; i++) {
                abilityIcons[i].setVisual(name.monsterIndex);
            }
        } else {
            highlightIcon(abilityHighlighted);
            switch (name.monsterIndex) {
                case 0:
                blurb.setText(minotaurAbilityBlurbs[abilityHighlighted]);
                break;
                case 1:
                blurb.setText(akhlutAbilityBlurbs[abilityHighlighted]);
                break;
                case 2:
                blurb.setText(gashaAbilityBlurbs[abilityHighlighted]);
                break;
                case 3:
                blurb.setText(quetzalAbilityBlurbs[abilityHighlighted]);
                break;
                case 4:
                blurb.setText(sphinxAbilityBlurbs[abilityHighlighted]);
                break;
                default:
                Debug.Log("Error: unknown monster index");
                break;
            }
        }
    }

    void highlightIcon(int input) {
        //Debug.Log("highlighting icon " + input);
        for (int i = 0; i < abilityIcons.Length; i++) {
            if (i == input) {
                abilityIcons[i].highlight();
                //Debug.Log("highlighting icon " + input);
            } else {
                abilityIcons[i].unhighlight();
                //Debug.Log("unhighlighting icon " + input);
            }
        }
    }

    void unhighlightAll() {
        for (int i = 0; i < abilityIcons.Length; i++) {
            abilityIcons[i].unhighlight();
        }
    }
}
