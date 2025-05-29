using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonColorCycler : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private Button button;
    public Color startColor;
    public Color endColor;
    public float time = 1f;
    bool goingForward;
    bool isCycling;
    public bool isSelected = true;

    // Start is called before the first frame update
    void Start()
    {
        goingForward = true;
        isCycling = false;
        button = GetComponent<Button>();
        startColor = button.colors.selectedColor;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("button is cycling? " + isCycling);
        if (!isCycling && isActiveAndEnabled)
        {
            if (goingForward)
            {
                StartCoroutine(CycleMaterial(startColor, endColor, time));
            }
            else
            {
                StartCoroutine(CycleMaterial(endColor, startColor, time));
            }
        }
    }

    private IEnumerator CycleMaterial(Color startColor, Color endColor, float cycleTime)
    {
        isCycling = true;
        float currentTime = 0;

        // lerp between start color and final color
        while (currentTime < cycleTime)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / cycleTime;
            Color currentColor = Color.Lerp(startColor, endColor, t);
            ColorBlock cb = button.colors;
            cb.selectedColor = currentColor;
            button.colors = cb;
            yield return null;
        }

        // tell code that the cycle has ended and switch direction
        isCycling = false;
        goingForward = !goingForward;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        // stop cycling
        //isCycling = false;
        //isSelected = false;
        //StopAllCoroutines();

        /*
        // reset color
        ColorBlock cb = button.colors;
        cb.selectedColor = startColor;
        button.colors = cb;
        */
    }

    public void OnSelect(BaseEventData eventData)
    {
        //Debug.Log("button has been selected");
        //isCycling = false;
        //isSelected = true;
    }

    private void OnEnable()
    {
        StopAllCoroutines();
        isCycling = false;
        isSelected = true;
    }
}
