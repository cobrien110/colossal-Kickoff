using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class MonsterAbilityHover : Selectable
{
    [SerializeField] private GameObject highlightVisual;
    [SerializeField] private Sprite[] abilityIcons;

    [SerializeField] private MonsterAbilityBlurb blurb;
    [SerializeField] private MonsterAbilityViewController controller;
    [SerializeField] private MonsterName name;
    [SerializeField] private int abilityIndex;
    [SerializeField] private MenuController MC;

    // Prevent clicks/submit from doing anything
    public void OnPointerClick(PointerEventData eventData) { }
    public void OnSubmit(BaseEventData eventData) { }

    // Called when EventSystem selects this object
    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        if (MC.monsterAbilityCanHover)
        {
            DoHover();
        }
    }

    // Called when EventSystem deselects this object
    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        if (MC.monsterAbilityCanHover)
        {
            DoUnhover();
        }
    }

    // Collider-based hover entry
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (MC.monsterAbilityCanHover && other.CompareTag("MenuCursor"))
        {
            DoHover();
        }
    }

    // Collider-based hover exit
    private void OnTriggerExit2D(Collider2D other)
    {
        if (MC.monsterAbilityCanHover && other.CompareTag("MenuCursor"))
        {
            DoUnhover();
        }
    }

    // Shared logic for hover
    private void DoHover()
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

    // Shared logic for unhover
    private void DoUnhover()
    {
        controller.unhighlightAll();
        blurb.setText(controller.monsterBlurbs[name.monsterIndex]);
    }

    public void setVisual(int input)
    {
        GetComponent<Image>().sprite = abilityIcons[input];
    }

    public void highlight()
    {
        highlightVisual.SetActive(true);
    }

    public void unhighlight()
    {
        highlightVisual.SetActive(false);
    }

}