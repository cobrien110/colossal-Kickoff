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
        MR.sortingOrder = layerNum;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
