using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningTileScript : MonoBehaviour
{
    [Header("Timing Settings")]
    public float TimeBetweenChange = 0.5f;
    public float TimeUntilStop = 5f;
    public bool Looping = true;

    [Header("Offset Settings")]
    public float AddedOffset = 0.1f;

    private Material mat;
    private MeshRenderer meshRenderer;
    private float offsetY = 0f;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            Debug.LogError("MaterialOffsetLooper: No MeshRenderer found!");
            enabled = false;
            return;
        }

        mat = meshRenderer.material;

        StartCoroutine(OffsetCycle());
    }

    IEnumerator OffsetCycle()
    {
        while (true)
        {
            float elapsed = 0f;

            meshRenderer.enabled = true;

            while (elapsed < TimeUntilStop)
            {
                offsetY += AddedOffset;
                mat.mainTextureOffset = new Vector2(mat.mainTextureOffset.x, offsetY);

                yield return new WaitForSeconds(TimeBetweenChange);
                elapsed += TimeBetweenChange;
            }

            meshRenderer.enabled = false;

            yield return new WaitForSeconds(1f);
            
            //Delete looping stuff after done demoing.
            if (!Looping)
                yield break;
        }
    }
}