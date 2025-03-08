using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallTVs : MonoBehaviour
{
    public MeshRenderer tvScreenRenderer;  
    public Material[] tvMaterials;         

    void Start()
    {
        if (tvScreenRenderer == null || tvMaterials.Length == 0)
        {
            Debug.LogWarning("Missing references in SmallTVScreen!");
            return;
        }

        StartCoroutine(SwapMaterial());
    }

    IEnumerator SwapMaterial()
    {
        while (true)
        {
          
            tvScreenRenderer.material = tvMaterials[Random.Range(0, tvMaterials.Length)];

            
            yield return new WaitForSeconds(Random.Range(8f, 12f));
        }
    }
}
