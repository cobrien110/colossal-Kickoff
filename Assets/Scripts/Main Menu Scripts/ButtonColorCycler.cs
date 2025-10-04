using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class ButtonColorCycler : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private Selectable selectable;
    public Color startColor = new Color(245f / 255f, 81f / 255f, 0f / 255f);   // #F55100
    public Color endColor = new Color(233f / 255f, 189f / 255f, 69f / 255f);   // #E9BD45
    public float time = 0.35f;
    public bool isSelected = false;

    bool goingForward;
    bool isCycling;
    private MenuController MC;
    AudioPlayer AP;

    // Start is called before the first frame update
    void Awake()
    {
        MC = GameObject.FindFirstObjectByType<MenuController>();
        if (MC != null) AP = MC.GetComponent<AudioPlayer>();
        else AP = GetComponent<AudioPlayer>();
        selectable = GetComponent<Selectable>();
        if (selectable == null)
        {
            Debug.LogWarning("No Selectable component (Button, Dropdown, Slider) found on " + gameObject.name);
            enabled = false;
            return;
        }

        goingForward = true;
        isCycling = false;

        startColor = selectable.colors.selectedColor;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("button is cycling? " + isCycling);
        if (!isCycling && isActiveAndEnabled)
        {
            if (goingForward)
                StartCoroutine(CycleColor(startColor, endColor, time));
            else
                StartCoroutine(CycleColor(endColor, startColor, time));
        }
    }

    private IEnumerator CycleColor(Color from, Color to, float duration)
    {
        isCycling = true;
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float lerpT = t / duration;
            Color current = Color.Lerp(from, to, lerpT);
            ColorBlock cb = selectable.colors;
            cb.selectedColor = current;
            selectable.colors = cb;
            yield return null;
        }

        isCycling = false;
        goingForward = !goingForward;
    }

    public void OnSelect(BaseEventData eventData)
    {
        isSelected = true;
        if (AP != null) AP.PlaySoundRandomPitch(AP.Find("menuMove2"));
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;
    }

    private void OnEnable()
    {
        StopAllCoroutines();
        isCycling = false;
        isSelected = true;
    }
}
