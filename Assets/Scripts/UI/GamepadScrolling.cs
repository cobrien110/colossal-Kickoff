using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GamepadScrolling : MonoBehaviour
{
    [Header("References")]
    public RectTransform content;
    public Transform targetTransform;

    [Header("Behavior")]
    public bool horizontal = false;
    public bool smoothScroll = true;
    public float scrollSpeed = 10f;

    private GameObject lastSelected;

    void Update()
    {
        if (content == null || targetTransform == null) return;

        GameObject current = EventSystem.current.currentSelectedGameObject;

        if (current != null && current != lastSelected)
        {
            if (current.transform.IsChildOf(content))
            {
                RectTransform selectedRect = current.GetComponent<RectTransform>();
                if (selectedRect != null)
                {
                    MoveTargetTo(selectedRect);
                }
            }

            lastSelected = current;
        }

        Vector3 contentPos = content.localPosition;
        Vector3 targetPos = targetTransform.localPosition;

        if (horizontal)
        {
            float targetX = -targetPos.x;
            contentPos.x = smoothScroll ? Mathf.Lerp(contentPos.x, targetX, Time.deltaTime * scrollSpeed) : targetX;
        }
        else
        {
            float targetY = -targetPos.y;
            contentPos.y = smoothScroll ? Mathf.Lerp(contentPos.y, targetY, Time.deltaTime * scrollSpeed) : targetY;
        }

        content.localPosition = contentPos;
    }

    /// <summary>
    /// Moves the target transform to align with a newly selected UI element inside the scroll content.
    /// </summary>
    /// <param name="selectedRect">The RectTransform of the selected UI element.</param>
    private void MoveTargetTo(RectTransform selectedRect)
    {
        Vector3 worldPos = selectedRect.position;
        Vector3 localPos = content.InverseTransformPoint(worldPos);

        targetTransform.localPosition = localPos;
    }
}