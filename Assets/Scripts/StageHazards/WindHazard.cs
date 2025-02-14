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

    [SerializeField] private GameObject wind;

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
            randomizeWindDirection();
            wind.SetActive(false);
        }

        if (GM.IsPlayingGet())
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= thisSpawnTime)
            {
                activeTime += Time.deltaTime;
                if (activeTime <= timeLimit)
                {
                    wind.SetActive(true);
                    if (goingRight)
                    {
                        BallBody.AddForce(new Vector3(windForce, 0.0f, 0.0f));
                    }
                    else
                    {
                        BallBody.AddForce(new Vector3(-windForce, 0.0f, 0.0f));
                    }
                }
                else
                {
                    randomizeWindDirection();
                    wind.SetActive(false);
                    activeTime = 0.0f;
                    spawnTimer = 0.0f;
                }
            }
        }
    }

    void randomizeWindDirection()
    {
        goingRight = Random.Range(0, 2) == 0;

        if(goingRight)
        {
            wind.transform.rotation = Quaternion.Euler(0, 0, 0);
        } else
        {
            wind.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }
}
