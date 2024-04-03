using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallProperties : MonoBehaviour
{
    [HideInInspector] public GameObject ballOwner = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Warrior"))
        {
            ballOwner = other.gameObject;
        }
    }
}
