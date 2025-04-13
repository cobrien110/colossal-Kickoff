using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class MonsterAbilityHover : MonoBehaviour
{
    [SerializeField] private MonsterAbilityBlurb blurb;
    [SerializeField] private MonsterAbilityViewController controller;
    [SerializeField] private MonsterName name;
    [SerializeField] private int abilityIndex;
    [SerializeField] private MenuController MC;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (MC.monsterAbilityCanHover && other.gameObject.tag == "MenuCursor")
        {
            controller.highlightIcon(abilityIndex);
            switch (name.monsterIndex)
            {
                case 0:
                    blurb.setText(controller.minotaurAbilityBlurbs[abilityIndex]);
                    break;
                case 1:
                    blurb.setText(controller.akhlutAbilityBlurbs[abilityIndex]);
                    break;
                case 2:
                    blurb.setText(controller.gashaAbilityBlurbs[abilityIndex]);
                    break;
                case 3:
                    blurb.setText(controller.quetzalAbilityBlurbs[abilityIndex]);
                    break;
                case 4:
                    blurb.setText(controller.sphinxAbilityBlurbs[abilityIndex]);
                    break;
                default:
                    Debug.Log("Error: unknown monster index");
                    break;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (MC.monsterAbilityCanHover)
        {
            if (other.gameObject.tag == "MenuCursor")
            {
                controller.unhighlightAll();
                blurb.setText(controller.monsterBlurbs[name.monsterIndex]);
            }
        }
    }
}
