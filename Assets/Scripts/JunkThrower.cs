using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JunkThrower : MonoBehaviour
{
    public GameObject junkPrefab;
    public float range = 8f;
    public float minHeight = 4f;
    public float coolDown = 0.25f;
    private float startingCooldown = 0.25f;
    private float timer = 0f;
    public bool isSpawning = false;
    public GameObject monster;

    public float arcTime = 2f;
    public float arcHeight = 3f;
    //private float elapsedArcTime = 0f;

    private void Start()
    {
        startingCooldown = coolDown;
    }

    public void Throw()
    {
        Vector3 spawnPos = Random.onUnitSphere * range;
        if (spawnPos.y < minHeight) spawnPos = new Vector3(spawnPos.x, minHeight, spawnPos.y);
        if (junkPrefab != null)
        {
            Junk j = Instantiate(junkPrefab, spawnPos, Quaternion.identity).GetComponent<Junk>();
            j.arcHeight = arcHeight;
            j.jumpInTime = arcTime;
        }

        coolDown = Random.Range(startingCooldown - 0.15f, startingCooldown + 0.15f);
        coolDown = Mathf.Clamp(coolDown, 0.1f, 5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(Vector3.zero, range);
        Vector3 cubePos = new Vector3(0f, minHeight, 0f);
        Vector3 cubSize = new Vector3(range*2, 0.1f, range*2);
        Gizmos.DrawWireCube(cubePos, cubSize);
    }

    private void Update()
    {
        if (isSpawning) timer += Time.deltaTime;
        if (timer >= coolDown)
        {
            Throw();
            timer = 0f;
        }
    }
}
