using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubSpriteController : MonoBehaviour
{
    SpriteRenderer sr;
    SpriteRenderer srParent;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        srParent = transform.parent.gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        sr.sprite = srParent.sprite;
    }
}
