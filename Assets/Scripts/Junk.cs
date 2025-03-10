using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Junk : MonoBehaviour
{
    public float jumpInTime = 2f;
    public float arcHeight = 3f;
    private float elapsedJumpTime = 0f;
    private Vector3 startingPos;
    public Vector3 monsterPos;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
        monsterPos = GameObject.FindGameObjectWithTag("Monster").transform.position;
        float rX = Random.Range(-.025f, .025f);
        float rZ = Random.Range(-.025f, .025f);
        monsterPos = new Vector3(monsterPos.x + rX, monsterPos.y - 0.45f, monsterPos.z + rZ);
    }

    // Update is called once per frame
    void Update()
    {
        if (monsterPos == null) return;

        if (elapsedJumpTime < jumpInTime)
        {
            elapsedJumpTime += Time.deltaTime;
        }
        float t = elapsedJumpTime / jumpInTime;

        // Linear interpolation between start and end
        Vector3 horizontalPosition = Vector3.Lerp(startingPos, monsterPos, t);

        // Parabolic height calculation
        float arc = arcHeight * Mathf.Sin(t * Mathf.PI);

        // Apply the arc to the Y-axis
        transform.position = new Vector3(horizontalPosition.x, horizontalPosition.y + arc, horizontalPosition.z);
    }
}
