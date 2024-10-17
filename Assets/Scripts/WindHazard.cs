using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindHazard : MonoBehaviour
{
    [SerializeField] private GameplayManager GM = null;
    [SerializeField] private GameObject Ball = null;
    [SerializeField] private Rigidbody BallBody = null;
    [SerializeField] private float spawnTimer = 0;
    [SerializeField] private float thisSpawnTime = 10;
    [SerializeField] private bool goingRight = false;
    [SerializeField] private float windForce = 0.1f;
    [SerializeField] private float activeTime = 0.0f;
    [SerializeField] private float timeLimit = 5.0f;

    // Temp Code to indicate Wind Direction
    [SerializeField] private GameObject RightArrow;
    [SerializeField] private GameObject LeftArrow;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Ball == null)
        {
            Ball = GameObject.FindGameObjectWithTag("Ball");
            BallBody = Ball.GetComponent<Rigidbody>();
            spawnTimer = 0.0f;
            activeTime = 0.0f;
            goingRight = !goingRight;
            RightArrow.SetActive(false);
            LeftArrow.SetActive(false);
        }

        if (GM.IsPlayingGet())
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= thisSpawnTime)
            {
                activeTime += Time.deltaTime;
                if (activeTime <= timeLimit)
                {
                    if (goingRight)
                    {
                        RightArrow.SetActive(true);
                        LeftArrow.SetActive(false);
                        BallBody.AddForce(new Vector3(windForce, 0.0f, 0.0f));
                    }
                    else
                    {
                        RightArrow.SetActive(false);
                        LeftArrow.SetActive(true);
                        BallBody.AddForce(new Vector3(-windForce, 0.0f, 0.0f));
                    }
                }
                else
                {
                    goingRight = !goingRight;
                    activeTime = 0.0f;
                    spawnTimer = 0.0f;
                    RightArrow.SetActive(false);
                    LeftArrow.SetActive(false);
                }
            }
        }
    }
}
