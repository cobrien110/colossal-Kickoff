using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerSorter : MonoBehaviour
{
    public int layerNum;
    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer MR = GetComponent<MeshRenderer>();
        if (MR != null) MR.sortingOrder = layerNum;
        SpriteRenderer SR = GetComponent<SpriteRenderer>();
        if (SR != null) SR.sortingOrder = layerNum;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
