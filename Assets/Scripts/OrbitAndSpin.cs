using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitAndSpin : MonoBehaviour
{
    public Transform center; //The central arena around which the island orbits
    public float orbitSpeed = 10f; //Speed at which the island orbits the center
    public float spinSpeed = 30f;  //Speed at which the island spins in place
    public float orbitRadius = 20f; //The radius of the island's orbit

    private float angle = 0f;

    private void Update()
    {
        //Orbit around the central arena
        OrbitAroundCenter();

        //Spin the island in place
        SpinInPlace();
    }

    private void OrbitAroundCenter()
    {
        if (center == null) return;

        //Increment the angle to simulate circular orbit
        angle += orbitSpeed * Time.deltaTime;
        angle = angle % 360f; //Keep the angle within 0-360 degrees

        //Calculate the new position of the island based on the orbit radius
        float x = Mathf.Cos(angle) * orbitRadius;
        float z = Mathf.Sin(angle) * orbitRadius;

        //Set the island's position to orbit around the center at the calculated position
        transform.position = new Vector3(center.position.x + x, transform.position.y, center.position.z + z);
    }

    private void SpinInPlace()
    {
        //Rotate the island around its Y-axis (spinning in place)
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);
    }
}
