using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdSortingOrder : MonoBehaviour
{
    SpriteRenderer SR;
    // Start is called before the first frame update
    void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        float dis = Vector3.Distance(transform.position, GameObject.Find("Main Camera").transform.position);
        SR.sortingOrder = (int) (-(dis * 100f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
