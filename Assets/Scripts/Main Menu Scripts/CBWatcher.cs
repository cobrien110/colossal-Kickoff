using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CBWatcher : MonoBehaviour
{
    private Button button;
    private Color lastSelectedColor;

    void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            lastSelectedColor = button.colors.selectedColor;
        }
    }

    void Update()
    {
        if (button == null) return;

        var cb = button.colors;
        if (cb.selectedColor != lastSelectedColor)
        {
            Debug.LogWarning("SelectedColor changed to " + cb.selectedColor, this);
            Debug.Log(StackTraceUtility.ExtractStackTrace());
            lastSelectedColor = cb.selectedColor;
        }
    }
}