using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallTVs : MonoBehaviour
{
    public MeshRenderer tvScreenRenderer;  // Assign the screen plane's renderer
    public Material[] tvMaterials;         // Assign 5+ materials in the Inspector

    void Start()
    {
        if (tvScreenRenderer == null || tvMaterials.Length == 0)
        {
            Debug.LogWarning("Missing references in SmallTVScreen!");
            return;
        }

        // Start the material swapping coroutine
        StartCoroutine(SwapMaterial());
    }

    IEnumerator SwapMaterial()
    {
        while (true)
        {
            // Assign a random material from the array
            tvScreenRenderer.material = tvMaterials[Random.Range(0, tvMaterials.Length)];

            // Wait for a random time between 8 to 12 seconds before swapping again
            yield return new WaitForSeconds(Random.Range(8f, 12f));
        }
    }
}
