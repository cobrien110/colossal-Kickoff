using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOnAxis : MonoBehaviour
{
    // Speed of rotation (degrees per second)
    public float rotationSpeed = 100f;
    private GameObject followObject = null;
    public bool disconnectFromParent = false;
    private float startingY;
    public int axis = 0;
    public bool randomSpeed = false;
    public float randomAmount = 500f;

    void LateUpdate()
    {
        // Calculate rotation for this frame
        float rotationY = rotationSpeed * Time.deltaTime;

        // Apply rotation to the object
        if (axis == 0) transform.Rotate(0f, rotationY, 0f);
        else transform.Rotate(0f, 0f, rotationY);

        if (followObject != null && disconnectFromParent)
        {
            transform.position = followObject.transform.position;
            transform.position = new Vector3(transform.position.x, startingY, transform.position.z);
        }
    }

    private void Start()
    {
        if (randomSpeed) rotationSpeed += Random.Range(-randomAmount, randomAmount);

        if (transform.parent != null && disconnectFromParent)
        {
            followObject = transform.parent.gameObject;
            transform.parent = null;
            startingY = transform.position.y;
        }
    }
}
