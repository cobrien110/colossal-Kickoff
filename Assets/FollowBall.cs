using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowBall : MonoBehaviour
{
    private GameObject followTarget;

    public BallProperties BP;
    public Color team1Col;
    public Color team2Col;
    public float alpha = 0.75f;
    
    private SpriteRenderer SR;

    public float yOffsetBall = 1f;
    public float yOffsetWarrior = 1.5f;
    public float yOffsetMonster = 2f;

    Vector3 velocity;
    public float smoothTime = 0.25f;
    // Start is called before the first frame update
    void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        BP = GameObject.FindGameObjectWithTag("Ball").GetComponent<BallProperties>();
    }

    // Update is called once per frame
    void Update()
    {
        if (BP == null) return;
        if (BP.ballOwner != null)
        {
            followTarget = BP.ballOwner;
        } else
        {
            followTarget = BP.gameObject;
        }
        float y;

        if (followTarget.GetComponent<WarriorController>())
        {
            SR.color = team2Col;
            y = yOffsetWarrior;
        } else if (followTarget.GetComponent<MonsterController>())
        {
            SR.color = team1Col;
            y = yOffsetMonster;
        } else
        {
            SR.color = Color.white;
            y = yOffsetBall;
        }
        SR.color = new Color(SR.color.r, SR.color.g, SR.color.b, alpha);

        Vector3 newPosition = followTarget.transform.position + new Vector3(0f, y, 0f);
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }
}
