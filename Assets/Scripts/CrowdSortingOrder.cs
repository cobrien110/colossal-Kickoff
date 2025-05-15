using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdSortingOrder : MonoBehaviour
{
    SpriteRenderer SR;
    private float randCol = 0.3f;
    // Start is called before the first frame update
    void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        float dis = Vector3.Distance(transform.position, GameObject.Find("Main Camera").transform.position);
        SR.sortingOrder = (int) (-(dis * 100f));

        //Color randomColor = new Color(1 - Random.Range(0, randCol), 1 - Random.Range(0, randCol), 1 - Random.Range(0, randCol));
        //SR.color = randomColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
