using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallOutlineColor : MonoBehaviour
{
    /*
    [SerializeField] private Color monCol;
    [SerializeField] private Color warCol;
    [SerializeField] private Color baseCol;
    */
    private FollowBall FB;
    private SpriteRenderer SR;
    // Start is called before the first frame update
    void Start()
    {
        FB = GameObject.FindAnyObjectByType<FollowBall>();
        SR = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerPrefs.GetInt("ballOutlineMatchesTeam") == 1)
        {
            Color fbCol = Color.white;
            if (FB != null) fbCol = FB.SR.color;
            fbCol = new Color(fbCol.r, fbCol.g, fbCol.b, 1);
            if (SR != null && SR.color != fbCol)
            {
                SR.color = fbCol;
            }
        }
    }
}
