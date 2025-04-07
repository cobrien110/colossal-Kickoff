using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundFinder : MonoBehaviour
{
    public LayerMask groundLayer;

    // Start is called before the first frame update
    void Start()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y + 0.02f, transform.position.z);
        }
    }

}
