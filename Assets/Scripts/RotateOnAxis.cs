using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOnAxis : MonoBehaviour
{
    // Speed of rotation (degrees per second)
    public float rotationSpeed = 100f;

    void Update()
    {
        // Calculate rotation for this frame
        float rotationY = rotationSpeed * Time.deltaTime;

        // Apply rotation to the object
        transform.Rotate(0f, rotationY, 0f);
    }
}
