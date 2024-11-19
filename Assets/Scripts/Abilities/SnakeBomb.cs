using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBomb : MonoBehaviour
{
    public float heightAmplitude = 0.45f;  // Difference between the min and max heights
    public float floatSpeed = 1f;  // Speed of the floating motion
    public float baseHeight = 0.5f;  // Starting height (midpoint between min and max height)
    public float burstSpeed = 10f;
    //public bool floatingUp = true;
    private Rigidbody RB;
    private GameObject sprite;
    //private Vector3 targetPos;
    //private Vector3 startingPos;
    // Start is called before the first frame update
    private float timeOffset;  // Time offset for object1 to control different starting points
    public GameObject explosionPrefab;
    public float radius = 1f;
    public float delay = 0.05f;
    public Vector3 centerOffset = new Vector3(0f, -.25f, 0f);
    void Start()
    {
        RB = GetComponent<Rigidbody>();
        Vector3 dir = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
        RB.AddForce(dir * burstSpeed, ForceMode.Impulse);
        timeOffset = Random.Range(0f, Mathf.PI * 2f);
        sprite = GetComponentInChildren<SpriteRenderer>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        float newY = baseHeight + Mathf.Sin(Time.time * floatSpeed + timeOffset) * heightAmplitude;
        sprite.transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    public void PrimeExplosion()
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        transform.localScale = Vector3.zero;
        Invoke("Explode", delay);
    }

    private void Explode()
    {
        Vector3 center = transform.position + centerOffset;
        //center.y = 0;

        Collider[] objectsInRange = Physics.OverlapSphere(center, radius);
        foreach (Collider obj in objectsInRange)
        {
            // Check for warriors
            if (obj.GetComponent<WarriorController>() != null)
            {
                // Damage warrior
                obj.GetComponent<WarriorController>().Die();
                // Debug.Log("Stunned Warrior: " + obj.name);
            }
        }
        
        Destroy(gameObject);
    }
}
