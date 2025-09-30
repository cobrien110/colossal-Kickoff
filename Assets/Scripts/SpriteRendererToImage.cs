using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class SpriteRendererToImage : EditorWindow
{
    [MenuItem("Tools/Convert SpriteRenderers To Images")]
    static void Convert()
    {
        SpriteRenderer[] renderers = FindObjectsOfType<SpriteRenderer>();

        foreach (SpriteRenderer sr in renderers)
        {
            GameObject go = sr.gameObject;

            Image img = go.GetComponent<Image>();
            if (img == null)
            {
                img = go.AddComponent<Image>();
            }

            img.sprite = sr.sprite;
            img.color = sr.color;
            img.material = sr.sharedMaterial;

            if (sr.flipX || sr.flipY)
            {
                RectTransform rt = go.GetComponent<RectTransform>();
                if (rt != null)
                {
                    Vector3 scale = rt.localScale;
                    if (sr.flipX) scale.x *= -1;
                    if (sr.flipY) scale.y *= -1;
                    rt.localScale = scale;
                }
            }

            DestroyImmediate(sr);
        }

        Debug.Log("Converted all SpriteRenderers to UI Images.");
    }
}