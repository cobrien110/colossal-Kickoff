using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCrystal : MonoBehaviour
{
    private DeleteAfterDelay DAD;
    public float stunTime = 1f;
    public float radius = 1f;
    public MonsterController MC;
    public float speed = 0.25f;
    public Vector3 movePoint;

    public GameObject attackVisualizer;
    private bool isEchoing = false;
    public bool firstPointSet = false;
    public GameObject iceWavePrefab;

    public void Awake()
    {
        attackVisualizer.SetActive(false);
        DAD = GetComponent<DeleteAfterDelay>();
    }

    public void Update()
    {
        
        if (movePoint != null && firstPointSet && !isEchoing)
        {
            //transform.position = 
                //Vector3.MoveTowards(transform.position, new Vector3(movePoint.x, transform.position.y, movePoint.z), speed * Time.deltaTime);
        }
        
    }

    public void Echo()
    {
        if (isEchoing) return;
        isEchoing = true;
        DAD.NewTimer(1f);
        attackVisualizer.SetActive(true);
        attackVisualizer.transform.localScale *= radius;
        AudioPlayer AP = GetComponent<AudioPlayer>();
        if (AP != null) AP.PlaySoundRandomPitch(AP.Find("iceBreak"));

        // Schedule the deactivation after 1 second
        Invoke(nameof(DeactivateVisualizer), 1f);

        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider obj in objectsInRange)
        {
            // Debug.Log("Objects in range of howl:" + obj.name);
            // Check for ball
            if (obj.GetComponent<BallProperties>() != null)
            {
                // Stop ball
                try
                {
                    MC.BP.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    MC.BP.GetComponent<Rigidbody>().rotation = Quaternion.identity;
                    // Debug.Log("Ball stopped by howl");
                }
                catch
                {
                    Rigidbody ball = GameObject.FindGameObjectWithTag("Ball").GetComponent<BallProperties>().GetComponent<Rigidbody>();
                    ball.velocity = Vector3.zero;
                    ball.rotation = Quaternion.identity;
                }
            }

            // Check for warriors
            if (obj.GetComponent<WarriorController>() != null)
            {
                // Stun warrior
                obj.GetComponent<WarriorController>().Stun(stunTime);
                // Debug.Log("Stunned Warrior: " + obj.name);
            }
        }

        // spawn Ice wave
        Instantiate(iceWavePrefab, transform.position, Quaternion.identity);
    }

    public void SetNewPoint(Vector3 newPoint)
    {
        movePoint = newPoint;
        firstPointSet = true;
        DAD.NewTimer(DAD.deathTimer);
    }

    private void OnTriggerStay(Collider other)
    {
        WarriorController WC = other.gameObject.GetComponent<WarriorController>();
        if (WC != null && WC.isSliding)
        {
            Echo();
        }
    }

    private void DeactivateVisualizer()
    {
        attackVisualizer.SetActive(false);
    }

    public void ResetTimer()
    {
        DAD.NewTimer(DAD.deathTimer);
    }

    private void OnDrawGizmos()
    {
        // Set the color of the gizmo
        Gizmos.color = Color.red;

        // Draw a wireframe sphere at the object's position with the radius of howlRadius
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
