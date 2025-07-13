using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public float pushForce = 50f;
    public Vector3 centerOffset = new Vector3(0f, -.25f, 0f);
    [HideInInspector] public bool isExploding = false;
    private AbilitySnakeMines ASM;
    private MonsterController MC;
    private HashSet<WarriorController> warriorsInRadius = new HashSet<WarriorController>();
    private bool isBallInRadius = false;
    void Start()
    {
        RB = GetComponent<Rigidbody>();
        Vector3 dir = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
        RB.AddForce(dir * burstSpeed, ForceMode.Impulse);
        timeOffset = Random.Range(0f, Mathf.PI * 2f);
        sprite = GetComponentInChildren<SpriteRenderer>().gameObject;
        transform.rotation = new Quaternion(0f, transform.rotation.y, transform.rotation.z, transform.rotation.w);
        ASM = GameObject.FindGameObjectWithTag("Monster").GetComponent<AbilitySnakeMines>();
        MC = GameObject.FindAnyObjectByType<MonsterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float newY = baseHeight + Mathf.Sin(Time.time * floatSpeed + timeOffset) * heightAmplitude;
        transform.position = new Vector3(transform.position.x, baseHeight, transform.position.z);
        sprite.transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        foreach (WarriorController wc in warriorsInRadius)
        {
            Debug.Log("warriorsInRadius: " + wc.name);
        }
    }

    public void PrimeExplosion(bool willPushBall)
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        transform.localScale = Vector3.zero;
        isExploding = true;
        if (!willPushBall) Invoke("Explode", delay);
        else Invoke("ExplodeAndPush", delay);
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
            SnakeBomb snakeBomb = obj.GetComponent<SnakeBomb>();
            if (snakeBomb != null && !snakeBomb.isExploding)
            {
                ASM.ExplodeSpecificBomb(snakeBomb);
            }
        }
        
        Destroy(gameObject);
    }

    private void ExplodeAndPush()
    {
        Vector3 center = transform.position + centerOffset;
        BallProperties BP = GameObject.FindAnyObjectByType<BallProperties>();
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
            SnakeBomb snakeBomb = obj.GetComponent<SnakeBomb>();
            if (snakeBomb != null && !snakeBomb.isExploding)
            {
                ASM.ExplodeSpecificBomb(snakeBomb);
            }

            if (obj.gameObject.CompareTag("Ball") && BP.ballOwner == null)
            {
                // push away ball
                Debug.Log("BOMB HIT BALL!");
                float kickForce = pushForce;
                
                Vector3 posA = new Vector3(BP.gameObject.transform.position.x, 0f, BP.gameObject.transform.position.z);
                Vector3 posB = new Vector3(transform.position.x, 0f, transform.position.z);
                Vector3 dir = (posA - posB).normalized;
                Vector3 forceToAdd = dir * kickForce;
                BP.GetComponent<Rigidbody>().AddForce(forceToAdd);
                //update owner
                BP.previousKicker = MC.gameObject;
                BP.playerTest = BP.previousKicker;
            }
        }

        Destroy(gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Warrior"))
        {
            // Debug.Log("SnakeBomb OnTriggerStay: " + other.gameObject.name);
            Vector3 center = transform.position + centerOffset;
            Collider[] objectsInRange = Physics.OverlapSphere(center, radius);
            WarriorController wc = other.GetComponent<WarriorController>();
            if (wc != null)
            {
                if (objectsInRange.Any(c => c.gameObject == wc.gameObject)) // Warrior is in hitbox
                {
                    warriorsInRadius.Add(wc);
                } else // Warrior is NOT in hitbox
                {
                    warriorsInRadius.Remove(wc);
                }
            }
        } else if (other.CompareTag("Ball"))
        {
            // Debug.Log("SnakeBomb OnTriggerStay: " + other.gameObject.name);
            Vector3 center = transform.position + centerOffset;
            Collider[] objectsInRange = Physics.OverlapSphere(center, radius);
            if (objectsInRange.Any(c => c.gameObject == other.gameObject)) // Ball is in hitbox
            {
                isBallInRadius = true;
            }
            else // Ball is NOT in hitbox
            {
                isBallInRadius = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Warrior"))
        {
            // Debug.Log("SnakeBomb OnTriggerExit: " + other.gameObject.name);
            WarriorController wc = other.GetComponent<WarriorController>();
            if (wc != null)
            {
                warriorsInRadius.Remove(wc);
            }
        }
        else if (other.CompareTag("Ball"))
        {
            isBallInRadius = false;
        }
    }

    public bool WarriorInRadius()
    {
        return warriorsInRadius.Count > 0;
    }

    public bool GetIsBallInRadius()
    {
        return isBallInRadius;
    }

    public void ResetObjectsInRadius()
    {
        warriorsInRadius.Clear();
        isBallInRadius = false;
    }
}
