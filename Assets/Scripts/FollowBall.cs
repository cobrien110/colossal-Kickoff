using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowBall : MonoBehaviour
{
    /*private GameObject followTarget;

    public float startingAlpha = -3f;
    public BallProperties BP;
    public Color team1Col;
    public Color team2Col;
    public float alpha = 0.75f;
    public float alphaGainRate = 0.1f;
    
    private SpriteRenderer SR;

    public float yOffsetBall = 1f;
    public float yOffsetWarrior = 1.5f;
    public float yOffsetMonster = 2f;

    Vector3 velocity;
    public float smoothTime = 0.25f;
    private float noOwnerAlpha = 1f;
    // Start is called before the first frame update
    void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        BP = GameObject.FindGameObjectWithTag("Ball").GetComponent<BallProperties>();
        noOwnerAlpha = startingAlpha;
        SR.color = new Color(SR.color.r, SR.color.g, SR.color.b, alpha * noOwnerAlpha);
    }

    // Update is called once per frame
    void LateUpdate()
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
            SR.color = new Color(SR.color.r, SR.color.g, SR.color.b, alpha);
            y = yOffsetWarrior;
            noOwnerAlpha = -1;
        } else if (followTarget.GetComponent<MonsterController>())
        {
            SR.color = team1Col;
            SR.color = new Color(SR.color.r, SR.color.g, SR.color.b, alpha);
            y = yOffsetMonster;
            noOwnerAlpha = -1;
        } else
        {
            SR.color = Color.white;
            if (noOwnerAlpha < 1) noOwnerAlpha += Time.deltaTime * alphaGainRate;
            SR.color = new Color(SR.color.r, SR.color.g, SR.color.b, alpha * noOwnerAlpha);
            y = yOffsetBall;
        }
        

        Vector3 newPosition = followTarget.transform.position + new Vector3(0f, y, 0f);
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }*/
}
