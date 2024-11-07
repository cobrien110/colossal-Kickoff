using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondaryArrow : MonoBehaviour
{
    private SpriteRenderer SR;
    public FollowBall FB;
    // Start is called before the first frame update
    void Start()
    {
        FB = GetComponentInParent<FollowBall>();
        SR = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        SR.color = FB.SR.color;
    }
}
