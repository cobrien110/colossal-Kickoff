using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuetzFlyCamTransform : MonoBehaviour
{
    public float yBelow;
    private Transform follow;
    private AbilityFly fly;
    private float startingY;
    // Start is called before the first frame update
    void Start()
    {
        follow = transform.parent;
        startingY = transform.position.y;
        fly = transform.parent.GetComponentInParent<AbilityFly>();
    }

    // Update is called once per frame
    void Update()
    {
        // whenever the fly ability is active, we want to set the position of the follow target to be
        // below the head as to not make the camera change as jarring
        
        if (fly.isActive)
        {
            transform.position = new Vector3(follow.position.x, follow.position.y - yBelow, follow.position.z);
            
        }

        // however, we don't want the transform to be under where we started
        if (transform.position.y < startingY) transform.position = new Vector3(transform.position.x, startingY, transform.position.z);
    }
}
