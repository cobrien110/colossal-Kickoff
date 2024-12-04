using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOnAxis : MonoBehaviour
{
    // Speed of rotation (degrees per second)
    public float rotationSpeed = 100f;
    private GameObject followObject = null;
    public bool disconnectFromParent = false;

    void LateUpdate()
    {
        // Calculate rotation for this frame
        float rotationY = rotationSpeed * Time.deltaTime;

        // Apply rotation to the object
        transform.Rotate(0f, rotationY, 0f);

        if (followObject != null && disconnectFromParent)
        {
            transform.position = followObject.transform.position;
        }
    }

    private void Start()
    {
        if (transform.parent != null && disconnectFromParent)
        {
            followObject = transform.parent.gameObject;
            transform.parent = null;
        }
    }
}
