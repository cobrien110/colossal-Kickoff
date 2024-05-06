using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnPlay : MonoBehaviour
{
    public MeshRenderer MR;
    // Start is called before the first frame update
    void Start()
    {
        MR = GetComponent<MeshRenderer>();
        if (MR != null)
        {
            MR.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
